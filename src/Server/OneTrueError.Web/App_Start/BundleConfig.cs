using System.Web.Optimization;

namespace OneTrueError.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/application.js",
                "~/Scripts/prism.js",
                "~/Scripts/marked.min.js",
                "~/Scripts/humane.js",
                "~/Scripts/Base64.js",
                "~/Scripts/transparency.min.js")
                );

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate*"));


            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.js",
                "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/app")
                .Include(
                    "~/Scripts/utils.js",
                    "~/Scripts/CqsClient.js",
                    "~/Scripts/Griffin.Yo.js",
                    "~/Scripts/Griffin.Net.js",
                    "~/Scripts/Griffin.WebApp.js",
                    "~/ViewModels/ChartViewModel.js",
                    "~/Scripts/Models/AllModels.js")
                .IncludeDirectory("~/app/", "*.js", true)
                );


            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/ote_bootstrap.min.css",
                "~/Content/humane.flatty.css",
                "~/Content/site.css"));
        }
    }
}