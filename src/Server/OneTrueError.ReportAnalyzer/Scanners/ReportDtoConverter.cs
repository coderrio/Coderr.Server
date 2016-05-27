using System.Linq;
using Newtonsoft.Json;
using OneTrueError.ReportAnalyzer.Domain.Reports;
using OneTrueError.ReportAnalyzer.LibContracts;

namespace OneTrueError.ReportAnalyzer.Scanners
{
    /// <summary>
    ///     Converts DTOs from the client library format to our internal DTO.
    /// </summary>
    public class ReportDtoConverter
    {
        /// <summary>
        ///     Convert exception to our internal format
        /// </summary>
        /// <param name="exception">exception</param>
        /// <returns>our format</returns>
        public ErrorReportException ConvertException(ReceivedReportException exception)
        {
            var ex = new ErrorReportException
            {
                AssemblyName = exception.AssemblyName,
                BaseClasses = exception.BaseClasses,
                Everything = exception.Everything,
                FullName = exception.FullName,
                Message = exception.Message,
                Name = exception.Name,
                Namespace = exception.Namespace,
                StackTrace = exception.StackTrace
            };
            if (exception.InnerException != null)
                ex.InnerException = ConvertException(exception.InnerException);
            return ex;
        }

        /// <summary>
        ///     Convert received report to our internal format
        /// </summary>
        /// <param name="report">client report</param>
        /// <param name="applicationId">application that we identified that the report belongs to</param>
        /// <returns>internal format</returns>
        public ErrorReportEntity ConvertReport(ReceivedReportDTO report, int applicationId)
        {
            ErrorReportException ex = null;
            if (report.Exception != null)
            {
                ex = ConvertException(report.Exception);
            }

            //var id = _idGeneratorClient.GetNextId(ErrorReportEntity.SEQUENCE);
            var contexts = report.ContextCollections.Select(x => new ErrorReportContext(x.Name, x.Items)).ToArray();
            var dto = new ErrorReportEntity(applicationId, report.ReportId, report.CreatedAtUtc, ex, contexts);
            return dto;
        }

        /// <summary>
        ///     Deserialize a client library formatted report
        /// </summary>
        /// <param name="json">JSON</param>
        /// <returns>DTO</returns>
        public ReceivedReportDTO LoadReportFromJson(string json)
        {
            var report =
                JsonConvert.DeserializeObject<ReceivedReportDTO>(json, new JsonSerializerSettings
                {
                    ObjectCreationHandling = ObjectCreationHandling.Auto,
                    TypeNameHandling = TypeNameHandling.Auto,
                    ContractResolver = new IncludeNonPublicMembersContractResolver(),
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                });
            return report;
        }
    }
}