using System;
using System.Configuration;
using System.IO;
using System.Web;
using codeRR.Server.Infrastructure;
using codeRR.Server.Web.Infrastructure;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;

[assembly: PreApplicationStartMethod(typeof(SchemaUpdateModule), "Register")]
namespace codeRR.Server.Web.Infrastructure
{
    public class SchemaUpdateModule : IHttpModule
    {
        private static string _errorMessage;

        private static bool IsDisabled => (ConfigurationManager.AppSettings["DisableMigrations"] == "true") ||
                                          (ConfigurationManager.AppSettings["Configured"] != "true");


        public static void Register()
        {
            DynamicModuleUtility.RegisterModule(typeof(SchemaUpdateModule));
        }
        public void Init(HttpApplication context)
        {
            if (IsDisabled)
                return;

            if (!SetupTools.DbTools.CanSchemaBeUpgraded())
            {
                return;
            }

            try
            {
                SetupTools.DbTools.UpgradeDatabaseSchema();
                return;
            }
            catch (Exception ex)
            {
                int nest = 1;
                var msg = ex.Message + "\r\n";
                ex = ex.InnerException;
                while (ex != null)
                {
                    msg += " ".PadLeft(nest*2) + ex.Message + "\r\n";
                    ex = ex.InnerException;
                }
                _errorMessage = msg;
            }

            context.BeginRequest += OnRequest;
        }

        private void OnRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication) sender;
            app.Response.StatusCode = 500;
            app.Response.ContentType = "text/plain";
            var sw = new StreamWriter(app.Response.OutputStream);
            sw.WriteLine("Database schema upgrade failed");
            sw.WriteLine();
            sw.WriteLine("Failed to update the database schema. Sorry for that. We do however promise to help you as fast as we can.");
            sw.WriteLine("Email the contents below to help@coderrapp.com. Include your MS SQL server version.");
            sw.WriteLine();
            sw.WriteLine("============================");
            sw.WriteLine(_errorMessage);
            sw.WriteLine("============================");
            sw.Flush();
            app.Response.TrySkipIisCustomErrors = true;
            app.Response.End();
        }

        public void Dispose()
        {
            
        }
    }
}