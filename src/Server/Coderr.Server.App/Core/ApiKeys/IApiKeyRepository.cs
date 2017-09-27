using System.Collections.Generic;
using System.Threading.Tasks;
using Griffin.Data;

namespace codeRR.Server.App.Core.ApiKeys
{
    /// <summary>
    ///     Repository for <see cref="ApiKey" />.
    /// </summary>
    public interface IApiKeyRepository
    {
        /// <summary>
        ///     Delete all mappings that are for a specific application
        /// </summary>
        /// <param name="apiKeyId">id for the ApiKey that the application is associated with</param>
        /// <param name="applicationId">Application to remove mapping for</param>
        /// <returns></returns>
        Task DeleteApplicationMappingAsync(int apiKeyId, int applicationId);

        /// <summary>
        ///     Delete a specific ApiKey.
        /// </summary>
        /// <param name="keyId"></param>
        /// <returns></returns>
        Task DeleteAsync(int keyId);

        /// <summary>
        ///     Get an key by using the generated string.
        /// </summary>
        /// <param name="apiKey">key</param>
        /// <returns>key</returns>
        /// <exception cref="EntityNotFoundException">Given key was not found.</exception>
        Task<ApiKey> GetByKeyAsync(string apiKey);

        /// <summary>
        ///     Get all ApiKeys that maps to a specific application
        /// </summary>
        /// <param name="applicationId">application id</param>
        /// <returns>list</returns>
        Task<IEnumerable<ApiKey>> GetForApplicationAsync(int applicationId);
    }
}