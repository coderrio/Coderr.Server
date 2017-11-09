using System.Collections.Generic;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Incidents.Queries;

namespace Coderr.Server.PluginApi
{
    public interface IHighlightedContextDataProvider
    {
        Task CollectAsync(int incidentId, ICollection<HighlightedContextData> data);
    }
}