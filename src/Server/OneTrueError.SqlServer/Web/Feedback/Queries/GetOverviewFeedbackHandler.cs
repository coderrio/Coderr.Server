using System.Linq;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using Griffin.Data;
using Griffin.Data.Mapper;
using OneTrueError.Api.Web.Feedback.Queries;

namespace OneTrueError.SqlServer.Web.Feedback.Queries
{
    [Component]
    public class GetOverviewFeedbackHandler : IQueryHandler<GetFeedbackForDashboardPage, GetFeedbackForDashboardPageResult>
    {
        private IAdoNetUnitOfWork _unitOfWork;

        public GetOverviewFeedbackHandler(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GetFeedbackForDashboardPageResult> ExecuteAsync(GetFeedbackForDashboardPage query)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText =
                    @"select IncidentFeedback.Description Message, IncidentFeedback.EmailAddress, IncidentFeedback.CreatedAtUtc as WrittenAtUtc, 
                        ApplicationId, Applications.Name ApplicationName
                        from IncidentFeedback
                        join Applications on (ApplicationId = Applications.Id)";
//WHERE Description is not null AND datalength(message) > 0";
                var items = await cmd.ToListAsync<GetFeedbackForDashboardPageResultItem>();
                return new GetFeedbackForDashboardPageResult
                {
                    Items = items.Where(x => !string.IsNullOrEmpty(x.Message)).ToArray(),
                    Emails =
                        items.Where(x => !string.IsNullOrEmpty(x.EmailAddress)).Select(x => x.EmailAddress).ToList()
                };
            }
        }
    }
}
