using System;
using System.Messaging;

namespace OneTrueError.Infrastructure
{
    public static class SetupTools
    {
        public static ISetupDatabaseTools DbTools { get; set; }

        public static bool ValidateMessageQueue(string queuePath, bool useAuthentication, bool useTransactions,
            out string errorMessage)
        {
            try
            {
                var queue = new MessageQueue(queuePath);

                var msg = new Message("Hello", new BinaryMessageFormatter());
                msg.UseAuthentication = useAuthentication;
                msg.UseDeadLetterQueue = true;
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