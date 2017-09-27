using System.ComponentModel.DataAnnotations;

namespace codeRR.Web.Areas.Admin.Models
{
    public class ErrorTrackingViewModel
    {
        [Display(Name = "Activate tracking")]
        public bool ActivateTracking { get; set; }

        [Display(Name = "Contact email"), EmailAddress]
        public string ContactEmail { get; set; }

        /// <summary>
        ///     A fixed identity which identifies this specific installation. You can generate a GUID and then store it.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Used to identify the number of installations that have the same issue.
        ///     </para>
        /// </remarks>
        public string InstallationId { get; set; }
    }
}