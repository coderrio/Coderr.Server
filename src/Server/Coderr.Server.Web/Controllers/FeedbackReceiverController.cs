using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Coderr.Client.Contracts;
using Coderr.Server.Domain.Core.Applications;
using Coderr.Server.Infrastructure.Messaging;
using Coderr.Server.ReportAnalyzer.Abstractions.Inbound.Commands;
using DotNetCqs;
using DotNetCqs.Queues;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Coderr.Server.Web.Controllers
{
    public class FeedbackReceiverController : Controller
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly ILog _logger = LogManager.GetLogger(typeof(FeedbackReceiverController));
        private readonly IMessageQueue _queue;

        public FeedbackReceiverController(IMessageQueueProvider queueProvider, IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
            _queue = queueProvider.Open("ErrorReports");
        }

        [HttpPost, Route("receiver/report/{appKey}/feedback")]
        public async Task<IActionResult> SupplyFeedback(string appKey, string sig)
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
                        RemoteAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                        ReportId = model.ReportId,
                        ReportVersion = "1"
                    };

                    await session.EnqueueAsync(ReportReceiverController.CreateReporterPrincipal(), new Message(dto));
                    await session.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.Warn(
                    "Failed to submit feedback: " + JsonConvert.SerializeObject(new { appKey, json }),
                    ex);
            }

            return NoContent();
        }

        private async Task<string> UnpackContent()
        {
            var buffer = new byte[HttpContext.Request.ContentLength.Value];
            Request.Body.Read(buffer, 0, buffer.Length);

            // not compressed.
            if (buffer[0] == '{')
            {
                return Encoding.UTF8.GetString(buffer);
            }

            var ms1 = new MemoryStream(buffer, 0, buffer.Length);
            var ms2 = new MemoryStream(buffer.Length);
            using (var zipStream = new GZipStream(ms1, CompressionMode.Decompress, true))
            {
                await zipStream.CopyToAsync(ms2);
            }

            ms2.Position = 0;
            var sr = new StreamReader(ms2, Encoding.UTF8);
            return sr.ReadToEnd();
        }
    }
}
