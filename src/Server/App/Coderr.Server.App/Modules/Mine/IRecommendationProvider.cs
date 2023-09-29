using System.Threading.Tasks;

namespace Coderr.Server.App.Modules.Mine
{
    /// <summary>
    ///     A provider that will suggest incidents
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A provider can recommend several incident. If they do, it's recommended that each incident get a different
    ///         score.
    ///         If all providers are equally worth, they can give a score of 100 to the most recommended incident. However,
    ///         some providers affect the business more (like partitions) and could therefore specify a score multiplier for
    ///         all suggested incidents.
    ///     </para>
    ///     <para>
    ///         The provider will be executed in a container scope.
    ///     </para>
    /// </remarks>
    public interface IRecommendationProvider
    {
        /// <summary>
        ///     Suggest an incident.
        /// </summary>
        /// <param name="context">Context information</param>
        /// <returns>task</returns>
        Task Recommend(RecommendIncidentContext context);
    }
}