using System;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using OneTrueError.Api.Core;
using OneTrueError.Api.Core.Applications;
using OneTrueError.Api.Core.Applications.Queries;
using OneTrueError.App.Core.Incidents;

namespace OneTrueError.App.Core.Applications.QueryHandlers
{
    /// <summary>
    ///     Handler for <see cref="GetApplicationInfo" />.
    /// </summary>
    [Component]
    public class GetApplicationInfoHandler : IQueryHandler<GetApplicationInfo, GetApplicationInfoResult>
    {
        private readonly IIncidentRepository _incidentRepository;
        private readonly IApplicationRepository _repository;

        /// <summary>
        ///     Creates a new instance of <see cref="GetApplicationInfoHandler" />.
        /// </summary>
        /// <param name="repository">repos</param>
        /// <param name="incidentRepository">used to count the number of incidents</param>
        public GetApplicationInfoHandler(IApplicationRepository repository, IIncidentRepository incidentRepository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            if (incidentRepository == null) throw new ArgumentNullException("incidentRepository");
            _repository = repository;
            _incidentRepository = incidentRepository;
        }

        /// <summary>
        ///     Method used to execute the query
        /// </summary>
        /// <param name="query">Query to execute.</param>
        /// <returns>
        ///     Task which will contain the result once completed.
        /// </returns>
        public async Task<GetApplicationInfoResult> ExecuteAsync(GetApplicationInfo query)
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

            return new GetApplicationInfoResult
            {
                AppKey = app.AppKey,
                ApplicationType = app.ApplicationType.ConvertEnum<TypeOfApplication>(),
                Id = app.Id,
                Name = app.Name,
                SharedSecret = app.SharedSecret,
                TotalIncidentCount = totalCount
            };
        }
    }
}