﻿using System.ComponentModel.DataAnnotations;

namespace codeRR.Web.Models.Account
{
    public class RequestPasswordResetViewModel
    {
        [Required]
        public string EmailAddress { get; set; }
    }
}