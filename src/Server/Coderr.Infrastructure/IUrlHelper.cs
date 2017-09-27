namespace codeRR.Server.Infrastructure
{
    public interface IUrlHelper
    {
        string ToAbsolutePath(string virtualPath);
    }
}
