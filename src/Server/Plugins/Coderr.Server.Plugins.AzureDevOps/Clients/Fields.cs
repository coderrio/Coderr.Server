namespace Coderr.Server.Common.AzureDevOps.App.Clients
{
    //public class StateFields
    //{
    //    public const string New = "New";
    //    public const string Active = "Active";
    //    public const string Committed = "Committed";
    //    public const string Done = "Done";
    //    public const string Removed = "Removed";
    //    public const string Approved = "Approved";
    //    public const string Resolved = "Resolved";
    //    public const string Closed = "Closed";
    //}

    public static class Fields
    {
        public const string Id = "System.Id";
        public const string AreaPath = "System.AreaPath";
        public const string TeamProject = "System.TeamProject";
        public const string IterationPath = "System.IterationPath";
        public const string WorkItemType = "System.WorkItemType";

        public const string State = "System.State";
        public const string Reason = "System.Reason";
        public const string CreatedDate = "System.CreatedDate";
        public const string CreatedBy = "System.CreatedBy";

        /// <summary>
        /// Used in some cases for bugs.
        /// </summary>
        public const string Description = "System.Description";
        
        public const string ChangedDate = "System.ChangedDate";
        public const string ChangedBy = "System.ChangedBy";
        public const string Title = "System.Title";
        public const string StateChangeDate = "Microsoft.VSTS.Common.StateChangeDate";
        public const string History = "System.History";
        public const string Tags = "System.Tags";
        public const string SystemInfo = "Microsoft.VSTS.TCM.SystemInfo";
        public const string AssignedTo = "System.AssignedTo";
        public const string ClosedDate = "Microsoft.VSTS.Common.ClosedDate";
        public const string ClosedBy = "Microsoft.VSTS.Common.ClosedBy";
        public const string ResolvedDate = "Microsoft.VSTS.Common.ResolvedDate";
        public const string ResolvedBy = "Microsoft.VSTS.Common.ResolvedBy";

        /// <summary>
        /// The reason why the b.ug was resolved
        /// </summary>
        public const string ResolvedReason = "Microsoft.VSTS.Common.ResolvedReason";
        public const string ActivatedBy = "Microsoft.VSTS.Common.ActivatedBy";
        public const string ActivatedDate = "Microsoft.VSTS.Common.ActivatedDate";
        
        /// <summary>
        /// Number, 1=must fix; 4=unimportant.
        /// </summary>
        public const string Priority = "Microsoft.VSTS.Common.Priority";
        public const string ReproduceSteps = "Microsoft.VSTS.TCM.ReproSteps";

        /// <summary>
        /// "2 - High", "3 - Medium"
        /// </summary>
        public const string Severity = "Microsoft.VSTS.Common.Severity";
    }
}