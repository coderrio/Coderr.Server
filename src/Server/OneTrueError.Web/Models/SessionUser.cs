using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OneTrueError.Web.Models
{
    public class SessionUser
    {
        private string[] _roles;

        public SessionUser(int accountId, string userName)
        {
            AccountId = accountId;
            UserName = userName;
            Applications = new Dictionary<int, string>();
        }

        public int AccountId { get; set; }
        public int ApplicationId { get; set; }

        public string UserName { get; set; }

        public string[] GetRoles()
        {
            if (_roles != null)
                return _roles;

            var roles = Applications.Select(x => "Application_" + x.Key).ToList();
            if (AccountId == 1)
                roles.Add("Sysadmin");
            _roles = roles.ToArray();
            return _roles;
        }

        #region "Static accessors"

        public static bool IsAuthenticated
        {
            get { return Current != null; }
        }

        public static SessionUser Current
        {
            get
            {
                return HttpContext.Current.Session == null
                    ? null
                    : HttpContext.Current.Session["SessionUser"] as SessionUser;
            }
            set { HttpContext.Current.Session["SessionUser"] = value; }
        }

        public IDictionary<int, string> Applications { get; set; }

        #endregion
    }
}