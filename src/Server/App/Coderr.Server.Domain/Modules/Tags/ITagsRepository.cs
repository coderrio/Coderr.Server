using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Coderr.Server.Domain.Modules.Tags
{
    /// <summary>
    ///     Repository for tags
    /// </summary>
    public interface ITagsRepository
    {
        /// <summary>
        ///     Add a new tag
        /// </summary>
        /// <param name="incidentId">incident that the tag is for</param>
        /// <param name="tags">tag collection</param>
        /// <returns>task</returns>
        Task AddAsync(int incidentId, Tag[] tags);

        Task UpdateTags(int incidentId, string[] tagsToAdd, string[] tagsToRemove);

        Task AddTag(int incidentId, string tag);

        /// <summary>
        ///     Get a list of tags for an application
        /// </summary>
        /// <param name="applicationId">application to get tags for</param>
        /// <returns>List of tags (or an empty list)</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        Task<IReadOnlyList<Tag>> GetApplicationTagsAsync(int applicationId);

        /// <summary>
        ///     Get a list of tags for a specific incident
        /// </summary>
        /// <param name="incidentId">incident to get tags for</param>
        /// <returns>List of tags (or an empty list)</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        Task<IReadOnlyList<Tag>> GetIncidentTagsAsync(int incidentId);

        /// <summary>
        ///     Get a list of tags for an application
        /// </summary>
        /// <param name="applicationId">application to get tags for</param>
        /// <param name="incidentId">incident to get tags for</param>
        /// <returns>List of tags (or an empty list)</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        Task<IReadOnlyList<Tag>> GetTagsAsync(int? applicationId, int? incidentId);

        Task<IReadOnlyList<int>> GetNewIncidentsForTag(int? applicationId, string tag);
    }
}