using System;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Api;
using Coderr.Server.Api.Core.Applications.Queries;
using Coderr.Server.App.Modules.Versions;
using Coderr.Server.Domain.Core.Applications;
using Coderr.Server.Domain.Core.Incidents;
using Coderr.Server.Domain.Modules.ApplicationVersions;
using DotNetCqs;
using TypeOfApplication = Coderr.Server.Api.Core.Applications.TypeOfApplication;

namespace Coderr.Server.App.Core.Applications.QueryHandlers
{
    /// <summary>
    ///     Handler for <see cref="GetApplicationInfo" />.
    /// </summary>
    public class GetApplicationInfoHandler : IQueryHandler<GetApplicationInfo, GetApplicationInfoResult>
    {
        private readonly IIncidentRepository _incidentRepository;
        private readonly IApplicationRepository _repository;
        private IApplicationVersionRepository _versionRepository;

        /// <summary>
        ///     Creates a new instance of <see cref="GetApplicationInfoHandler" />.
        /// </summary>
        /// <param name="repository">repos</param>
        /// <param name="incidentRepository">used to count the number of incidents</param>
        /// <param name="versionRepository">to fetch versions</param>
        public GetApplicationInfoHandler(IApplicationRepository repository, IIncidentRepository incidentRepository, IApplicationVersionRepository versionRepository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _incidentRepository = incidentRepository ?? throw new ArgumentNullException(nameof(incidentRepository));
            _versionRepository = versionRepository ?? throw new ArgumentNullException(nameof(versionRepository));
        }

        /// <summary>
        ///     Method used to execute the query
        /// </summary>
        /// <param name="query">Query to execute.</param>
        /// <returns>
        ///     Task which will contain the result once completed.
        /// </returns>
        public async Task<GetApplicationInfoResult> HandleAsync(IMessageContext context, GetApplicationInfo query)
        {
            Application app;
            if (!string.IsNullOrEmpty(query.AppKey))
            {
                app = await _repository.GetByKeyAsync(query.AppKey);
            }
            else
            {
                app = await _repository.GetByIdAsync(query.ApplicationId);
            }

            var totalCount = await _incidentRepository.GetTotalCountForAppInfoAsync(app.Id);
            var versions = await _versionRepository.FindVersionsAsync(app.Id);
            return new GetApplicationInfoResult
            {
                AppKey = app.AppKey,
                ApplicationType = app.ApplicationType.ConvertEnum<TypeOfApplication>(),
                Id = app.Id,
                Name = app.Name,
                SharedSecret = app.SharedSecret,
                TotalIncidentCount = totalCount,
                Versions = versions.ToArray(),
                ShowStatsQuestion = !app.MuteStatisticsQuestion,
                NumberOfDevelopers = app.NumberOfFtes
            };
        }
    }
}