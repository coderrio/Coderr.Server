using System.Threading.Tasks;
using Griffin.Data;

namespace OneTrueError.App.Core.ApiKeys
{
    /// <summary>
    ///     Repository for <see cref="ApiKey" />.
    /// </summary>
    public interface IApiKeyRepository
    {
        /// <summary>
        ///     Get an key by using the generated string.
        /// </summary>
        /// <param name="apiKey">key</param>
        /// <returns>key</returns>
        /// <exception cref="EntityNotFoundException">Given key was not found.</exception>
        Task<ApiKey> GetByKeyAsync(string apiKey);
    }
}