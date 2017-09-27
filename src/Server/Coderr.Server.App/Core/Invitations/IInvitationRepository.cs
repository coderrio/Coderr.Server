using System;
using System.Threading.Tasks;

namespace codeRR.Server.App.Core.Invitations
{
    /// <summary>
    ///     Invitation repository
    /// </summary>
    public interface IInvitationRepository
    {
        /// <summary>
        ///     Create a new invitation
        /// </summary>
        /// <param name="invitation">invitation</param>
        /// <returns>task</returns>
        /// <exception cref="ArgumentNullException">invitation</exception>
        Task CreateAsync(Invitation invitation);

        /// <summary>
        ///     Delete invitation
        /// </summary>
        /// <param name="invitationKey">Key that was sent out in the invitation email</param>
        /// <returns>task</returns>
        /// <exception cref="ArgumentNullException">invitationKey</exception>
        Task DeleteAsync(string invitationKey);

        /// <summary>
        ///     Find invitation by email
        /// </summary>
        /// <param name="email">email for the invited user.</param>
        /// <returns>invitation if found; otherwise <c>null</c>.</returns>
        /// <exception cref="ArgumentNullException">email</exception>
        Task<Invitation> FindByEmailAsync(string email);

        /// <summary>
        ///     Get invitation by key.
        /// </summary>
        /// <param name="invitationKey">Key sent out in the invitation email.</param>
        /// <returns>Invitation</returns>
        Task<Invitation> GetByInvitationKeyAsync(string invitationKey);

        /// <summary>
        ///     Update existing invitation
        /// </summary>
        /// <param name="invitation">invitation</param>
        /// <returns>task</returns>
        /// <exception cref="ArgumentNullException">invitation</exception>
        Task UpdateAsync(Invitation invitation);
    }
}