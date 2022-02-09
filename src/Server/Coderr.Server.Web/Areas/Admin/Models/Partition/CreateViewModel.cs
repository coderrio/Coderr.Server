using System.ComponentModel.DataAnnotations;

namespace Coderr.Server.Web.Areas.Admin.Models.Partition
{
    public class CreateViewModel
    {
        [Required]
        public int? ApplicationId { get; set; }

        public string Title { get; set; }

        public int? NumberOfItems { get; set; }

        [Required, StringLength(40, MinimumLength = 1)]
        public string PartitionKey { get; set; }

        [Range(1, 1000)] public int Weight { get; set; } = 1;
    }
}