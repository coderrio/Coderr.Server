namespace Coderr.Server.Abstractions.Directory
{
    public interface IDirectoryEntry
    {
        string FullName { get; }
        string Identity { get; }
        string Name { get; }
        //IDictionary<string, object> Properties { get; }
        string SchemaClassName { get; }
    }
}