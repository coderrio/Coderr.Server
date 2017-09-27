using System.ComponentModel.DataAnnotations;

namespace codeRR.Server.Web.Areas.Admin.Models
{
    public class ReportingViewModel
    {
        public ReportingViewModel()
        {
            MaxReportsPerIncident = 500;
            RetentionDays = 90;
        }

        [Required, Range(1, 10000)]
        public int MaxReportsPerIncident { get; set; }

        [Required, Range(1, 365)]
        public int RetentionDays { get; set; }
    }
}