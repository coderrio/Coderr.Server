using System.Web.Mvc;

namespace codeRR.Server.Web.Areas.Admin
{
    public static class AdminMenu
    {
        public static readonly MenuItem[] Steps =
        {
            new MenuItem("Start page", "~/admin"),
            new MenuItem("Base configuration", "~/admin/home/basics/"),
            new MenuItem("Error tracking", "~/admin/home/errors/"),
            new MenuItem("Api keys", "~/admin/apikeys/"),
            new MenuItem("Applications", "~/admin/application/"),
            new MenuItem("Mail settings", "~/admin/messaging/email/"),
            new MenuItem("Report settings", "~/admin/reporting/")
        };


        public static string GetNextWizardStep(this UrlHelper urlHelper)
        {
            var index = FindCurrentIndex(urlHelper);
            if (index == -1)
                return null;
            if (index < Steps.Length - 1)
                index++;

            var step = Steps[index];
            return urlHelper.Content(step.VirtualPath);
        }

        public static string GetNextWizardStepLink(this UrlHelper urlHelper)
        {
            var index = FindCurrentIndex(urlHelper);
            if (index == -1)
                return "";
            if (index < Steps.Length - 1)
                index++;

            var step = Steps[index];
            return
                $@"<a class=""btn btn- btn-default"" href=""{urlHelper.Content(step.VirtualPath)}"">{step.DisplayName} &gt;&gt;</a>";
        }

        public static string GetPreviousWizardStepLink(this UrlHelper urlHelper)
        {
            var index = FindCurrentIndex(urlHelper);
            if (index == -1)
                return "";
            if (index > 0)
                index--;

            var step = Steps[index];
            return
                $@"<a class=""btn btn-default"" href=""{urlHelper.Content(step.VirtualPath)}"">&lt;&lt; {step.DisplayName}</a>";
        }

        public static bool IsCurrentStep(this UrlHelper urlHelper, MenuItem step)
        {
            var currentIndex = FindCurrentIndex(urlHelper);
            var indexOfGivenStep = -1;
            for (var i = 0; i < Steps.Length; i++)
            {
                if (Steps[i] == step)
                {
                    indexOfGivenStep = i;
                    break;
                }
            }

            return currentIndex == indexOfGivenStep;
        }

        private static int FindCurrentIndex(UrlHelper urlHelper)
        {
            var currentPath = urlHelper.RequestContext.HttpContext.Request.Url.AbsolutePath;
            for (var i = 0; i < Steps.Length; i++)
            {
                if (Steps[i].IsForAbsolutePath(currentPath, urlHelper))
                    return i;
            }

            return -1;
        }
    }
}