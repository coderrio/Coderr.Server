using System.Threading.Tasks;
using Coderr.Server.Abstractions.WorkItems;
using Coderr.Server.Api.WorkItems.Queries;
using DotNetCqs;

namespace Coderr.Server.App.WorkItems.Queries
{
    internal class FindIntegrationHandler : IQueryHandler<FindIntegration, FindIntegrationResult>
    {
        private readonly IWorkItemServiceProvider _workItemServiceProvider;

        public FindIntegrationHandler(IWorkItemServiceProvider workItemServiceProvider)
        {
            _workItemServiceProvider = workItemServiceProvider;
        }

        public async Task<FindIntegrationResult> HandleAsync(IMessageContext context, FindIntegration query)
        {
            var service = await _workItemServiceProvider.FindService(query.ApplicationId);
            if (service == null)
                return new FindIntegrationResult {HaveIntegration = false};

            return new FindIntegrationResult {HaveIntegration = true, Title = service.Title, Name = service.Name};
        }
    }
}