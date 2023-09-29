using System.Collections.Generic;

namespace Coderr.Server.Api.Modules.Mine.Queries
{
    /// <summary>
    /// Result for <see cref="ListMyIncidents"/>.
    /// </summary>
    public class ListMyIncidentsResult
    {
        public string Comment { get; set; }
        public IList<ListMyIncidentsResultItem> Items { get; set; }
        public IList<ListMySuggestedItem> Suggestions { get; set; }
    }
}