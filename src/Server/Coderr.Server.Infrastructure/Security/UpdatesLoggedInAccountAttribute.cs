using System;

namespace codeRR.Server.Infrastructure.Security
{
    /// <summary>
    ///     The handler that this attribute is placed on will update the currently logged in user, so the host need to update
    ///     it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class UpdatesLoggedInAccountAttribute : Attribute
    {
    }
}