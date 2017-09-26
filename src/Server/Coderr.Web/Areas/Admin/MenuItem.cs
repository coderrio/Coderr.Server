using System;
using System.Web.Mvc;

namespace OneTrueError.Web.Areas.Admin
{
    public class MenuItem
    {
        public MenuItem(string displayName, string virtualPath)
        {
            DisplayName = displayName;
            VirtualPath = virtualPath;
        }

        public string DisplayName { get; set; }

        public string VirtualPath { get; set; }

        public bool IsForAbsolutePath(string currentPath, UrlHelper helper)
        {
            var myPath = helper.Content(VirtualPath).TrimEnd('/');
            currentPath = currentPath.TrimEnd('/');
            return myPath.Equals(currentPath, StringComparison.OrdinalIgnoreCase);
        }
    }
}