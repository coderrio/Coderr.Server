using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Abstractions.Security;
using Coderr.Server.App.Core.Reports.Config;
using Coderr.Server.ReportAnalyzer.Inbound;
using Coderr.Server.Web.Infrastructure;
using Coderr.Server.Web.Infrastructure.Results;
using DotNetCqs.Queues;
using Griffin.Data;
using Griffin.Net.Protocols.Http;
using log4net;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Coderr.Server.Web.Controllers
{
    [EnableCors("CorsPolicy")]
    public class ReportReceiverController : Controller
    {
        private const int CompressedReportSizeLimit = 1000000;

        private static int _currentReportCount;
        private readonly ConfigurationStore _configStore;
        private readonly ILog _logger = LogManager.GetLogger(typeof(ReportReceiverController));
        private readonly IMessageQueue _messageQueue;
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public ReportReceiverController(IMessageQueueProvider queueProvider, IAdoNetUnitOfWork unitOfWork,
            ConfigurationStore configStore)
        {
            _unitOfWork = unitOfWork;
            _configStore = configStore;
            _messageQueue = queueProvider.Open("ErrorReports");
        }

        [HttpGet]
        [Route("receiver/report/")]
        public IActionResult Index()
        {
            return Content("Hello world", "text/plain");
        }

        [HttpPost]
        [Route("receiver/report/{appKey}"), Transactional]
        public async Task<IActionResult> Post(string appKey, string sig)
        {
            var contentLength = Request.ContentLength;
            if (contentLength > CompressedReportSizeLimit)
                return await KillLargeReportAsync(appKey);
            if (contentLength == null || contentLength < 1)
                return BadRequest("Content required.");

            try
            {
                var buffer = new byte[contentLength.Value];
                var bytesRead = 0;
                while (bytesRead < contentLength.Value)
                {
                    bytesRead += await Request.Body.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);
                }

                var reportConfig = _configStore.Load<ReportConfig>();
                var config = new ReportConfigWrapper(reportConfig);
                var handler = new SaveReportHandler(_messageQueue, _unitOfWork, config);
                var principal = CreateReporterPrincipal();
                var remoteIp = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                await handler.BuildReportAsync(principal, appKey, sig, remoteIp, buffer);
                return NoContent();
            }
            catch (InvalidCredentialException)
            {
                return BadRequest(new ErrorMessage("INVALID_APP_KEY"));
            }
            catch (HttpException ex)
            {
                _logger.InfoFormat(ex.Message);
                return new ContentResult
                {
                    Content = ex.Message,
                    StatusCode = ex.HttpCode
                };
            }
            catch (Exception exception)
            {
                _logger.Error(
                    "Failed to handle request from " + appKey + " / " + Request.HttpContext.Connection.RemoteIpAddress,
                    exception);
                return new ContentResult
                {
                    Content = exception.Message,
                    StatusCode = (int) HttpStatusCode.InternalServerError
                };
            }
        }

        internal static ClaimsPrincipal CreateReporterPrincipal()
        {
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Name, "ReportReceiver"),
                new Claim(ClaimTypes.NameIdentifier, "0"),
                new Claim(ClaimTypes.Role, CoderrRoles.System)
            }));
            return principal;
        }

        private Task<IActionResult> KillLargeReportAsync(string appKey)
        {
            _logger.Error(appKey + "Too large report: " + Request.ContentLength + " from " +
                          Request.HttpContext.Connection.RemoteIpAddress);
            //TODO: notify
            return Task.FromResult<IActionResult>(NoContent());
        }
    }
}