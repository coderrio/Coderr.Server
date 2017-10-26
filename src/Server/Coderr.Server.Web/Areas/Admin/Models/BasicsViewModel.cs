using System.ComponentModel.DataAnnotations;

namespace codeRR.Server.Web.Areas.Admin.Models
{
    public class BasicsViewModel
    {
        /// <summary>
        /// URL to coderr (typically "http://yourHostName/" or "http://somehost/coderr/")
        /// </summary>
        [Required, MinLength(8)]
        public string BaseUrl { get; set; }

        /// <summary>
        /// Email used when codeRR users ask for internal support
        /// </summary>
        [Required, EmailAddress]
        public string SupportEmail { get; set; }

        /// <summary>
        /// Allow users to register new accounts
        /// </summary>
        public bool AllowRegistrations { get; set; }
    }
}