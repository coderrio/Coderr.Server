using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using codeRR.Server.Infrastructure.Configuration;
using codeRR.Server.ReportAnalyzer.Inbound;
using codeRR.Server.Web.Areas.Receiver.Helpers;
using codeRR.Server.Web.Areas.Receiver.Models;
using Coderr.Server.PluginApi.Config;
using DotNetCqs.Queues;
using Griffin.Data;
using log4net;

namespace codeRR.Server.Web.Areas.Receiver.Controllers
{
    [AllowAnonymous]
    public class ReportController : ApiController
    {
        private static readonly SamplingCounter _samplingCounter = new SamplingCounter();
        private readonly ConfigurationStore _configStore;
        private readonly ILog _logger = LogManager.GetLogger(typeof(ReportController));
        private readonly IMessageQueue _messageQueue;
        private readonly IAdoNetUnitOfWork _unitOfWork;
        private const int MaxSizeCompressedReport = 5000000;

        static ReportController()
        {
            _samplingCounter.Load();
        }

        public ReportController(IMessageQueueProvider queueProvider, IAdoNetUnitOfWork unitOfWork,
            ConfigurationStore configStore)
        {
            _unitOfWork = unitOfWork;
            _configStore = configStore;
            _messageQueue = queueProvider.Open("Reports");
        }

        [HttpGet]
        [Route("receiver/report/")]
        public HttpResponseMessage Index()
        {
            var content = new StringContent("Hello world 2");
            //content.Headers.Add("Content-Type", "text/plain");
            var resp = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = content
            };
            return resp;
        }

        [HttpPost]
        [Route("receiver/report/{appKey}")]
        public async Task<HttpResponseMessage> Post(string appKey, string sig)
        {
            if (HttpContext.Current.Request.InputStream.Length > MaxSizeCompressedReport)
                return await KillLargeReportAsync(appKey);

            if (!_samplingCounter.CanAccept(appKey))
                return Request.CreateResponse(HttpStatusCode.OK);


            try
            {
                var buffer = new byte[HttpContext.Current.Request.InputStream.Length];
                HttpContext.Current.Request.InputStream.Read(buffer, 0, buffer.Length);
                var handler = new SaveReportHandler(_messageQueue, _unitOfWork, _configStore);
                var principal = CreateReporterPrincipal();
                await handler.BuildReportAsync(principal, appKey, sig, Request.GetClientIpAddress(), buffer);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (InvalidCredentialException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "INVALID_APP_KEY", ex);
            }
            catch (HttpException ex)
            {
                _logger.InfoFormat(ex.Message);
                return Request.CreateErrorResponse((HttpStatusCode) ex.GetHttpCode(), ex.Message);
            }
            catch (Exception exception)
            {
                _logger.Error("Failed to handle request from " + appKey + " / " + Request.GetClientIpAddress(),
                    exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exception);
            }
        }

        internal static ClaimsPrincipal CreateReporterPrincipal()
        {
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Name, "ReportReceiver"),
                new Claim(ClaimTypes.NameIdentifier, "0")
            }));
            return principal;
        }
        private Task<HttpResponseMessage> KillLargeReportAsync(string appKey)
        {
            _logger.Error(appKey + "Too large report: " + HttpContext.Current.Request.InputStream.Length + " from " +
                          Request.GetClientIpAddress());
            //TODO: notify
            return Task.FromResult(Request.CreateResponse(HttpStatusCode.OK));
        }
    }
}