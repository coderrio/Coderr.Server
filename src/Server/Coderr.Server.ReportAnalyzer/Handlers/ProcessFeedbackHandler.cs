using System;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Feedback.Commands;
using codeRR.Server.ReportAnalyzer.LibContracts;
using DotNetCqs;
using Griffin.Container;
using log4net;
using Newtonsoft.Json;

namespace codeRR.Server.ReportAnalyzer.Handlers
{
    [Component]
    public class ProcessFeedbackHandler : IMessageHandler<ProcessFeedback>
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(ProcessFeedbackHandler));

        public async Task HandleAsync(IMessageContext context, ProcessFeedback message)
        {
            try
            {
                var submitCmd = new SubmitFeedback(message.ReportId, message.RemoteAddress)
                {
                    CreatedAtUtc = message.ReceivedAtUtc,
                    Email = message.EmailAddress,
                    Feedback = message.Description
                };
                await context.SendAsync(submitCmd);
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to process " + JsonConvert.SerializeObject(message), ex);
            }
        }
    }
}