using System;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Security;
using Coderr.Server.Domain.Core.Incidents;
using Coderr.Server.Domain.Core.Incidents.Events;
using Coderr.Server.Domain.Modules.History;
using Coderr.Server.Infrastructure.Security;
using DotNetCqs;

namespace Coderr.Server.App.Modules.History.Events
{
    public class IncidentReOpenedHandler : IMessageHandler<IncidentReOpened>
    {
        private readonly IHistoryRepository _repository;

        public IncidentReOpenedHandler(IHistoryRepository repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(IMessageContext context, IncidentReOpened message)
        {
            // Can be reopened by the system
            int? accountId = context.Principal.GetAccountId();
            if (accountId == 0)
            {
                if (context.Principal.IsInRole(CoderrRoles.System))
                    accountId = null;
                else
                    throw new InvalidOperationException("Expected either the system or a specified account.");
            }
                

            var e = new HistoryEntry(message.IncidentId, accountId, IncidentState.ReOpened)
            {
                ApplicationVersion = message.ApplicationVersion
            };
            await _repository.CreateAsync(e);
        }
    }
}