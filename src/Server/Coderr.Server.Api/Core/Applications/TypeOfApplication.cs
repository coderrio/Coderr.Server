namespace codeRR.Server.Api.Core.Applications
{
    /// <summary>
    ///     Kind of application that this is
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Used to determine how different analytics should be made, like analyzing memory usage (which has to guess the
    ///         total amount of memory if not included as context information).
    ///     </para>
    ///     <para>
    ///         For instance a <c>OutOfMemoryException</c> isn't as fatal in a mobile application, like it is in a large server
    ///         application, as the latter is supposed to have large amount of resources.
    ///     </para>
    /// </remarks>
    public enum TypeOfApplication
    {
        /// <summary>
        ///     Cellphone application
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         An application with limited system resources (memory and usage).
        ///     </para>
        /// </remarks>
        Mobile,

        /// <summary>
        ///     DesktopApplication application (i.e. a windows end user computer)
        /// </summary>
        DesktopApplication,

        /// <summary>
        ///     Server, as a web server or a WCF service.
        /// </summary>
        Server
    }
}