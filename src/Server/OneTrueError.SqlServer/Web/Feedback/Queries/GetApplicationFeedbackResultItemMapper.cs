using Griffin.Data.Mapper;
using OneTrueError.Api.Web.Feedback.Queries;

namespace OneTrueError.SqlServer.Web.Feedback.Queries
{
    public class GetApplicationFeedbackResultItemMapper : EntityMapper<GetFeedbackForApplicationPageResultItem>
    {
        public GetApplicationFeedbackResultItemMapper()
        {
            Property(x => x.EmailAddress);
            Property(x => x.IncidentId);
            Property(x => x.IncidentName)
                .NotForCrud()
                .ToPropertyValue(FirstLine);
            Property(x => x.Message);
            Property(x => x.WrittenAtUtc);
        }

        private string FirstLine(object arg)
        {
            var str = arg.ToString();
            var pos = str.IndexOfAny(new[] {'\r', '\n'});
            return pos != -1 ? str.Substring(0, pos) : str;
        }
    }
}