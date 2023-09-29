using System.Threading.Tasks;

namespace Coderr.Server.App.Insights.Metrics
{
    public interface IKeyMetricGenerator
    {
        Task<KeyMetricDataResult> Collect(KeyMetricGeneratorContext context);
    }
}