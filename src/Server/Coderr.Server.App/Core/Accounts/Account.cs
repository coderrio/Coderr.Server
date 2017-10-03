using System;
using System.Security.Authentication;
using System.Security.Cryptography;
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace codeRR.Server.App.Core.Accounts
{
    /// <summary>
    ///     An account (i.e. just allows a user to login, but do not give access to teams etc).
    /// </summary>
    public class Account
    {
        /// <summary>
        ///     Maximum number of password attempts before account becomes locked.
        /// </summary>
        public const int MaxPasswordAttempts = 3;

        /// <summary>
        ///     Create a new instance of <see cref="Account" />-
        /// </summary>
        /// <param name="accountId">Predefined account id</param>
        /// <param name="userName">User name</param>
        /// <param name="password">password</param>
        public Account(int accountId, string userName, string password)
            : this(userName, password)
        {
            if (accountId <= 0) throw new ArgumentOutOfRangeException(nameof(accountId));
            Id = accountId;
        }

        /// <summary>
        ///     Create a new instance of <see cref="Account" />-
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="password">password</param>
        public Account(string userName, string password)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));

            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            CreatedAtUtc = DateTime.UtcNow;
            ActivationKey = Guid.NewGuid().ToString("N");
            AccountState = AccountState.VerificationRequired;
            HashedPassword = HashNewPassword(password);
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected Account()
        {
        }

        /// <summary>
        ///     Current state
        /// </summary>
        public AccountState AccountState { get; private set; }

        /// <summary>
        ///     Used to verify the mail address (if verification is activated)
        /// </summary>
        public string ActivationKey { get; private set; }

        /// <summary>
        ///     When this account was created.
        /// </summary>
        public DateTime CreatedAtUtc { get; private set; }

        /// <summary>
        ///     Private setter since new emails needs to be verifier (verification email with a link)
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     Password salted and hashed.
        /// </summary>
        public string HashedPassword { get; private set; }


        /// <summary>
        ///     Primary key
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public int Id { get; private set; }

        /// <summary>
        ///     IS system administrator
        /// </summary>
        public bool IsSysAdmin { get; set; }

        /// <summary>
        ///     When last successful login attempt was made.
        /// </summary>
        public DateTime LastLoginAtUtc { get; private set; }

        /// <summary>
        ///     Number of failed login attempts (reseted on each successful login attempt).
        /// </summary>
        public int LoginAttempts { get; private set; }

        /// <summary>
        ///     Password salt.
        /// </summary>
        public string Salt { get; private set; }


        /// <summary>
        ///     Last time a property was updated.
        /// </summary>
        public DateTime UpdatedAtUtc { get; private set; }

        /// <summary>
        ///     User name / login name
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        ///     Activate account (i.e. allow logins).
        /// </summary>
        public void Activate()
        {
            AccountState = AccountState.Active;
            ActivationKey = null;
            UpdatedAtUtc = DateTime.UtcNow;
            LoginAttempts = 0;
            LastLoginAtUtc = DateTime.UtcNow;
        }

        /// <summary>
        ///     Change password
        /// </summary>
        /// <param name="newPassword">New password as entered by the user.</param>
        public void ChangePassword(string newPassword)
        {
            if (newPassword == null) throw new ArgumentNullException(nameof(newPassword));
            HashedPassword = HashNewPassword(newPassword);
            ActivationKey = null;
            UpdatedAtUtc = DateTime.UtcNow;
            AccountState = AccountState.Active;
            LoginAttempts = 0;
        }

        /// <summary>
        ///     Login
        /// </summary>
        /// <param name="password">Password as specified by the user</param>
        /// <returns><c>true</c> if password was the correct one; otherwise <c>false</c>.</returns>
        /// <exception cref="AuthenticationException">Account is not active, or too many failed login attempts.</exception>
        public bool Login(string password)
        {
            if (AccountState == AccountState.VerificationRequired)
                throw new AuthenticationException("You have to activate your account first. Check your email.");

            if (AccountState == AccountState.Locked)
                throw new AuthenticationException("Your account has been locked. Contact support.");

            // null for cookie logins.
            if (password == null)
            {
                LastLoginAtUtc = DateTime.UtcNow;
                LoginAttempts = 0;
                return true;
            }

            var validPw = ValidatePassword(password);
            if (validPw)
            {
                LastLoginAtUtc = DateTime.UtcNow;
                LoginAttempts = 0;
                return true;
            }
            LoginAttempts++;

            //need to have it at the bottom too so that we can throw on the failed max attempt.
            if (LoginAttempts >= MaxPasswordAttempts)
            {
                AccountState = AccountState.Locked;
                throw new AuthenticationException("Too many login attempts.");
            }
            return false;
        }

        /// <summary>
        ///     Want to reset password.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Changes user state to <see cref="codeRR.Server.App.Core.Accounts.AccountState.ResetPassword" /> and generates a new
        ///         <see cref="ActivationKey" />.
        ///     </para>
        /// </remarks>
        public void RequestPasswordReset()
        {
            AccountState = AccountState.ResetPassword;
            ActivationKey = Guid.NewGuid().ToString("N");
        }


        /// <summary>
        ///     Email has been verified.
        /// </summary>
        /// <param name="email">Email address</param>
        public void SetVerifiedEmail(string email)
        {
            Email = email ?? throw new ArgumentNullException(nameof(email));
        }

        /// <summary>
        ///     Check if the given password is the current one.
        /// </summary>
        /// <param name="enteredPassword">Password as entered by the user.</param>
        /// <returns><c>true</c> if the password is the same as the current one; otherwise false.</returns>
        public bool ValidatePassword(string enteredPassword)
        {
            if (enteredPassword == null) throw new ArgumentNullException(nameof(enteredPassword));
            var salt = Convert.FromBase64String(Salt);
            var algorithm2 = new Rfc2898DeriveBytes(enteredPassword, salt);
            var pw = algorithm2.GetBytes(128);

            var hashedPw = Convert.ToBase64String(pw);
            return hashedPw == HashedPassword;
        }


        /// <summary>
        ///     Hash password and generate a new salt.
        /// </summary>
        /// <param name="password">Password as entered by the user</param>
        /// <returns>Salted and hashed password</returns>
        private string HashNewPassword(string password)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            var algorithm2 = new Rfc2898DeriveBytes(password, 64);
            var salt = algorithm2.Salt;
            Salt = Convert.ToBase64String(salt);
            var pw = algorithm2.GetBytes(128);
            return Convert.ToBase64String(pw);
        }
    }
}