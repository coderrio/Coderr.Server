using System;
using codeRR.Server.Api.Core.Feedback.Commands;
using codeRR.Server.Infrastructure.Queueing;
using codeRR.Server.ReportAnalyzer.LibContracts;
using DotNetCqs;
using Griffin.ApplicationServices;
using Griffin.Container;
using log4net;
using Newtonsoft.Json;

namespace codeRR.Server.SqlServer.Analysis
{
    /// <summary>
    ///     TODO: In a perfect world, the BL should be moved to BL and the data should remain in the data. But let's not
    ///     concern the separation.
    /// </summary>
    [Component(Lifetime = Lifetime.Singleton)]
    public class ScanForNewFeedback : ApplicationServiceTimer
    {
        private readonly ICommandBus _cmdBus;
        private readonly ILog _logger = LogManager.GetLogger(typeof(ScanForNewFeedback));
        private readonly IMessageQueue _messageQueue;

        public ScanForNewFeedback(ICommandBus cmdBus, IMessageQueueProvider queueProvider)
        {
            _cmdBus = cmdBus;
            _messageQueue = queueProvider.Open("FeedbackQueue");
            Interval = TimeSpan.FromSeconds(1);
        }


        protected override void Execute()
        {
            while (true)
            {
                var dto = _messageQueue.Receive<ReceivedFeedbackDTO>();
                if (dto == null)
                    break;

                try
                {
                    var submitCmd = new SubmitFeedback(dto.ReportId, dto.RemoteAddress)
                    {
                        CreatedAtUtc = dto.ReceivedAtUtc,
                        Email = dto.EmailAddress,
                        Feedback = dto.Description
                    };
                    _cmdBus.ExecuteAsync(submitCmd).Wait();
                }
                catch (Exception ex)
                {
                    _logger.Error("Failed to process " + JsonConvert.SerializeObject(dto), ex);
                }
            }
        }
    }
}