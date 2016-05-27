using System;
using System.Web.Mvc;

namespace OneTrueError.Web.Areas.Installation
{
    public class WizardStepInfo
    {
        public WizardStepInfo(string name, string virtualPath)
        {
            Name = name;
            VirtualPath = virtualPath;
        }

        public string VirtualPath { get; set; }
        public string Name { get; set; }

        public bool IsForAbsolutePath(string currentPath, UrlHelper helper)
        {
            var myPath = helper.Content(VirtualPath).TrimEnd('/');
            currentPath = currentPath.TrimEnd('/');
            return myPath.Equals(currentPath, StringComparison.OrdinalIgnoreCase);
        }
    }
}