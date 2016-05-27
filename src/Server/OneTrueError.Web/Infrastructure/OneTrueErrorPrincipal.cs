using System;
using System.Security.Principal;

namespace OneTrueError.Web.Infrastructure
{
    public class OneTrueErrorPrincipal : IPrincipal
    {
        public OneTrueErrorPrincipal()
        {
            if (Assigned != null)
                Assigned(this, EventArgs.Empty);
        }
        public int AccountId { get; set; }
        public int ApplicationId { get; set; }

        public bool IsInRole(string role)
        {
            return false;
        }

        public IIdentity Identity { get; set; }

        public static event EventHandler Assigned;
    }
}