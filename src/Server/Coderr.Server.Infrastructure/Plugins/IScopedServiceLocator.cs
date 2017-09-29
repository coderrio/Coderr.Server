namespace codeRR.Server.Infrastructure.Plugins
{
    /// <summary>
    ///     Will move to the definition in Microsoft.Extensions soon
    /// </summary>
    public interface IScopedServiceLocator
    {
        /// <summary>
        ///     Get a service in the HTTP scoped container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetService<T>() where T : class;
    }
}