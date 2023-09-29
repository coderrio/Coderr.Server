using System.Threading.Tasks;

namespace Coderr.Server.App.Metrics
{
    public interface IMetricGenerator
    {
        /// <summary>
        /// Gets information about this metric (what it generate etc)
        /// </summary>
        MetricDefinition Definition { get; }

        Task Generate(IMetricGenerationContext context);
    }
}
