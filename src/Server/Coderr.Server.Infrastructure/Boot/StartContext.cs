using System;

namespace Coderr.Server.Infrastructure.Boot
{
    public class StartContext
    {
        public IServiceProvider ServiceProvider { get; set; }
    }

    public interface IConfiguration : IConfigurationSection
    {
        IConfigurationSection GetSection(string name);
        string GetConnectionString(string name);
    }

    public interface IConfigurationSection
    {
        string this[string name] { get; }
    }

}
