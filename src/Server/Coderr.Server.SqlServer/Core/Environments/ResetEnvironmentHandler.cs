using System.Threading.Tasks;
using Coderr.Server.Api.Core.Environments.Commands;
using DotNetCqs;
using Griffin.Data;
using log4net;

namespace Coderr.Server.SqlServer.Core.Environments
{
    internal class ResetEnvironmentHandler : IMessageHandler<ResetEnvironment>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;
        private ILog _loggr = LogManager.GetLogger(typeof(ResetEnvironmentHandler));

        public ResetEnvironmentHandler(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task HandleAsync(IMessageContext context, ResetEnvironment message)
        {
            _loggr.Info("Resetting environmentId " + message.EnvironmentId + " for app " + message.ApplicationId);

            // Start by deleting incidents that are in just our environment.
            var sql = @"WITH JustOurIncidents (IncidentId) AS
                            (
	                            select ie.IncidentId
	                            from IncidentEnvironments ie 
	                            join Incidents i ON (i.Id = ie.IncidentId)
	                            join Environments e ON (ie.EnvironmentId = e.Id)
	                            where i.ApplicationId = @applicationId AND i.State = 0
	                            group by ie.IncidentId
	                            having count(e.Id) = 1
                            )
                            DELETE Incidents
                            FROM IncidentEnvironments
                            JOIN JustOurIncidents ON (JustOurIncidents.IncidentId = IncidentEnvironments.IncidentId)
                            WHERE IncidentEnvironments.EnvironmentId = @environmentId";

            _unitOfWork.ExecuteNonQuery(sql, new { message.ApplicationId, message.EnvironmentId });

            // Next delete all environment mappings that are for the given environment.
            sql = @"WITH JustOurIncidents (IncidentId) AS
                            (
	                            select ie.IncidentId
	                            from IncidentEnvironments ie 
	                            join Incidents i ON (i.Id = ie.IncidentId)
	                            join Environments e ON (ie.EnvironmentId = e.Id)
	                            where i.ApplicationId = @applicationId AND i.State = 0
	                            group by ie.IncidentId
                            )
                            DELETE IncidentEnvironments
                            FROM IncidentEnvironments
                            JOIN JustOurIncidents ON (JustOurIncidents.IncidentId = IncidentEnvironments.IncidentId)
                            WHERE IncidentEnvironments.EnvironmentId = @environmentId";

            _unitOfWork.ExecuteNonQuery(sql, new { message.ApplicationId, message.EnvironmentId });
            _loggr.Info("Resetting environmentId " + message.EnvironmentId + " for app " + message.ApplicationId);
            return Task.CompletedTask;
        }
    }
}