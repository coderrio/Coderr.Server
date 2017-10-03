#if NET452
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace codeRR.Server.Infrastructure.Queueing.Msmq
{
    /// <summary>
    /// Tools for configuring MSMQ
    /// </summary>
    public class MsMqTools
    {

        public static bool ValidateMessageQueue(string queuePath, bool useAuthentication, bool useTransactions,
            out string errorMessage)
        {
            try
            {
                var queue = new MessageQueue(queuePath);

                var msg = new Message("Hello", new BinaryMessageFormatter())
                {
                    UseAuthentication = useAuthentication,
                    UseDeadLetterQueue = true
                };
                if (useTransactions)
                {
                    queue.Send(msg, MessageQueueTransactionType.Single);
                    var msg2 = queue.Receive(TimeSpan.FromSeconds(1), MessageQueueTransactionType.Single);
                }
                else
                {
                    queue.Send(msg, MessageQueueTransactionType.None);
                    var msg2 = queue.Receive(TimeSpan.FromSeconds(1), MessageQueueTransactionType.None);
                }

                errorMessage = "";
                return true;
            }
            catch (MessageQueueException ex)
            {
                if (ex.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                {
                    errorMessage = "'" + queuePath +
                                   "' failed. Reason: Failed to read (timeout). However it can also be that send is incorrectly configured. Check the Dead-letter queue for hints.";
                }
                else
                {
                    errorMessage = "'" + queuePath + "' failed. Reason: " + ex.Message;
                }
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = "'" + queuePath + "' failed. Reason: " + ex.Message;
                return false;
            }
        }
    }
}
#endif