using System;
using System.Data.Common;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using codeRR.Client.Contracts;
using codeRR.Server.App.Core.Applications;
using codeRR.Server.Infrastructure;
using codeRR.Server.Infrastructure.Messaging;
using codeRR.Server.ReportAnalyzer.LibContracts;
using codeRR.Server.Web.Areas.Receiver.Helpers;
using codeRR.Server.Web.Areas.Receiver.Models;
using codeRR.Server.Web.Cqs;
using codeRR.Server.Web.Infrastructure.Cqs;
using DotNetCqs;
using DotNetCqs.Queues;
using Griffin.Data;
using log4net;
using Newtonsoft.Json;

namespace codeRR.Server.Web.Areas.Receiver.Controllers
{
    [AllowAnonymous]
    public class FeedbackController : ApiController
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly ILog _logger = LogManager.GetLogger(typeof(FeedbackController));
        private readonly IMessageQueue _queue;

        public FeedbackController(IMessageQueueProvider queueProvider, IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
            _queue = queueProvider.Open("Feedback");
        }

        [HttpPost, Route("receiver/report/{appKey}/feedback")]
        public async Task<HttpResponseMessage> SupplyFeedback(string appKey, string sig)
        {

            var json = await UnpackContent();

            try
            {
                var ser = new MessagingSerializer();
                var model = (FeedbackDTO)ser.Deserialize(typeof(FeedbackDTO), json);

                var app = await _applicationRepository.GetByKeyAsync(appKey);
                using (var session = _queue.BeginSession())
                {
                    var dto = new ProcessFeedback
                    {
                        ApplicationId = app.Id,
                        Description = model.Description,
                        EmailAddress = model.EmailAddress,
                        ReceivedAtUtc = DateTime.UtcNow,
                        RemoteAddress = Request.GetClientIpAddress(),
                        ReportId = model.ReportId,
                        ReportVersion = "1"
                    };

                    await session.EnqueueAsync(ReportController.CreateReporterPrincipal(), new Message(dto));
                    await session.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.Warn(
                    "Failed to submit feedback: " + JsonConvert.SerializeObject(new { appKey, json }),
                    ex);
            }

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        private async Task<string> UnpackContent()
        {
            var ms = new MemoryStream();
            await HttpContext.Current.Request.InputStream.CopyToAsync(ms);
            ms.Position = 0;

            // not compressed.
            if (ms.GetBuffer()[0] == '{')
            {
                var str = new StreamReader(ms);
                return str.ReadToEnd();
            }

            ms.Position = 0;
            using (var zipStream = new GZipStream(HttpContext.Current.Request.InputStream, CompressionMode.Decompress))
            {
                await zipStream.CopyToAsync(ms);
            }

            ms.Position = 0;
            var sr = new StreamReader(ms, Encoding.UTF8);
            return sr.ReadToEnd();
        }
    }
}