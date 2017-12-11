namespace Coderr.Server.PluginApi.Config
{
    public interface IConfiguration<out TConfigType> where TConfigType : IConfigurationSection, new()
    {
        TConfigType Value { get; }
    }
}