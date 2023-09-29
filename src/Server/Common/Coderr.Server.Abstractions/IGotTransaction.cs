using System.Data.Common;

namespace Coderr.Server.Abstractions
{
    public interface IGotTransaction
    {
        DbTransaction Transaction { get; }
    }
}