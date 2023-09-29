using System;
using System.Threading.Tasks;
using Coderr.Server.Api.Core.Incidents.Events;
using Coderr.Server.Domain.Core.Incidents.Events;
using Coderr.Server.Domain.Modules.Statistics;
using Coderr.Server.ReportAnalyzer.Abstractions.Incidents;
using DotNetCqs;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.ReportAnalyzer.InsidentInsights
{
    internal class AggregateIncidentEventsSubscriber
            : IMessageHandler<ReportAddedToIncident>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public AggregateIncidentEventsSubscriber(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task HandleAsync(IMessageContext context, IncidentAssigned message)
        {
            var entity =
                await _unitOfWork.FirstOrDefaultAsync<IncidentProgressTracker>("IncidentId = @incidentId", new { incidentId = message.IncidentId });
            if (entity == null) return;

            // är en bugg ngnstans där assignedAt kommer med som MinValue
            entity.AssignedAtUtc = message.AssignedAtUtc != DateTime.MinValue
                ? message.AssignedAtUtc
                : DateTime.UtcNow;
            entity.AssignedToId = message.AssignedToId;
            await _unitOfWork.UpdateAsync(entity);
        }

        public async Task HandleAsync(IMessageContext context, IncidentClosed message)
        {
            var entity =
                await _unitOfWork.FirstOrDefaultAsync<IncidentProgressTracker>("IncidentId = @incidentId", new { incidentId = message.IncidentId });
            if (entity == null) return;

            entity.ClosedAtUtc = message.ClosedAtUtc == DateTime.MinValue
                ? DateTime.UtcNow
                : message.ClosedAtUtc;
            entity.ClosedById = message.ClosedById;
            await _unitOfWork.UpdateAsync(entity);
        }

        public async Task HandleAsync(IMessageContext context, IncidentCreated message)
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText =
                    @"BEGIN TRY
                            INSERT INTO CommonIncidentProgressTracking (IncidentId, ApplicationId, CreatedAtUtc, Versions, VersionCount)
                            VALUES (@IncidentId, @applicationId, @CreatedAtUtc, @versions, 1)
                        END TRY
                        BEGIN CATCH
                            IF ERROR_NUMBER() <> 2627
                              THROW
                        END CATCH";
                cmd.AddParameter("CreatedAtUtc", message.CreatedAtUtc);
                cmd.AddParameter("IncidentId", message.IncidentId);
                cmd.AddParameter("ApplicationId", message.ApplicationId);
                cmd.AddParameter("Versions", $";{message.ApplicationVersion};");
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task HandleAsync(IMessageContext context, IncidentReOpened message)
        {
            var entity =
                await _unitOfWork.FirstOrDefaultAsync<IncidentProgressTracker>("IncidentId = @incidentId", new { incidentId = message.IncidentId });
            if (entity == null) return;

            entity.ReOpenedAtUtc = message.CreatedAtUtc.Date;
            entity.ReOpenCount += 1;
            await _unitOfWork.UpdateAsync(entity);
        }

        public async Task HandleAsync(IMessageContext context, ReportAddedToIncident message)
        {
            var entity =
                await _unitOfWork.FirstOrDefaultAsync<IncidentProgressTracker>("IncidentId = @incidentId", new { incidentId = message.Incident.Id });
            if (entity == null) return;

            if (entity.Versions?.Contains(message.Report.ApplicationVersion) == true)
                return;

            if (string.IsNullOrEmpty(entity.Versions))
                entity.Versions = ";";
            entity.Versions += $";{message.Report.ApplicationVersion};";
            entity.VersionCount++;

            await _unitOfWork.UpdateAsync(entity);
        }

        public async Task HandleAsync(IMessageContext context, IncidentDeleted message)
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText =
                    @"DELETE FROM CommonIncidentProgressTracking WHERE IncidentId = @id";
                cmd.AddParameter("id", message.IncidentId);
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}