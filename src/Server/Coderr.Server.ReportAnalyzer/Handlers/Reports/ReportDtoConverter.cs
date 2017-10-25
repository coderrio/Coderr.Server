using System.Linq;
using codeRR.Server.ReportAnalyzer.Domain.Reports;
using codeRR.Server.ReportAnalyzer.LibContracts;
using Newtonsoft.Json;

namespace codeRR.Server.ReportAnalyzer.Scanners
{
    /// <summary>
    ///     Converts DTOs from the client library format to our internal DTO.
    /// </summary>
    public class ReportDtoConverter
    {
        /// <summary>
        ///     Convert exception to our internal format
        /// </summary>
        /// <param name="exceptionDto">exception</param>
        /// <returns>our format</returns>
        public ErrorReportException ConvertException(ProcessReportExceptionDto exceptionDto)
        {
            var ex = new ErrorReportException
            {
                AssemblyName = exceptionDto.AssemblyName,
                BaseClasses = exceptionDto.BaseClasses,
                Everything = exceptionDto.Everything,
                FullName = exceptionDto.FullName,
                Message = exceptionDto.Message,
                Name = exceptionDto.Name,
                Namespace = exceptionDto.Namespace,
                StackTrace = exceptionDto.StackTrace
            };
            if (exceptionDto.InnerExceptionDto != null)
                ex.InnerException = ConvertException(exceptionDto.InnerExceptionDto);
            return ex;
        }

        /// <summary>
        ///     Convert received report to our internal format
        /// </summary>
        /// <param name="report">client report</param>
        /// <param name="applicationId">application that we identified that the report belongs to</param>
        /// <returns>internal format</returns>
        public ErrorReportEntity ConvertReport(ProcessReport report, int applicationId)
        {
            ErrorReportException ex = null;
            if (report.Exception != null)
            {
                ex = ConvertException(report.Exception);
            }

            //var id = _idGeneratorClient.GetNextId(ErrorReportEntity.SEQUENCE);
            var contexts = report.ContextCollections.Select(x => new ErrorReportContext(x.Name, x.Properties)).ToArray();
            var dto = new ErrorReportEntity(applicationId, report.ReportId, report.CreatedAtUtc, ex, contexts);
            return dto;
        }

        /// <summary>
        ///     Deserialize a client library formatted report
        /// </summary>
        /// <param name="json">JSON</param>
        /// <returns>DTO</returns>
        public ProcessReport LoadReportFromJson(string json)
        {
            var report =
                JsonConvert.DeserializeObject<ProcessReport>(json, new JsonSerializerSettings
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