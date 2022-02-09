using System.Collections.Generic;

namespace Coderr.Server.Web.Areas.Admin.Models.Partition
{
    public class ManageViewModel
    {
        public int ApplicationId { get; set; }
        public IEnumerable<ListItem> Items { get; set; }
    }
}