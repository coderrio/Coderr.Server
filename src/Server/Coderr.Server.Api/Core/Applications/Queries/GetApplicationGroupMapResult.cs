namespace Coderr.Server.Api.Core.Applications.Queries
{
    public class GetApplicationGroupMapResult
    {
        /// <summary>
        /// If application id was specified, this will only contain one item; otherwise all maps.
        /// </summary>
        public GetApplicationGroupMapResultItem[] Items { get; set; }
    }
}