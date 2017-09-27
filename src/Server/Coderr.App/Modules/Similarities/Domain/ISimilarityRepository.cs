using System.Threading.Tasks;

namespace codeRR.Server.App.Modules.Similarities.Domain
{
    /// <summary>
    ///     Store and fetch all similarities in the database
    /// </summary>
    public interface ISimilarityRepository
    {
        /// <summary>
        ///     Create a similarity report (one time per incident)
        /// </summary>
        /// <param name="similarity">report</param>
        /// <returns>task</returns>
        Task CreateAsync(SimilaritiesReport similarity);

        /// <summary>
        /// </summary>
        /// <param name="incidentId"></param>
        /// <returns>Report if found; otherwise <c>null</c></returns>
        SimilaritiesReport FindForIncident(int incidentId);

        /// <summary>
        ///     Update existing report
        /// </summary>
        /// <param name="similarity">similarity report</param>
        /// <returns>task</returns>
        Task UpdateAsync(SimilaritiesReport similarity);
    }
}