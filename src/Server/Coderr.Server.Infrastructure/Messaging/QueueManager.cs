using System;
using System.Data;
using System.Security.Claims;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.Security;
using Coderr.Server.Infrastructure.Messaging.Disk.DotNetCqs;
using Coderr.Server.Infrastructure.Messaging.Tests;
using DotNetCqs.Queues;
using DotNetCqs.Queues.AdoNet;

namespace Coderr.Server.Infrastructure.Messaging
{
    public class QueueManager : IDisposable
    {
        public static readonly TestQueueProvider TestProvider = new TestQueueProvider();

        public static QueueManager Instance { get; private set; }

        public IMessageQueueProvider QueueProvider { get; private set; }

        public bool UseDiskQueues { get; private set; }

        public void Dispose()
        {
            if (QueueProvider is IDisposable d)
                d.Dispose();
        }

        public static void UseTestProvider()
        {
            Instance = new QueueManager {QueueProvider = TestProvider};
        }

        public void Configure(IConfiguration configuration, Func<ClaimsPrincipal, IDbConnection> connectionFactory)
        {
            Instance = this;
            UseDiskQueues = configuration.GetSection("Queues")["Type"] == "Disk";

            if (UseDiskQueues)
            {
                var f1 = AppDomain.CurrentDomain.GetData("DataDirectory");
                var folder = configuration.GetSection("Queues")["Folder"];
                var diskProvider = new DiskQueueProvider(folder);
                diskProvider.ShutdownRequested = OnInnerShutdownRequested;
                QueueProvider = diskProvider;
            }
            else
            {
                IDbConnection Factory()
                {
                    return connectionFactory(CoderrClaims.SystemPrincipal);
                }

                var serializer = new MessagingSerializer(typeof(AdoNetMessageDto));
                QueueProvider = new AdoNetMessageQueueProvider(Factory, serializer);
            }
        }

        public IMessageQueue GetQueue(string queueName)
        {
            if (QueueProvider == null)
                throw new InvalidOperationException("Must configure first.");

            return QueueProvider.Open(queueName);
        }

        public void SetCustomProvider(IMessageQueueProvider provider)
        {
            QueueProvider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        /// <summary>
        /// </summary>
        public event EventHandler<ShutdownRequestCompletedEventArgs> ShutdownRequestCompleted;

        /// <summary>
        ///     We have been requested to shut down.
        /// </summary>
        public event EventHandler<ShuttingDownEventArgs> ShutdownRequested;

        private Task<bool> OnInnerShutdownRequested()
        {
            var e = new ShuttingDownEventArgs();
            ShutdownRequested?.Invoke(this, e);

            var e2 = new ShutdownRequestCompletedEventArgs(e.CanShutdown);
            ShutdownRequestCompleted?.Invoke(this, e2);

            if (e.CanShutdown) Dispose();

            return Task.FromResult(e.CanShutdown);
        }
    }
}