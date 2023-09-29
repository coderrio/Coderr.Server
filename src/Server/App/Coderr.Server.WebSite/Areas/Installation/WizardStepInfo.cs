using System;
using Microsoft.AspNetCore.Mvc;

namespace Coderr.Server.WebSite.Areas.Installation
{
    public class WizardStepInfo
    {
        public WizardStepInfo(string name, string virtualPath)
        {
            Name = name;
            VirtualPath = virtualPath;
        }

        public string Name { get; set; }

        public string VirtualPath { get; set; }

        public bool IsForAbsolutePath(string currentPath, IUrlHelper helper)
        {
            currentPath = currentPath.TrimEnd('/');

            var stepPath = helper.Content(VirtualPath).TrimEnd('/');
            if (stepPath.Equals(currentPath, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            stepPath = VirtualPath.Replace("~", "").TrimEnd('/');
            return currentPath.Equals(stepPath, StringComparison.OrdinalIgnoreCase);
        }
    }
}