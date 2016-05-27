using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OneTrueError.Web.Areas.Installation.Models
{
    
    public class BasicsViewModel
    {
        [Required, MinLength(4)]
        public string BaseUrl { get; set; }

        [Required, EmailAddress]
        public string SupportEmail { get; set; }
    }
}