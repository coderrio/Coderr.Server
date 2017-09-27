using DotNetCqs;

namespace codeRR.Server.Api.Core.Applications.Events
{
    namespace codeRR.Api.Core.Accounts.Events
    {
        /// <summary>
        ///     A user have been added directly, or through an invitation
        /// </summary>
        public class UserAddedToApplication : ApplicationEvent
        {
            /// <summary>
            ///     Creates a new instance of <see cref="UserAddedToApplication" />.
            /// </summary>
            /// <param name="applicationId">Identifier for the application that the user was added to.</param>
            /// <param name="accountId">Account identifier for the user that was added to the application</param>
            public UserAddedToApplication(int applicationId, int accountId)
            {
                ApplicationId = applicationId;
                AccountId = accountId;
            }

            /// <summary>
            ///     Serialization constructor
            /// </summary>
            protected UserAddedToApplication()
            {
            }

            /// <summary>
            ///     Account identifier for the user that was added to the application
            /// </summary>
            public int AccountId { get; private set; }

            /// <summary>
            ///     Identifier for the application that the user was added to.
            /// </summary>
            public int ApplicationId { get; private set; }
        }
    }
}