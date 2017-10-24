using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using codeRR.Server.Api.Web.Feedback.Queries;
using codeRR.Server.Infrastructure.Security;
using DotNetCqs;
using Griffin.Container;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Web.Feedback.Queries
{
    [Component]
    public class GetOverviewFeedbackHandler :
        IQueryHandler<GetFeedbackForDashboardPage, GetFeedbackForDashboardPageResult>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public GetOverviewFeedbackHandler(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GetFeedbackForDashboardPageResult> HandleAsync(IMessageContext context, GetFeedbackForDashboardPage query)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                var sql =
                    @"select IncidentFeedback.Description Message, IncidentFeedback.EmailAddress, IncidentFeedback.CreatedAtUtc as WrittenAtUtc, 
                        ApplicationId, Applications.Name ApplicationName
                        from IncidentFeedback
                        join Applications on (ApplicationId = Applications.Id)
                        WHERE ApplicationId IN ({0})";

                // roundtrip to int to prevent
                // something getting a string into our claims
                // since the code below would otherwise
                // allow SQL injection 
                var appIds = context.Principal
                    .FindAll(x => x.Type == CoderrClaims.Application)
                    .Select(x => int.Parse(x.Value).ToString())
                    .ToList();
                if (appIds.Count == 0)
                {
                    return new GetFeedbackForDashboardPageResult
                    {
                        Items = new GetFeedbackForDashboardPageResultItem[0],
                        Emails = new List<string>(),
                        TotalCount = 0
                    };
                }

                cmd.CommandText = sql.Replace("{0}", string.Join(",", appIds));
                //WHERE Description is not null AND datalength(message) > 0";
                var items = await cmd.ToListAsync<GetFeedbackForDashboardPageResultItem>();
                return new GetFeedbackForDashboardPageResult
                {
                    Items = items.Where(x => !string.IsNullOrEmpty(x.Message)).ToArray(),
                    Emails =
                        items.Where(x => !string.IsNullOrEmpty(x.EmailAddress)).Select(x => x.EmailAddress).ToList<string>()
                };
            }
        }
    }
}