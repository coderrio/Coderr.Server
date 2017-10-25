using System;
using codeRR.Server.Api.Core.Feedback.Commands;
using codeRR.Server.ReportAnalyzer.LibContracts;
using DotNetCqs;
using DotNetCqs.Queues;
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
        private readonly IMessageBus _messageBus;
        private readonly ILog _logger = LogManager.GetLogger(typeof(ScanForNewFeedback));
        private readonly IMessageQueue _messageQueue;

        public ScanForNewFeedback(IMessageBus messageBus, IMessageQueueProvider queueProvider)
        {
            _messageBus = messageBus;
            _messageQueue = queueProvider.Open("Feedback");
            Interval = TimeSpan.FromSeconds(1);
        }


        protected override void Execute()
        {
            while (true)
            {
                using (var session = _messageQueue.BeginSession())
                {
                    var msg = session.DequeueWithCredentials(TimeSpan.FromSeconds(1)).GetAwaiter().GetResult();
                    if (msg == null)
                    {
                        session.SaveChanges();
                        break;
                    }


                    var dto = (ProcessFeedback)msg.Message.Body;

                    try
                    {
                        var submitCmd = new SubmitFeedback(dto.ReportId, dto.RemoteAddress)
                        {
                            CreatedAtUtc = dto.ReceivedAtUtc,
                            Email = dto.EmailAddress,
                            Feedback = dto.Description
                        };
                        _messageBus.SendAsync(msg.Principal, submitCmd).Wait();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Failed to process " + JsonConvert.SerializeObject(dto), ex);
                    }

                    session.SaveChanges();
                }
              
            }
        }
    }
}