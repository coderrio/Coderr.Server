namespace Coderr.Server.Infrastructure
{
    public interface IUrlHelper
    {
        string ToAbsolutePath(string virtualPath);
    }
}
