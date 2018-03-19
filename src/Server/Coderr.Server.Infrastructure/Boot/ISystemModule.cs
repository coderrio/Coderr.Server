namespace Coderr.Server.Infrastructure.Boot
{
    public interface ISystemModule
    {
        void Configure(ConfigurationContext context);
        void Start(StartContext context);

        void Stop();
    }
}