using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions;
using Coderr.Server.Infrastructure.Messaging;
using Coderr.Server.ReportAnalyzer.Abstractions.Incidents;
using DotNetCqs;
using DotNetCqs.Queues;

namespace Coderr.Server.ReportAnalyzer.Partitions.Events
{
    public class FindPartitionsInNewReports : IMessageHandler<ReportAddedToIncident>
    {
        private readonly IMessageQueue _queue;

        public FindPartitionsInNewReports()
        {
            _queue = QueueManager.Instance.GetQueue(ServerConfig.Instance.Queues.InboundPartitions);
        }

        public async Task HandleAsync(IMessageContext context, ReportAddedToIncident message)
        {
            var collection =
                message.Report.ContextCollections.FirstOrDefault(x =>
                    x.Name == InboundDTO.ContextCollectionName);
            var partitionKeys = collection != null
                ? new Dictionary<string, string>(collection.Properties, StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            string userValue = null;
            foreach (var col in message.Report.ContextCollections)
            {
                foreach (var prop in col.Properties)
                {
                    // Misconfigured client library.
                    if (string.IsNullOrEmpty(prop.Value))
                        continue;

                    var value = prop.Value.Length > 40
                        ? prop.Value.Substring(0, 40)
                        : prop.Value;

                    switch (prop.Key)
                    {
                        case "InstallationId":
                            partitionKeys[prop.Key] = value;
                            break;
                        case "UserToken":
                        case "UserId":
                        case "UserName":
                            userValue = value;
                            break;
                    }

                    if (col.Name == "ErrPartitions")
                        partitionKeys[prop.Key] = value;
                    else if (prop.Key.StartsWith(InboundDTO.ContextCollectionPropertyPrefix))
                    {
                        //+1 to remove the dot/underscore.
                        var key = prop.Key.Remove(0,
                            InboundDTO.ContextCollectionPropertyPrefix.Length + 1);
                        partitionKeys[key] = value;
                    }
                }
            }

            if (userValue != null
                    && !partitionKeys.ContainsKey("UserName")
                    && !partitionKeys.ContainsKey("UserId")
                    && !partitionKeys.ContainsKey("UserToken"))
            {
                partitionKeys["User"] = userValue;
            }

            if (partitionKeys.Count == 0)
                return;

            using (var session = _queue.BeginSession())
            {
                var messages = new List<Message>();
                foreach (var partitionKey in partitionKeys)
                {
                    var dto = new InboundDTO
                    {
                        IncidentId = message.Incident.Id,
                        Value = partitionKey.Value,
                        PartitionKey = partitionKey.Key,
                        ReceivedAtUtc = message.Report.CreatedAtUtc
                    };
                    var msg = new Message(dto);
                    messages.Add(msg);
                }

                await session.EnqueueAsync(context.Principal, messages);
            }
        }
    }
}