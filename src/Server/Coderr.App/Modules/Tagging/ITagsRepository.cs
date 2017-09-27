using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using codeRR.App.Modules.Tagging.Domain;

namespace codeRR.App.Modules.Tagging
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
        ///     Get a list of tag
        /// </summary>
        /// <param name="incidentId">incident to get tags for</param>
        /// <returns>List of tags (or an empty list)</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        Task<IReadOnlyList<Tag>> GetTagsAsync(int incidentId);
    }
}