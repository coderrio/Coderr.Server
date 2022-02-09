using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Coderr.Server.ReportAnalyzer.Abstractions.ErrorReports;
using Coderr.Server.ReportAnalyzer.Abstractions.Inbound.Commands;
using Coderr.Server.ReportAnalyzer.Abstractions.Inbound.Models;
using DotNetCqs;
using DotNetCqs.Queues;
using Griffin.Data;
using log4net;
using Newtonsoft.Json;

namespace Coderr.Server.ReportAnalyzer.Inbound
{
    /// <summary>
    ///     Validates inbound report and store it in our internal queue for analysis.
    /// </summary>
    public class SaveReportHandler
    {
        private static readonly DuplicateChecker DuplicateChecker = new DuplicateChecker();
        private static readonly Dictionary<int, RateLimit> _lastReportPerApplication = new Dictionary<int, RateLimit>();
        private readonly List<Func<NewReportDTO, bool>> _filters = new List<Func<NewReportDTO, bool>>();
        private readonly ILog _logger = LogManager.GetLogger(typeof(SaveReportHandler));
        private readonly int _maxSizeForJsonErrorReport;
        private readonly IMessageQueue _queue;
        private readonly IAdoNetUnitOfWork _unitOfWork;

        /// <summary>
        ///     Creates a new instance of <see cref="SaveReportHandler" />.
        /// </summary>
        /// <param name="queue">Queue to store inbound reports in</param>
        /// <exception cref="ArgumentNullException">queueProvider;connectionFactory</exception>
        public SaveReportHandler(IMessageQueue queue, IAdoNetUnitOfWork unitOfWork, IReportConfig reportConfig)
        {
            _unitOfWork = unitOfWork;
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));
            _maxSizeForJsonErrorReport = reportConfig.MaxReportJsonSize;
        }

        public void AddFilter(Func<NewReportDTO, bool> filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));
            _filters.Add(filter);
        }

        public async Task BuildReportAsync(ClaimsPrincipal user, string appKey, string signatureProvidedByTheClient,
            string remoteAddress,
            byte[] reportBody)
        {
            if (!Guid.TryParse(appKey, out var tempKey))
            {
                _logger.Warn("Incorrect appKeyFormat: " + appKey + " from " + remoteAddress);
                throw new InvalidCredentialException("AppKey must be a valid GUID which '" + appKey + "' is not.");
            }

            var application = await GetAppAsync(appKey);
            if (application == null)
            {
                _logger.Warn($"Unknown appKey: {appKey} from {remoteAddress}");
                throw new InvalidCredentialException($"AppKey was not found in the database. Key '{appKey}'.");
            }

            lock (_lastReportPerApplication)
            {
                if (_lastReportPerApplication.TryGetValue(application.Id, out var limiter))
                {
                    if (limiter.IsRateExceeded())
                    {
                        _logger.Info($"Rate limit is exceeded for application {application.Id}.");
                        return;
                    }
                        
                    limiter.AddReport();
                }
                else
                {
                    _lastReportPerApplication[application.Id] = new RateLimit();
                }
            }


            // web(js) applications do not sign the body
            if (signatureProvidedByTheClient != null && !ReportValidator.ValidateBody(application.SharedSecret,
                signatureProvidedByTheClient, reportBody))
            {
                await StoreInvalidReportAsync(appKey, signatureProvidedByTheClient, remoteAddress, reportBody);
                throw new AuthenticationException(
                    "You either specified the wrong SharedSecret, or someone tampered with the data.");
            }

            var report = DeserializeBody(reportBody);
            if (report == null)
                return;

            if (Debugger.IsAttached) _logger.Debug("Received " + JsonConvert.SerializeObject(report));

            // Ignore our weird error where the HashSource is incorrect
            if (report.Exception?.Message.Contains("Slow POST /receiver/report") == true) return;

            if (DuplicateChecker.IsDuplicate(remoteAddress, report))
            {
                _logger.Debug($"Duplicate report {report.ReportId} from {remoteAddress}");
                return;
            }

            // correct incorrect clients
            if (report.CreatedAtUtc > DateTime.UtcNow)
                report.CreatedAtUtc = DateTime.UtcNow;

            if (_filters.Any(x => !x(report)))
                return;

            var internalDto = new ProcessReport
            {
                ApplicationId = application.Id,
                RemoteAddress = remoteAddress,
                ContextCollections = report.ContextCollections.Select(ConvertCollection).ToArray(),
                CreatedAtUtc = report.CreatedAtUtc,
                DateReceivedUtc = DateTime.UtcNow,
                EnvironmentName = report.EnvironmentName,
                Exception = ConvertException(report.Exception),
                LogEntries = ConvertLogEntries(report.LogEntries),
                ReportId = report.ReportId,
                ReportVersion = report.ReportVersion
            };

            await StoreReportAsync(user, internalDto);
        }

        private static ProcessReportContextInfoDto ConvertCollection(NewReportContextInfo arg)
        {
            return new ProcessReportContextInfoDto(arg.Name, arg.Properties);
        }

        private static ProcessReportExceptionDto ConvertException(NewReportException exception)
        {
            var ex = new ProcessReportExceptionDto
            {
                Name = exception.Name,
                AssemblyName = exception.AssemblyName,
                BaseClasses = exception.BaseClasses,
                Everything = exception.Everything,
                FullName = exception.FullName,
                Message = exception.Message,
                Namespace = exception.Namespace,
                Properties = exception.Properties,
                StackTrace = exception.StackTrace
            };
            if (exception.InnerException != null)
                ex.InnerExceptionDto = ConvertException(exception.InnerException);
            return ex;
        }

        private ProcessReportLogEntry[] ConvertLogEntries(NewReportLogEntry[] dtos)
        {
            if (dtos == null) return null;

            return dtos
                .Select(x => new ProcessReportLogEntry
                {
                    Message = x.Message,
                    Exception = x.Exception,
                    LogLevel = x.LogLevel,
                    Source = x.Source,
                    TimestampUtc = x.TimestampUtc
                })
                .ToArray();
        }

        private NewReportDTO DeserializeBody(byte[] body)
        {
            string json;
            if (body[0] == 0x1f && body[1] == 0x8b)
            {
                var decompressor = new ReportDecompressor();
                json = decompressor.Deflate(body);
            }
            else
            {
                json = Encoding.UTF8.GetString(body);
            }

            // protection against very large error reports.
            if (json.Length > _maxSizeForJsonErrorReport)
                return null;

            // to support clients that still use the OneTrueError client library.
            json = json.Replace("OneTrueError", "Coderr");

            var dto = JsonConvert.DeserializeObject<NewReportDTO>(json,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    ContractResolver =
                        new IncludeNonPublicMembersContractResolver()
                });

            //if (Debugger.IsAttached && Environment.MachineName.IndexOf("jg", StringComparison.OrdinalIgnoreCase) != -1)
            //    File.WriteAllText($@"C:\Temp\Report_{dto.Exception.Name}.{DateTime.Now:yyyyMMdd_hhmmss_fff}.json",
            //        json);

            if (string.IsNullOrEmpty(dto.EnvironmentName) && !string.IsNullOrEmpty(dto.Environment))
                dto.EnvironmentName = dto.Environment;

            // Safeguard against malformed reports (other clients than the built in ones)
            if (dto.Exception == null)
                return null;
            if (string.IsNullOrWhiteSpace(dto.Exception.Name) && string.IsNullOrWhiteSpace(dto.Exception.FullName))
                return null;
            if (string.IsNullOrWhiteSpace(dto.Exception.Name))
                dto.Exception.Name = dto.Exception.FullName;
            if (string.IsNullOrWhiteSpace(dto.Exception.FullName))
                dto.Exception.FullName = dto.Exception.Name;
            if (dto.Exception.BaseClasses == null)
                dto.Exception.BaseClasses = new string[0];
            if (dto.Exception.Namespace == null)
                dto.Exception.Namespace = "";

            return dto;
        }

        private async Task<AppInfo> GetAppAsync(string appKey)
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = "SELECT Id, SharedSecret FROM Applications WHERE AppKey = @key OR AppKey = @key2";
                cmd.AddParameter("key", appKey);
                cmd.AddParameter("key2", appKey.Replace("-", ""));
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;

                    return new AppInfo {Id = reader.GetInt32(0), SharedSecret = reader.GetString(1)};
                }
            }
        }

        private async Task StoreInvalidReportAsync(string appKey, string sig, string remoteAddress, byte[] reportBody)
        {
            try
            {
                //TODO: Make something generic.
                using (var cmd = (DbCommand)_unitOfWork.CreateCommand())
                {
                    cmd.CommandText =
                        @"INSERT INTO InvalidReports(appkey, signature, reportbody, errormessage, createdatutc)
                                            VALUES (@appkey, @signature, @reportbody, @errormessage, @createdatutc);";
                    cmd.AddParameter("appKey", appKey);
                    cmd.AddParameter("signature", sig);
                    var p = cmd.CreateParameter();
                    p.DbType = DbType.Binary;
                    p.ParameterName = "reportbody";
                    p.Value = reportBody;
                    cmd.Parameters.Add(p);
                    //cmd.AddParameter("reportbody", reportBody);
                    cmd.AddParameter("errormessage", "Failed to validate signature");
                    cmd.AddParameter("createdatutc", DateTime.UtcNow);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to save invalid report.", ex);
            }
        }

        private async Task StoreReportAsync(ClaimsPrincipal user, ProcessReport report)
        {
            try
            {
                using (var session = _queue.BeginSession())
                {
                    await session.EnqueueAsync(user, new Message(report));
                    await session.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(
                    "Failed to StoreReport: " + JsonConvert.SerializeObject(new {model = report}), ex);
            }
        }

        private class RateLimit
        {
            private readonly LinkedList<DateTime> _reportDates = new LinkedList<DateTime>();

            public RateLimit()
            {
                _reportDates.AddLast(DateTime.UtcNow);
            }

            public void AddReport()
            {
                lock (_reportDates)
                {
                    _reportDates.AddLast(DateTime.UtcNow);
                }
            }

            public bool IsRateExceeded()
            {
                if (Debugger.IsAttached)
                {
                    return false;
                }

                var twentySecondsAgo = DateTime.UtcNow.AddSeconds(-20);
                lock (_reportDates)
                {
                    while (_reportDates.First != null && _reportDates.First.Value < twentySecondsAgo)
                        _reportDates.RemoveFirst();

                    return _reportDates.Count > 20;
                }
            }
        }

        private class AppInfo
        {
            public int Id { get; set; }
            public string SharedSecret { get; set; }
        }
    }
}