using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Abstractions.Reports;
using Coderr.Server.Abstractions.Security;
using Coderr.Server.App.Modules.Whitelists;
using Coderr.Server.ReportAnalyzer.Inbound;
using Coderr.Server.WebSite.Infrastructure.Adapters;
using Coderr.Server.WebSite.Models;
using DotNetCqs.Queues;
using Griffin.Data;
using Griffin.Net.Protocols.Http;
using log4net;
using Microsoft.AspNetCore.Mvc;

namespace Coderr.Server.WebSite.Controllers
{
    //[EnableCors("CorsPolicy")]
    public class ReportReceiverController : Controller
    {
        private const int CompressedReportSizeLimit = 1000000;
        private readonly ILog _logger = LogManager.GetLogger(typeof(ReportReceiverController));
        private readonly IMessageQueue _messageQueue;
        private readonly IAdoNetUnitOfWork _unitOfWork;
        private readonly IConfiguration<ReportConfig> _reportConfig;
        private readonly IWhitelistService _whitelistService;

        public ReportReceiverController(IMessageQueueProvider queueProvider, IAdoNetUnitOfWork unitOfWork,
            IConfiguration<ReportConfig> reportConfig, IWhitelistService whitelistService)
        {
            _unitOfWork = unitOfWork;
            _reportConfig = reportConfig;
            _whitelistService = whitelistService;
            _messageQueue = queueProvider.Open("ErrorReports");
        }

        [HttpGet]
        [Route("receiver/report/")]
        public IActionResult Index()
        {
            return Content("Hello world", "text/plain");
        }

        [HttpPost]
        [Route("receiver/report/{appKey}")]
        public async Task<IActionResult> Post(string appKey, string sig)
        {
            var contentLength = Request.ContentLength;
            if (contentLength > CompressedReportSizeLimit)
                return await KillLargeReportAsync(appKey);
            if (contentLength == null || contentLength < 1)
                return BadRequest("Content required.");
            var remoteIp = Request.HttpContext.Connection.RemoteIpAddress;

            _logger.Debug($"Report from  {remoteIp} for {appKey}");

            // Sig may be null for web applications
            // as I don't know how to protect the secretKey in web applications
            if (sig == null)
            {
                if (!await _whitelistService.Validate(appKey, remoteIp))
                    return BadRequest($"Must sign error report with the sharedSecret or add ip/domain to the whitelist.");
            }

            try
            {
                var buffer = new byte[contentLength.Value];
                var bytesRead = 0;
                while (bytesRead < contentLength.Value)
                {
                    bytesRead += await Request.Body.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);
                }

                var config = new ReportConfigWrapper(_reportConfig.Value);
                var handler = new SaveReportHandler(_messageQueue, _unitOfWork, config);
                var principal = CreateReporterPrincipal();

                await handler.BuildReportAsync(principal, appKey, sig, remoteIp.ToString(), buffer);
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
                    StatusCode = ex.HttpCode,
                    ContentType = "text/plain"
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
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    ContentType = "text/plain"
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
            }, "AppKey"));
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