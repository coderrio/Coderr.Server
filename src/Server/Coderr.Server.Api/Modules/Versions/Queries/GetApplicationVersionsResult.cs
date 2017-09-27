namespace codeRR.Server.Api.Modules.Versions.Queries
{
    /// <summary>
    ///     Result for <see cref="GetApplicationVersions" />
    /// </summary>
    public class GetApplicationVersionsResult
    {
        /// <summary>
        ///     All versions
        /// </summary>
        public GetApplicationVersionsResultItem[] Items { get; set; }
    }
}