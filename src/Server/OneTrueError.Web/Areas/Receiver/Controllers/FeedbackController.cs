using System;
using System.Data.Common;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Griffin.Data;
using log4net;
using Newtonsoft.Json;
using OneTrueError.Infrastructure;
using OneTrueError.Infrastructure.Queueing;
using OneTrueError.ReportAnalyzer.LibContracts;
using OneTrueError.Web.Areas.Receiver.Helpers;
using OneTrueError.Web.Areas.Receiver.Models;

namespace OneTrueError.Web.Areas.Receiver.Controllers
{
    [AllowAnonymous]
    public class FeedbackController : ApiController
    {
        private ILog _logger = LogManager.GetLogger(typeof(FeedbackController));
        private IMessageQueue _queue;

        public FeedbackController(OneTrueError.Infrastructure.Queueing.IMessageQueueProvider queueProvider)
        {
            _queue = queueProvider.Open("FeedbackQueue");
        }

        [HttpPost, Route("receiver/report/{appKey}/feedback")]
        public async Task<HttpResponseMessage> SupplyFeedback(string appKey, string sig, FeedbackModel model)
        {
            try
            {
                int appId;
                using (var connection = ConnectionFactory.Create())
                {
                    using (var cmd = (DbCommand) connection.CreateCommand())
                    {
                        cmd.CommandText = "SELECT Id FROM Applications WHERE AppKey = @key";
                        cmd.AddParameter("key", appKey);
                        appId = (int) await cmd.ExecuteScalarAsync();
                    }

                }
                using (var transaction = _queue.BeginTransaction())
                {
                    var dto = new ReceivedFeedbackDTO
                    {
                        ApplicationId = appId,
                        Description = model.Description,
                        EmailAddress = model.EmailAddress,
                        ReceivedAtUtc = DateTime.UtcNow,
                        RemoteAddress = Request.GetClientIpAddress(),
                        ReportId = model.ReportId,
                        ReportVersion = "1"
                    };
                    _queue.Write(appId, dto);

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                _logger.Warn(
                    "Failed to submit feedback: " + JsonConvert.SerializeObject(new {appKey = appKey, model = model}),
                    ex);
            }

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

    }
}
