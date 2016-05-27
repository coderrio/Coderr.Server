using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OneTrueError.Web.Areas.Installation.Models
{
    public class AccountViewModel
    {
        [Required, StringLength(40)]
        public string UserName { get; set; }

        [Required, StringLength(255)]
        public string EmailAddress { get; set; }

        [Required, StringLength(40)]
        public string Password { get; set; }

    }
}