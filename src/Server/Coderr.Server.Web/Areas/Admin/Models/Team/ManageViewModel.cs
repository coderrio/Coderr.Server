using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Coderr.Server.Web.Areas.Admin.Models.Team
{
    public class ManageViewModel
    {
        public IList<ApplicationViewModel> Applications { get; set; }
    }

    public class ApplicationViewModel
    {
        public int ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public IList<ApplicationMemberViewModel> Members { get; set; }
    }
    
    public class ApplicationMemberViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
}