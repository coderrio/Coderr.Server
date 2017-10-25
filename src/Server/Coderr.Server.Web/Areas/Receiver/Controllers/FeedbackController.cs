using System;
using System.Data.Common;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using codeRR.Server.App.Core.Applications;
using codeRR.Server.Infrastructure;
using codeRR.Server.ReportAnalyzer.LibContracts;
using codeRR.Server.Web.Areas.Receiver.Helpers;
using codeRR.Server.Web.Areas.Receiver.Models;
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
        public async Task<HttpResponseMessage> SupplyFeedback(string appKey, string sig, FeedbackModel model)
        {
            try
            {
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

                    await session.EnqueueAsync(User as ClaimsPrincipal, new Message(dto));
                    await session.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.Warn(
                    "Failed to submit feedback: " + JsonConvert.SerializeObject(new { appKey, model }),
                    ex);
            }

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }
    }
}