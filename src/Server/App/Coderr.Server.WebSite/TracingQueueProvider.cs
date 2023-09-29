//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Diagnostics;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Web;
//using DotNetCqs;
//using DotNetCqs.DependencyInjection;
//using DotNetCqs.MessageProcessor;
//using DotNetCqs.Queues;
//using DotNetCqs.Queues.AdoNet;
//using log4net;
//using Newtonsoft.Json;

//namespace Coderr.Server.Web
//{
//    public class TracingQueueProvider : IMessageQueueProvider
//    {
//        private readonly IMessageQueueProvider _inner;

//        public TracingQueueProvider(IMessageQueueProvider inner)
//        {
//            _inner = inner;
//        }

//        public IMessageQueue Open(string queueName)
//        {
//            var q = _inner.Open(queueName);
//            return new TracingQueue(q);
//        }
//    }

//    public class TracingQueue : IMessageQueue
//    {
//        private readonly IMessageQueue _inner;

//        public TracingQueue(IMessageQueue inner)
//        {
//            _inner = inner;

//        }

//        public IMessageQueueSession BeginSession()
//        {
//            var s = _inner.BeginSession();

//            return new TracingSession(_inner.Name, s);
//        }

//        public string Name { get { return _inner.Name; } }
//    }

//    public class TracingSession : IMessageQueueSession
//    {
//        private readonly string _queueName;
//        private readonly IMessageQueueSession _inner;
//        private ILog _logger = LogManager.GetLogger(typeof(TracingSession));
//        private bool _gotData = false;
//        private bool _saved;
//        private bool _dequeue;
//        private bool _enqueue;
//        public TracingSession(string queueName, IMessageQueueSession inner)
//        {
//            _queueName = queueName;
//            _inner = inner;
//        }

//        public void Dispose()
//        {
//            if (!_saved)
//                Debugger.Break();
//            _inner.Dispose();
//        }

//        public async Task<Message> Dequeue(TimeSpan suggestedWaitPeriod)
//        {
//            if (_enqueue)
//                Debugger.Break();
//            _dequeue = true;
//            var t = await _inner.Dequeue(suggestedWaitPeriod);
//            if (t != null)
//            {
//                _logger.Debug($"{_queueName}{GetHashCode()} DEQUEUE " + JsonConvert.SerializeObject(t.Body));
//                _gotData = true;
//            }

//            return t;
//        }

//        public async Task<DequeuedMessage> DequeueWithCredentials(TimeSpan suggestedWaitPeriod)
//        {
//            if (_enqueue)
//                Debugger.Break();
//            _dequeue = true;

//            var t = await _inner.DequeueWithCredentials(suggestedWaitPeriod);
//            if (t != null)
//            {
//                _logger.Debug($"{_queueName}{GetHashCode()} DEQUEUE " + JsonConvert.SerializeObject(t.Message.Body));
//                _gotData = true;

//            }
//            return t;
//        }

//        public async Task EnqueueAsync(ClaimsPrincipal principal, IReadOnlyCollection<Message> messages)
//        {
//            if (_dequeue)
//                Debugger.Break();
//            _enqueue = true;
//            foreach (var message in messages)
//            {
//                _logger.Debug($"{_queueName}{GetHashCode()} ENQUEUE " + JsonConvert.SerializeObject(message.Body));
//            }
//            _gotData = true;
//            await _inner.EnqueueAsync(principal, messages);
//        }

//        public async Task EnqueueAsync(IReadOnlyCollection<Message> messages)
//        {
//            if (!messages.Any())
//                Debugger.Break();
//            foreach (var message in messages)
//            {
//                _logger.Debug($"{_queueName}{GetHashCode()} ENQUEUE " + JsonConvert.SerializeObject(message.Body));
//            }
//            _gotData = true;
//            await _inner.EnqueueAsync(messages);
//        }

//        public async Task EnqueueAsync(ClaimsPrincipal principal, Message message)
//        {
//            _logger.Debug($"{_queueName}{GetHashCode()} ENQUEUE " + JsonConvert.SerializeObject(message.Body));
//            _gotData = true;
//            await _inner.EnqueueAsync(principal, message);
//        }

//        public async Task EnqueueAsync(Message message)
//        {
//            _logger.Debug($"{_queueName}{GetHashCode()} ENQUEUE " + JsonConvert.SerializeObject(message.Body));
//            _gotData = true;
//            await _inner.EnqueueAsync(message);
//        }

//        public Task SaveChanges()
//        {
//            if (_gotData)
//                _logger.Debug($"{_queueName}{GetHashCode()} SaveChanges ");
//            if (_saved)
//                Debugger.Break();
//            _saved = true;
//            return _inner.SaveChanges();
//        }
//    }

//    public class AdoNetMessageQueueProvider2 : IMessageQueueProvider
//    {
//        private readonly Func<IDbConnection> _connection;
//        private readonly IMessageSerializer<string> _serializer;

//        public AdoNetMessageQueueProvider2(Func<IDbConnection> connection, IMessageSerializer<string> serializer)
//        {
//            _connection = connection;
//            _serializer = serializer;
//            TableName = "MessageQueue";
//        }

//        /// <summary>
//        /// </summary>
//        public string TableName { get; set; }

//        public IMessageQueue Open(string queueName)
//        {
//            return new AdoNetMessageQueue2(queueName, _connection, _serializer)
//            {
//                TableName = "MessageQueue"
//            };
//        }
//    }
//    public class AdoNetMessageQueue2 : IMessageQueue
//    {
//        private readonly Func<IDbConnection> _connectionFactory;
//        private readonly IMessageSerializer<string> _messageSerializer;
//        private ILog _logger = LogManager.GetLogger(typeof(AdoNetMessageQueue2));

//        public AdoNetMessageQueue2(string queueName, Func<IDbConnection> connectionFactory,
//            IMessageSerializer<string> messageSerializer)
//        {
//            Name = queueName;
//            _connectionFactory = connectionFactory;
//            _messageSerializer = messageSerializer;
//            TableName = "MessageQueue";
//            IsolationLevel = IsolationLevel.ReadCommitted;
//        }

//        public IsolationLevel IsolationLevel { get; set; }

//        public string TableName { get; set; }

//        public string Name { get; }

//        public IMessageQueueSession BeginSession()
//        {
//            _logger.Debug("New session");
//            var connection = _connectionFactory();
//            if (IsolationLevel == IsolationLevel.Unspecified)
//                return new AdoNetMessageQueueSession2(TableName, Name, connection, _messageSerializer);

//            var transaction = connection.BeginTransaction(IsolationLevel);
//            return new AdoNetMessageQueueSession2(TableName, Name, transaction, _messageSerializer);
//        }
//    }

//    public class AdoNetMessageQueueSession2 : IMessageQueueSession
//    {
//        private ILog _log = LogManager.GetLogger(typeof(AdoNetMessageQueueSession2));
//        private readonly IMessageSerializer<string> _messageSerializer;
//        private readonly string _queueName;
//        private readonly string _tableName;
//        private IDbConnection _connection;
//        private bool _dequeued;
//        private IDbTransaction _transaction;

//        public AdoNetMessageQueueSession2(string tableName, string queueName, IDbConnection connection,
//            IMessageSerializer<string> messageSerializer)
//        {
//            _messageSerializer = messageSerializer;
//            _tableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
//            _queueName = queueName ?? throw new ArgumentNullException(nameof(queueName));
//            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
//            _transaction = connection.BeginTransaction();
//            _log.Debug($"{_queueName} Open1 " + _connection.GetHashCode());
//        }


//        public AdoNetMessageQueueSession2(string tableName, string queueName, IDbTransaction transaction,
//            IMessageSerializer<string> messageSerializer)
//        {
//            _messageSerializer = messageSerializer;
//            _tableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
//            _queueName = queueName ?? throw new ArgumentNullException(nameof(queueName));
//            _transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
//            _connection = transaction.Connection;
//            _log.Debug($"{_queueName} Open2 " + _connection.GetHashCode());
//        }

//        public LoggerHandler Logger;

//        public Task<Message> Dequeue(TimeSpan suggestedWaitTime)
//        {
//            EnsureNotDequeued();

//            var id = 0;
//            AdoNetMessageDto msg;
//            using (var cmd = _connection.CreateCommand())
//            {
//                cmd.Transaction = _transaction;
//                cmd.CommandText = $"SELECT TOP(1) Id, Body FROM {_tableName} WITH (ROWLOCK) WHERE QueueName = @name";
//                cmd.AddParameter("name", _queueName);
//                using (var reader = cmd.ExecuteReader())
//                {
//                    if (!reader.Read())
//                        return Task.FromResult<Message>(null);

//                    id = reader.GetInt32(0);
//                    var data = reader.GetString(1);
//                    _log.Debug($"{_queueName}{GetHashCode()}  Read MSGID " + id);
//                    msg = (AdoNetMessageDto)_messageSerializer.Deserialize("Message", data);
//                }
//            }
//            using (var cmd = _connection.CreateCommand())
//            {
//                cmd.Transaction = _transaction;
//                cmd.CommandText = $"DELETE FROM {_tableName} WHERE Id = @id";
//                cmd.AddParameter("id", id);
//                _log.Debug($"{_queueName}{GetHashCode()}  DELETE MSGID " + id);
//                cmd.ExecuteNonQuery();
//            }

//            msg.ToMessage(_messageSerializer, out var message, out var user);
//            msg.Properties["X-AdoNet-Id"] = id.ToString();
//            return Task.FromResult(message);
//        }

//        public async Task<DequeuedMessage> DequeueWithCredentials(TimeSpan suggestedWaitTime)
//        {
//            EnsureNotDequeued();

//            int id;
//            AdoNetMessageDto msg;
//            using (var cmd = _connection.CreateCommand())
//            {
//                cmd.Transaction = _transaction;
//                cmd.CommandText = $"SELECT TOP(1) Id, Body FROM {_tableName} WITH (updlock, readpast) WHERE QueueName = @name";
//                cmd.AddParameter("name", _queueName);
//                using (var reader = cmd.ExecuteReader())
//                {
//                    if (!reader.Read())
//                    {
//                        await Task.Delay(500);
//                        return null;
//                    }


//                    id = reader.GetInt32(0);
//                    var data = reader.GetString(1);

//                    _log.Debug($"{_queueName}{GetHashCode()}  Read MSGID " + id);
//                    msg = (AdoNetMessageDto)_messageSerializer.Deserialize("Message", data);
//                }
//            }
//            using (var cmd = _connection.CreateCommand())
//            {
//                _log.Debug($"{_queueName}{GetHashCode()}  DELETE MSGID " + id);
//                cmd.Transaction = _transaction;
//                cmd.CommandText = $"DELETE FROM {_tableName} WHERE Id = @id";
//                cmd.AddParameter("id", id);
//                cmd.ExecuteNonQuery();
//            }

//            msg.ToMessage(_messageSerializer, out var message, out var principal);
//            msg.Properties["X-AdoNet-Id"] = id.ToString();
//            return new DequeuedMessage(principal, message);
//        }

//        public Task EnqueueAsync(ClaimsPrincipal user, IReadOnlyCollection<Message> messages)
//        {
//            foreach (var msg in messages)
//            {
//                var dto = new AdoNetMessageDto(user, msg, _messageSerializer);
//                _messageSerializer.Serialize(dto, out var json, out var contentType);
//                InsertMessage(json, contentType);
//            }
//            return Task.FromResult<object>(null);
//        }

//        public Task EnqueueAsync(IReadOnlyCollection<Message> messages)
//        {
//            foreach (var msg in messages)
//            {
//                var dto = new AdoNetMessageDto(null, msg, _messageSerializer);
//                _messageSerializer.Serialize(dto, out var json, out var contentType);

//                InsertMessage(json, contentType);
//            }
//            return Task.FromResult<object>(null);
//        }

//        public Task EnqueueAsync(ClaimsPrincipal user, Message msg)
//        {
//            var dto = new AdoNetMessageDto(user, msg, _messageSerializer);
//            _messageSerializer.Serialize(dto, out var json, out var contentType);

//            InsertMessage(json, contentType);

//            return Task.FromResult<object>(null);
//        }

//        public Task EnqueueAsync(Message msg)
//        {
//            var dto = new AdoNetMessageDto(null, msg, _messageSerializer);
//            _messageSerializer.Serialize(dto, out var json, out var contentType);

//            InsertMessage(json, contentType);

//            return Task.FromResult<object>(null);
//        }

//        public Task SaveChanges()
//        {
//            _transaction.Commit();
//            return Task.FromResult<object>(null);
//        }

//        public void Dispose()
//        {
//            if (_connection != null)
//                _log.Debug($"{_queueName} Close " + _connection.GetHashCode());

//            _transaction?.Dispose();
//            _transaction = null;
//            _connection?.Dispose();
//            _connection = null;
//        }

//        private void EnsureNotDequeued()
//        {
//            if (_dequeued)
//                throw new NotSupportedException(
//                    "The ADO.NET queue do not support multiple dequeues in the same message scope. It's because we use 'SELECT TOP(1)' internally which means that the same message is returned every time until the internal transaction is comitted.");
//            _dequeued = true;
//        }

//        private void InsertMessage(string json, string typeName)
//        {
//            using (var cmd = _connection.CreateCommand())
//            {
//                cmd.Transaction = _transaction;
//                cmd.CommandText =
//                    $"INSERT INTO {_tableName} (CreatedAtUtc, QueueName, MessageType, Body) VALUES(GetUtcDate(),  @name, @MessageType, @Body);SELECT SCOPE_IDENTITY()";
//                cmd.AddParameter("name", _queueName);
//                cmd.AddParameter("Body", json);
//                cmd.AddParameter("MessageType", typeName);
//                var id = cmd.ExecuteScalar();
//                _log.Debug($"{_queueName}{GetHashCode()}  INSERT MSGID " + id);
//            }
//        }
//    }

//    /// <summary>
//    ///     Listens on inbound inboundQueue to be able to process messages.
//    /// </summary>
//    public class QueueListener2
//    {
//        private readonly IMessageRouter _outboundRouter;
//        private readonly IMessageQueue _queue;
//        private readonly IHandlerScopeFactory _scopeFactory;
//        private TimeSpan[] _retryAttempts;
//        static List<string> _items = new List<string>();

//        public LoggerHandler Logger;

//        /// <summary>
//        ///     Creates a new instance of <see cref="QueueListener" />.
//        /// </summary>
//        /// <param name="inboundQueue">Used to receive messages</param>
//        /// <param name="scopeFactory"></param>
//        public QueueListener2(IMessageQueue inboundQueue, IMessageQueue outboundQueue, IHandlerScopeFactory scopeFactory)
//        {
//            _outboundRouter = new SingleQueueRouter(outboundQueue);
//            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
//            _queue = inboundQueue ?? throw new ArgumentNullException(nameof(inboundQueue));
//        }

//        /// <summary>
//        ///     Creates a new instance of <see cref="QueueListener" />.
//        /// </summary>
//        /// <param name="inboundQueue">Used to receive messages</param>
//        /// <param name="outboundRouter">Routes messages through different queues</param>
//        /// <param name="scopeFactory"></param>
//        public QueueListener2(IMessageQueue inboundQueue, IMessageRouter outboundRouter,
//            IHandlerScopeFactory scopeFactory)
//        {
//            _outboundRouter = outboundRouter;

//            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
//            _queue = inboundQueue ?? throw new ArgumentNullException(nameof(inboundQueue));
//        }

//        /// <summary>
//        ///     Intervals at which retries should be made for messages that can not be handled.
//        /// </summary>
//        public TimeSpan[] RetryAttempts
//        {
//            get => _retryAttempts;
//            set => _retryAttempts = value ?? throw new ArgumentNullException(nameof(value));
//        }

//        /// <summary>
//        ///     Have attempted several times (<see cref="RetryAttempts" />) to handle a message and failed.
//        /// </summary>
//        public event EventHandler<PoisonMessageEventArgs> PoisonMessageDetected;

//        public Task ProcessMessageAsync(ClaimsPrincipal principal, Message message)
//        {
//            return ProcessMessageAsync(new DequeuedMessage(principal, message));
//        }

//        /// <summary>
//        /// </summary>
//        /// <returns></returns>
//        public async Task ReceiveSingleMessageAsync()
//        {
//            var wrapper = new MsgWrapper();
//            using (var session = _queue.BeginSession())
//            {
//                await ReceiveSingleMessageAsync(wrapper, session);
//                await session.SaveChanges();
//            }
//        }

//        public async Task RunAsync(CancellationToken token)
//        {
//            lock (_queue)
//            {
//                if (_items.Contains(_queue.Name))
//                    Debugger.Break();
//                _items.Add(_queue.Name);
//            }

            
//            var wrapper = new MsgWrapper();
//            while (!token.IsCancellationRequested)
//            {
//                using (var session = _queue.BeginSession())
//                {
//                    try
//                    {
//                        await ReceiveSingleMessageAsync(wrapper, session);
//                        await session.SaveChanges();
//                        if (wrapper.Message == null)
//                        {
//                            session.Dispose();
//                            await Task.Delay(100);
//                        }

//                    }
//                    catch (Exception ex)
//                    {
//                        Logger?.Invoke(LogLevel.Warning, _queue.Name,
//                            "Message handling failed, attempt: " + wrapper.AttemptCount + ", " + ex);

//                        // do not retry at all, consume invalid messages
//                        if (RetryAttempts.Length == 0)
//                        {
//                            PoisonMessageDetected?.Invoke(this,
//                                new PoisonMessageEventArgs(wrapper.Message.Principal, wrapper.Message.Message, ex));
//                            await session.SaveChanges();
//                            await Task.Delay(100, token);
//                        }
//                        else if (wrapper.AttemptCount < RetryAttempts.Length)
//                        {
//                            session.Dispose();
//                            await Task.Delay(RetryAttempts[wrapper.AttemptCount], token);
//                        }
//                        else
//                        {
//                            Logger?.Invoke(LogLevel.Error, _queue.Name, "Removing poison message.");

//                            PoisonMessageDetected?.Invoke(this,
//                                new PoisonMessageEventArgs(wrapper.Message.Principal, wrapper.Message.Message, ex));
//                            await session.SaveChanges();
//                        }
//                    }
//                }
//            }
//        }

//        /// <summary>
//        ///     Closing container scope.
//        /// </summary>
//        public event EventHandler<ScopeClosingEventArgs> ScopeClosing;

//        /// <summary>
//        ///     A new scope have been created for a message
//        /// </summary>
//        public event EventHandler<ScopeCreatedEventArgs> ScopeCreated;

//        private async Task ProcessMessageAsync(DequeuedMessage msg)
//        {
//            var outboundMessages = new List<Message>();
//            using (var scope = _scopeFactory.CreateScope())
//            {
//                Logger?.Invoke(LogLevel.Debug, _queue.Name, $"Created scope: {scope.GetHashCode()}");
//                var e = new ScopeCreatedEventArgs(scope, msg.Principal, msg.Message);
//                ScopeCreated?.Invoke(this, e);

//                var invoker = scope.ResolveDependency<IMessageInvoker>().First();
//                var context = new InvocationContext(_queue.Name, msg.Principal, invoker, outboundMessages);

//                Logger?.Invoke(LogLevel.Debug, _queue.Name, "Invoking message handler(s).");
//                await invoker.ProcessAsync(context, msg.Message);

//                ScopeClosing?.Invoke(this, new ScopeClosingEventArgs(scope, msg.Message, e.ApplicationState));
//                Logger?.Invoke(LogLevel.Debug, _queue.Name, $"Closing scope: {scope.GetHashCode()}");
//            }

//            if (msg.Principal == null)
//                await _outboundRouter.SendAsync(outboundMessages);
//            else
//                await _outboundRouter.SendAsync(msg.Principal, outboundMessages);
//        }

//        private async Task ReceiveSingleMessageAsync(MsgWrapper wrapper, IMessageQueueSession session)
//        {
//            var msg = await session.DequeueWithCredentials(TimeSpan.FromSeconds(1));
//            if (msg == null)
//            {
//                wrapper.Clear();
//                return;
//            }

//            if (msg.Principal != null)
//                Logger?.Invoke(LogLevel.Info, _queue.Name,
//                    $"Received[{msg.Principal.Identity.Name}]: {msg.Message.Body}");
//            else
//                Logger?.Invoke(LogLevel.Info, _queue.Name, $"Received[Anonymous]: {msg.Message.Body}");

//            wrapper.Assign(msg, _retryAttempts.Length);
//            await ProcessMessageAsync(msg);
//        }

//        private class MsgWrapper
//        {
//            private int _maxAttempts;
//            private DequeuedMessage _message;

//            /// <summary>
//            ///     0 when first attempt is made.
//            /// </summary>
//            public int AttemptCount { get; private set; }

//            public Guid Id => Message?.Message.MessageId ?? Guid.Empty;

//            public bool IsLastAttempt => AttemptCount >= _maxAttempts;

//            public DequeuedMessage Message
//            {
//                get { return _message; }
//            }

//            public void Assign(DequeuedMessage message, int maxAttempts)
//            {
//                if (Id == message.Message.MessageId)
//                {
//                    AttemptCount++;
//                }
//                else
//                {
//                    _maxAttempts = maxAttempts;
//                    _message = message;
//                    AttemptCount = 0;
//                }
//            }

//            public void Clear()
//            {
//                AttemptCount = 0;
//                _message = null;
//            }
//        }
//    }
//}

