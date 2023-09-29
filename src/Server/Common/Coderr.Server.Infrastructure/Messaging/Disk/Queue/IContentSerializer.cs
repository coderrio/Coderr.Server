using System;
using System.IO;
using System.Threading.Tasks;

namespace Coderr.Server.Infrastructure.Messaging.Disk.Queue
{
    /// <summary>
    ///     Takes care of reading and writing to the queue data file.
    /// </summary>
    public interface IContentSerializer
    {
        Task<object> DeserializeAsync(Stream source, int recordSize, Type entityType);
        Task SerializeAsync(Stream destination, object entity);
    }
}