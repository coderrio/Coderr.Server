using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Domain.Core.ErrorReports;
using Coderr.Server.Domain.Core.Incidents.Events;
using Coderr.Server.ReportAnalyzer.Abstractions;
using Coderr.Server.ReportAnalyzer.Abstractions.ErrorReports;
using Coderr.Server.ReportAnalyzer.Abstractions.Incidents;
using Coderr.Server.ReportAnalyzer.ErrorReports;
using Coderr.Server.ReportAnalyzer.Incidents;
using DotNetCqs;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Coderr.Server.ReportAnalyzer.Inbound.Handlers.Reports
{
    /// <summary>
    ///     Runs analysis for the report.
    /// </summary>
    [ContainerService]
    public class ReportAnalyzer : IReportAnalyzer
    {
        public const string AppAssemblyVersion = "AppAssemblyVersion";
        private readonly IHashCodeGenerator _hashCodeGenerator;
        private readonly ILog _logger = LogManager.GetLogger(typeof(ReportAnalyzer));
        private readonly IAnalyticsRepository _repository;
        private IDomainQueue _domainQueue;
        /// <summary>
        ///     Creates a new instance of <see cref="ReportAnalyzer" />.
        /// </summary>
        /// <param name="hashCodeGenerator">Used to identify is this is a new unique exception</param>
        /// <param name="messageBus">
        ///     to publish the
        ///     <see cref="Coderr.Server.ReportAnalyzer.Abstractions.Incidents.ReportAddedToIncident" /> event
        /// </param>
        /// <param name="repository">repos</param>
        public ReportAnalyzer(IHashCodeGenerator hashCodeGenerator, IAnalyticsRepository repository, IDomainQueue domainQueue)
        {
            _hashCodeGenerator = hashCodeGenerator;
            _repository = repository;
            _domainQueue = domainQueue;
        }

        /// <summary>
        ///     Analyze report
        /// </summary>
        /// <param name="report">report</param>
        /// <exception cref="ArgumentNullException">report</exception>
        public async Task Analyze(IMessageContext context, ErrorReportEntity report)
        {
            if (report == null) throw new ArgumentNullException("report");

            var countThisMonth = _repository.GetMonthReportCount();
            if (countThisMonth >= 500)
            {
                _repository.AddMissedReport(DateTime.Today);
                return;
            }

            var exists = _repository.ExistsByClientId(report.ClientReportId);
            if (exists)
            {
                _logger.Warn("Report have already been uploaded: " + report.ClientReportId);
                return;
            }

            try
            {
                var hashCode = _hashCodeGenerator.GenerateHashCode(report);
                report.Init(hashCode);
            }
            catch (Exception ex)
            {
                var reportJson = JsonConvert.SerializeObject(report);
                if (reportJson.Length > 1000000)
                    reportJson = reportJson.Substring(0, 100000) + "[....]";
                _logger.Fatal("Failed to init report " + reportJson, ex);
                return;
            }

            var applicationVersion = GetVersionFromReport(report);
            var isReOpened = false;
            var firstLine = report.GenerateHashCodeIdentifier();
            var incident = _repository.FindIncidentForReport(report.ApplicationId, report.ReportHashCode, firstLine);
            if (incident == null)
            {
                incident = BuildIncident(report);
                _repository.CreateIncident(incident);

                var evt = new IncidentCreated(incident.ApplicationId, incident.Id, incident.Description, incident.FullName)
                {
                    ApplicationVersion = applicationVersion
                };

                await _domainQueue.PublishAsync(context.Principal, evt);
                await context.SendAsync(evt);
            }
            else
            {
                if (incident.ReportCount > 1000)
                {
                    _logger.Debug("Report count is more than 10000. Ignoring report for incident " + incident.Id);
                    return;
                }

                if (incident.IsIgnored)
                {
                    _logger.Info("Incident is ignored: " + JsonConvert.SerializeObject(report));
                    incident.WasJustIgnored();
                    _repository.UpdateIncident(incident);
                    return;
                }

                if (incident.IsClosed)
                {
                    if (applicationVersion != null && incident.IsReportIgnored(applicationVersion))
                    {
                        _logger.Info("Ignored report since it's for a version less that the solution version: " + JsonConvert.SerializeObject(report));
                        incident.WasJustIgnored();
                        _repository.UpdateIncident(incident);
                        return;
                    }

                    isReOpened = true;
                    incident.ReOpen();
                    var evt = new IncidentReOpened(incident.ApplicationId, incident.Id,
                        incident.CreatedAtUtc)
                    {
                        ApplicationVersion = applicationVersion
                    };
                    await context.SendAsync(evt);
                    await _domainQueue.PublishAsync(context.Principal, evt);
                }

                incident.AddReport(report);
                _repository.UpdateIncident(incident);
            }

            report.IncidentId = incident.Id;
            _repository.CreateReport(report);
            _logger.Debug("saving report " + report.Id + " for incident " + incident.Id);
            var appName = _repository.GetAppName(incident.ApplicationId);

            var summary = new IncidentSummaryDTO(incident.Id, incident.Description)
            {
                ApplicationId = incident.ApplicationId,
                ApplicationName = appName,
                CreatedAtUtc = incident.CreatedAtUtc,
                LastUpdateAtUtc = incident.UpdatedAtUtc,
                IsReOpened = incident.IsReOpened,
                Name = incident.Description,
                ReportCount = incident.ReportCount
            };
            var sw = new Stopwatch();
            sw.Start();
            _logger.Debug("Publishing now: " + report.ClientReportId);
            var e = new ReportAddedToIncident(summary, ConvertToCoreReport(report), isReOpened);
            await context.SendAsync(e);

            await context.SendAsync(new ProcessInboundContextCollections());

            if (sw.ElapsedMilliseconds > 200)
                _logger.Debug("PublishAsync took " + sw.ElapsedMilliseconds);
            sw.Stop();
        }

        private string GetVersionFromReport(ErrorReportEntity report)
        {
            foreach (var contextCollection in report.ContextCollections)
            {
                if (contextCollection.Properties.TryGetValue(AppAssemblyVersion, out var version))
                    return version;
            }

            return null;
        }

        private IncidentBeingAnalyzed BuildIncident(ErrorReportEntity entity)
        {
            if (entity.Exception == null)
                return new IncidentBeingAnalyzed(entity);

            if (entity.Exception.Name == "AggregateException")
                try
                {
                    var exception = entity.Exception;

                    //TODO: Check if there are more than one InnerExceptions and then abort this specialization.
                    while (exception != null && exception.Name == "AggregateException")
                    {
                        exception = exception.InnerException;
                    }

                    var incident = new IncidentBeingAnalyzed(entity, exception);
                    return incident;
                }
                catch (Exception)
                {
                }

            if (entity.Exception.Name == "ReflectionTypeLoadException")
                try
                {
                    var item = JObject.Parse(entity.Exception.Everything);
                    var i = new IncidentBeingAnalyzed(entity);
                    var items = (JObject)item["LoaderExceptions"];
                    var exception = items.First;


                    //var incident = new Incident(entity, exception);
                    //incident.AddIncidentTags(new[] { "ReflectionTypeLoadException" });
                    //return incident;

                    //TODO: load LoaderExceptions which is an Exception[] array
                }
                catch (Exception)
                {
                }

            return new IncidentBeingAnalyzed(entity);
        }

        private ReportExeptionDTO ConvertToCoreException(ErrorReportException exception)
        {
            var ex = new ReportExeptionDTO
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
            if (ex.InnerException != null)
                ex.InnerException = ConvertToCoreException(exception.InnerException);
            return ex;
        }

        private ReportDTO ConvertToCoreReport(ErrorReportEntity report)
        {
            var dto = new ReportDTO
            {
                ApplicationId = report.ApplicationId,
                ContextCollections =
                    report.ContextCollections.Select(x => new ContextCollectionDTO(x.Name, x.Properties)).ToArray(),
                CreatedAtUtc = report.CreatedAtUtc,
                Id = report.Id,
                IncidentId = report.IncidentId,
                RemoteAddress = report.RemoteAddress,
                ReportId = report.ClientReportId,
                ReportVersion = "1"
            };
            if (report.Exception != null)
                dto.Exception = ConvertToCoreException(report.Exception);
            return dto;
        }
    }
}