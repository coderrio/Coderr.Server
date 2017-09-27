using Griffin.Data.Mapper;
using codeRR.App.Core.Feedback;

namespace codeRR.SqlServer.Core.Feedback
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