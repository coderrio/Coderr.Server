using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coderr.Server.Domain.Modules.Partitions
{
    public interface IPartitionRepository
    {
        Task CreateAsync(IncidentPartitionValue incidentPartitionValues);

        Task CreateAsync(PartitionDefinition definition);
        Task CreateAsync(ApplicationPartitionValue applicationValues);

        Task<IList<ApplicationPartitionValue>> FindForApplication(int applicationId);

        Task<IList<PartitionDefinition>> GetDefinitions(int applicationId);

        Task<IList<PartitionDefinition>> GetDefinitionsToTag();

        Task<IReadOnlyList<IncidentToTag>> GetIncidentsToTag(PartitionDefinition definition);

        /// <summary>
        ///     Get a summary for each partition that we've received for the specified incident.
        /// </summary>
        /// <param name="applicationId">Application that the incident belongs to</param>
        /// <param name="incidentId">Incident to get information for</param>
        /// <returns></returns>
        Task<IList<IncidentPartitionSummary>> GetIncidentSummary(int applicationId, int incidentId);

        Task<IEnumerable<IncidentPartitionSummary>> GetPrioritized(int[] incidentIds = null);
        Task<IEnumerable<IncidentPartitionSummary>> GetPrioritized(int applicationId);

        Task UpdateAsync(IncidentPartitionValue incidentPartitionValues);

        Task UpdateAsync(PartitionDefinition definition);
        Task UpdateAsync(ApplicationPartitionValue applicationValues);
        Task<PartitionDefinition> GetByIdAsync(int id);
        Task DeleteDefinitionByIdAsync(int partitionId);
    }
}