using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using codeRR.Server.Api.Web.Feedback.Queries;
using DotNetCqs;
using Griffin.Container;
using Griffin.Data;

namespace codeRR.Server.SqlServer.Web.Feedback.Queries
{
    [Component]
    public class GetIncidentFeedbackHandler : IQueryHandler<GetIncidentFeedback, GetIncidentFeedbackResult>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public GetIncidentFeedbackHandler(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GetIncidentFeedbackResult> HandleAsync(IMessageContext context, GetIncidentFeedback query)
        {
            using (var cmd = (DbCommand) _unitOfWork.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT IncidentId, IncidentFeedback.CreatedAtUtc, EmailAddress, IncidentFeedback.Description" +
                    " FROM IncidentFeedback" +
                    " JOIN Incidents ON (Incidents.Id = IncidentFeedback.IncidentId)" +
                    " WHERE IncidentId = @id";
                cmd.AddParameter("id", query.IncidentId);
                cmd.CommandText += " ORDER BY IncidentFeedback.CreatedAtUtc DESC";

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var emails = new List<string>();
                    var items = new List<GetIncidentFeedbackResultItem>();
                    while (reader.Read())
                    {
                        var description = Convert.ToString(reader["Description"]);
                        var email = Convert.ToString(Convert.ToString(reader["EmailAddress"]));
                        if (!string.IsNullOrEmpty(description))
                        {
                            var item = new GetIncidentFeedbackResultItem();
                            item.EmailAddress = email;
                            item.Message = description;
                            item.WrittenAtUtc = (DateTime) reader["CreatedAtUtc"];
                            items.Add(item);
                        }
                        if (!string.IsNullOrEmpty(email) && !emails.Contains(email))
                        {
                            emails.Add(email);
                        }
                    }

                    return new GetIncidentFeedbackResult(items, emails);
                }
            }
        }
    }
}