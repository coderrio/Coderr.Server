using System.Threading.Tasks;

namespace Coderr.Server.Abstractions.Incidents
{
    public interface IQuickfactProvider
    {
        Task CollectAsync(QuickFactContext context);
    }
}
