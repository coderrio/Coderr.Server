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
using OneTrueError.Web.Areas.Receiver.Helpers;
using OneTrueError.Web.Areas.Receiver.Models;

namespace OneTrueError.Web.Areas.Receiver.Controllers
{
    [AllowAnonymous]
    public class FeedbackController : ApiController
    {
        private ILog _logger = LogManager.GetLogger(typeof(FeedbackController));

        public FeedbackController()
        {
        }

        [HttpPost, Route("receiver/report/{appKey}/feedback")]
        public async Task<HttpResponseMessage> SupplyFeedback(string appKey, string sig, FeedbackModel model)
        {
            try
            {
                using (var connection = ConnectionFactory.Create())
                {
                    using (var cmd = (DbCommand)connection.CreateCommand())
                    {
                        cmd.CommandText =
                            @"INSERT INTO QueueFeedback (appkey, signature, createdatutc, reportid, ipaddress, description, email)
                                            VALUES (@appkey, @signature, @createdatutc, @reportid, @ipaddress, @description, @email);";
                        cmd.AddParameter("createdatutc", DateTime.UtcNow);
                        cmd.AddParameter("appKey", appKey);
                        cmd.AddParameter("signature", sig);
                        cmd.AddParameter("reportid", model.ReportId);
                        cmd.AddParameter("ipaddress", Request.GetClientIpAddress());
                        cmd.AddParameter("description", model.Description);
                        cmd.AddParameter("email", model.EmailAddress);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Warn("Failed to submit feedback: " + JsonConvert.SerializeObject(new { appKey = appKey, model = model }), ex);
            }

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

    }
}
