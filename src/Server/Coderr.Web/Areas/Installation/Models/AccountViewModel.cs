﻿using System.ComponentModel.DataAnnotations;

namespace codeRR.Web.Areas.Installation.Models
{
    public class AccountViewModel
    {
        [Required, StringLength(255)]
        public string EmailAddress { get; set; }

        [Required, StringLength(40)]
        public string Password { get; set; }

        [Required, StringLength(40)]
        public string UserName { get; set; }
    }
}