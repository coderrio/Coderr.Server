using System.Web.Mvc;

namespace codeRR.Server.Web.Areas.Installation
{
    public static class WizardSteps
    {
        private static readonly WizardStepInfo[] Steps =
        {
            new WizardStepInfo("Introduction", "~/installation"),
            new WizardStepInfo("Configure database", "~/installation/sql/"),
            new WizardStepInfo("Create tables", "~/installation/sql/tables/"),
            new WizardStepInfo("Base configuration", "~/installation/setup/basics/"),
            new WizardStepInfo("Error tracking", "~/installation/setup/errors/"),
            new WizardStepInfo("Create admin account", "~/installation/account/admin/"),
            new WizardStepInfo("Mail settings", "~/installation/messaging/email/"),
            new WizardStepInfo("Support", "~/installation/setup/support"),
            new WizardStepInfo("Completed", "~/installation/setup/completed/")
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
                $@"<a class=""btn btn-outline-primary"" data-name=""nextLink"" href=""{urlHelper.Content(step.VirtualPath)}"">{step.Name} &gt;&gt;</a>";
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
                $@"<a class=""btn btn-outline-dark"" href=""{urlHelper.Content(step.VirtualPath)}"">&lt;&lt; {step.Name}</a>";
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