namespace Coderr.Server.ReportAnalyzer.Abstractions.Boot
{
    public interface IReportAnalyzerModule
    {
        void Configure(ConfigurationContext context);
        void Start(StartContext context);

        void Stop();
    }
}