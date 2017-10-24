using DotNetCqs;

namespace codeRR.Server.Api.Core.Applications.Queries
{
    /// <summary>
    ///     Get a list of applications.
    /// </summary>
    [Message]
    public class GetApplicationList : Query<ApplicationListItem[]>
    {
        /// <summary>
        ///     Get all applications that the given user have access to
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         0 = get all applications
        ///     </para>
        /// </remarks>
        public int AccountId { get; set; }

        /// <summary>
        ///     Only list applications that the given account is administrator for.
        /// </summary>
        public bool FilterAsAdmin { get; set; }
    }
}