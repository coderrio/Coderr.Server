using System;
using System.Collections.Generic;
using System.IO;
using System.Messaging;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Newtonsoft.Json;

namespace codeRR.Infrastructure.Queueing.Msmq
{
    public class MsmqMessageQueue : IMessageQueue, IDisposable
    {
        private readonly bool _useAuthentication;
        private readonly bool _useTransactions;
        private MessageQueue _queue;

        public MsmqMessageQueue(string queueName, bool useAuthentication, bool useTransactions)
        {
            if (queueName == null) throw new ArgumentNullException(nameof(queueName));

            _useAuthentication = useAuthentication;
            _useTransactions = useTransactions;
            _queue = new MessageQueue(queueName);
            _queue.MessageReadPropertyFilter.Extension = true;
            _queue.Formatter = null;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
        }

        public T Receive<T>()
        {
            var message =
                _queue.Receive();
            return Deserialize<T>(message);
        }

        public object Receive()
        {
            var msg = _queue.Receive();

            var reader = new StreamReader(msg.BodyStream, Encoding.UTF8);
            var json = reader.ReadToEnd();
            var metadata = MetadataHeader.Deserialize(msg);
            var type = Type.GetType(metadata.AssemblyQualifiedTypeName);
            if (type == null)
                throw new NotSupportedException("Failed to get type class from string '" +
                                                metadata.AssemblyQualifiedTypeName + "'.");

            return OneTrueSerializer.Deserialize(json, type);
        }

        public void Write(int applicationId, object message)
        {
            var msg = new Message {UseAuthentication = _useAuthentication};
            SerializeBody(message, msg);
            if (_useTransactions)
                _queue.Send(msg, MessageQueueTransactionType.Single);
            else
                _queue.Send(msg);
        }

        public IQueueTransaction BeginTransaction()
        {
            var trans = new MsmqTransactionAdapter();
            trans.Transaction.Begin();
            return trans;
        }

        public T TryReceive<T>(IQueueTransaction transaction, TimeSpan waitTimeout)
        {
            try
            {
                var trans = ((MsmqTransactionAdapter) transaction).Transaction;
                var message = _queue.Receive(waitTimeout, trans);
                return Deserialize<T>(message);
            }
            catch (MessageQueueException ex)
            {
                if (ex.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                    return default(T);
                throw;
            }
        }

        public T Receive<T>(IQueueTransaction transaction)
        {
            try
            {
                var trans = ((MsmqTransactionAdapter) transaction).Transaction;
                var message = _queue.Receive(trans);
                return Deserialize<T>(message);
            }
            catch (MessageQueueException ex)
            {
                if (ex.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                    return default(T);
                throw;
            }
        }

        public T TryReceive<T>(TimeSpan timeout)
        {
            try
            {
                var message = _queue.Receive(timeout);
                return Deserialize<T>(message);
            }
            catch (MessageQueueException ex)
            {
                if (ex.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                    return default(T);
                throw;
            }
        }

        public void Write(object message)
        {
            var msg = new Message {UseAuthentication = _useAuthentication};
            SerializeBody(message, msg);

            if (_useTransactions)
                _queue.Send(msg, MessageQueueTransactionType.Single);
            else
                _queue.Send(msg);
        }

        /// <summary>
        ///     Dispose pattern
        /// </summary>
        /// <param name="isDisposing">Invoked from Dispose()</param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (_queue != null)
            {
                _queue.Dispose();
                _queue = null;
            }
        }

        private T Deserialize<T>(Message message)
        {
            var reader = new StreamReader(message.BodyStream, Encoding.UTF8);
            var json = reader.ReadToEnd();
            return OneTrueSerializer.Deserialize<T>(json);
        }

        private static void SerializeBody(object message, Message msg)
        {
            msg.Extension = new MetadataHeader(message).Serialize();
            var json = JsonConvert.SerializeObject(message);
            var buf = Encoding.UTF8.GetBytes(json);
            var ms = new MemoryStream(buf, 0, buf.Length);
            msg.BodyStream = ms;
        }

        [Serializable]
        private class MetadataHeader
        {
            public MetadataHeader(object message)
            {
                Headers = new Dictionary<string, string>();
                AssemblyQualifiedTypeName = message.GetType().FullName + ", " +
                                            message.GetType().Assembly.GetName().Name;
            }


            protected MetadataHeader()
            {
                Headers = new Dictionary<string, string>();
            }

            public string AssemblyQualifiedTypeName { get; }

            public Dictionary<string, string> Headers { get; set; }

            public static MetadataHeader Deserialize(Message message)
            {
                var formatter = CreateFormatter();
                var ms = new MemoryStream(message.Extension);
                return (MetadataHeader) formatter.Deserialize(ms);
            }

            public byte[] Serialize()
            {
                var formatter = CreateFormatter();
                var ms = new MemoryStream();
                formatter.Serialize(ms, this);
                var buf = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(buf, 0, buf.Length);
                return buf;
            }

            private static BinaryFormatter CreateFormatter()
            {
                return new BinaryFormatter
                {
                    AssemblyFormat = FormatterAssemblyStyle.Simple,
                    TypeFormat = FormatterTypeStyle.TypesWhenNeeded
                };
            }
        }
    }
}