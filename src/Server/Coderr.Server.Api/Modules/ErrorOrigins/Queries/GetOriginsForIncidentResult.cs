namespace codeRR.Server.Api.Modules.ErrorOrigins.Queries
{
    /// <summary>
    ///     Result for <see cref="GetOriginsForIncident" />.
    /// </summary>
    public class GetOriginsForIncidentResult
    {
        /// <summary>
        ///     One item per geographic location
        /// </summary>
        public GetOriginsForIncidentResultItem[] Items { get; set; }
    }
}