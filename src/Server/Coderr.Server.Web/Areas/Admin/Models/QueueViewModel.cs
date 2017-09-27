namespace codeRR.Server.Web.Areas.Admin.Models
{
    public class QueueViewModel
    {
        public bool EventAuthentication { get; set; }

        public string EventQueue { get; set; }

        public bool EventTransactions { get; set; }

        public bool FeedbackAuthentication { get; set; }

        public string FeedbackQueue { get; set; }

        public bool FeedbackTransactions { get; set; }

        public bool ReportAuthentication { get; set; }
        public string ReportQueue { get; set; }

        public bool ReportTransactions { get; set; }

        public bool UseSql { get; set; }
    }
}