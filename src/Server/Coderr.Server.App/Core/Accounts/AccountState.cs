namespace codeRR.Server.App.Core.Accounts
{
    /// <summary>
    ///     Account state
    /// </summary>
    public enum AccountState
    {
        /// <summary>
        ///     Account have been created but not yet verified.
        /// </summary>
        VerificationRequired,

        /// <summary>
        ///     Account is active
        /// </summary>
        Active,

        /// <summary>
        ///     Account have been locked, typically by too many login attempts.
        /// </summary>
        Locked,

        /// <summary>
        ///     Password reset have been requested (an password reset link have been sent).
        /// </summary>
        ResetPassword
    }
}