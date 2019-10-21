namespace Coderr.Server.PostgreSQL.Modules.Versions
{
    public class ApplicationVersionConfig
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public string AssemblyName { get; set; }
    }
}