using System;

namespace OneTrueError.Infrastructure.Queueing.Ado
{
    public class AdoNetQueueEntry
    {
        public int Id { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public string AssemblyQualifiedTypeName { get; set; }

        public int ApplicationId { get; set; }

        public string Body { get; set; }
    }
}
