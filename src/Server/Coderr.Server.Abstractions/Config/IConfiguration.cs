namespace Coderr.Server.Abstractions.Config
{
    public interface IConfiguration<out TConfigType> where TConfigType : new()
    {
        TConfigType Value { get; }
    }
}