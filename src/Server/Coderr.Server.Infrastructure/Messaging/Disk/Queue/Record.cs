namespace Coderr.Server.Infrastructure.Messaging.Disk.Queue
{
    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Record<T>
    {
        public Record(T entity, int recordOffset)
        {
            Entity = entity;
            RecordOffset = recordOffset;
        }

        public T Entity { get; }

        /// <summary>
        ///     Offset of our record (pointing at the header).
        /// </summary>
        /// <remarks>We had a bug where it pointed at the data, so when reading, check if we need to move backwards 8 bytes.</remarks>
        public int RecordOffset { get; set; }
    }
}