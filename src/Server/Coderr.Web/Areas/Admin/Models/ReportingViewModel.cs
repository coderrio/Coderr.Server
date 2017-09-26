using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OneTrueError.Web.Areas.Admin.Models
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