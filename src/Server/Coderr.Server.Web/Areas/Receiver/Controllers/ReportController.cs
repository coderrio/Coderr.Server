using System;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using codeRR.Server.Infrastructure;
using codeRR.Server.ReportAnalyzer.Inbound;
using codeRR.Server.Web.Areas.Receiver.Helpers;
using codeRR.Server.Web.Areas.Receiver.Models;
using DotNetCqs.Queues;
using Griffin.Data;
using log4net;

namespace codeRR.Server.Web.Areas.Receiver.Controllers
{
    [AllowAnonymous]
    public class ReportController : ApiController
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;
        private static readonly SamplingCounter _samplingCounter = new SamplingCounter();
        private readonly ILog _logger = LogManager.GetLogger(typeof(ReportController));
        private readonly IMessageQueue _messageQueue;

        static ReportController()
        {
            _samplingCounter.Load();
        }

        public ReportController(IMessageQueueProvider queueProvider, IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _messageQueue = queueProvider.Open("Reports");
        }

        [HttpGet, Route("receiver/report/")]
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

        [HttpPost, Route("receiver/report/{appKey}")]
        public async Task<HttpResponseMessage> Post(string appKey, string sig)
        {
            if (HttpContext.Current.Request.InputStream.Length > 20000000)
            {
                return await KillLargeReportAsync(appKey);
            }

            if (!_samplingCounter.CanAccept(appKey))
                return Request.CreateResponse(HttpStatusCode.OK);


            try
            {
                var buffer = new byte[HttpContext.Current.Request.InputStream.Length];
                HttpContext.Current.Request.InputStream.Read(buffer, 0, buffer.Length);
                var handler = new SaveReportHandler(_messageQueue, _unitOfWork);
                await handler.BuildReportAsync(User as ClaimsPrincipal, appKey, sig, Request.GetClientIpAddress(), buffer);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (InvalidCredentialException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "INVALID_APP_KEY", ex);
            }
            catch (HttpException ex)
            {
                _logger.InfoFormat(ex.Message);
                return Request.CreateErrorResponse((HttpStatusCode)ex.GetHttpCode(), ex.Message);
            }
            catch (Exception exception)
            {
                _logger.Error("Failed to handle request from " + appKey + " / " + Request.GetClientIpAddress(),
                    exception);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exception);
            }
        }
#pragma warning disable 1998
        private async Task<HttpResponseMessage> KillLargeReportAsync(string appKey)
#pragma warning restore 1998
        {
            _logger.Error(appKey + "Too large report: " + HttpContext.Current.Request.InputStream.Length + " from " +
                          Request.GetClientIpAddress());
            //TODO: notify
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}