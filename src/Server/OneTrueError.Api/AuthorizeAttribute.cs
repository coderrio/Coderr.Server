using System;

namespace OneTrueError.Api
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AuthorizeAttribute : Attribute
    {
        public AuthorizeAttribute(params string[] roles)
        {
            if (roles == null) throw new ArgumentNullException("roles");
            Roles = roles;
        }

        public string[] Roles { get; set; }
    }
}