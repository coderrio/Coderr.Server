using System;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Abstractions.WorkItems;
using Coderr.Server.App.Partitions;
using Coderr.Server.Domain.Core.Incidents;
using Coderr.Server.Domain.Modules.ApplicationVersions;
using Coderr.Server.Domain.Modules.Partitions;
using Coderr.Server.Domain.Modules.Tags;
using Coderr.Server.Infrastructure.Configuration;

namespace Coderr.Server.App.WorkItems
{
    [ContainerService(IsTransient = true)]
    internal class WorkItemServiceProvider : IWorkItemServiceProvider
    {
        private readonly IApplicationVersionRepository _applicationVersionRepository;
        private readonly IIncidentRepository _incidentRepository;
        private readonly IWorkItemRepository _workItemRepository;
        private readonly IServiceProvider _scope;
        private ITagsRepository _tagsRepository;
        private IPartitionRepository _partitionRepository;
        private BaseConfiguration _baseConfig;
        private IWorkItemServiceRegistrar _registrar;

        public WorkItemServiceProvider(
            IIncidentRepository incidentRepository,
            IApplicationVersionRepository applicationVersionRepository,
            IWorkItemRepository workItemRepository,
            IServiceProvider scope,
            IConfiguration<BaseConfiguration> config, 
            IPartitionRepository partitionRepository, 
            ITagsRepository tagsRepository, 
            IWorkItemServiceRegistrar registrar)
        {
            _incidentRepository = incidentRepository;
            _applicationVersionRepository = applicationVersionRepository;
            _workItemRepository = workItemRepository;
            _scope = scope;
            _partitionRepository = partitionRepository;
            _tagsRepository = tagsRepository;
            _registrar = registrar;
            _baseConfig = config.Value;
        }


        public async Task CreateAsync(int applicationId, int incidentId)
        {
            // validate that one have not been created
            if (await _workItemRepository.Find(incidentId) != null)
            {
                return;
            }
            
            var version =
                (await _applicationVersionRepository.FindForIncidentAsync(incidentId))?.FirstOrDefault()?.Version ??
                "Not specified";
            var incident = await _incidentRepository.GetAsync(incidentId);

            var summary = await _partitionRepository.GetIncidentSummary(applicationId, incidentId);
            var impact = "";
            foreach (var partition in summary)
            {
                impact +=
                    $"Affected {partition.PartitionTitle.Pluralize()}: {partition.ValueCount} ({partition.ValueCount * 100 / partition.TotalCount}%)<br>\r\n";
            }

            var tags = await _tagsRepository.GetIncidentTagsAsync(incidentId);

            var dto = new WorkItemDTO(applicationId, incidentId)
            {
                Title = Shorten(incident.Description),
                ReproduceSteps =
                    incident.Description + "\r\n" +
                    incident.FullName + "\r\n" +
                    incident.StackTrace,
                State = WorkItemState.New,
                IntegrationName = "AzureDevOps",
                SystemInformation =
                    $"Version: {version}<br>\r\n{impact}<br>For more information, view the bug in <a href=\"{_baseConfig.BaseUrl}go/incident/{incidentId}\">Coderr</a>.",
                Tags = tags.Select(x => x.Name).ToArray()
            };

            var service = await FindService(applicationId);
            await service.Create(dto);

            if (incident.State == IncidentState.Active)
            {
                await service.Assign(applicationId, dto.WorkItemId, incident.AssignedToId.Value);
            }

            var mapping = new WorkItemMapping(applicationId, incidentId, service.Name)
            {
                Name = dto.Title,
                UiUrl = dto.UiUrl,
                ApiUrl = dto.ApiUrl,
                WorkItemId = dto.WorkItemId,
                LastSynchronizationAtUtc = DateTime.UtcNow
            };
            await _workItemRepository.Create(mapping);
        }

        private string Shorten(string text)
        {
            if (text.Length < 40)
                return text;
            return text.Substring(0, 35) + "[..]";
        }

        public async Task MapApplication(int applicationId, string integrationName)
        {
            if (integrationName == null) throw new ArgumentNullException(nameof(integrationName));
            if (applicationId <= 0) throw new ArgumentOutOfRangeException(nameof(applicationId));



            await _workItemRepository.MapApplication(applicationId, integrationName);
        }

        public async Task<IWorkItemService> FindService(int applicationId)
        {
            var name = await _workItemRepository.GetIntegrationName(applicationId);
            if (name == null)
                return null;

            var type = _registrar.GetServiceType(name);
            return (IWorkItemService)_scope.GetService(type);
        }
    }
}