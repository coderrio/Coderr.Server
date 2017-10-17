using System;
using System.Data.Common;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using codeRR.Server.Infrastructure;
using codeRR.Server.Infrastructure.Queueing;
using codeRR.Server.ReportAnalyzer.LibContracts;
using codeRR.Server.Web.Areas.Receiver.Helpers;
using codeRR.Server.Web.Areas.Receiver.Models;
using Griffin.Data;
using log4net;
using Newtonsoft.Json;

namespace codeRR.Server.Web.Areas.Receiver.Controllers
{
    [AllowAnonymous]
    public class FeedbackController : ApiController
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(FeedbackController));
        private readonly IMessageQueue _queue;
        private readonly IConnectionFactory _connectionFactory;

        public FeedbackController(IMessageQueueProvider queueProvider, IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
            _queue = queueProvider.Open("FeedbackQueue");
        }

        [HttpPost, Route("receiver/report/{appKey}/feedback")]
        public async Task<HttpResponseMessage> SupplyFeedback(string appKey, string sig, FeedbackModel model)
        {
            try
            {
                int appId;
                using (var connection = _connectionFactory.Open())
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
                    "Failed to submit feedback: " + JsonConvert.SerializeObject(new {appKey, model}),
                    ex);
            }

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }
    }
}