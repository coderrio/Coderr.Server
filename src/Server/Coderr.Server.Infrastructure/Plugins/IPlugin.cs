namespace codeRR.Server.Infrastructure.Plugins
{
    /// <summary>
    ///     Represents a plugin in codeRR.
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        ///     Done after the boot to collect application configuration.
        /// </summary>
        /// <param name="config">configuration</param>
        void Configure(Configuration config);

        /// <summary>
        ///     The application is starting, nothing is yet configured.
        /// </summary>
        void Preload();
    }
}