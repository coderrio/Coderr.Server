using System;
using System.Data.Common;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Feedback.Commands;
using codeRR.Server.Api.Core.Feedback.Events;
using codeRR.Server.Api.Core.Reports;
using codeRR.Server.App.Core.Reports;
using DotNetCqs;
using Griffin.Container;
using Griffin.Data;
using log4net;

namespace codeRR.Server.SqlServer.Core.Feedback.Commands
{
    [Component]
    public class SubmitFeedbackHandler : IMessageHandler<SubmitFeedback>
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(SubmitFeedbackHandler));
        private readonly IReportsRepository _reportsRepository;
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public SubmitFeedbackHandler(IAdoNetUnitOfWork unitOfWork, IReportsRepository reportsRepository)
        {
            _unitOfWork = unitOfWork;
            _reportsRepository = reportsRepository;
        }

        public async Task HandleAsync(IMessageContext context, SubmitFeedback command)
        {
            ReportDTO report;
            if (command.ReportId > 0)
                report = await _reportsRepository.GetAsync(command.ReportId);
            else
                report = await _reportsRepository.FindByErrorIdAsync(command.ErrorId);


            // storing it without connections as the report might not have been uploaded yet.
            if (report == null)
            {
                _logger.InfoFormat(
                    "Failed to find report. Let's enqueue it instead for report {0}/{1}. Email: {2}, Feedback: {3}",
                    command.ReportId, command.ErrorId, command.Email, command.Feedback);
                try
                {
                    using (var cmd = _unitOfWork.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO IncidentFeedback (ErrorReportId, RemoteAddress, Description, EmailAddress, CreatedAtUtc, Conversation, ConversationLength) "
                                          +
                                          "VALUES (@ErrorReportId, @RemoteAddress, @Description, @EmailAddress, @CreatedAtUtc, '', 0)";
                        cmd.AddParameter("ErrorReportId", command.ErrorId);
                        cmd.AddParameter("RemoteAddress", command.RemoteAddress);
                        cmd.AddParameter("Description", command.Feedback);
                        cmd.AddParameter("EmailAddress", command.Email);
                        cmd.AddParameter("CreatedAtUtc", DateTime.UtcNow);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception exception)
                {
                    _logger.Error(
                        string.Format("{0}: Failed to store '{1}' '{2}'", command.ErrorId, command.Email,
                            command.Feedback), exception);
                    //hide errors.
                }

                return;
            }

            using (var cmd = (DbCommand) _unitOfWork.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO IncidentFeedback (ErrorReportId, ApplicationId, ReportId, IncidentId, RemoteAddress, Description, EmailAddress, CreatedAtUtc, Conversation, ConversationLength) "
                                  +
                                  "VALUES (@ErrorReportId, @ApplicationId, @ReportId, @IncidentId, @RemoteAddress, @Description, @EmailAddress, @CreatedAtUtc, @Conversation, 0)";
                cmd.AddParameter("ErrorReportId", command.ErrorId);
                cmd.AddParameter("ApplicationId", report.ApplicationId);
                cmd.AddParameter("ReportId", report.Id);
                cmd.AddParameter("IncidentId", report.IncidentId);
                cmd.AddParameter("RemoteAddress", command.RemoteAddress);
                cmd.AddParameter("Description", command.Feedback);
                cmd.AddParameter("EmailAddress", command.Email);
                cmd.AddParameter("Conversation", "");
                cmd.AddParameter("CreatedAtUtc", DateTime.UtcNow);

                var evt = new FeedbackAttachedToIncident
                {
                    Message = command.Feedback,
                    UserEmailAddress = command.Email,
                    IncidentId = report.IncidentId
                };
                await context.SendAsync(evt);

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}