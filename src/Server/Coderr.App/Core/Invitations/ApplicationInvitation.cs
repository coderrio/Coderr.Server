using System;

namespace codeRR.Server.App.Core.Invitations
{
    /// <summary>
    ///     Invitation to a specific application.
    /// </summary>
    public class ApplicationInvitation
    {
        /// <summary>
        ///     The application that this invitation is for when it comes to access of the application.
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        ///     When the invitation was created, skipping shit like daylight faking time and time stones.
        /// </summary>
        public DateTime InvitedAtUtc { get; set; }

        /// <summary>
        ///     Username of the user that invited the user user for the application that both uses.
        /// </summary>
        public string InvitedBy { get; set; }
    }
}