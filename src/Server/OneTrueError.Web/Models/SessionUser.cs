using System.Collections.Generic;
using System.Web;

namespace OneTrueError.Web.Models
{
    public class SessionUser
    {
        public SessionUser(int accountId, string userName)
        {
            AccountId = accountId;
            UserName = userName;
            Applications = new Dictionary<int, string>();
        }

        public int AccountId { get; set; }
        public int ApplicationId { get; set; }

        public string UserName { get; set; }


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

        public IDictionary<int,string> Applications { get; set; }

        #endregion
    }
}