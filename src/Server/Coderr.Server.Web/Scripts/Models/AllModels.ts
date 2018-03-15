module codeRR.Web.Overview.Queries {
    export class GetOverview {
        public static TYPE_NAME: string = 'GetOverview';
        public NumberOfDays: number;
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
        public IncidentsPerApplication: codeRR.Web.Overview.Queries.GetOverviewApplicationResult[];
        public StatSummary: codeRR.Web.Overview.Queries.OverviewStatSummary;
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
module codeRR.Web.Feedback.Queries {
    export class GetFeedbackForApplicationPage {
        public static TYPE_NAME: string = 'GetFeedbackForApplicationPage';
        public ApplicationId: number;
        public constructor(applicationId: number) {
            this.ApplicationId = applicationId;
        }
    }

    export class GetFeedbackForApplicationPageResult {
        public static TYPE_NAME: string = 'GetFeedbackForApplicationPageResult';
        public Emails: string[];
        public Items: codeRR.Web.Feedback.Queries.GetFeedbackForApplicationPageResultItem[];
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

    export class GetFeedbackForDashboardPage {
        public static TYPE_NAME: string = 'GetFeedbackForDashboardPage';
    }

    export class GetFeedbackForDashboardPageResult {
        public static TYPE_NAME: string = 'GetFeedbackForDashboardPageResult';
        public Emails: string[];
        public Items: codeRR.Web.Feedback.Queries.GetFeedbackForDashboardPageResultItem[];
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

    export class GetIncidentFeedback {
        public static TYPE_NAME: string = 'GetIncidentFeedback';
        public IncidentId: number;
        public constructor(incidentId: number) {
            this.IncidentId = incidentId;
        }
    }

    export class GetIncidentFeedbackResult {
        public static TYPE_NAME: string = 'GetIncidentFeedbackResult';
        public Emails: string[];
        public Items: codeRR.Web.Feedback.Queries.GetIncidentFeedbackResultItem[];
        public constructor(items: codeRR.Web.Feedback.Queries.GetIncidentFeedbackResultItem[], emails: string[]) {
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

}
module codeRR.Modules.Versions.Queries {
    export class GetApplicationVersions {
        public static TYPE_NAME: string = 'GetApplicationVersions';
        public ApplicationId: number;
        public constructor(applicationId: number) {
            this.ApplicationId = applicationId;
        }
    }

    export class GetApplicationVersionsResult {
        public static TYPE_NAME: string = 'GetApplicationVersionsResult';
        public Items: codeRR.Modules.Versions.Queries.GetApplicationVersionsResultItem[];
    }

    export class GetApplicationVersionsResultItem {
        public static TYPE_NAME: string = 'GetApplicationVersionsResultItem';
        public FirstReportReceivedAtUtc: any;
        public IncidentCount: number;
        public LastReportReceivedAtUtc: any;
        public ReportCount: number;
        public Version: string;
    }

}
module codeRR.Modules.Triggers {
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

    export class TriggerDTO {
        public static TYPE_NAME: string = 'TriggerDTO';
        public Description: string;
        public Id: string;
        public Name: string;
        public Summary: string;
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
module codeRR.Modules.Triggers.Queries {
    export class GetContextCollectionMetadata {
        public static TYPE_NAME: string = 'GetContextCollectionMetadata';
        public ApplicationId: number;
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
        public constructor(id: number) {
            this.Id = id;
        }
    }

    export class GetTriggerDTO {
        public static TYPE_NAME: string = 'GetTriggerDTO';
        public Actions: codeRR.Modules.Triggers.TriggerActionDataDTO[];
        public ApplicationId: number;
        public Description: string;
        public Id: number;
        public LastTriggerAction: codeRR.Modules.Triggers.LastTriggerActionDTO;
        public Name: string;
        public Rules: codeRR.Modules.Triggers.TriggerRuleBase[];
        public RunForExistingIncidents: boolean;
        public RunForNewIncidents: boolean;
        public RunForReOpenedIncidents: boolean;
    }

    export class GetTriggersForApplication {
        public static TYPE_NAME: string = 'GetTriggersForApplication';
        public ApplicationId: number;
        public constructor(applicationId: number) {
            this.ApplicationId = applicationId;
        }
    }

}
module codeRR.Modules.Triggers.Commands {
    export class CreateTrigger {
        public static TYPE_NAME: string = 'CreateTrigger';
        public Actions: codeRR.Modules.Triggers.TriggerActionDataDTO[];
        public ApplicationId: number;
        public Description: string;
        public Id: number;
        public LastTriggerAction: codeRR.Modules.Triggers.LastTriggerActionDTO;
        public Name: string;
        public Rules: codeRR.Modules.Triggers.TriggerRuleBase[];
        public RunForExistingIncidents: boolean;
        public RunForNewIncidents: boolean;
        public RunForReOpenedIncidents: boolean;
        public constructor(applicationId: number, name: string) {
            this.ApplicationId = applicationId;
            this.Name = name;
        }
    }

    export class DeleteTrigger {
        public static TYPE_NAME: string = 'DeleteTrigger';
        public Id: number;
        public constructor(id: number) {
            this.Id = id;
        }
    }

    export class UpdateTrigger {
        public static TYPE_NAME: string = 'UpdateTrigger';
        public Actions: codeRR.Modules.Triggers.TriggerActionDataDTO[];
        public Description: string;
        public Id: number;
        public LastTriggerAction: codeRR.Modules.Triggers.LastTriggerActionDTO;
        public Name: string;
        public Rules: codeRR.Modules.Triggers.TriggerRuleBase[];
        public RunForExistingIncidents: boolean;
        public RunForNewIncidents: boolean;
        public RunForReOpenedIncidents: boolean;
        public constructor(id: number, name: string) {
            this.Id = id;
            this.Name = name;
        }
    }

}
module codeRR.Modules.Tagging {
    export class TagDTO {
        public static TYPE_NAME: string = 'TagDTO';
        public Name: string;
        public OrderNumber: number;
    }

}
module codeRR.Modules.Tagging.Queries {
    export class GetTagsForApplication {
        public static TYPE_NAME: string = 'GetTagsForApplication';
        public ApplicationId: number;
        public constructor(applicationId: number) {
            this.ApplicationId = applicationId;
        }
    }

    export class GetTagsForIncident {
        public static TYPE_NAME: string = 'GetTagsForIncident';
        public IncidentId: number;
        public constructor(incidentId: number) {
            this.IncidentId = incidentId;
        }
    }

}
module codeRR.Modules.Tagging.Events {
    export class TagAttachedToIncident {
        public static TYPE_NAME: string = 'TagAttachedToIncident';
        public IncidentId: number;
        public Tags: codeRR.Modules.Tagging.TagDTO[];
        public constructor(incidentId: number, tags: codeRR.Modules.Tagging.TagDTO[]) {
            this.IncidentId = incidentId;
            this.Tags = tags;
        }
    }

}
module codeRR.Modules.ErrorOrigins.Queries {
    export class GetOriginsForIncident {
        public static TYPE_NAME: string = 'GetOriginsForIncident';
        public IncidentId: number;
        public constructor(incidentId: number) {
            this.IncidentId = incidentId;
        }
    }

    export class GetOriginsForIncidentResult {
        public static TYPE_NAME: string = 'GetOriginsForIncidentResult';
        public Items: codeRR.Modules.ErrorOrigins.Queries.GetOriginsForIncidentResultItem[];
    }

    export class GetOriginsForIncidentResultItem {
        public static TYPE_NAME: string = 'GetOriginsForIncidentResultItem';
        public Latitude: number;
        public Longitude: number;
        public NumberOfErrorReports: number;
    }

}
module codeRR.Modules.ContextData.Queries {
    export class GetSimilarities {
        public static TYPE_NAME: string = 'GetSimilarities';
        public IncidentId: number;
        public constructor(incidentId: number) {
            this.IncidentId = incidentId;
        }
    }

    export class GetSimilaritiesCollection {
        public static TYPE_NAME: string = 'GetSimilaritiesCollection';
        public Name: string;
        public Similarities: codeRR.Modules.ContextData.Queries.GetSimilaritiesSimilarity[];
    }

    export class GetSimilaritiesResult {
        public static TYPE_NAME: string = 'GetSimilaritiesResult';
        public Collections: codeRR.Modules.ContextData.Queries.GetSimilaritiesCollection[];
    }

    export class GetSimilaritiesSimilarity {
        public static TYPE_NAME: string = 'GetSimilaritiesSimilarity';
        public Name: string;
        public Values: codeRR.Modules.ContextData.Queries.GetSimilaritiesValue[];
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
module codeRR.Core {
    export class EnumExtensions {
        public static TYPE_NAME: string = 'EnumExtensions';
    }

}
module codeRR.Core.Users {
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
module codeRR.Core.Users.Queries {
    export class GetUserSettings {
        public static TYPE_NAME: string = 'GetUserSettings';
        public ApplicationId: number;
        public UserId: number;
    }

    export class GetUserSettingsResult {
        public static TYPE_NAME: string = 'GetUserSettingsResult';
        public EmailAddress: string;
        public FirstName: string;
        public LastName: string;
        public MobileNumber: string;
        public Notifications: codeRR.Core.Users.NotificationSettings;
    }

}
module codeRR.Core.Users.Commands {
    export class UpdateNotifications {
        public static TYPE_NAME: string = 'UpdateNotifications';
        public ApplicationId: number;
        public NotifyOnNewIncidents: codeRR.Core.Users.NotificationState;
        public NotifyOnNewReport: codeRR.Core.Users.NotificationState;
        public NotifyOnPeaks: codeRR.Core.Users.NotificationState;
        public NotifyOnReOpenedIncident: codeRR.Core.Users.NotificationState;
        public NotifyOnUserFeedback: codeRR.Core.Users.NotificationState;
        public UserId: number;
    }

    export class UpdatePersonalSettings {
        public static TYPE_NAME: string = 'UpdatePersonalSettings';
        public EmailAddress: string;
        public FirstName: string;
        public LastName: string;
        public MobileNumber: string;
        public UserId: number;
    }

}
module codeRR.Core.Support {
    export class SendSupportRequest {
        public static TYPE_NAME: string = 'SendSupportRequest';
        public Message: string;
        public Subject: string;
        public Url: string;
    }

}
module codeRR.Core.Reports {
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
        public ContextCollections: codeRR.Core.Reports.ContextCollectionDTO[];
        public CreatedAtUtc: any;
        public Exception: codeRR.Core.Reports.ReportExeptionDTO;
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
        public InnerException: codeRR.Core.Reports.ReportExeptionDTO;
        public Message: string;
        public Name: string;
        public Namespace: string;
        public Properties: string[];
        public StackTrace: string;
    }

}
module codeRR.Core.Reports.Queries {
    export class GetReport {
        public static TYPE_NAME: string = 'GetReport';
        public ReportId: number;
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
        public InnerException: codeRR.Core.Reports.Queries.GetReportException;
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
        public constructor(incidentId: number) {
            this.IncidentId = incidentId;
        }
    }

    export class GetReportListResult {
        public static TYPE_NAME: string = 'GetReportListResult';
        public Items: codeRR.Core.Reports.Queries.GetReportListResultItem[];
        public PageNumber: number;
        public PageSize: number;
        public TotalCount: number;
        public constructor(items: codeRR.Core.Reports.Queries.GetReportListResultItem[]) {
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
        public ContextCollections: codeRR.Core.Reports.Queries.GetReportResultContextCollection[];
        public CreatedAtUtc: any;
        public EmailAddress: string;
        public ErrorId: string;
        public Exception: codeRR.Core.Reports.Queries.GetReportException;
        public Id: string;
        public IncidentId: string;
        public Message: string;
        public StackTrace: string;
        public UserFeedback: string;
    }

    export class GetReportResultContextCollection {
        public static TYPE_NAME: string = 'GetReportResultContextCollection';
        public Name: string;
        public Properties: codeRR.Core.Reports.Queries.KeyValuePair[];
        public constructor(name: string, properties: codeRR.Core.Reports.Queries.KeyValuePair[]) {
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
module codeRR.Core.Notifications {
    export class AddNotification {
        public static TYPE_NAME: string = 'AddNotification';
        public AccountId: number;
        public HoldbackInterval: any;
        public Message: string;
        public NotificationType: string;
        public RoleName: string;
        public constructor(accountId: number, message: string) {
            this.AccountId = accountId;
            this.Message = message;
        }
    }

}
module codeRR.Core.Messaging {
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
        public Recipients: codeRR.Core.Messaging.EmailAddress[];
        public ReplyTo: codeRR.Core.Messaging.EmailAddress;
        public Resources: codeRR.Core.Messaging.EmailResource[];
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
module codeRR.Core.Messaging.Commands {
    export class SendEmail {
        public static TYPE_NAME: string = 'SendEmail';
        public EmailMessage: codeRR.Core.Messaging.EmailMessage;
    }

    export class SendSms {
        public static TYPE_NAME: string = 'SendSms';
        public Message: string;
        public PhoneNumber: string;
        public constructor(phoneNumber: string, message: string) {
            this.PhoneNumber = phoneNumber;
            this.Message = message;
        }
    }

    export class SendTemplateEmail {
        public static TYPE_NAME: string = 'SendTemplateEmail';
        public MailTitle: string;
        public Model: any;
        public Resources: codeRR.Core.Messaging.EmailResource[];
        public Subject: string;
        public TemplateName: string;
        public To: string;
        public constructor(mailTitle: string, templateName: string) {
            this.MailTitle = mailTitle;
            this.TemplateName = templateName;
        }
    }

}
module codeRR.Core.Invitations.Queries {
    export class GetInvitationByKey {
        public static TYPE_NAME: string = 'GetInvitationByKey';
        public InvitationKey: string;
        public constructor(invitationKey: string) {
            this.InvitationKey = invitationKey;
        }
    }

    export class GetInvitationByKeyResult {
        public static TYPE_NAME: string = 'GetInvitationByKeyResult';
        public EmailAddress: string;
    }

}
module codeRR.Core.Invitations.Commands {
    export class InviteUser {
        public static TYPE_NAME: string = 'InviteUser';
        public ApplicationId: number;
        public EmailAddress: string;
        public Text: string;
        public UserId: number;
        public constructor(applicationId: number, emailAddress: string) {
            this.ApplicationId = applicationId;
            this.EmailAddress = emailAddress;
        }
    }

}
module codeRR.Core.Incidents {
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
        public AssignedToUserId: number;
        public LastUpdateAtUtc: any;
        public Name: string;
        public ReportCount: number;
        public constructor(id: number, name: string) {
            this.Id = id;
            this.Name = name;
        }
    }

}
module codeRR.Core.Incidents.Queries {
    export class FindIncidents {
        public static TYPE_NAME: string = 'FindIncidents';
        public ApplicationIds: number[];
        public FreeText: string;
        public IsAssigned: boolean;
        public IsClosed: boolean;
        public IsIgnored: boolean;
        public IsNew: boolean;
        public ItemsPerPage: number;
        public MaxDate: any;
        public MinDate: any;
        public PageNumber: number;
        public ReOpened: boolean;
        public SortAscending: boolean;
        public SortType: codeRR.Core.Incidents.IncidentOrder;
        public Version: string;
        public Tags: string[];
        public AssignedToId: number;
    }

    export class FindIncidentsResult {
        public static TYPE_NAME: string = 'FindIncidentsResult';
        public Items: codeRR.Core.Incidents.Queries.FindIncidentsResultItem[];
        public PageNumber: number;
        public PageSize: number;
        public TotalCount: number;
    }

    export class FindIncidentsResultItem {
        public static TYPE_NAME: string = 'FindIncidentsResultItem';
        public ApplicationId: string;
        public ApplicationName: string;
        public CreatedAtUtc: any;
        public Id: number;
        public IsReOpened: boolean;
        public LastUpdateAtUtc: any;
        public Name: string;
        public ReportCount: number;
        public LastReportReceivedAtUtc: any;
        public constructor(id: number, name: string) {
            this.Id = id;
            this.Name = name;
        }
    }

    export class GetIncident {
        public static TYPE_NAME: string = 'GetIncident';
        public IncidentId: number;
        public constructor(incidentId: number) {
            this.IncidentId = incidentId;
        }
    }

    export class GetIncidentForClosePage {
        public static TYPE_NAME: string = 'GetIncidentForClosePage';
        public IncidentId: number;
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
        public AssignedAtUtc: any;
        public AssignedTo: string;
        public AssignedToId: number;
        public ContextCollections: string[];
        public CreatedAtUtc: any;
        public DayStatistics: codeRR.Core.Incidents.Queries.ReportDay[];
        public Description: string;
        public Facts: codeRR.Core.Incidents.Queries.QuickFact[];
        public FullName: string;
        public HashCodeIdentifier: string;
        public Id: number;
        public IncidentState: number;
        public IsIgnored: boolean;
        public IsReOpened: boolean;
        public IsSolutionShared: boolean;
        public IsSolved: boolean;
        public LastReportReceivedAtUtc: any;
        public PreviousSolutionAtUtc: any;
        public ReOpenedAtUtc: any;
        public ReportCount: number;
        public ReportHashCode: string;
        public Solution: string;
        public SolvedAtUtc: any;
        public StackTrace: string;
        public Tags: string[];
        public UpdatedAtUtc: any;
        public SuggestedSolutions: codeRR.Core.Incidents.Queries.SuggestedIncidentSolution[];
        public HighlightedContextData: codeRR.Core.Incidents.Queries.HighlightedContextData[];
    }

    export class GetIncidentStatistics {
        public static TYPE_NAME: string = 'GetIncidentStatistics';
        public IncidentId: number;
        public NumberOfDays: number;
    }

    export class GetIncidentStatisticsResult {
        public static TYPE_NAME: string = 'GetIncidentStatisticsResult';
        public Labels: string[];
        public Values: number[];
    }

    export class HighlightedContextData {
        public static TYPE_NAME: string = 'HighlightedContextData';
        public Description: string;
        public Name: string;
        public Url: string;
        public Value: string[];
    }

    export class QuickFact {
        public static TYPE_NAME: string = 'QuickFact';
        public Description: string;
        public Title: string;
        public Url: string;
        public Value: string;
    }

    export class ReportDay {
        public static TYPE_NAME: string = 'ReportDay';
        public Count: number;
        public Date: any;
    }

    export class SuggestedIncidentSolution {
        public static TYPE_NAME: string = 'SuggestedIncidentSolution';
        public Reason: string;
        public SuggestedSolution: string;
    }

}
module codeRR.Core.Incidents.Events {
    export class IncidentAssigned {
        public static TYPE_NAME: string = 'IncidentAssigned';
        public AssignedById: number;
        public AssignedToId: number;
        public IncidentId: number;
        public constructor(incidentId: number, assignedById: number, assignedToId: number) {
            this.IncidentId = incidentId;
            this.AssignedById = assignedById;
            this.AssignedToId = assignedToId;
        }
    }

    export class IncidentIgnored {
        public static TYPE_NAME: string = 'IncidentIgnored';
        public AccountId: number;
        public IncidentId: number;
        public UserName: string;
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
        public constructor(applicationId: number, incidentId: number, createdAtUtc: any) {
            this.ApplicationId = applicationId;
            this.IncidentId = incidentId;
            this.CreatedAtUtc = createdAtUtc;
        }
    }

    export class ReportAddedToIncident {
        public static TYPE_NAME: string = 'ReportAddedToIncident';
        public Incident: codeRR.Core.Incidents.IncidentSummaryDTO;
        public IsReOpened: boolean;
        public Report: codeRR.Core.Reports.ReportDTO;
        public constructor(incident: codeRR.Core.Incidents.IncidentSummaryDTO, report: codeRR.Core.Reports.ReportDTO, isReOpened: boolean) {
            this.Incident = incident;
            this.Report = report;
            this.IsReOpened = isReOpened;
        }
    }

}
module codeRR.Core.Incidents.Commands {
    export class AssignIncident {
        public static TYPE_NAME: string = 'AssignIncident';
        public AssignedBy: number;
        public AssignedTo: number;
        public IncidentId: number;
        public constructor(incidentId: number, assignedTo: number, assignedBy: number) {
            this.IncidentId = incidentId;
            this.AssignedTo = assignedTo;
            this.AssignedBy = assignedBy;
        }
    }

    export class CloseIncident {
        public static TYPE_NAME: string = 'CloseIncident';
        public CanSendNotification: boolean;
        public IncidentId: number;
        public NotificationText: string;
        public NotificationTitle: string;
        public ShareSolution: boolean;
        public Solution: string;
        public UserId: number;
        public constructor(solution: string, incidentId: number) {
            this.Solution = solution;
            this.IncidentId = incidentId;
        }
    }

    export class IgnoreIncident {
        public static TYPE_NAME: string = 'IgnoreIncident';
        public IncidentId: number;
        public UserId: number;
        public constructor(incidentId: number) {
            this.IncidentId = incidentId;
        }
    }

    export class ReOpenIncident {
        public static TYPE_NAME: string = 'ReOpenIncident';
        public IncidentId: number;
        public UserId: number;
        public constructor(incidentId: number) {
            this.IncidentId = incidentId;
        }
    }

}
module codeRR.Core.Feedback.Events {
    export class FeedbackAttachedToIncident {
        public static TYPE_NAME: string = 'FeedbackAttachedToIncident';
        public IncidentId: number;
        public Message: string;
        public UserEmailAddress: string;
    }

}
module codeRR.Core.Feedback.Commands {
    export class SubmitFeedback {
        public static TYPE_NAME: string = 'SubmitFeedback';
        public CreatedAtUtc: any;
        public Email: string;
        public ErrorId: string;
        public Feedback: string;
        public RemoteAddress: string;
        public ReportId: number;
        public constructor(errorId: string, remoteAddress: string) {
            this.ErrorId = errorId;
            this.RemoteAddress = remoteAddress;
        }
    }

}
module codeRR.Core.Applications {
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
module codeRR.Core.Applications.Queries {
    export class GetApplicationIdByKey {
        public static TYPE_NAME: string = 'GetApplicationIdByKey';
        public ApplicationKey: string;
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
    }

    export class GetApplicationInfoResult {
        public static TYPE_NAME: string = 'GetApplicationInfoResult';
        public AppKey: string;
        public ApplicationType: codeRR.Core.Applications.TypeOfApplication;
        public Id: number;
        public Name: string;
        public SharedSecret: string;
        public TotalIncidentCount: number;
        public Versions: string[];
    }

    export class GetApplicationList {
        public static TYPE_NAME: string = 'GetApplicationList';
        public AccountId: number;
        public FilterAsAdmin: boolean;
    }

    export class GetApplicationOverview {
        public static TYPE_NAME: string = 'GetApplicationOverview';
        public ApplicationId: number;
        public NumberOfDays: number;
        public Version: string;
        public constructor(applicationId: number) {
            this.ApplicationId = applicationId;
        }
    }

    export class GetApplicationOverviewResult {
        public static TYPE_NAME: string = 'GetApplicationOverviewResult';
        public Days: number;
        public ErrorReports: number[];
        public Incidents: number[];
        public StatSummary: codeRR.Core.Applications.Queries.OverviewStatSummary;
        public TimeAxisLabels: string[];
    }

    export class GetApplicationTeam {
        public static TYPE_NAME: string = 'GetApplicationTeam';
        public ApplicationId: number;
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

    export class GetApplicationTeamResult {
        public static TYPE_NAME: string = 'GetApplicationTeamResult';
        public Invited: codeRR.Core.Applications.Queries.GetApplicationTeamResultInvitation[];
        public Members: codeRR.Core.Applications.Queries.GetApplicationTeamMember[];
    }

    export class GetApplicationTeamResultInvitation {
        public static TYPE_NAME: string = 'GetApplicationTeamResultInvitation';
        public EmailAddress: string;
        public InvitedAtUtc: any;
        public InvitedByUserName: string;
    }

    export class OverviewStatSummary {
        public static TYPE_NAME: string = 'OverviewStatSummary';
        public Followers: number;
        public Incidents: number;
        public Reports: number;
        public UserFeedback: number;
    }

}
module codeRR.Core.Applications.Events {
    export class ApplicationCreated {
        public static TYPE_NAME: string = 'ApplicationCreated';
        public AppKey: string;
        public ApplicationId: number;
        public ApplicationName: string;
        public CreatedById: number;
        public SharedSecret: string;
        public constructor(id: number, name: string, createdById: number, appKey: string, sharedSecret: string) {
            this.CreatedById = createdById;
            this.AppKey = appKey;
            this.SharedSecret = sharedSecret;
        }
    }

    export class ApplicationDeleted {
        public static TYPE_NAME: string = 'ApplicationDeleted';
        public AppKey: string;
        public ApplicationId: number;
        public ApplicationName: string;
    }

    export class UserAddedToApplication {
        public static TYPE_NAME: string = 'UserAddedToApplication';
        public AccountId: number;
        public ApplicationId: number;
        public constructor(applicationId: number, accountId: number) {
            this.ApplicationId = applicationId;
            this.AccountId = accountId;
        }
    }

    export class UserInvitedToApplication {
        public static TYPE_NAME: string = 'UserInvitedToApplication';
        public ApplicationId: number;
        public ApplicationName: string;
        public EmailAddress: string;
        public InvitationKey: string;
        public InvitedBy: string;
        public constructor(invitationKey: string, applicationId: number, applicationName: string, emailAddress: string, invitedBy: string) {
            this.InvitationKey = invitationKey;
            this.ApplicationId = applicationId;
            this.ApplicationName = applicationName;
            this.EmailAddress = emailAddress;
            this.InvitedBy = invitedBy;
        }
    }

}
module codeRR.Core.Applications.Commands {
    export class CreateApplication {
        public static TYPE_NAME: string = 'CreateApplication';
        public ApplicationKey: string;
        public Name: string;
        public TypeOfApplication: codeRR.Core.Applications.TypeOfApplication;
        public UserId: number;
        public constructor(name: string, typeOfApplication: codeRR.Core.Applications.TypeOfApplication) {
            this.Name = name;
            this.TypeOfApplication = typeOfApplication;
        }
    }

    export class DeleteApplication {
        public static TYPE_NAME: string = 'DeleteApplication';
        public Id: number;
        public constructor(id: number) {
            this.Id = id;
        }
    }

    export class RemoveTeamMember {
        public static TYPE_NAME: string = 'RemoveTeamMember';
        public ApplicationId: number;
        public UserToRemove: number;
        public constructor(applicationId: number, userToRemove: number) {
            this.ApplicationId = applicationId;
            this.UserToRemove = userToRemove;
        }
    }

    export class UpdateApplication {
        public static TYPE_NAME: string = 'UpdateApplication';
        public ApplicationId: number;
        public Name: string;
        public TypeOfApplication: codeRR.Core.Applications.TypeOfApplication;
        public constructor(applicationId: number, name: string) {
            this.ApplicationId = applicationId;
            this.Name = name;
        }
    }

}
module codeRR.Core.ApiKeys.Queries {
    export class GetApiKey {
        public static TYPE_NAME: string = 'GetApiKey';
        public ApiKey: string;
        public Id: number;
        public constructor(id: number) {
            this.Id = id;
        }
    }

    export class GetApiKeyResult {
        public static TYPE_NAME: string = 'GetApiKeyResult';
        public AllowedApplications: codeRR.Core.ApiKeys.Queries.GetApiKeyResultApplication[];
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
    }

    export class ListApiKeysResult {
        public static TYPE_NAME: string = 'ListApiKeysResult';
        public Keys: codeRR.Core.ApiKeys.Queries.ListApiKeysResultItem[];
    }

    export class ListApiKeysResultItem {
        public static TYPE_NAME: string = 'ListApiKeysResultItem';
        public ApiKey: string;
        public ApplicationName: string;
        public Id: number;
    }

}
module codeRR.Core.ApiKeys.Events {
    export class ApiKeyCreated {
        public static TYPE_NAME: string = 'ApiKeyCreated';
        public ApiKey: string;
        public ApplicationIds: number[];
        public ApplicationNameForTheAppUsingTheKey: string;
        public CreatedById: number;
        public SharedSecret: string;
        public constructor(applicationNameForTheAppUsingTheKey: string, apiKey: string, sharedSecret: string, applicationIds: number[], createdById: number) {
            this.ApplicationNameForTheAppUsingTheKey = applicationNameForTheAppUsingTheKey;
            this.ApiKey = apiKey;
            this.SharedSecret = sharedSecret;
            this.ApplicationIds = applicationIds;
            this.CreatedById = createdById;
        }
    }

    export class ApiKeyRemoved {
        public static TYPE_NAME: string = 'ApiKeyRemoved';
    }

}
module codeRR.Core.ApiKeys.Commands {
    export class CreateApiKey {
        public static TYPE_NAME: string = 'CreateApiKey';
        public AccountId: number;
        public ApiKey: string;
        public ApplicationIds: number[];
        public ApplicationName: string;
        public SharedSecret: string;
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
        public constructor(id: number) {
            this.Id = id;
        }
    }

    export class EditApiKey {
        public static TYPE_NAME: string = 'EditApiKey';
        public ApplicationIds: number[];
        public ApplicationName: string;
        public Id: number;
        public constructor(id: number) {
            this.Id = id;
        }
    }

}
module codeRR.Core.Accounts {
    export class RegisterSimple {
        public static TYPE_NAME: string = 'RegisterSimple';
        public EmailAddress: string;
        public constructor(emailAddress: string) {
            this.EmailAddress = emailAddress;
        }
    }

}
module codeRR.Core.Accounts.Requests {
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
        public constructor(userName: string, password: string, invitationKey: string) {
            this.UserName = userName;
            this.Password = password;
            this.InvitationKey = invitationKey;
        }
    }

    export class ChangePassword {
        public static TYPE_NAME: string = 'ChangePassword';
        public CurrentPassword: string;
        public NewPassword: string;
        public UserId: number;
        public constructor(currentPassword: string, newPassword: string) {
            this.CurrentPassword = currentPassword;
            this.NewPassword = newPassword;
        }
    }

    export class ValidateNewLoginReply {
        public static TYPE_NAME: string = 'ValidateNewLoginReply';
        public EmailIsTaken: boolean;
        public UserNameIsTaken: boolean;
    }

}
module codeRR.Core.Accounts.Queries {
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
        public constructor(userName: string) {
            this.UserName = userName;
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

    export class GetAccountById {
        public static TYPE_NAME: string = 'GetAccountById';
        public AccountId: number;
        public constructor(accountId: number) {
            this.AccountId = accountId;
        }
    }

    export class GetAccountEmailById {
        public static TYPE_NAME: string = 'GetAccountEmailById';
        public AccountId: number;
        public constructor(accountId: number) {
            this.AccountId = accountId;
        }
    }

}
module codeRR.Core.Accounts.Events {
    export class AccountActivated {
        public static TYPE_NAME: string = 'AccountActivated';
        public AccountId: number;
        public EmailAddress: string;
        public UserName: string;
        public constructor(accountId: number, userName: string) {
            this.AccountId = accountId;
            this.UserName = userName;
        }
    }

    export class AccountRegistered {
        public static TYPE_NAME: string = 'AccountRegistered';
        public AccountId: number;
        public IsSysAdmin: boolean;
        public UserName: string;
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
        public constructor(userName: string) {
            this.UserName = userName;
        }
    }

}
module codeRR.Core.Accounts.Commands {
    export class DeclineInvitation {
        public static TYPE_NAME: string = 'DeclineInvitation';
        public InvitationId: string;
        public constructor(invitationId: string) {
            this.InvitationId = invitationId;
        }
    }

    export class RegisterAccount {
        public static TYPE_NAME: string = 'RegisterAccount';
        public AccountId: number;
        public ActivateDirectly: boolean;
        public Email: string;
        public Password: string;
        public UserName: string;
        public constructor(userName: string, password: string, email: string) {
            this.UserName = userName;
            this.Password = password;
            this.Email = email;
        }
    }

    export class RequestPasswordReset {
        public static TYPE_NAME: string = 'RequestPasswordReset';
        public EmailAddress: string;
        public constructor(emailAddress: string) {
            this.EmailAddress = emailAddress;
        }
    }

}
