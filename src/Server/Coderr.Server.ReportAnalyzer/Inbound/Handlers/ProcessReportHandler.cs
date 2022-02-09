using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Domain.Core.ErrorReports;
using DotNetCqs;
using Coderr.Server.ReportAnalyzer.Abstractions.Inbound.Commands;
using log4net;

namespace Coderr.Server.ReportAnalyzer.Inbound.Handlers
{
    public class ProcessReportHandler : IMessageHandler<ProcessReport>
    {
        private readonly Reports.IReportAnalyzer _analyzer;
        private readonly ILog _logger = LogManager.GetLogger(typeof(ProcessReportHandler));

        public ProcessReportHandler(Reports.IReportAnalyzer analyzer)
        {
            _analyzer = analyzer;
        }


        public async Task HandleAsync(IMessageContext context, ProcessReport message)
        {
            try
            {
                ErrorReportException ex = null;
                if (message.Exception != null)
                    ex = ConvertException(message.Exception);
                var contexts = message.ContextCollections.Select(ConvertContext).ToList();
                ConvertContextDataToCollections(contexts);

                var entity = new ErrorReportEntity(message.ApplicationId, message.ReportId, message.CreatedAtUtc, ex,
                    contexts)
                {
                    RemoteAddress = message.RemoteAddress,
                    EnvironmentName = message.EnvironmentName
                };


                await _analyzer.Analyze(context, entity);

                // 0 = we ignored the report.
                if (entity.Id > 0)
                {
                    await ProcessLogEntries(message, entity);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to analyze report ", ex);
            }
        }

        private static void ConvertContextDataToCollections(ICollection<ErrorReportContextCollection> contexts)
        {
            var collection = contexts.FirstOrDefault(x => x.Name == "ContextData");
            if (collection == null)
            {
                return;
            }

            foreach (var property in collection.Properties)
            {
                var pos = property.Key.IndexOf('.');
                if (pos == -1 || pos == property.Key.Length - 1)
                {
                    continue;
                }

                var contextName = property.Key.Substring(0, pos);
                var propertyName = property.Key.Substring(pos + 1);
                var newContext = contexts.FirstOrDefault(x => x.Name == contextName);
                if (newContext == null)
                {
                    newContext = new ErrorReportContextCollection(contextName,
                        new Dictionary<string, string>());
                    contexts.Add(newContext);
                }

                newContext.Properties[propertyName] = property.Value;
            }
        }

        private async Task ProcessLogEntries(ProcessReport message, ErrorReportEntity entity)
        {
            if (message.LogEntries == null || message.LogEntries.Length == 0)
            {
                return;
            }

            // Only in commercial editions.
        }


        private ErrorReportContextCollection ConvertContext(ProcessReportContextInfoDto arg)
        {
            return new ErrorReportContextCollection(arg.Name, arg.Properties);
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