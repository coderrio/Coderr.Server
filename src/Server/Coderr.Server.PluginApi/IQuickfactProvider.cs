using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Incidents.Queries;

namespace Coderr.Server.PluginApi
{
    public interface IQuickfactProvider
    {
        Task AssignAsync(int incidentId, ICollection<QuickFact> facts);
    }
}
