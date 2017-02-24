module OneTrueError.Web.Overview.Queries {
    export class GetOverview {
        public static TYPE_NAME: string = 'GetOverview';
        public NumberOfDays: number;
        public QueryId: string;
    }

    export class GetOverviewApplicationResult {
        public static TYPE_NAME: string = 'GetOverviewApplicationResult';
        public Label: string;
        public Values: number[];
        public constructor(label: string, startDate: any, days: number) {
            this.Label = label;
        }
    }

    export class GetOverviewResult {
        public static TYPE_NAME: string = 'GetOverviewResult';
        public Days: number;
        public IncidentsPerApplication: OneTrueError.Web.Overview.Queries.GetOverviewApplicationResult[];
        public StatSummary: OneTrueError.Web.Overview.Queries.OverviewStatSummary;
        public TimeAxisLabels: string[];
    }

    export class OverviewStatSummary {
        public static TYPE_NAME: string = 'OverviewStatSummary';
        public Followers: number;
        public Incidents: number;
        public Reports: number;
        public UserFeedback: number;
    }

}
module OneTrueError.Web.Feedback.Queries {
    export class GetFeedbackForApplicationPage {
        public static TYPE_NAME: string = 'GetFeedbackForApplicationPage';
        public ApplicationId: number;
        public QueryId: string;
        public constructor(applicationId: number) {
            this.ApplicationId = applicationId;
        }
    }

    export class GetFeedbackForApplicationPageResult {
        public static TYPE_NAME: string = 'GetFeedbackForApplicationPageResult';
        public Emails: string[];
        public Items: OneTrueError.Web.Feedback.Queries.GetFeedbackForApplicationPageResultItem[];
        public TotalCount: number;
    }

    export class GetFeedbackForApplicationPageResultItem {
        public static TYPE_NAME: string = 'GetFeedbackForApplicationPageResultItem';
        public EmailAddress: string;
        public IncidentId: number;
        public IncidentName: string;
        public Message: string;
        public WrittenAtUtc: any;
    }

    export class GetIncidentFeedback {
        public static TYPE_NAME: string = 'GetIncidentFeedback';
        public IncidentId: number;
        public QueryId: string;
        public constructor(incidentId: number) {
            this.IncidentId = incidentId;
        }
    }

    export class GetIncidentFeedbackResult {
        public static TYPE_NAME: string = 'GetIncidentFeedbackResult';
        public Emails: string[];
        public Items: OneTrueError.Web.Feedback.Queries.GetIncidentFeedbackResultItem[];
        public constructor(items: OneTrueError.Web.Feedback.Queries.GetIncidentFeedbackResultItem[], emails: string[]) {
            this.Items = items;
            this.Emails = emails;
        }
    }

    export class GetIncidentFeedbackResultItem {
        public static TYPE_NAME: string = 'GetIncidentFeedbackResultItem';
        public EmailAddress: string;
        public Message: string;
        public WrittenAtUtc: any;
    }

    export class GetFeedbackForDashboardPage {
        public static TYPE_NAME: string = 'GetFeedbackForDashboardPage';
        public QueryId: string;
    }

    export class GetFeedbackForDashboardPageResult {
        public static TYPE_NAME: string = 'GetFeedbackForDashboardPageResult';
        public Emails: string[];
        public Items: OneTrueError.Web.Feedback.Queries.GetFeedbackForDashboardPageResultItem[];
        public TotalCount: number;
    }

    export class GetFeedbackForDashboardPageResultItem {
        public static TYPE_NAME: string = 'GetFeedbackForDashboardPageResultItem';
        public ApplicationId: number;
        public ApplicationName: string;
        public EmailAddress: string;
        public Message: string;
        public WrittenAtUtc: any;
    }

}
module OneTrueError.Modules.Triggers {
    export enum LastTriggerActionDTO {
        ExecuteActions = 0,
        AbortTrigger = 1,
    }

    export class TriggerActionDataDTO {
        public static TYPE_NAME: string = 'TriggerActionDataDTO';
        public ActionContext: string;
        public ActionName: string;
    }

    export class TriggerContextRule {
        public static TYPE_NAME: string = 'TriggerContextRule';
        public ContextName: string;
        public PropertyName: string;
        public PropertyValue: string;
        public Filter: TriggerFilterCondition;
        public ResultToUse: TriggerRuleAction;
    }

    export class TriggerExceptionRule {
        public static TYPE_NAME: string = 'TriggerExceptionRule';
        public FieldName: string;
        public Value: string;
        public Filter: TriggerFilterCondition;
        public ResultToUse: TriggerRuleAction;
    }

    export enum TriggerFilterCondition {
        StartsWith = 0,
        EndsWith = 1,
        Contains = 2,
        DoNotContain = 3,
        Equals = 4,
    }

    export class TriggerDTO {
        public static TYPE_NAME: string = 'TriggerDTO';
        public Description: string;
        public Id: string;
        public Name: string;
        public Summary: string;
    }

    export enum TriggerRuleAction {
        AbortTrigger = 0,
        ContinueWithNextRule = 1,
        ExecuteActions = 2,
    }

    export class TriggerRuleBase {
        public static TYPE_NAME: string = 'TriggerRuleBase';
        public Filter: TriggerFilterCondition;
        public ResultToUse: TriggerRuleAction;
    }

}
module OneTrueError.Modules.Triggers.Queries {
    export class GetContextCollectionMetadata {
        public static TYPE_NAME: string = 'GetContextCollectionMetadata';
        public ApplicationId: number;
        public QueryId: string;
        public constructor(applicationId: number) {
            this.ApplicationId = applicationId;
        }
    }

    export class GetContextCollectionMetadataItem {
        public static TYPE_NAME: string = 'GetContextCollectionMetadataItem';
        public Name: string;
        public Properties: string[];
    }

    export class GetTrigger {
        public static TYPE_NAME: string = 'GetTrigger';
        public Id: number;
        public QueryId: string;
        public constructor(id: number) {
            this.Id = id;
        }
    }

    export class GetTriggerDTO {
        public static TYPE_NAME: string = 'GetTriggerDTO';
        public Actions: OneTrueError.Modules.Triggers.TriggerActionDataDTO[];
        public ApplicationId: number;
        public Description: string;
        public Id: number;
        public LastTriggerAction: OneTrueError.Modules.Triggers.LastTriggerActionDTO;
        public Name: string;
        public Rules: OneTrueError.Modules.Triggers.TriggerRuleBase[];
        public RunForExistingIncidents: boolean;
        public RunForNewIncidents: boolean;
        public RunForReOpenedIncidents: boolean;
    }

    export class GetTriggersForApplication {
        public static TYPE_NAME: string = 'GetTriggersForApplication';
        public ApplicationId: number;
        public QueryId: string;
        public constructor(applicationId: number) {
            this.ApplicationId = applicationId;
        }
    }

}
module OneTrueError.Modules.Triggers.Commands {
    export class CreateTrigger {
        public static TYPE_NAME: string = 'CreateTrigger';
        public Actions: OneTrueError.Modules.Triggers.TriggerActionDataDTO[];
        public ApplicationId: number;
        public Description: string;
        public Id: number;
        public LastTriggerAction: OneTrueError.Modules.Triggers.LastTriggerActionDTO;
        public Name: string;
        public Rules: OneTrueError.Modules.Triggers.TriggerRuleBase[];
        public RunForExistingIncidents: boolean;
        public RunForNewIncidents: boolean;
        public RunForReOpenedIncidents: boolean;
        public CommandId: string;
        public constructor(applicationId: number, name: string) {
            this.ApplicationId = applicationId;
            this.Name = name;
        }
    }

    export class DeleteTrigger {
        public static TYPE_NAME: string = 'DeleteTrigger';
        public Id: number;
        public CommandId: string;
        public constructor(id: number) {
            this.Id = id;
        }
    }

    export class UpdateTrigger {
        public static TYPE_NAME: string = 'UpdateTrigger';
        public Actions: OneTrueError.Modules.Triggers.TriggerActionDataDTO[];
        public Description: string;
        public Id: number;
        public LastTriggerAction: OneTrueError.Modules.Triggers.LastTriggerActionDTO;
        public Name: string;
        public Rules: OneTrueError.Modules.Triggers.TriggerRuleBase[];
        public RunForExistingIncidents: boolean;
        public RunForNewIncidents: boolean;
        public RunForReOpenedIncidents: boolean;
        public CommandId: string;
        public constructor(id: number, name: string) {
            this.Id = id;
            this.Name = name;
        }
    }

}
module OneTrueError.Modules.Tagging {
    export class TagDTO {
        public static TYPE_NAME: string = 'TagDTO';
        public Name: string;
        public OrderNumber: number;
    }

}
module OneTrueError.Modules.Tagging.Queries {
    export class GetTagsForIncident {
        public static TYPE_NAME: string = 'GetTagsForIncident';
        public IncidentId: number;
        public QueryId: string;
        public constructor(incidentId: number) {
            this.IncidentId = incidentId;
        }
    }

}
module OneTrueError.Modules.Tagging.Events {
    export class TagAttachedToIncident {
        public static TYPE_NAME: string = 'TagAttachedToIncident';
        public IncidentId: number;
        public Tags: OneTrueError.Modules.Tagging.TagDTO[];
        public EventId: string;
        public constructor(incidentId: number, tags: OneTrueError.Modules.Tagging.TagDTO[]) {
            this.IncidentId = incidentId;
            this.Tags = tags;
        }
    }

}
module OneTrueError.Modules.ContextData.Queries {
    export class GetSimilarities {
        public static TYPE_NAME: string = 'GetSimilarities';
        public IncidentId: number;
        public QueryId: string;
        public constructor(incidentId: number) {
            this.IncidentId = incidentId;
        }
    }

    export class GetSimilaritiesCollection {
        public static TYPE_NAME: string = 'GetSimilaritiesCollection';
        public Name: string;
        public Similarities: OneTrueError.Modules.ContextData.Queries.GetSimilaritiesSimilarity[];
    }

    export class GetSimilaritiesResult {
        public static TYPE_NAME: string = 'GetSimilaritiesResult';
        public Collections: OneTrueError.Modules.ContextData.Queries.GetSimilaritiesCollection[];
    }

    export class GetSimilaritiesSimilarity {
        public static TYPE_NAME: string = 'GetSimilaritiesSimilarity';
        public Name: string;
        public Values: OneTrueError.Modules.ContextData.Queries.GetSimilaritiesValue[];
        public constructor(name: string) {
            this.Name = name;
        }
    }

    export class GetSimilaritiesValue {
        public static TYPE_NAME: string = 'GetSimilaritiesValue';
        public Count: number;
        public Percentage: number;
        public Value: string;
        public constructor(value: string, percentage: number, count: number) {
            this.Value = value;
            this.Percentage = percentage;
            this.Count = count;
        }
    }

}
module OneTrueError.Modules.ErrorOrigins.Queries {
    export class GetOriginsForIncident {
        public static TYPE_NAME: string = 'GetOriginsForIncident';
        public IncidentId: number;
        public QueryId: string;
        public constructor(incidentId: number) {
            this.IncidentId = incidentId;
        }
    }

    export class GetOriginsForIncidentResult {
        public static TYPE_NAME: string = 'GetOriginsForIncidentResult';
        public Items: OneTrueError.Modules.ErrorOrigins.Queries.GetOriginsForIncidentResultItem[];
    }

    export class GetOriginsForIncidentResultItem {
        public static TYPE_NAME: string = 'GetOriginsForIncidentResultItem';
        public Latitude: number;
        public Longitude: number;
        public NumberOfErrorReports: number;
    }

}
module OneTrueError.Core {
    export class IgnoreFieldAttribute {
        public static TYPE_NAME: string = 'IgnoreFieldAttribute';
        public TypeId: any;
    }

}
module OneTrueError.Core.Users {
    export class NotificationSettings {
        public static TYPE_NAME: string = 'NotificationSettings';
        public NotifyOnNewIncidents: NotificationState;
        public NotifyOnNewReport: NotificationState;
        public NotifyOnPeaks: NotificationState;
        public NotifyOnReOpenedIncident: NotificationState;
        public NotifyOnUserFeedback: NotificationState;
    }

    export enum NotificationState {
        UseGlobalSetting = 0,
        Disabled = 1,
        Cellphone = 2,
        Email = 3,
    }

}
module OneTrueError.Core.Users.Queries {
    export class GetUserSettings {
        public static TYPE_NAME: string = 'GetUserSettings';
        public ApplicationId: number;
        public UserId: number;
        public QueryId: string;
    }

    export class GetUserSettingsResult {
        public static TYPE_NAME: string = 'GetUserSettingsResult';
        public FirstName: string;
        public LastName: string;
        public MobileNumber: string;
        public Notifications: OneTrueError.Core.Users.NotificationSettings;
    }

}
module OneTrueError.Core.Users.Commands {
    export class UpdateNotifications {
        public static TYPE_NAME: string = 'UpdateNotifications';
        public ApplicationId: number;
        public NotifyOnNewIncidents: OneTrueError.Core.Users.NotificationState;
        public NotifyOnNewReport: OneTrueError.Core.Users.NotificationState;
        public NotifyOnPeaks: OneTrueError.Core.Users.NotificationState;
        public NotifyOnReOpenedIncident: OneTrueError.Core.Users.NotificationState;
        public NotifyOnUserFeedback: OneTrueError.Core.Users.NotificationState;
        public UserId: number;
        public CommandId: string;
    }

    export class UpdatePersonalSettings {
        public static TYPE_NAME: string = 'UpdatePersonalSettings';
        public FirstName: string;
        public LastName: string;
        public MobileNumber: string;
        public UserId: number;
        public CommandId: string;
    }

}
module OneTrueError.Core.Support {
    export class SendSupportRequest {
        public static TYPE_NAME: string = 'SendSupportRequest';
        public Message: string;
        public Subject: string;
        public Url: string;
        public CommandId: string;
    }

}
module OneTrueError.Core.Reports {
    export class ContextCollectionDTO {
        public static TYPE_NAME: string = 'ContextCollectionDTO';
        public Name: string;
        public Properties: string[];
        public constructor(name: string, items: string[]) {
            this.Name = name;
        }
    }

    export class ReportDTO {
        public static TYPE_NAME: string = 'ReportDTO';
        public ApplicationId: number;
        public ContextCollections: OneTrueError.Core.Reports.ContextCollectionDTO[];
        public CreatedAtUtc: any;
        public Exception: OneTrueError.Core.Reports.ReportExeptionDTO;
        public Id: number;
        public IncidentId: number;
        public RemoteAddress: string;
        public ReportId: string;
        public ReportVersion: string;
    }

    export class ReportExeptionDTO {
        public static TYPE_NAME: string = 'ReportExeptionDTO';
        public AssemblyName: string;
        public BaseClasses: string[];
        public Everything: string;
        public FullName: string;
        public InnerException: OneTrueError.Core.Reports.ReportExeptionDTO;
        public Message: string;
        public Name: string;
        public Namespace: string;
        public Properties: string[];
        public StackTrace: string;
    }

}
module OneTrueError.Core.Reports.Queries {
    export class GetReport {
        public static TYPE_NAME: string = 'GetReport';
        public ReportId: number;
        public QueryId: string;
        public constructor(reportId: number) {
            this.ReportId = reportId;
        }
    }

    export class GetReportException {
        public static TYPE_NAME: string = 'GetReportException';
        public AssemblyName: string;
        public BaseClasses: string[];
        public Everything: string;
        public FullName: string;
        public InnerException: OneTrueError.Core.Reports.Queries.GetReportException;
        public Message: string;
        public Name: string;
        public Namespace: string;
        public StackTrace: string;
    }

    export class GetReportList {
        public static TYPE_NAME: string = 'GetReportList';
        public IncidentId: number;
        public PageNumber: number;
        public PageSize: number;
        public QueryId: string;
        public constructor(incidentId: number) {
            this.IncidentId = incidentId;
        }
    }

    export class GetReportListResult {
        public static TYPE_NAME: string = 'GetReportListResult';
        public Items: OneTrueError.Core.Reports.Queries.GetReportListResultItem[];
        public PageNumber: number;
        public PageSize: number;
        public TotalCount: number;
        public constructor(items: OneTrueError.Core.Reports.Queries.GetReportListResultItem[]) {
            this.Items = items;
        }
    }

    export class GetReportListResultItem {
        public static TYPE_NAME: string = 'GetReportListResultItem';
        public CreatedAtUtc: any;
        public Id: number;
        public Message: string;
        public RemoteAddress: string;
    }

    export class GetReportResult {
        public static TYPE_NAME: string = 'GetReportResult';
        public ContextCollections: OneTrueError.Core.Reports.Queries.GetReportResultContextCollection[];
        public CreatedAtUtc: any;
        public EmailAddress: string;
        public ErrorId: string;
        public Exception: OneTrueError.Core.Reports.Queries.GetReportException;
        public Id: string;
        public IncidentId: string;
        public Message: string;
        public StackTrace: string;
        public UserFeedback: string;
    }

    export class GetReportResultContextCollection {
        public static TYPE_NAME: string = 'GetReportResultContextCollection';
        public Name: string;
        public Properties: OneTrueError.Core.Reports.Queries.KeyValuePair[];
        public constructor(name: string, properties: OneTrueError.Core.Reports.Queries.KeyValuePair[]) {
            this.Name = name;
            this.Properties = properties;
        }
    }

    export class KeyValuePair {
        public static TYPE_NAME: string = 'KeyValuePair';
        public Key: string;
        public Value: string;
        public constructor(key: string, value: string) {
            this.Key = key;
            this.Value = value;
        }
    }

}
module OneTrueError.Core.Messaging {
    export class EmailAddress {
        public static TYPE_NAME: string = 'EmailAddress';
        public Address: string;
        public Name: string;
        public constructor(address: string) {
            this.Address = address;
        }
    }

    export class EmailMessage {
        public static TYPE_NAME: string = 'EmailMessage';
        public HtmlBody: string;
        public Recipients: OneTrueError.Core.Messaging.EmailAddress[];
        public Resources: OneTrueError.Core.Messaging.EmailResource[];
        public Subject: string;
        public TextBody: string;
    }

    export class EmailResource {
        public static TYPE_NAME: string = 'EmailResource';
        public Content: any;
        public Name: string;
        public constructor(name: string, content: any) {
            this.Name = name;
            this.Content = content;
        }
    }

}
module OneTrueError.Core.Messaging.Commands {
    export class SendSms {
        public static TYPE_NAME: string = 'SendSms';
        public Message: string;
        public PhoneNumber: string;
        public CommandId: string;
        public constructor(phoneNumber: string, message: string) {
            this.PhoneNumber = phoneNumber;
            this.Message = message;
        }
    }

    export class SendEmail {
        public static TYPE_NAME: string = 'SendEmail';
        public EmailMessage: OneTrueError.Core.Messaging.EmailMessage;
        public CommandId: string;
    }

    export class SendTemplateEmail {
        public static TYPE_NAME: string = 'SendTemplateEmail';
        public MailTitle: string;
        public Model: any;
        public Resources: OneTrueError.Core.Messaging.EmailResource[];
        public Subject: string;
        public TemplateName: string;
        public To: string;
        public CommandId: string;
        public constructor(mailTitle: string, templateName: string) {
            this.MailTitle = mailTitle;
            this.TemplateName = templateName;
        }
    }

}
module OneTrueError.Core.Invitations.Queries {
    export class GetInvitationByKey {
        public static TYPE_NAME: string = 'GetInvitationByKey';
        public InvitationKey: string;
        public QueryId: string;
        public constructor(invitationKey: string) {
            this.InvitationKey = invitationKey;
        }
    }

    export class GetInvitationByKeyResult {
        public static TYPE_NAME: string = 'GetInvitationByKeyResult';
        public EmailAddress: string;
    }

}
module OneTrueError.Core.Invitations.Commands {
    export class InviteUser {
        public static TYPE_NAME: string = 'InviteUser';
        public ApplicationId: number;
        public EmailAddress: string;
        public Text: string;
        public UserId: number;
        public CommandId: string;
        public constructor(applicationId: number, emailAddress: string) {
            this.ApplicationId = applicationId;
            this.EmailAddress = emailAddress;
        }
    }

}
module OneTrueError.Core.Incidents {
    export enum IncidentOrder {
        Newest = 0,
        MostReports = 1,
        MostFeedback = 2,
    }

    export class IncidentSummaryDTO {
        public static TYPE_NAME: string = 'IncidentSummaryDTO';
        public ApplicationId: number;
        public ApplicationName: string;
        public CreatedAtUtc: any;
        public Id: number;
        public IsReOpened: boolean;
        public LastUpdateAtUtc: any;
        public Name: string;
        public ReportCount: number;
        public constructor(id: number, name: string) {
            this.Id = id;
            this.Name = name;
        }
    }

}
module OneTrueError.Core.Incidents.Queries {
    export class FindIncidentResult {
        public static TYPE_NAME: string = 'FindIncidentResult';
        public Items: OneTrueError.Core.Incidents.Queries.FindIncidentResultItem[];
        public PageNumber: number;
        public PageSize: number;
        public TotalCount: number;
    }

    export class FindIncidentResultItem {
        public static TYPE_NAME: string = 'FindIncidentResultItem';
        public ApplicationId: string;
        public ApplicationName: string;
        public CreatedAtUtc: any;
        public Id: number;
        public IsReOpened: boolean;
        public LastUpdateAtUtc: any;
        public Name: string;
        public ReportCount: number;
        public constructor(id: number, name: string) {
            this.Id = id;
            this.Name = name;
        }
    }

    export class FindIncidents {
        public static TYPE_NAME: string = 'FindIncidents';
        public ApplicationId: number;
        public Closed: boolean;
        public Ignored: boolean;
        public ItemsPerPage: number;
        public MaxDate: any;
        public MinDate: any;
        public Open: boolean;
        public PageNumber: number;
        public ReOpened: boolean;
        public FreeText: string;
        public SortAscending: boolean;
        public SortType: OneTrueError.Core.Incidents.IncidentOrder;
        public QueryId: string;
    }

    export class GetIncident {
        public static TYPE_NAME: string = 'GetIncident';
        public IncidentId: number;
        public QueryId: string;
        public constructor(incidentId: number) {
            this.IncidentId = incidentId;
        }
    }

    export class GetIncidentForClosePage {
        public static TYPE_NAME: string = 'GetIncidentForClosePage';
        public IncidentId: number;
        public QueryId: string;
        public constructor(incidentId: number) {
            this.IncidentId = incidentId;
        }
    }

    export class GetIncidentForClosePageResult {
        public static TYPE_NAME: string = 'GetIncidentForClosePageResult';
        public Description: string;
        public SubscriberCount: number;
    }

    export class GetIncidentResult {
        public static TYPE_NAME: string = 'GetIncidentResult';
        public ApplicationId: number;
        public ContextCollections: string[];
        public CreatedAtUtc: any;
        public DayStatistics: OneTrueError.Core.Incidents.Queries.ReportDay[];
        public Description: string;
        public FeedbackCount: number;
        public FullName: string;
        public HashCodeIdentifier: string;
        public Id: number;
        public IsIgnored: boolean;
        public IsReOpened: boolean;
        public IsSolutionShared: boolean;
        public IsSolved: boolean;
        public PreviousSolutionAtUtc: any;
        public ReOpenedAtUtc: any;
        public ReportCount: number;
        public ReportHashCode: string;
        public Solution: string;
        public SolvedAtUtc: any;
        public StackTrace: string;
        public Tags: string[];
        public UpdatedAtUtc: any;
        public WaitingUserCount: number;
    }

    export class GetIncidentStatistics {
        public static TYPE_NAME: string = 'GetIncidentStatistics';
        public IncidentId: number;
        public NumberOfDays: number;
        public QueryId: string;
    }

    export class GetIncidentStatisticsResult {
        public static TYPE_NAME: string = 'GetIncidentStatisticsResult';
        public Labels: string[];
        public Values: number[];
    }

    export class ReportDay {
        public static TYPE_NAME: string = 'ReportDay';
        public Count: number;
        public Date: any;
    }

}
module OneTrueError.Core.Incidents.Events {
    export class IncidentIgnored {
        public static TYPE_NAME: string = 'IncidentIgnored';
        public AccountId: number;
        public IncidentId: number;
        public UserName: string;
        public EventId: string;
        public constructor(incidentId: number, accountId: number, userName: string) {
            this.IncidentId = incidentId;
            this.AccountId = accountId;
            this.UserName = userName;
        }
    }

    export class IncidentReOpened {
        public static TYPE_NAME: string = 'IncidentReOpened';
        public ApplicationId: number;
        public CreatedAtUtc: any;
        public IncidentId: number;
        public EventId: string;
        public constructor(applicationId: number, incidentId: number, createdAtUtc: any) {
            this.ApplicationId = applicationId;
            this.IncidentId = incidentId;
            this.CreatedAtUtc = createdAtUtc;
        }
    }

    export class ReportAddedToIncident {
        public static TYPE_NAME: string = 'ReportAddedToIncident';
        public Incident: OneTrueError.Core.Incidents.IncidentSummaryDTO;
        public IsReOpened: boolean;
        public Report: OneTrueError.Core.Reports.ReportDTO;
        public EventId: string;
        public constructor(incident: OneTrueError.Core.Incidents.IncidentSummaryDTO, report: OneTrueError.Core.Reports.ReportDTO, isReOpened: boolean) {
            this.Incident = incident;
            this.Report = report;
            this.IsReOpened = isReOpened;
        }
    }

}
module OneTrueError.Core.Incidents.Commands {
    export class CloseIncident {
        public static TYPE_NAME: string = 'CloseIncident';
        public CanSendNotification: boolean;
        public IncidentId: number;
        public NotificationText: string;
        public NotificationTitle: string;
        public ShareSolution: boolean;
        public Solution: string;
        public UserId: number;
        public CommandId: string;
        public constructor(solution: string, incidentId: number) {
            this.Solution = solution;
            this.IncidentId = incidentId;
        }
    }

    export class IgnoreIncident {
        public static TYPE_NAME: string = 'IgnoreIncident';
        public IncidentId: number;
        public UserId: number;
        public CommandId: string;
        public constructor(incidentId: number) {
            this.IncidentId = incidentId;
        }
    }

}
module OneTrueError.Core.Feedback.Commands {
    export class SubmitFeedback {
        public static TYPE_NAME: string = 'SubmitFeedback';
        public CreatedAtUtc: any;
        public Email: string;
        public ErrorId: string;
        public Feedback: string;
        public RemoteAddress: string;
        public ReportId: number;
        public CommandId: string;
        public constructor(errorId: string, remoteAddress: string) {
            this.ErrorId = errorId;
            this.RemoteAddress = remoteAddress;
        }
    }

}
module OneTrueError.Core.Feedback.Events {
    export class FeedbackAttachedToIncident {
        public static TYPE_NAME: string = 'FeedbackAttachedToIncident';
        public IncidentId: number;
        public Message: string;
        public UserEmailAddress: string;
        public EventId: string;
    }

}
module OneTrueError.Core.ApiKeys.Queries {
    export class GetApiKey {
        public static TYPE_NAME: string = 'GetApiKey';
        public ApiKey: string;
        public Id: number;
        public QueryId: string;
        public constructor(id: number) {
            this.Id = id;
        }
    }

    export class GetApiKeyResult {
        public static TYPE_NAME: string = 'GetApiKeyResult';
        public AllowedApplications: OneTrueError.Core.ApiKeys.Queries.GetApiKeyResultApplication[];
        public ApplicationName: string;
        public CreatedAtUtc: any;
        public CreatedById: number;
        public GeneratedKey: string;
        public Id: number;
        public SharedSecret: string;
    }

    export class GetApiKeyResultApplication {
        public static TYPE_NAME: string = 'GetApiKeyResultApplication';
        public ApplicationId: number;
        public ApplicationName: string;
    }

    export class ListApiKeys {
        public static TYPE_NAME: string = 'ListApiKeys';
        public QueryId: string;
    }

    export class ListApiKeysResult {
        public static TYPE_NAME: string = 'ListApiKeysResult';
        public Keys: OneTrueError.Core.ApiKeys.Queries.ListApiKeysResultItem[];
    }

    export class ListApiKeysResultItem {
        public static TYPE_NAME: string = 'ListApiKeysResultItem';
        public ApiKey: string;
        public ApplicationName: string;
        public Id: number;
    }

}
module OneTrueError.Core.ApiKeys.Events {
    export class ApiKeyCreated {
        public static TYPE_NAME: string = 'ApiKeyCreated';
        public ApiKey: string;
        public ApplicationIds: number[];
        public ApplicationNameForTheAppUsingTheKey: string;
        public CreatedById: number;
        public SharedSecret: string;
        public EventId: string;
        public constructor(applicationNameForTheAppUsingTheKey: string, apiKey: string, sharedSecret: string, applicationIds: number[], createdById: number) {
            this.ApplicationNameForTheAppUsingTheKey = applicationNameForTheAppUsingTheKey;
            this.ApiKey = apiKey;
            this.SharedSecret = sharedSecret;
            this.ApplicationIds = applicationIds;
            this.CreatedById = createdById;
        }
    }

}
module OneTrueError.Core.ApiKeys.Commands {
    export class CreateApiKey {
        public static TYPE_NAME: string = 'CreateApiKey';
        public AccountId: number;
        public ApiKey: string;
        public ApplicationIds: number[];
        public ApplicationName: string;
        public SharedSecret: string;
        public CommandId: string;
        public constructor(applicationName: string, apiKey: string, sharedSecret: string, applicationIds: number[]) {
            this.ApplicationName = applicationName;
            this.ApiKey = apiKey;
            this.SharedSecret = sharedSecret;
            this.ApplicationIds = applicationIds;
        }
    }

    export class DeleteApiKey {
        public static TYPE_NAME: string = 'DeleteApiKey';
        public ApiKey: string;
        public Id: number;
        public CommandId: string;
        public constructor(id: number) {
            this.Id = id;
        }
    }

}
module OneTrueError.Core.Applications {
    export class ApplicationListItem {
        public static TYPE_NAME: string = 'ApplicationListItem';
        public Id: number;
        public Name: string;
        public IsAdmin: boolean;
        public constructor(id: number, name: string) {
            this.Id = id;
            this.Name = name;
        }
    }

    export enum TypeOfApplication {
        Mobile = 0,
        DesktopApplication = 1,
        Server = 2,
    }

}
module OneTrueError.Core.Applications.Queries {
    export class GetApplicationTeamResult {
        public static TYPE_NAME: string = 'GetApplicationTeamResult';
        public Invited: OneTrueError.Core.Applications.Queries.GetApplicationTeamResultInvitation[];
        public Members: OneTrueError.Core.Applications.Queries.GetApplicationTeamMember[];
    }

    export class OverviewStatSummary {
        public static TYPE_NAME: string = 'OverviewStatSummary';
        public Followers: number;
        public Incidents: number;
        public Reports: number;
        public UserFeedback: number;
    }

    export class GetApplicationIdByKey {
        public static TYPE_NAME: string = 'GetApplicationIdByKey';
        public ApplicationKey: string;
        public QueryId: string;
        public constructor(applicationKey: string) {
            this.ApplicationKey = applicationKey;
        }
    }

    export class GetApplicationIdByKeyResult {
        public static TYPE_NAME: string = 'GetApplicationIdByKeyResult';
        public Id: number;
    }

    export class GetApplicationInfo {
        public static TYPE_NAME: string = 'GetApplicationInfo';
        public AppKey: string;
        public ApplicationId: number;
        public QueryId: string;
    }

    export class GetApplicationInfoResult {
        public static TYPE_NAME: string = 'GetApplicationInfoResult';
        public AppKey: string;
        public ApplicationType: OneTrueError.Core.Applications.TypeOfApplication;
        public Id: number;
        public Name: string;
        public SharedSecret: string;
        public TotalIncidentCount: number;
    }

    export class GetApplicationList {
        public static TYPE_NAME: string = 'GetApplicationList';
        public AccountId: number;
        public FilterAsAdmin: boolean;
        public QueryId: string;
    }

    export class GetApplicationOverviewResult {
        public static TYPE_NAME: string = 'GetApplicationOverviewResult';
        public Days: number;
        public ErrorReports: number[];
        public Incidents: number[];
        public StatSummary: OneTrueError.Core.Applications.Queries.OverviewStatSummary;
        public TimeAxisLabels: string[];
    }

    export class GetApplicationTeam {
        public static TYPE_NAME: string = 'GetApplicationTeam';
        public ApplicationId: number;
        public QueryId: string;
        public constructor(applicationId: number) {
            this.ApplicationId = applicationId;
        }
    }

    export class GetApplicationTeamMember {
        public static TYPE_NAME: string = 'GetApplicationTeamMember';
        public JoinedAtUtc: any;
        public UserId: number;
        public UserName: string;
    }

    export class GetApplicationTeamResultInvitation {
        public static TYPE_NAME: string = 'GetApplicationTeamResultInvitation';
        public EmailAddress: string;
        public InvitedAtUtc: any;
        public InvitedByUserName: string;
    }

    export class GetApplicationOverview {
        public static TYPE_NAME: string = 'GetApplicationOverview';
        public ApplicationId: number;
        public NumberOfDays: number;
        public QueryId: string;
        public constructor(applicationId: number) {
            this.ApplicationId = applicationId;
        }
    }

}
module OneTrueError.Core.Applications.Events {
    export class ApplicationDeleted {
        public static TYPE_NAME: string = 'ApplicationDeleted';
        public AppKey: string;
        public ApplicationId: number;
        public ApplicationName: string;
        public EventId: string;
    }

    export class ApplicationCreated {
        public static TYPE_NAME: string = 'ApplicationCreated';
        public AppKey: string;
        public ApplicationId: number;
        public ApplicationName: string;
        public CreatedById: number;
        public SharedSecret: string;
        public EventId: string;
        public constructor(id: number, name: string, createdById: number, appKey: string, sharedSecret: string) {
            this.CreatedById = createdById;
            this.AppKey = appKey;
            this.SharedSecret = sharedSecret;
        }
    }

    export class UserInvitedToApplication {
        public static TYPE_NAME: string = 'UserInvitedToApplication';
        public ApplicationId: number;
        public ApplicationName: string;
        public EmailAddress: string;
        public InvitationKey: string;
        public InvitedBy: string;
        public EventId: string;
        public constructor(invitationKey: string, applicationId: number, applicationName: string, emailAddress: string, invitedBy: string) {
            this.InvitationKey = invitationKey;
            this.ApplicationId = applicationId;
            this.ApplicationName = applicationName;
            this.EmailAddress = emailAddress;
            this.InvitedBy = invitedBy;
        }
    }

}
module OneTrueError.Core.Applications.Events.OneTrueError.Api.Core.Accounts.Events {
    export class UserAddedToApplication {
        public static TYPE_NAME: string = 'UserAddedToApplication';
        public AccountId: number;
        public ApplicationId: number;
        public EventId: string;
        public constructor(applicationId: number, accountId: number) {
            this.ApplicationId = applicationId;
            this.AccountId = accountId;
        }
    }

}
module OneTrueError.Core.Applications.Commands {
    export class RemoveTeamMember {
        public static TYPE_NAME: string = 'RemoveTeamMember';
        public ApplicationId: number;
        public UserToRemove: number;
        public CommandId: string;
        public constructor(applicationId: number, userToRemove: number) {
            this.ApplicationId = applicationId;
            this.UserToRemove = userToRemove;
        }
    }

    export class UpdateApplication {
        public static TYPE_NAME: string = 'UpdateApplication';
        public ApplicationId: number;
        public Name: string;
        public TypeOfApplication: OneTrueError.Core.Applications.TypeOfApplication;
        public CommandId: string;
        public constructor(applicationId: number, name: string) {
            this.ApplicationId = applicationId;
            this.Name = name;
        }
    }

    export class CreateApplication {
        public static TYPE_NAME: string = 'CreateApplication';
        public ApplicationKey: string;
        public Name: string;
        public TypeOfApplication: OneTrueError.Core.Applications.TypeOfApplication;
        public UserId: number;
        public CommandId: string;
        public constructor(name: string, typeOfApplication: OneTrueError.Core.Applications.TypeOfApplication) {
            this.Name = name;
            this.TypeOfApplication = typeOfApplication;
        }
    }

    export class DeleteApplication {
        public static TYPE_NAME: string = 'DeleteApplication';
        public Id: number;
        public CommandId: string;
        public constructor(id: number) {
            this.Id = id;
        }
    }

}
module OneTrueError.Core.Accounts {
    export class RegisterSimple {
        public static TYPE_NAME: string = 'RegisterSimple';
        public EmailAddress: string;
        public CommandId: string;
        public constructor(emailAddress: string) {
            this.EmailAddress = emailAddress;
        }
    }

}
module OneTrueError.Core.Accounts.Requests {
    export class AcceptInvitation {
        public static TYPE_NAME: string = 'AcceptInvitation';
        public AcceptedEmail: string;
        public AccountId: number;
        public EmailUsedForTheInvitation: string;
        public FirstName: string;
        public InvitationKey: string;
        public LastName: string;
        public Password: string;
        public UserName: string;
        public RequestId: string;
        public constructor(userName: string, password: string, invitationKey: string) {
            this.UserName = userName;
            this.Password = password;
            this.InvitationKey = invitationKey;
        }
    }

    export class AcceptInvitationReply {
        public static TYPE_NAME: string = 'AcceptInvitationReply';
        public AccountId: number;
        public UserName: string;
        public constructor(accountId: number, userName: string) {
            this.AccountId = accountId;
            this.UserName = userName;
        }
    }

    export class ActivateAccount {
        public static TYPE_NAME: string = 'ActivateAccount';
        public ActivationKey: string;
        public RequestId: string;
        public constructor(activationKey: string) {
            this.ActivationKey = activationKey;
        }
    }

    export class ActivateAccountReply {
        public static TYPE_NAME: string = 'ActivateAccountReply';
        public AccountId: number;
        public UserName: string;
        public constructor(accountId: number, userName: string) {
            this.AccountId = accountId;
            this.UserName = userName;
        }
    }

    export class ChangePassword {
        public static TYPE_NAME: string = 'ChangePassword';
        public CurrentPassword: string;
        public NewPassword: string;
        public UserId: number;
        public RequestId: string;
        public constructor(currentPassword: string, newPassword: string) {
            this.CurrentPassword = currentPassword;
            this.NewPassword = newPassword;
        }
    }

    export class ChangePasswordReply {
        public static TYPE_NAME: string = 'ChangePasswordReply';
        public Success: boolean;
    }

    export class IgnoreFieldAttribute {
        public static TYPE_NAME: string = 'IgnoreFieldAttribute';
        public TypeId: any;
    }

    export class Login {
        public static TYPE_NAME: string = 'Login';
        public Password: string;
        public UserName: string;
        public RequestId: string;
        public constructor(userName: string, password: string) {
            this.UserName = userName;
            this.Password = password;
        }
    }

    export class LoginReply {
        public static TYPE_NAME: string = 'LoginReply';
        public AccountId: number;
        public Result: LoginResult;
        public UserName: string;
    }

    export enum LoginResult {
        Locked = 0,
        IncorrectLogin = 1,
        Successful = 2,
    }

    export class ResetPassword {
        public static TYPE_NAME: string = 'ResetPassword';
        public ActivationKey: string;
        public NewPassword: string;
        public RequestId: string;
        public constructor(activationKey: string, newPassword: string) {
            this.ActivationKey = activationKey;
            this.NewPassword = newPassword;
        }
    }

    export class ResetPasswordReply {
        public static TYPE_NAME: string = 'ResetPasswordReply';
        public Success: boolean;
    }

    export class ValidateNewLogin {
        public static TYPE_NAME: string = 'ValidateNewLogin';
        public Email: string;
        public UserName: string;
        public RequestId: string;
    }

    export class ValidateNewLoginReply {
        public static TYPE_NAME: string = 'ValidateNewLoginReply';
        public EmailIsTaken: boolean;
        public UserNameIsTaken: boolean;
    }

}
module OneTrueError.Core.Accounts.Queries {
    export class AccountDTO {
        public static TYPE_NAME: string = 'AccountDTO';
        public CreatedAtUtc: any;
        public Email: string;
        public Id: number;
        public LastLoginAtUtc: any;
        public State: AccountState;
        public UpdatedAtUtc: any;
        public UserName: string;
    }

    export enum AccountState {
        VerificationRequired = 0,
        Active = 1,
        Locked = 2,
        ResetPassword = 3,
    }

    export class FindAccountByUserName {
        public static TYPE_NAME: string = 'FindAccountByUserName';
        public UserName: string;
        public QueryId: string;
        public constructor(userName: string) {
            this.UserName = userName;
        }
    }

    export class GetAccountById {
        public static TYPE_NAME: string = 'GetAccountById';
        public AccountId: number;
        public QueryId: string;
        public constructor(accountId: number) {
            this.AccountId = accountId;
        }
    }

    export class GetAccountEmailById {
        public static TYPE_NAME: string = 'GetAccountEmailById';
        public AccountId: number;
        public QueryId: string;
        public constructor(accountId: number) {
            this.AccountId = accountId;
        }
    }

    export class FindAccountByUserNameResult {
        public static TYPE_NAME: string = 'FindAccountByUserNameResult';
        public AccountId: number;
        public DisplayName: string;
        public constructor(accountId: number, displayName: string) {
            this.AccountId = accountId;
            this.DisplayName = displayName;
        }
    }

}
module OneTrueError.Core.Accounts.Events {
    export class AccountActivated {
        public static TYPE_NAME: string = 'AccountActivated';
        public AccountId: number;
        public EmailAddress: string;
        public UserName: string;
        public EventId: string;
        public constructor(accountId: number, userName: string) {
            this.AccountId = accountId;
            this.UserName = userName;
        }
    }

    export class AccountRegistered {
        public static TYPE_NAME: string = 'AccountRegistered';
        public AccountId: number;
        public UserName: string;
        public EventId: string;
        public constructor(accountId: number, userName: string) {
            this.AccountId = accountId;
            this.UserName = userName;
        }
    }

    export class InvitationAccepted {
        public static TYPE_NAME: string = 'InvitationAccepted';
        public AcceptedEmailAddress: string;
        public AccountId: number;
        public ApplicationIds: number[];
        public InvitedByUserName: string;
        public InvitedEmailAddress: string;
        public UserName: string;
        public EventId: string;
        public constructor(accountId: number, invitedByUserName: string, userName: string) {
            this.AccountId = accountId;
            this.InvitedByUserName = invitedByUserName;
            this.UserName = userName;
        }
    }

    export class LoginFailed {
        public static TYPE_NAME: string = 'LoginFailed';
        public InvalidLogin: boolean;
        public IsActivated: boolean;
        public IsLocked: boolean;
        public UserName: string;
        public EventId: string;
        public constructor(userName: string) {
            this.UserName = userName;
        }
    }

}
module OneTrueError.Core.Accounts.Commands {
    export class DeclineInvitation {
        public static TYPE_NAME: string = 'DeclineInvitation';
        public InvitationId: string;
        public CommandId: string;
        public constructor(invitationId: string) {
            this.InvitationId = invitationId;
        }
    }

    export class RegisterAccount {
        public static TYPE_NAME: string = 'RegisterAccount';
        public Email: string;
        public Password: string;
        public UserName: string;
        public CommandId: string;
        public constructor(userName: string, password: string, email: string) {
            this.UserName = userName;
            this.Password = password;
            this.Email = email;
        }
    }

    export class RequestPasswordReset {
        public static TYPE_NAME: string = 'RequestPasswordReset';
        public EmailAddress: string;
        public CommandId: string;
        public constructor(emailAddress: string) {
            this.EmailAddress = emailAddress;
        }
    }

}
