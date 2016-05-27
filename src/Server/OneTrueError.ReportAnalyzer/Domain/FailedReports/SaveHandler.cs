using System;
using System.Data.Common;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Griffin.Data;
using log4net;
using Newtonsoft.Json;
using OneTrueError.Infrastructure;
using OneTrueError.ReportAnalyzer.Domain.Reports;
using OneTrueError.ReportAnalyzer.LibContracts;

namespace OneTrueError.ReportAnalyzer.Domain.FailedReports
{
    /// <summary>
    /// TODO: Remove or refactor?
    /// </summary>
    internal class SaveReportHandlerOld
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof (SaveReportHandlerOld));
        private IAdoNetUnitOfWork _unitOfWork;

        public SaveReportHandlerOld(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        protected CustomerApplication GetApplication(string appKey)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = "SELECT SharedSecret FROM Applications WHERE ApplicationKey = @key";
                cmd.AddParameter("key", appKey);
                var secret = (string) cmd.ExecuteScalar();
                return new CustomerApplication
                {
                    SharedSecret = secret
                };

            }
        }
        public async Task<bool> BuildReportAsync(string fileId, string appKey, string sig, byte[] reportBody)
        {
            Guid appKeyGuid;
            if (!Guid.TryParse(appKey, out appKeyGuid))
            {
                _logger.Warn("Incorrect appKeyFormat: " + appKey + ".");
                return true;
            }


            var customer = GetApplication(appKey);
            if (customer == null)
            {
                _logger.Error("Failed to identify appKey " + appKey + ".");
                return false;
            }

            if (!customer.ValidateBody(sig, reportBody))
            {
                _logger.Error("Failed to validate body for " + fileId);
                return true;
            }

            var report = DeserializeBody(reportBody);
            await StoreReport(appKey, "", report);
            return true;
        }

        private ReceivedReportDTO DeserializeBody(byte[] body)
        {
            var decompressor = new ReportDecompressor();
            var json = decompressor.Deflate(body);

            return JsonConvert.DeserializeObject<ReceivedReportDTO>(json,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    ContractResolver =
                        new IncludeNonPublicMembersContractResolver()
                });
        }

        private async Task StoreReport(string appKey, string remoteAddress, ReceivedReportDTO report)
        {
            try
            {
                using (var connection = ConnectionFactory.Create())
                {
                    using (var cmd = (DbCommand) connection.CreateCommand())
                    {
                        cmd.CommandText =
                            @"INSERT INTO QueueReports (appkey, CreatedAtUtc, RemoteAddress, Body)
                                            VALUES (@appkey, @CreatedAtUtc, @RemoteAddress, @Body);";
                        cmd.AddParameter("createdatutc", DateTime.UtcNow);
                        cmd.AddParameter("appKey", appKey);
                        cmd.AddParameter("RemoteAddress", remoteAddress);
                        cmd.AddParameter("Body", JsonConvert.SerializeObject(report));
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Warn(
                    "Failed to StoreReport: " + JsonConvert.SerializeObject(new {appKey, model = report}), ex);
            }
        }
    }
}