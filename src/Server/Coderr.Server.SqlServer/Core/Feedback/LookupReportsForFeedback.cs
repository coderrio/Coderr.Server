using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using codeRR.Server.App.Core.Feedback;
using Griffin.ApplicationServices;
using Griffin.Data;
using Griffin.Data.Mapper;
using log4net;

namespace codeRR.Server.SqlServer.Core.Feedback
{
    //TODO: invent some way to execute jobs for all customer databases.
    //[Component(RegisterAsSelf = true)]
    public class LookupReportsForFeedback : IBackgroundJobAsync
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(LookupReportsForFeedback));
        private readonly IAdoNetUnitOfWork _unitOfWork;


        public LookupReportsForFeedback(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync()
        {
            var items = new List<FeedbackEntity>();
            await GetPendingFeedback(items);
            await LookupReportInfo(items);
            foreach (var item in items)
            {
                if (item.CanUpdate)
                {
                    using (var cmd = (DbCommand) _unitOfWork.CreateCommand())
                    {
                        _logger.Debug("Attaching report to " + item.ReportId + " to feedback " + item.Id);
                        cmd.CommandText =
                            "UPDATE IncidentFeedback SET IncidentId=@incidentId, ApplicationId = @appId, ReportId = @reportId WHERE Id = @id";
                        cmd.AddParameter("incidentId", item.IncidentId);
                        cmd.AddParameter("appId", item.ApplicationId);
                        cmd.AddParameter("reportId", item.ReportId);
                        cmd.AddParameter("id", item.Id);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                else if (item.CanRemove)
                {
                    using (var cmd = (DbCommand) _unitOfWork.CreateCommand())
                    {
                        _logger.Debug("Deleting feedback " + item.Id);
                        cmd.CommandText =
                            "DELETE FROM IncidentFeedback WHERE Id = @id";
                        cmd.AddParameter("id", item.Id);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                else
                {
                    _logger.Debug("Paria: " + item.Id + "/" + item.ErrorId);
                }
            }
        }

        private async Task GetPendingFeedback(ICollection<FeedbackEntity> items)
        {
            using (var cmd = (DbCommand) _unitOfWork.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT * FROM incidentfeedback WHERE IncidentId is null";
                var myItems = await cmd.ToListAsync<FeedbackEntity>();
                foreach (var item in myItems)
                {
                    items.Add(item);
                }

                if (items.Count > 0)
                    if (items.Count > 0)
                        _logger.Debug("Added " + items.Count + " items.");
            }
        }

        private async Task LookupReportInfo(IEnumerable<FeedbackEntity> items)
        {
            foreach (var item in items)
            {
                using (var cmd = (DbCommand) _unitOfWork.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, ApplicationId, IncidentId FROM ErrorReports WITH(NOLOCK) WHERE ";
                    if (item.ErrorId != null)
                    {
                        cmd.CommandText += "ErrorId = @id";
                        cmd.AddParameter("id", item.ErrorId);
                    }
                    else
                    {
                        cmd.CommandText += "Id = @id";
                        cmd.AddParameter("id", item.ReportId);
                    }

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (!await reader.ReadAsync())
                            continue;

                        item.AssignToReport((int) reader["Id"],
                            (int) reader["IncidentId"],
                            (int) reader["ApplicationId"]);
                        _logger.Debug("Identified report " + item.ReportId + "/" + item.ReportId + " for feedback " +
                                      item.Id);
                    }
                }
            }
        }
    }
}