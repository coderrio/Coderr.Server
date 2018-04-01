using System.Data;

namespace Coderr.Server.Abstractions
{
    public interface IConnectionFactory
    {
        IDbConnection OpenConnection();
        bool IsConfigured { get; set; }
    }
}