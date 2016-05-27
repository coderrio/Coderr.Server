using DotNetCqs;

namespace OneTrueError.Api.Core.Accounts.Requests
{
    /// <summary>
    ///     Login user.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The user must have registered an account and activated it.
    ///     </para>
    /// </remarks>
    public class Login : Request<LoginReply>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="Login" />-
        /// </summary>
        /// <param name="userName">user name</param>
        /// <param name="password">password as entered by the user. Can be empty for cookie logins.</param>
        public Login(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        /// <summary>
        ///     Serialization constructor.
        /// </summary>
        protected Login()
        {
        }

        /// <summary>
        ///     Password may be empty for cookie logins.
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        ///     Username
        /// </summary>
        public string UserName { get; private set; }
    }
}