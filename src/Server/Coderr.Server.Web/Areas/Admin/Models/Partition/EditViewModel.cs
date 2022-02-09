using System.ComponentModel.DataAnnotations;

namespace Coderr.Server.Web.Areas.Admin.Models.Partition
{
    public class EditViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int? NumberOfItems { get; set; }

        [Range(1, 1000)] public int Weight { get; set; } = 1;
    }
}