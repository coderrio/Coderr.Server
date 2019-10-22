using Coderr.Server.Domain.Core.Feedback;
using Griffin.Data.Mapper;

namespace Coderr.Server.PostgreSQL.Core.Feedback
{
    public class FeedbackEntityMapper : CrudEntityMapper<UserFeedback>
    {
        public FeedbackEntityMapper() : base("IncidentFeedback")
        {
            Property(x => x.ErrorId).ColumnName("ErrorReportId");
            Property(x => x.CanRemove).Ignore();
            Property(x => x.CanUpdate).Ignore();
        }
    }
}