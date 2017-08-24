using System;
using System.Diagnostics;
using System.Linq;
using DotNetCqs;
using Griffin.Container;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OneTrueError.Api.Core.Incidents;
using OneTrueError.Api.Core.Incidents.Events;
using OneTrueError.Api.Core.Reports;
using OneTrueError.ReportAnalyzer.Domain.Incidents;
using OneTrueError.ReportAnalyzer.Domain.Reports;

namespace OneTrueError.ReportAnalyzer.Services
{
    /// <summary>
    ///     Runs analysis for the report.
    /// </summary>
    [Component(Lifetime = Lifetime.Scoped)]
    public class ReportAnalyzer
    {
        private readonly IEventBus _eventBus;
        private readonly HashCodeGenerator _hashCodeGenerator;
        private readonly ILog _logger = LogManager.GetLogger(typeof(ReportAnalyzer));
        private readonly IAnalyticsRepository _repository;

        /// <summary>
        ///     Creates a new instance of <see cref="ReportAnalyzer" />.
        /// </summary>
        /// <param name="hashCodeGenerator">Used to identify is this is a new unique exception</param>
        /// <param name="eventBus">to publish the <see cref="ReportAddedToIncident" /> event</param>
        /// <param name="repository">repos</param>
        public ReportAnalyzer(HashCodeGenerator hashCodeGenerator, IEventBus eventBus, IAnalyticsRepository repository)
        {
            _hashCodeGenerator = hashCodeGenerator;
            _eventBus = eventBus;
            _repository = repository;
        }

        /// <summary>
        ///     Analyze report
        /// </summary>
        /// <param name="report">report</param>
        /// <exception cref="ArgumentNullException">report</exception>
        public void Analyze(ErrorReportEntity report)
        {
            if (report == null) throw new ArgumentNullException("report");

            var exists = _repository.ExistsByClientId(report.ClientReportId);
            if (exists)
            {
                _logger.Warn("Report have already been uploaded: " + report.ClientReportId);
                return;
            }

            try
            {
                report.Init(_hashCodeGenerator.GenerateHashCode(report));
            }
            catch (Exception ex)
            {
                _logger.Fatal("Failed to store report " + JsonConvert.SerializeObject(report), ex);
                return;
            }

            var isReOpened = false;
            var incident = _repository.FindIncidentForReport(report.ApplicationId, report.ReportHashCode,
                report.GenerateHashCodeIdentifier());


            if (incident == null)
            {
                incident = BuildIncident(report);
                _repository.CreateIncident(incident);
            }
            else
            {
                if (incident.ReportCount > 10000)
                {
                    _logger.Debug("Reportcount is more than 10000. Ignoring report for incident " + incident.Id);
                    return;
                }

                if (incident.IsIgnored)
                {
                    _logger.Info("Incident is ignored: " + JsonConvert.SerializeObject(report));
                    incident.WasJustIgnored();
                    _repository.UpdateIncident(incident);
                    return;
                }
                if (incident.IsSolved)
                {
                    isReOpened = true;
                    incident.ReOpen();
                    _eventBus.PublishAsync(new IncidentReOpened(incident.ApplicationId, incident.Id,
                        incident.CreatedAtUtc));
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
            _eventBus.PublishAsync(e);
            if (sw.ElapsedMilliseconds > 200)
                _logger.Debug("Publish took " + sw.ElapsedMilliseconds);
            sw.Stop();
        }

        private IncidentBeingAnalyzed BuildIncident(ErrorReportEntity entity)
        {
            if (entity.Exception == null)
                return new IncidentBeingAnalyzed(entity);

            if (entity.Exception.Name == "AggregateException")
            {
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
            }

            if (entity.Exception.Name == "ReflectionTypeLoadException")
            {
                try
                {
                    var item = JObject.Parse(entity.Exception.Everything);
                    var i = new IncidentBeingAnalyzed(entity);
                    var items = (JObject) item["LoaderExceptions"];
                    var exception = items.First;


                    //var incident = new Incident(entity, exception);
                    //incident.AddIncidentTags(new[] { "ReflectionTypeLoadException" });
                    //return incident;

                    //TODO: load LoaderExceptions which is an Exception[] array
                }
                catch (Exception)
                {
                }
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
                    report.ContextInfo.Select(x => new ContextCollectionDTO(x.Name, x.Properties)).ToArray(),
                CreatedAtUtc = report.CreatedAtUtc,
                Id = report.Id,
                IncidentId = report.IncidentId,
                RemoteAddress = report.RemoteAddress,
                ReportId = report.ClientReportId,
                ReportVersion = "1"
            };
            if (report.Exception != null)
            {
                dto.Exception = ConvertToCoreException(report.Exception);
            }
            return dto;
        }
    }
}