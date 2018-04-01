using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Domain.Core.Feedback;
using Coderr.Server.ReportAnalyzer.Feedback;
using Coderr.Server.ReportAnalyzer.Abstractions;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Core.Feedback
{
    [ContainerService]
    public class FeedbackRepository : IFeedbackRepository, IUserFeedbackRepository
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public FeedbackRepository(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UserFeedback> FindPendingAsync(string reportId)
        {
            return await _unitOfWork.FirstOrDefaultAsync<UserFeedback>(new {ErrorId = reportId});
        }

        public async Task UpdateAsync(UserFeedback feedback)
        {
            await _unitOfWork.UpdateAsync(feedback);
        }

        public async Task<IReadOnlyList<string>> GetEmailAddressesAsync(int incidentId)
        {
            var emailAddresses = new List<string>();
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText =
                    "SELECT distinct EmailAddress FROM IncidentFeedback WHERE IncidentId = @id AND EmailAddress IS NOT NULL";
                cmd.AddParameter("id", incidentId);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        emailAddresses.Add(reader.GetString(0));
                    }
                }
            }

            return emailAddresses;
        }

        public Task CreateAsync(NewFeedback feedback)
        {
            throw new NotImplementedException();
        }
    }
}