using System;
using System.Threading.Tasks;
using Coderr.Server.Api.Core.ApiKeys.Queries;
using Coderr.Server.App.Core.ApiKeys;
using Coderr.Server.PostgreSQL.Core.ApiKeys.Mappings;
using DotNetCqs;
using Coderr.Server.ReportAnalyzer.Abstractions;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.PostgreSQL.Core.ApiKeys.Queries
{
    /// <summary>
    ///     Handler for <see cref="GetApiKey" />.
    /// </summary>
    public class GetApiKeyHandler : IQueryHandler<GetApiKey, GetApiKeyResult>
    {
        private static readonly MirrorMapper<GetApiKeyResultApplication> _appMapping =
            new MirrorMapper<GetApiKeyResultApplication>();

        private readonly IAdoNetUnitOfWork _uow;

        /// <summary>
        ///     Creates a new instance of <see cref="GetApiKeyHandler" />.
        /// </summary>
        /// <param name="uow">valid uow</param>
        public GetApiKeyHandler(IAdoNetUnitOfWork uow)
        {
            if (uow == null) throw new ArgumentNullException(nameof(uow));

            _uow = uow;
        }

        /// <summary>Method used to execute the query</summary>
        /// <param name="query">Query to execute.</param>
        /// <returns>Task which will contain the result once completed.</returns>
        public async Task<GetApiKeyResult> HandleAsync(IMessageContext context, GetApiKey query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            ApiKey key;
            if (!string.IsNullOrEmpty(query.ApiKey))
                key = await _uow.FirstAsync<ApiKey>("GeneratedKey=@1", query.ApiKey);
            else
                key = await _uow.FirstAsync<ApiKey>("Id=@1", query.Id);

            var result = new GetApiKeyResult
            {
                ApplicationName = key.ApplicationName,
                CreatedAtUtc = key.CreatedAtUtc,
                CreatedById = key.CreatedById,
                GeneratedKey = key.GeneratedKey,
                Id = key.Id,
                SharedSecret = key.SharedSecret
            };

            var sql = @"SELECT Id as ApplicationId, Name as ApplicationName 
FROM Applications 
JOIN ApiKeyApplications ON (Id = ApplicationId)
WHERE ApiKeyId = @1;";
            var apps = await _uow.ToListAsync(_appMapping, sql, key.Id);
            result.AllowedApplications = apps.ToArray();
            return result;
        }
    }
}