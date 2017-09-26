using System;
using System.Linq;
using Griffin.Container;
using log4net;
using OneTrueError.Infrastructure.Queueing;
using OneTrueError.ReportAnalyzer.Domain.Reports;
using OneTrueError.ReportAnalyzer.LibContracts;

namespace OneTrueError.ReportAnalyzer.Scanners
{
    /// <summary>
    ///     Loads a set of reports that should be analyzed and then cast some wizardry on them.
    /// </summary>
    [Component]
    public class ScanForNewErrorReports
    {
        private readonly Services.ReportAnalyzer _analyzer;
        private readonly ILog _logger = LogManager.GetLogger(typeof(ScanForNewErrorReports));
        private readonly IMessageQueue _queue;

        /// <summary>
        ///     Creates a new instance of <see cref="ScanForNewErrorReports" />.
        /// </summary>
        /// <param name="analyzer">to analyze inbound reprots</param>
        /// <param name="queueProvider">To be able to read inbound reports</param>
        public ScanForNewErrorReports(Services.ReportAnalyzer analyzer,
            IMessageQueueProvider queueProvider)
        {
            _analyzer = analyzer;
            _queue = queueProvider.Open("ReportQueue");
        }


        /// <summary>
        ///     Execute on a set of report mastery.
        /// </summary>
        /// <returns><c>false</c> if there are no more reports to analyze.</returns>
        public bool Execute()
        {
            using (var transaction = _queue.BeginTransaction())
            {
                var dto = _queue.TryReceive<ReceivedReportDTO>(transaction, TimeSpan.FromMilliseconds(500));
                if (dto == null)
                    return false;

                try
                {
                    ErrorReportException ex = null;
                    if (dto.Exception != null)
                    {
                        ex = ConvertException(dto.Exception);
                    }
                    var contexts = dto.ContextCollections.Select(ConvertContext).ToArray();
                    var entity = new ErrorReportEntity(dto.ApplicationId, dto.ReportId, dto.CreatedAtUtc, ex, contexts)
                    {
                        RemoteAddress = dto.RemoteAddress
                    };
                    _analyzer.Analyze(entity);
                }
                catch (Exception ex)
                {
                    _logger.Error("Failed to analyze report ", ex);
                }

                transaction.Commit();
            }
            return true;
        }

        private ErrorReportContext ConvertContext(ReceivedReportContextInfo arg)
        {
            return new ErrorReportContext(arg.Name, arg.Properties);
        }

        private ErrorReportException ConvertException(ReceivedReportException dto)
        {
            var entity = new ErrorReportException
            {
                Message = dto.Message,
                FullName = dto.FullName,
                Name = dto.Name,
                AssemblyName = dto.AssemblyName,
                BaseClasses = dto.BaseClasses,
                Everything = dto.Everything,
                Namespace = dto.Namespace,
                StackTrace = dto.StackTrace
            };
            if (dto.InnerException != null)
                entity.InnerException = ConvertException(dto.InnerException);
            return entity;
        }
    }
}