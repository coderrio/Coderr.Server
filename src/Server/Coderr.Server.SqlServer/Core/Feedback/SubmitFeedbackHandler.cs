using System;
using System.Data.Common;
using System.Threading.Tasks;
using Coderr.Server.Api.Core.Feedback.Commands;
using Coderr.Server.Domain.Core.Applications;
using Coderr.Server.Domain.Core.ErrorReports;
using Coderr.Server.ReportAnalyzer.Abstractions.Feedback;
using DotNetCqs;
using Griffin.Data;
using log4net;

namespace Coderr.Server.SqlServer.Core.Feedback
{
    public class SubmitFeedbackHandler : IMessageHandler<SubmitFeedback>
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly ILog _logger = LogManager.GetLogger(typeof(SubmitFeedbackHandler));
        private readonly IReportsRepository _reportsRepository;
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public SubmitFeedbackHandler(IAdoNetUnitOfWork unitOfWork, IReportsRepository reportsRepository,
            IApplicationRepository applicationRepository)
        {
            _unitOfWork = unitOfWork;
            _reportsRepository = reportsRepository;
            _applicationRepository = applicationRepository;
        }

        public async Task HandleAsync(IMessageContext context, SubmitFeedback command)
        {
            if (string.IsNullOrEmpty(command.Email))
            {
                if (string.IsNullOrEmpty(command.Feedback))
                    return;
                if (command.Feedback.Length < 3)
                    return;
            }

            ReportMapping report2;
            int? reportId = null;
            if (command.ReportId > 0)
            {
                var report = await _reportsRepository.GetAsync(command.ReportId);
                report2 = new ReportMapping
                {
                    ApplicationId = report.ApplicationId,
                    ErrorId = report.ClientReportId,
                    IncidentId = report.IncidentId,
                    ReceivedAtUtc = report.CreatedAtUtc
                };
                reportId = report.Id;
            }
            else
            {
                report2 = await _reportsRepository.FindByErrorIdAsync(command.ErrorId);
                if (report2 == null) _logger.Warn("Failed to find report by error id: " + command.ErrorId);
            }

            // storing it without connections as the report might not have been uploaded yet.
            if (report2 == null)
            {
                _logger.InfoFormat(
                    "Failed to find report. Let's enqueue it instead for report {0}/{1}. Email: {2}, Feedback: {3}",
                    command.ReportId, command.ErrorId, command.Email, command.Feedback);
                try
                {
                    using (var cmd = _unitOfWork.CreateCommand())
                    {
                        cmd.CommandText =
                            "INSERT INTO IncidentFeedback (ErrorReportId, RemoteAddress, Description, EmailAddress, CreatedAtUtc, Conversation, ConversationLength) "
                            +
                            "VALUES (@ErrorReportId, @RemoteAddress, @Description, @EmailAddress, @CreatedAtUtc, '', 0)";
                        cmd.AddParameter("ErrorReportId", command.ErrorId);
                        cmd.AddParameter("RemoteAddress", command.RemoteAddress);
                        cmd.AddParameter("Description", command.Feedback);
                        cmd.AddParameter("EmailAddress", command.Email);
                        cmd.AddParameter("CreatedAtUtc", DateTime.UtcNow);
                        cmd.ExecuteNonQuery();
                    }

                    _logger.Info("** STORING FEEDBACK");
                }
                catch (Exception exception)
                {
                    _logger.Error(
                        $"{command.ErrorId}: Failed to store '{command.Email}' '{command.Feedback}'", exception);
                    //hide errors.
                }

                return;
            }

            using (var cmd = (DbCommand)_unitOfWork.CreateCommand())
            {
                cmd.CommandText =
                    "INSERT INTO IncidentFeedback (ErrorReportId, ApplicationId, ReportId, IncidentId, RemoteAddress, Description, EmailAddress, CreatedAtUtc, Conversation, ConversationLength) "
                    +
                    "VALUES (@ErrorReportId, @ApplicationId, @ReportId, @IncidentId, @RemoteAddress, @Description, @EmailAddress, @CreatedAtUtc, @Conversation, 0)";
                cmd.AddParameter("ErrorReportId", command.ErrorId);
                cmd.AddParameter("ApplicationId", report2.ApplicationId);
                cmd.AddParameter("ReportId", reportId);
                cmd.AddParameter("IncidentId", report2.IncidentId);
                cmd.AddParameter("RemoteAddress", command.RemoteAddress);
                cmd.AddParameter("Description", command.Feedback);
                cmd.AddParameter("EmailAddress", command.Email);
                cmd.AddParameter("Conversation", "");
                cmd.AddParameter("CreatedAtUtc", DateTime.UtcNow);

                var app = await _applicationRepository.GetByIdAsync(report2.ApplicationId);
                var evt = new FeedbackAttachedToIncident
                {
                    ApplicationId = report2.ApplicationId,
                    ApplicationName = app.Name,
                    Message = command.Feedback,
                    UserEmailAddress = command.Email,
                    IncidentId = report2.IncidentId
                };
                await context.SendAsync(evt);

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}