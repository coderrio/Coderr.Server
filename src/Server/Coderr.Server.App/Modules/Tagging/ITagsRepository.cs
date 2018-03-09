using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using codeRR.Server.App.Modules.Tagging.Domain;

namespace codeRR.Server.App.Modules.Tagging
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
    }
}