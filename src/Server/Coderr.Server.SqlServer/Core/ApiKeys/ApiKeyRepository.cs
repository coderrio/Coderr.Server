using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using codeRR.Server.App.Core.ApiKeys;
using codeRR.Server.Infrastructure.Security;
using codeRR.Server.SqlServer.Core.ApiKeys.Mappings;
using Griffin.Container;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Core.ApiKeys
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
        ///     Delete all mappings that are for a specific application
        /// </summary>
        /// <param name="apiKeyId">id for the ApiKey that the application is associated with</param>
        /// <param name="applicationId">Application to remove mapping for</param>
        /// <returns></returns>
        public Task DeleteApplicationMappingAsync(int apiKeyId, int applicationId)
        {
            _uow.ExecuteNonQuery("DELETE FROM [ApiKeyApplications] WHERE ApiKeyId = @keyId AND ApplicationId = @appId",
                new {appId = applicationId, keyId = apiKeyId});
            return Task.FromResult<object>(null);
        }

        /// <summary>
        ///     Delete a specific ApiKey.
        /// </summary>
        /// <param name="keyId"></param>
        /// <returns></returns>
        public Task DeleteAsync(int keyId)
        {
            _uow.ExecuteNonQuery("DELETE FROM [ApiKeyApplications] WHERE ApiKeyId = @keyId", new {keyId});
            _uow.ExecuteNonQuery("DELETE FROM [ApiKeys] WHERE Id = @keyId", new {keyId});
            return Task.FromResult<object>(null);
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

            // apikeys without application restrictions should have access to all applications.
            if (apps.Count == 0)
            {
                sql = "SELECT [Id] FROM [Applications]";
                apps = await _uow.ToListAsync(new IntMapper(), sql);

            }

            foreach (var app in apps)
            {
                key.Add(app);
            }

            
            return key;
        }

        /// <summary>
        ///     Get all ApiKeys that maps to a specific application
        /// </summary>
        /// <param name="applicationId">application id</param>
        /// <returns>list</returns>
        public async Task<IEnumerable<ApiKey>> GetForApplicationAsync(int applicationId)
        {
            var apiKeyIds = new List<int>();
            using (var cmd = _uow.CreateDbCommand())
            {
                cmd.CommandText = "SELECT ApiKeyId FROM ApiKeyApplications WHERE ApplicationId = @id";
                cmd.AddParameter("id", applicationId);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        apiKeyIds.Add(reader.GetInt32(0));
                    }
                }
            }

            var keys = new List<ApiKey>();
            foreach (var id in apiKeyIds)
            {
                var key = await GetByKeyIdAsync(id);
                keys.Add(key);
            }
            return keys;
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
            foreach (var claim in key.Claims.Where(x => x.Type == CoderrClaims.Application))
            {
                AddApplication(key.Id, int.Parse(claim.Value));
            }
        }

        /// <summary>
        ///     Get an key by using the generated string.
        /// </summary>
        /// <param name="id">PK</param>
        /// <returns>key</returns>
        /// <exception cref="EntityNotFoundException">Given key was not found.</exception>
        public async Task<ApiKey> GetByKeyIdAsync(int id)
        {
            var key = await _uow.FirstAsync<ApiKey>("id=@1", id);
            var sql = "SELECT [ApplicationId] FROM [ApiKeyApplications] WHERE [ApiKeyId] = @1";
            var apps = await _uow.ToListAsync(new IntMapper(), sql, key.Id);
            foreach (var app in apps)
            {
                key.Add(app);
            }
            return key;
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

            var apps = key.Claims.Select(x => int.Parse(x.Value));
            var removed = existingMappings.Except(apps);
            foreach (var applicationId in removed)
            {
                _uow.Execute("DELETE FROM ApiKeyApplications WHERE ApiKeyId = @1 AND ApplicationId = @2",
                    new[] {key.Id, applicationId});
            }

            var added = apps.Except(existingMappings);
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