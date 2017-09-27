using System;

namespace codeRR.Infrastructure.Queueing.Ado
{
    public class AdoNetQueueEntry
    {
        public int ApplicationId { get; set; }
        public string AssemblyQualifiedTypeName { get; set; }

        public string Body { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public int Id { get; set; }
    }
}