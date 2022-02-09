namespace Coderr.Server.Domain.Core.Applications
{
    /// <summary>
    /// Application that a specific user is a member of
    /// </summary>
    public class UserApplication
    {
        /// <summary>
        /// App name
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// ID
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        /// Logical group (used to organize applications).
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// Name of the group.
        /// </summary>
        public string GroupName { get; set; }
            
        /// <summary>
        /// If the user that this app is requested for is the admin
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// Number of full time developers.
        /// </summary>
        public decimal? NumberOfDevelopers { get; set; }
    }
}