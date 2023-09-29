using System;
using System.Data;
using System.Threading.Tasks;
using Coderr.Client;
using Coderr.Server.Abstractions;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Infrastructure.Messaging;
using DotNetCqs.Queues;
using Griffin.ApplicationServices;
using Griffin.Data;
using Griffin.Data.Mapper;
using log4net;

namespace Coderr.Server.ReportAnalyzer.Partitions.Jobs
{
    /// <summary>
    ///     Take inbound values and add them if they are unique. Otherwise just toss the values away.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         To improve performance we insert all inbound values directly to a table without any queries or checks. Then we
    ///         use this job
    ///         to find new unique values which can be inserted.
    ///     </para>
    ///     <para>
    ///         A new instance are created of this class on every execution, hence we can safely use member fields.
    ///     </para>
    /// </remarks>
    [ContainerService(RegisterAsSelf = true)]
    public class ProcessInboundPartitionValuesJob : IBackgroundJobAsync
    {
        private static bool _running;
        private readonly IConnectionFactory _connectionFactory;
        private readonly IMessageQueue _queue;
        private readonly ILog _logger = LogManager.GetLogger(typeof(ProcessInboundPartitionValuesJob));

        public ProcessInboundPartitionValuesJob(IConnectionFactory connectionFactory)
        {
            _queue = QueueManager.Instance.GetQueue(ServerConfig.Instance.Queues.InboundPartitions);
            _connectionFactory = connectionFactory;
        }

        public async Task ExecuteAsync()
        {
            if (_running) return;

            _running = true;
            try
            {
                await ProcessValues();
            }
            catch (Exception ex)
            {
                Err.Report(ex);
                _logger.Error("Failed to process values", ex);
            }

            _running = false;
        }

        private async Task ProcessValues()
        {
            IDbConnection connection = null;
            var reportsLeft = 1000;
            while (reportsLeft-- > 0)
            {
                var lastOrgId = -1;
                using (var session = _queue.BeginSession())
                {
                    var entry = await session.DequeueWithCredentials(TimeSpan.FromSeconds(1));
                    if (entry == null) break;

                    var organizationIdStr = entry.Principal
                        ?.FindFirst(x => x.Type == "http://coderr/claims/organization/id")?.Value;
                    if (organizationIdStr != null)
                    {
                        var orgId = int.Parse(organizationIdStr);
                        if (orgId != lastOrgId)
                        {
                            connection?.Close();
                            connection = null;
                        }
                    }

                    if (connection == null)
                    {
                        connection = _connectionFactory.OpenConnection(entry.Principal);
                    }

                    var dto = (InboundDTO)entry.Message.Body;
                    using (var cmd = connection.CreateDbCommand())
                    {
                        cmd.CommandText =
                            @"declare @partitionId int;
    declare @valueId int;
    declare @appId int;
    declare @insightId int;

    select @appId = applicationId FROM Incidents WHERE id = @incidentId;
	if @appId IS NOT NULL -- NULL if incident have been deleted.
	begin

        set @partitionId = null;
	    SELECT @partitionId = Id FROM PartitionDefinitions WITH (ReadUncommitted) WHERE PartitionKey = @key AND applicationId=@appId;
	    IF @partitionId IS NULL
	    BEGIN
		      declare @name varchar(40);
		      set @name = CASE @key
			      WHEN 'UserId' THEN 'User'
			      WHEN 'UserToken' THEN 'User'
			      WHEN 'UserName' THEN 'User'
			      WHEn 'ApplicationId' THEN 'Application'
			      ELSE @key
		      END
		      INSERT PartitionDefinitions ( ApplicationId, Name, PartitionKey, Weight)
		      VALUES ( @appId, @key, @key, 2);
		      SELECT @partitionId = SCOPE_IDENTITY();
	    END

        set @valueId = null;
	    SELECT @valueId = Id FROM ApplicationPartitionValues WITH (ReadUncommitted) WHERE PartitionId = @partitionId AND [value]=@value
	    IF @valueId IS NULL
	    BEGIN
		      INSERT ApplicationPartitionValues ( PartitionId, Value ) VALUES ( @partitionId, @value);
		      SELECT @valueId = SCOPE_IDENTITY();
		      INSERT IncidentPartitionValues ( IncidentId, PartitionId, ReceivedAtUtc, ValueId ) VALUES ( @incidentId, @partitionId, GetUtcDate(), @valueId);
	    END --if valueId
	    ELSE
	    BEGIN
            declare @incidentPartitionId int;
            SELECT @incidentPartitionId=Id FROM IncidentPartitionValues WITH (ReadUncommitted) WHERE IncidentId = @incidentId AND PartitionId = @partitionId AND ValueId = @valueId
		    IF @incidentPartitionId IS NULL 
		    BEGIN
		      INSERT IncidentPartitionValues ( IncidentId, PartitionId, ReceivedAtUtc, ValueId) VALUES ( @incidentId, @partitionId,GetUtcDate(), @valueId);
            END
            ELSE
            BEGIN
                UPDATE IncidentPartitionValues SET ReceivedAtUtc=GetUtcDate() WHERE Id = @incidentPartitionId;
		    END
	    END --else IF @valueId IS NULL


        set @insightId = null;
	    SELECT @insightId = Id FROM ApplicationPartitionInsights WITH (ReadUncommitted) WHERE PartitionId = @partitionId AND [value]=@value AND YearMonth = @receivedMonth;
	    IF @insightId IS NULL
	    BEGIN
		      INSERT ApplicationPartitionInsights ( PartitionId, Value, YearMonth) VALUES ( @partitionId, @value, @receivedMonth);
	    END --if insightId
    END --if appId
";
                        cmd.AddParameter("key", dto.PartitionKey);
                        cmd.AddParameter("value", dto.Value);
                        cmd.AddParameter("incidentId", dto.IncidentId);
                        cmd.AddParameter("receivedMonth", dto.ReceivedAtUtc.ToString("yyyy-MM-01"));
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }

            connection?.Close();
        }
    }
}