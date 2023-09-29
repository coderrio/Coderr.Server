using System.Threading.Tasks;

namespace Coderr.Server.WebSite.Hubs
{
    public interface CoderrHub
    {
        Task OnEvent(HubEvent evt);
    }
}