namespace Coderr.Server.Abstractions.Boot
{
    public interface IAppModule
    {
        void Configure(ConfigurationContext context);
        void Start(StartContext context);

        void Stop();
    }
}