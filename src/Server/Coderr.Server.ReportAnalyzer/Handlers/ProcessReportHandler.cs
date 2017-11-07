using System;
using System.Linq;
using System.Threading.Tasks;
using codeRR.Server.ReportAnalyzer.Domain.Reports;
using codeRR.Server.ReportAnalyzer.LibContracts;
using DotNetCqs;
using Griffin.Container;
using log4net;

namespace codeRR.Server.ReportAnalyzer.Handlers
{
    [Component]
    public class ProcessReportHandler : IMessageHandler<ProcessReport>
    {
        private readonly Reports.ReportAnalyzer _analyzer;
        private readonly ILog _logger = LogManager.GetLogger(typeof(ProcessReportHandler));

        public ProcessReportHandler(Reports.ReportAnalyzer analyzer)
        {
            _analyzer = analyzer;
        }


        public Task HandleAsync(IMessageContext context, ProcessReport message)
        {
            try
            {
                ErrorReportException ex = null;
                if (message.Exception != null)
                    ex = ConvertException(message.Exception);
                var contexts = message.ContextCollections.Select(ConvertContext).ToArray();
                var entity = new ErrorReportEntity(message.ApplicationId, message.ReportId, message.CreatedAtUtc, ex,
                    contexts)
                {
                    RemoteAddress = message.RemoteAddress
                };
                _analyzer.Analyze(context, entity);
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to analyze report ", ex);
            }

            return Task.FromResult<object>(null);
        }


        private ErrorReportContext ConvertContext(ProcessReportContextInfoDto arg)
        {
            return new ErrorReportContext(arg.Name, arg.Properties);
        }

        private ErrorReportException ConvertException(ProcessReportExceptionDto dto)
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
            if (dto.InnerExceptionDto != null)
                entity.InnerException = ConvertException(dto.InnerExceptionDto);
            return entity;
        }
    }
}