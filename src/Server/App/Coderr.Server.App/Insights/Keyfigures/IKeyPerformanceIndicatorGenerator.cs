using System.Threading.Tasks;

namespace Coderr.Server.App.Insights.Keyfigures
{
    public interface IKeyPerformanceIndicatorGenerator
    {
        Task CollectAsync(KeyPerformanceIndicatorContext context);
    }
}