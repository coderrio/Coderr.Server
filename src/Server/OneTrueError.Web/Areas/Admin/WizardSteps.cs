using System.Web.Mvc;

namespace OneTrueError.Web.Areas.Admin
{
    public static class WizardSteps
    {
        public static readonly WizardStepInfo[] Steps =
        {
            new WizardStepInfo("Start page", "~/admin"),
            new WizardStepInfo("Base configuration", "~/admin/home/basics/"),
            new WizardStepInfo("Error tracking", "~/admin/home/errors/"),
            new WizardStepInfo("Api keys", "~/admin/apikeys/"),
            new WizardStepInfo("Applications", "~/admin/application/"),
            new WizardStepInfo("Mail settings", "~/admin/messaging/email/"),
            new WizardStepInfo("Message queues", "~/admin/queues/"),
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
            return $@"<a class=""btn btn- btn-default"" href=""{urlHelper.Content(step.VirtualPath)}"">{step.Name} &gt;&gt;</a>";
        }

        public static string GetPreviousWizardStepLink(this UrlHelper urlHelper)
        {
            var index = FindCurrentIndex(urlHelper);
            if (index == -1)
                return "";
            if (index > 0)
                index--;

            var step = Steps[index];
            return $@"<a class=""btn btn-default"" href=""{urlHelper.Content(step.VirtualPath)}"">&lt;&lt; {step.Name}</a>";
        }

        public static bool IsCurrentStep(this UrlHelper urlHelper, WizardStepInfo step)
        {
            var currentIndex = FindCurrentIndex(urlHelper);
            var indexOfGivenStep = -1;
            for (int i = 0; i < Steps.Length; i++)
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