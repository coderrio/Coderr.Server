using codeRR.Server.App.Core.Feedback;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Core.Feedback
{
    public class FeedbackEntityMapper : CrudEntityMapper<FeedbackEntity>
    {
        public FeedbackEntityMapper() : base("IncidentFeedback")
        {
            Property(x => x.ErrorId).ColumnName("ErrorReportId");
            Property(x => x.CanRemove).Ignore();
            Property(x => x.CanUpdate).Ignore();
        }
    }
}