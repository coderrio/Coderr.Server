using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Griffin.Data;
using Griffin.Data.Mapper;
using log4net;
using Newtonsoft.Json;
using OneTrueError.Infrastructure;
using OneTrueError.Infrastructure.Queueing;
using OneTrueError.ReportAnalyzer.LibContracts;
using OneTrueError.Web.Areas.Receiver.Models;
using OneTrueError.Web.Areas.Receiver.ReportingApi;

namespace OneTrueError.Web.Areas.Receiver.Helpers
{
    /// <summary>
    /// Validates inbound report and store it in our internal queue for analysis.
    /// </summary>
    public class SaveReportHandler
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof (SaveReportHandler));
        private IMessageQueue _queue;

        /// <summary>
        /// Creates a new instance of <see cref="SaveReportHandler"/>.
        /// </summary>
        /// <param name="queueProvider">provider</param>
        public SaveReportHandler(IMessageQueueProvider queueProvider)
        {
            if (queueProvider == null) throw new ArgumentNullException("queueProvider");
            _queue = queueProvider.Open("ReportQueue");
        }

        public async Task BuildReportAsync(string appKey, string signatureProvidedByTheClient, string remoteAddress, byte[] reportBody)
        {
            Guid tempKey;
            if (!Guid.TryParse(appKey, out tempKey))
            {
                _logger.Warn("Incorrect appKeyFormat: " + appKey + " from " + remoteAddress);
                throw new HttpException(400, "AppKey must be a valid GUID which '" + appKey + "' is not.");
            }

            var application = await GetAppAsync(appKey);
            if (application == null)
            {
                _logger.Warn("Unknown appKey: " + appKey + " from " + remoteAddress);
                throw new HttpException(400, "AppKey was not found in the database. Key '" + appKey + "'.");
            }

            if (!ReportValidator.ValidateBody(application.SharedSecret, signatureProvidedByTheClient, reportBody))
            {
                await StoreInvalidReportAsync(appKey, signatureProvidedByTheClient, remoteAddress, reportBody);
                throw new HttpException(403,
                    "You either specified the wrong SharedSecret, or someone tampered with the data.");
            }

            var report = DeserializeBody(reportBody);
            var internalDto = new ReceivedReportDTO
            {
                ApplicationId = application.Id,
                RemoteAddress = remoteAddress,
                ContextCollections = report.ContextCollections.Select(ConvertCollection).ToArray(),
                CreatedAtUtc = report.CreatedAtUtc,
                DateReceivedUtc = DateTime.UtcNow,
                Exception = ConvertException(report.Exception),
                ReportId = report.ReportId,
                ReportVersion = report.ReportVersion
            };

            await StoreReportAsync(internalDto);
        }

        private static async Task<AppInfo> GetAppAsync(string appKey)
        {
            using (var con = ConnectionFactory.Create())
            {
                using (var cmd = con.CreateDbCommand())
                {
                    cmd.CommandText = "SELECT Id, SharedSecret FROM Applications WHERE AppKey = @key";
                    cmd.AddParameter("key", appKey);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (!await reader.ReadAsync())
                            return null;

                        return new AppInfo
                        {
                            Id = reader.GetInt32(0),
                            SharedSecret = reader.GetString(1)
                        };
                    }
                }
            }
        }

        private static ReceivedReportException ConvertException(NewReportException exception)
        {
            var ex = new ReceivedReportException
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
                ex.InnerException = ConvertException(exception.InnerException);
            return ex;
        }

        private static ReceivedReportContextInfo ConvertCollection(NewReportContextInfo arg)
        {
             return new ReceivedReportContextInfo(arg.Name, arg.Properties);
        }

        private NewReportDTO DeserializeBody(byte[] body)
        {
            var decompressor = new ReportDecompressor();
            var json = decompressor.Deflate(body);

            return JsonConvert.DeserializeObject<NewReportDTO>(json,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    ContractResolver =
                        new IncludeNonPublicMembersContractResolver()
                });
        }

        private async Task StoreInvalidReportAsync(string appKey, string sig, string remoteAddress, byte[] reportBody)
        {
            try
            {
                using (var connection = ConnectionFactory.Create())
                {
                    //TODO: Make something generic.
                    using (var cmd = (SqlCommand) connection.CreateCommand())
                    {
                        cmd.CommandText =
                            @"INSERT INTO InvalidReports(appkey, signature, reportbody, errormessage, createdatutc)
                                            VALUES (@appkey, @signature, @reportbody, @errormessage, @createdatutc);";
                        cmd.AddParameter("appKey", appKey);
                        cmd.AddParameter("signature", sig);
                        var p = cmd.CreateParameter();
                        p.SqlDbType = SqlDbType.Image;
                        p.ParameterName = "reportbody";
                        p.Value = reportBody;
                        cmd.Parameters.Add(p);
                        //cmd.AddParameter("reportbody", reportBody);
                        cmd.AddParameter("errormessage", "Failed to validate signature");
                        cmd.AddParameter("createdatutc", DateTime.UtcNow);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception)
            {
                //TODO: LOG
            }
        }

#pragma warning disable 1998
        private async Task StoreReportAsync(ReceivedReportDTO report)
#pragma warning restore 1998
        {
            try
            {
                _queue.Write(report.ApplicationId, report);
            }
            catch (Exception ex)
            {
                _logger.Warn(
                    "Failed to StoreReport: " + JsonConvert.SerializeObject(new {model = report}), ex);
            }
        }

        private class AppInfo
        {
            public int Id { get; set; }
            public string SharedSecret { get; set; }
        }
    }
}