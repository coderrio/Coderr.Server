using System.Configuration;
using System.Web.Configuration;
using codeRR.Server.SqlServer.Tools;

namespace codeRR.Server.Web.Infrastructure
{
    public class InstallationHelper
    {
        public static bool IsInstallationRequired()
        {
            return ConfigurationManager.AppSettings["Configured"] != "true" ||
                   string.IsNullOrEmpty(ConfigurationManager.ConnectionStrings["Db"]?.ConnectionString);
        }
        
        public static void UpdateConnectionString(string name, string value)
        {
            var configuration = WebConfigurationManager.OpenWebConfiguration("~");
            var section = (ConnectionStringsSection) configuration.GetSection("connectionStrings");
            section.ConnectionStrings[name].ConnectionString = value;
            configuration.Save();
            ConfigurationManager.RefreshSection("connectionStrings");
        }
    }
}