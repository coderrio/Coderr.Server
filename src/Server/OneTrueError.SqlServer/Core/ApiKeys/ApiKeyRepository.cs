using System;
using System.Linq;
using System.Threading.Tasks;
using Griffin.Container;
using Griffin.Data;
using Griffin.Data.Mapper;
using OneTrueError.App.Core.ApiKeys;
using OneTrueError.SqlServer.Core.ApiKeys.Mappings;

namespace OneTrueError.SqlServer.Core.ApiKeys
{
    /// <summary>
    ///     SQL Server implementation of <see cref="IApiKeyRepository" />.
    /// </summary>
    [Component]
    public class ApiKeyRepository : IApiKeyRepository
    {
        private readonly IAdoNetUnitOfWork _uow;

        /// <summary>
        ///     Creates a new instance of <see cref="ApiKeyRepository" />.
        /// </summary>
        /// <param name="uow">Active unit of work</param>
        public ApiKeyRepository(IAdoNetUnitOfWork uow)
        {
            if (uow == null) throw new ArgumentNullException(nameof(uow));

            _uow = uow;
        }

        /// <summary>
        ///     Get an key by using the generated string.
        /// </summary>
        /// <param name="apiKey">key</param>
        /// <returns>key</returns>
        /// <exception cref="EntityNotFoundException">Given key was not found.</exception>
        public async Task<ApiKey> GetByKeyAsync(string apiKey)
        {
            if (apiKey == null) throw new ArgumentNullException(nameof(apiKey));
            var key = await _uow.FirstAsync<ApiKey>("GeneratedKey=@1", apiKey);
            var sql = "SELECT [ApplicationId] FROM [ApiKeyApplications] WHERE [ApiKeyId] = @1";
            var apps = await _uow.ToListAsync(new IntMapper(), sql, key.Id);
            foreach (var app in apps)
            {
                key.Add(app);
            }
            return key;
        }

        /// <summary>
        ///     Create a new key
        /// </summary>
        /// <param name="key">key to create</param>
        /// <returns>task</returns>
        public async Task CreateAsync(ApiKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            await _uow.InsertAsync(key);
            foreach (var applicationId in key.AllowedApplications)
            {
                AddApplication(key.Id, applicationId);
            }
        }

        /// <summary>
        ///     Update an existing key
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>task</returns>
        public async Task UpdateAsync(ApiKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            await _uow.InsertAsync(key);

            var existingMappings =
                await _uow.ToListAsync<int>("SELECT ApplicationId FROM ApiKeyApplications WHERE ApiKeyId=@1",
                    key);

            var removed = existingMappings.Except(key.AllowedApplications);
            foreach (var applicationId in removed)
            {
                _uow.Execute("DELETE FROM ApiKeyApplications WHERE ApiKeyId = @1 AND ApplicationId = @2",
                    new[] { key.Id, applicationId });
            }

            var added = key.AllowedApplications.Except(existingMappings);
            foreach (var id in added)
            {
                AddApplication(key.Id, id);
            }
        }

        private void AddApplication(int apiKeyId, int applicationId)
        {
            _uow.Execute("INSERT INTO [ApiKeyApplications] (ApiKeyId, ApplicationId) VALUES(@api, @app)", new
            {
                api = apiKeyId,
                app = applicationId
            });
        }

    }
}