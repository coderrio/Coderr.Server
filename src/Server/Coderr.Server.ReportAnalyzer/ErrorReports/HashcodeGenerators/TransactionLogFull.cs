using System.Linq;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Domain.Core.ErrorReports;

namespace Coderr.Server.ReportAnalyzer.ErrorReports.HashcodeGenerators
{
    /// <summary>
    /// Groups SQL Server about the transaction log into a single error (instead of different errors depending on the stack trace).
    /// </summary>
    [ContainerService]
    class TransactionLogFull : IHashCodeSubGenerator
    {
        public bool CanGenerateFrom(ErrorReportEntity entity)
        {
            if (entity.Exception.Name != "SqlException")
                return false;

            if (!TryGetSqlErrorNumber(entity, out var numberStr)) return false;

            return numberStr == "9002";
        }

        private static bool TryGetSqlErrorNumber(ErrorReportEntity entity, out string numberStr)
        {
            numberStr = null;
            var collection = entity.ContextCollections.FirstOrDefault(x => x.Name == "ExceptionProperties");
            return collection?.Properties.TryGetValue("Number", out numberStr) == true;
        }

        public ErrorHashCode GenerateHashCode(ErrorReportEntity entity)
        {
            if (!TryGetSqlErrorNumber(entity, out var numberStr))
                return null;

            return new ErrorHashCode
            {
                CollisionIdentifier = entity.Exception.Message,
                HashCode = HashCodeUtility.GetPersistentHashCode(entity.Exception.Message).ToString("X")
            };
        }
    }
}
