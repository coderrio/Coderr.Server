module OneTrueError.Web.Overview.Queries {
    export class GetOverview {
        static TYPE_NAME = "GetOverview";
        NumberOfDays: number;
        QueryId: string;
    }

    export class GetOverviewApplicationResult {
        static TYPE_NAME = "GetOverviewApplicationResult";
        Label: string;
        Values: number[];

        public constructor(label: string, startDate: any, days: number) {
            this.Label = label;
        }
    }

    export class GetOverviewResult {
        static TYPE_NAME = "GetOverviewResult";
        StatSummary: OverviewStatSummary;
        Days: number;
        TimeAxisLabels: string[];
        IncidentsPerApplication: GetOverviewApplicationResult[];
    }

    export class OverviewStatSummary {
        static TYPE_NAME = "OverviewStatSummary";
        Reports: number;
        Incidents: number;
        UserFeedback: number;
        Followers: number;
    }

}

module OneTrueError.Web.Feedback.Queries {
    export class GetFeedbackForApplicationPage {
        static TYPE_NAME = "GetFeedbackForApplicationPage";
        ApplicationId: number;
        QueryId: string;

        public constructor(applicationId: number) {
            this.ApplicationId = applicationId;
        }
    }

    export class GetFeedbackForApplicationPageResult {
        static TYPE_NAME = "GetFeedbackForApplicationPageResult";
        TotalCount: number;
        Items: GetFeedbackForApplicationPageResultItem[];
        Emails: string[];
    }

    export class GetFeedbackForApplicationPageResultItem {
        static TYPE_NAME = "GetFeedbackForApplicationPageResultItem";
        EmailAddress: string;
        IncidentId: number;
        IncidentName: string;
        Message: string;
        WrittenAtUtc: any;
    }

    export class GetIncidentFeedback {
        static TYPE_NAME = "GetIncidentFeedback";
        IncidentId: number;
        QueryId: string;

        public constructor(incidentId: number) {
            this.IncidentId = incidentId;
        }
    }

    export class GetIncidentFeedbackResult {
        static TYPE_NAME = "GetIncidentFeedbackResult";
        Emails: string[];
        Items: GetIncidentFeedbackResultItem[];

        public constructor(items: GetIncidentFeedbackResultItem[], emails: string[]) {
            this.Items = items;
            this.Emails = emails;
        }
    }

    export class GetIncidentFeedbackResultItem {
        static TYPE_NAME = "GetIncidentFeedbackResultItem";
        EmailAddress: string;
        Message: string;
        WrittenAtUtc: any;
    }

    export class GetFeedbackForDashboardPage {
        static TYPE_NAME = "GetFeedbackForDashboardPage";
        QueryId: string;
    }

    export class GetFeedbackForDashboardPageResult {
        static TYPE_NAME = "GetFeedbackForDashboardPageResult";
        Emails: string[];
        Items: GetFeedbackForDashboardPageResultItem[];
        TotalCount: number;
    }

    export class GetFeedbackForDashboardPageResultItem {
        static TYPE_NAME = "GetFeedbackForDashboardPageResultItem";
        ApplicationId: number;
        ApplicationName: string;
        EmailAddress: string;
        Message: string;
        WrittenAtUtc: any;
    }

}

module OneTrueError.Modules.Triggers {
    export enum LastTriggerActionDTO {
        ExecuteActions = 0,
        AbortTrigger = 1,
    }

    export class TriggerActionDataDTO {
        static TYPE_NAME = "TriggerActionDataDTO";
        ActionContext: string;
        ActionName: string;
    }

    export class TriggerContextRule {
        static TYPE_NAME = "TriggerContextRule";
        ContextName: string;
        PropertyName: string;
        PropertyValue: string;
        Filter: TriggerFilterCondition;
        ResultToUse: TriggerRuleAction;
    }

    export class TriggerExceptionRule {
        static TYPE_NAME = "TriggerExceptionRule";
        FieldName: string;
        Value: string;
        Filter: TriggerFilterCondition;
        ResultToUse: TriggerRuleAction;
    }

    export enum TriggerFilterCondition {
        StartsWith = 0,
        EndsWith = 1,
        Contains = 2,
        DoNotContain = 3,
        Equals = 4,
    }

    export class TriggerDTO {
        static TYPE_NAME = "TriggerDTO";
        Id: string;
        Name: string;
        Description: string;
        Summary: string;
    }

    export enum TriggerRuleAction {
        AbortTrigger = 0,
        ContinueWithNextRule = 1,
        ExecuteActions = 2,
    }

    export class TriggerRuleBase {
        static TYPE_NAME = "TriggerRuleBase";
        Filter: TriggerFilterCondition;
        ResultToUse: TriggerRuleAction;
    }

}

module OneTrueError.Modules.Triggers.Queries {
    export class GetContextCollectionMetadata {
        static TYPE_NAME = "GetContextCollectionMetadata";
        ApplicationId: number;
        QueryId: string;

        public constructor(applicationId: number) {
            this.ApplicationId = applicationId;
        }
    }

    export class GetContextCollectionMetadataItem {
        static TYPE_NAME = "GetContextCollectionMetadataItem";
        Name: string;
        Properties: string[];
    }

    export class GetTrigger {
        static TYPE_NAME = "GetTrigger";
        Id: number;
        QueryId: string;

        public constructor(id: number) {
            this.Id = id;
        }
    }

    export class GetTriggerDTO {
        static TYPE_NAME = "GetTriggerDTO";
        Actions: TriggerActionDataDTO[];
        ApplicationId: number;
        Description: string;
        Id: number;
        LastTriggerAction: Triggers.LastTriggerActionDTO;
        Name: string;
        Rules: TriggerRuleBase[];
        RunForExistingIncidents: boolean;
        RunForNewIncidents: boolean;
        RunForReOpenedIncidents: boolean;
    }

    export class GetTriggersForApplication {
        static TYPE_NAME = "GetTriggersForApplication";
        ApplicationId: number;
        QueryId: string;

        public constructor(applicationId: number) {
            this.ApplicationId = applicationId;
        }
    }

}

module OneTrueError.Modules.Triggers.Commands {
    export class CreateTrigger {
        static TYPE_NAME = "CreateTrigger";
        Actions: TriggerActionDataDTO[];
        ApplicationId: number;
        Description: string;
        Id: number;
        LastTriggerAction: Triggers.LastTriggerActionDTO;
        Name: string;
        Rules: TriggerRuleBase[];
        RunForExistingIncidents: boolean;
        RunForNewIncidents: boolean;
        RunForReOpenedIncidents: boolean;
        CommandId: string;

        public constructor(applicationId: number, name: string) {
            this.ApplicationId = applicationId;
            this.Name = name;
        }
    }

    export class DeleteTrigger {
        static TYPE_NAME = "DeleteTrigger";
        Id: number;
        CommandId: string;

        public constructor(id: number) {
            this.Id = id;
        }
    }

    export class UpdateTrigger {
        static TYPE_NAME = "UpdateTrigger";
        Actions: TriggerActionDataDTO[];
        Description: string;
        Id: number;
        LastTriggerAction: Triggers.LastTriggerActionDTO;
        Name: string;
        Rules: TriggerRuleBase[];
        RunForExistingIncidents: boolean;
        RunForNewIncidents: boolean;
        RunForReOpenedIncidents: boolean;
        CommandId: string;

        public constructor(id: number, name: string) {
            this.Id = id;
            this.Name = name;
        }
    }

}

module OneTrueError.Modules.Tagging {
    export class TagDTO {
        static TYPE_NAME = "TagDTO";
        Name: string;
        OrderNumber: number;
    }

}

module OneTrueError.Modules.Tagging.Queries {
    export class GetTagsForIncident {
        static TYPE_NAME = "GetTagsForIncident";
        IncidentId: number;
        QueryId: string;

        public constructor(incidentId: number) {
            this.IncidentId = incidentId;
        }
    }

}

module OneTrueError.Modules.Tagging.Events {
    export class TagAttachedToIncident {
        static TYPE_NAME = "TagAttachedToIncident";
        IncidentId: number;
        Tags: TagDTO[];
        EventId: string;

        public constructor(incidentId: number, tags: TagDTO[]) {
            this.IncidentId = incidentId;
            this.Tags = tags;
        }
    }

}

module OneTrueError.Modules.ContextData.Queries {
    export class GetSimilarities {
        static TYPE_NAME = "GetSimilarities";
        IncidentId: number;
        QueryId: string;

        public constructor(incidentId: number) {
            this.IncidentId = incidentId;
        }
    }

    export class GetSimilaritiesCollection {
        static TYPE_NAME = "GetSimilaritiesCollection";
        Name: string;
        Similarities: GetSimilaritiesSimilarity[];
    }

    export class GetSimilaritiesResult {
        static TYPE_NAME = "GetSimilaritiesResult";
        Collections: GetSimilaritiesCollection[];
    }

    export class GetSimilaritiesSimilarity {
        static TYPE_NAME = "GetSimilaritiesSimilarity";
        Name: string;
        Values: GetSimilaritiesValue[];

        public constructor(name: string) {
            this.Name = name;
        }
    }

    export class GetSimilaritiesValue {
        static TYPE_NAME = "GetSimilaritiesValue";
        Count: number;
        Percentage: number;
        Value: string;

        public constructor(value: string, percentage: number, count: number) {
            this.Value = value;
            this.Percentage = percentage;
            this.Count = count;
        }
    }

}

module OneTrueError.Modules.ErrorOrigins.Queries {
    export class GetOriginsForIncident {
        static TYPE_NAME = "GetOriginsForIncident";
        IncidentId: number;
        QueryId: string;

        public constructor(incidentId: number) {
            this.IncidentId = incidentId;
        }
    }

    export class GetOriginsForIncidentResult {
        static TYPE_NAME = "GetOriginsForIncidentResult";
        Items: GetOriginsForIncidentResultItem[];
    }

    export class GetOriginsForIncidentResultItem {
        static TYPE_NAME = "GetOriginsForIncidentResultItem";
        Latitude: number;
        Longitude: number;
        NumberOfErrorReports: number;
    }

}

module OneTrueError.Core {
    export class IgnoreFieldAttribute {
        static TYPE_NAME = "IgnoreFieldAttribute";
        TypeId: any;
    }

}

module OneTrueError.Core.Users {
    export class NotificationSettings {
        static TYPE_NAME = "NotificationSettings";
        NotifyOnPeaks: NotificationState;
        NotifyOnNewIncidents: NotificationState;
        NotifyOnNewReport: NotificationState;
        NotifyOnUserFeedback: NotificationState;
        NotifyOnReOpenedIncident: NotificationState;
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
        static TYPE_NAME = "GetUserSettings";
        ApplicationId: number;
        UserId: number;
        QueryId: string;
    }

    export class GetUserSettingsResult {
        static TYPE_NAME = "GetUserSettingsResult";
        FirstName: string;
        LastName: string;
        MobileNumber: string;
        Notifications: NotificationSettings;
    }

}

module OneTrueError.Core.Users.Commands {
    export class UpdateNotifications {
        static TYPE_NAME = "UpdateNotifications";
        ApplicationId: number;
        NotifyOnNewIncidents: Users.NotificationState;
        NotifyOnNewReport: Users.NotificationState;
        NotifyOnPeaks: Users.NotificationState;
        NotifyOnReOpenedIncident: Users.NotificationState;
        NotifyOnUserFeedback: Users.NotificationState;
        UserId: number;
        CommandId: string;
    }

    export class UpdatePersonalSettings {
        static TYPE_NAME = "UpdatePersonalSettings";
        FirstName: string;
        LastName: string;
        MobileNumber: string;
        UserId: number;
        CommandId: string;
    }

}

module OneTrueError.Core.Support {
    export class SendSupportRequest {
        static TYPE_NAME = "SendSupportRequest";
        Message: string;
        Subject: string;
        Url: string;
        CommandId: string;
    }

}

module OneTrueError.Core.Reports {
    export class ContextCollectionDTO {
        static TYPE_NAME = "ContextCollectionDTO";
        Name: string;
        Properties: string[];

        public constructor(name: string, items: string[]) {
            this.Name = name;
        }
    }

    export class ReportDTO {
        static TYPE_NAME = "ReportDTO";
        ApplicationId: number;
        ContextCollections: ContextCollectionDTO[];
        CreatedAtUtc: any;
        Exception: ReportExeptionDTO;
        Id: number;
        IncidentId: number;
        RemoteAddress: string;
        ReportId: string;
        ReportVersion: string;
    }

    export class ReportExeptionDTO {
        static TYPE_NAME = "ReportExeptionDTO";
        FullName: string;
        Name: string;
        Namespace: string;
        AssemblyName: string;
        Message: string;
        StackTrace: string;
        InnerException: ReportExeptionDTO;
        BaseClasses: string[];
        Everything: string;
        Properties: string[];
    }

}

module OneTrueError.Core.Reports.Queries {
    export class GetReport {
        static TYPE_NAME = "GetReport";
        ReportId: number;
        QueryId: string;

        public constructor(reportId: number) {
            this.ReportId = reportId;
        }
    }

    export class GetReportException {
        static TYPE_NAME = "GetReportException";
        AssemblyName: string;
        BaseClasses: string[];
        Everything: string;
        FullName: string;
        InnerException: GetReportException;
        Message: string;
        Name: string;
        Namespace: string;
        StackTrace: string;
    }

    export class GetReportList {
        static TYPE_NAME = "GetReportList";
        IncidentId: number;
        PageNumber: number;
        PageSize: number;
        QueryId: string;

        public constructor(incidentId: number) {
            this.IncidentId = incidentId;
        }
    }

    export class GetReportListResult {
        static TYPE_NAME = "GetReportListResult";
        Items: GetReportListResultItem[];
        PageNumber: number;
        PageSize: number;
        TotalCount: number;

        public constructor(items: GetReportListResultItem[]) {
            this.Items = items;
        }
    }

    export class GetReportListResultItem {
        static TYPE_NAME = "GetReportListResultItem";
        CreatedAtUtc: any;
        Id: number;
        Message: string;
        RemoteAddress: string;
    }

    export class GetReportResult {
        static TYPE_NAME = "GetReportResult";
        ContextCollections: GetReportResultContextCollection[];
        CreatedAtUtc: any;
        EmailAddress: string;
        ErrorId: string;
        Exception: GetReportException;
        Id: string;
        IncidentId: string;
        Message: string;
        StackTrace: string;
        UserFeedback: string;
    }

    export class GetReportResultContextCollection {
        static TYPE_NAME = "GetReportResultContextCollection";
        Name: string;
        Properties: KeyValuePair[];

        public constructor(name: string, properties: KeyValuePair[]) {
            this.Name = name;
            this.Properties = properties;
        }
    }

    export class KeyValuePair {
        static TYPE_NAME = "KeyValuePair";
        Key: string;
        Value: string;

        public constructor(key: string, value: string) {
            this.Key = key;
            this.Value = value;
        }
    }

}

module OneTrueError.Core.Messaging {
    export class EmailAddress {
        static TYPE_NAME = "EmailAddress";
        Address: string;
        Name: string;

        public constructor(address: string) {
            this.Address = address;
        }
    }

    export class EmailMessage {
        static TYPE_NAME = "EmailMessage";
        HtmlBody: string;
        Recipients: EmailAddress[];
        Resources: EmailResource[];
        Subject: string;
        TextBody: string;
    }

    export class EmailResource {
        static TYPE_NAME = "EmailResource";
        Content: any;
        Name: string;

        public constructor(name: string, content: any) {
            this.Name = name;
            this.Content = content;
        }
    }

}

module OneTrueError.Core.Messaging.Commands {
    export class SendEmail {
        static TYPE_NAME = "SendEmail";
        EmailMessage: EmailMessage;
        CommandId: string;
    }

    export class SendTemplateEmail {
        static TYPE_NAME = "SendTemplateEmail";
        MailTitle: string;
        Model: any;
        Resources: EmailResource[];
        Subject: string;
        TemplateName: string;
        To: string;
        CommandId: string;

        public constructor(mailTitle: string, templateName: string) {
            this.MailTitle = mailTitle;
            this.TemplateName = templateName;
        }
    }

}

module OneTrueError.Core.Invitations.Queries {
    export class GetInvitationByKey {
        static TYPE_NAME = "GetInvitationByKey";
        InvitationKey: string;
        QueryId: string;

        public constructor(invitationKey: string) {
            this.InvitationKey = invitationKey;
        }
    }

    export class GetInvitationByKeyResult {
        static TYPE_NAME = "GetInvitationByKeyResult";
        EmailAddress: string;
    }

}

module OneTrueError.Core.Invitations.Commands {
    export class InviteUser {
        static TYPE_NAME = "InviteUser";
        ApplicationId: number;
        EmailAddress: string;
        Text: string;
        UserId: number;
        CommandId: string;

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
        static TYPE_NAME = "IncidentSummaryDTO";
        ApplicationId: number;
        ApplicationName: string;
        CreatedAtUtc: any;
        Id: number;
        IsReOpened: boolean;
        LastUpdateAtUtc: any;
        Name: string;
        ReportCount: number;

        public constructor(id: number, name: string) {
            this.Id = id;
            this.Name = name;
        }
    }

}

module OneTrueError.Core.Incidents.Queries {
    export class FindIncidentResult {
        static TYPE_NAME = "FindIncidentResult";
        Items: FindIncidentResultItem[];
        PageNumber: number;
        PageSize: number;
        TotalCount: number;
    }

    export class FindIncidentResultItem {
        static TYPE_NAME = "FindIncidentResultItem";
        ApplicationId: string;
        ApplicationName: string;
        CreatedAtUtc: any;
        Id: number;
        IsReOpened: boolean;
        LastUpdateAtUtc: any;
        Name: string;
        ReportCount: number;

        public constructor(id: number, name: string) {
            this.Id = id;
            this.Name = name;
        }
    }

    export class FindIncidents {
        static TYPE_NAME = "FindIncidents";
        ApplicationId: number;
        Closed: boolean;
        Ignored: boolean;
        ItemsPerPage: number;
        MaxDate: any;
        MinDate: any;
        Open: boolean;
        PageNumber: number;
        ReOpened: boolean;
        SortAscending: boolean;
        SortType: Incidents.IncidentOrder;
        QueryId: string;
    }

    export class GetIncident {
        static TYPE_NAME = "GetIncident";
        IncidentId: number;
        QueryId: string;

        public constructor(incidentId: number) {
            this.IncidentId = incidentId;
        }
    }

    export class GetIncidentForClosePage {
        static TYPE_NAME = "GetIncidentForClosePage";
        IncidentId: number;
        QueryId: string;

        public constructor(incidentId: number) {
            this.IncidentId = incidentId;
        }
    }

    export class GetIncidentForClosePageResult {
        static TYPE_NAME = "GetIncidentForClosePageResult";
        Description: string;
        SubscriberCount: number;
    }

    export class GetIncidentResult {
        static TYPE_NAME = "GetIncidentResult";
        ApplicationId: number;
        ContextCollections: string[];
        CreatedAtUtc: any;
        DayStatistics: ReportDay[];
        Description: string;
        FeedbackCount: number;
        FullName: string;
        HashCodeIdentifier: string;
        Id: number;
        IsIgnored: boolean;
        IsReOpened: boolean;
        IsSolutionShared: boolean;
        IsSolved: boolean;
        PreviousSolutionAtUtc: any;
        ReOpenedAtUtc: any;
        ReportCount: number;
        ReportHashCode: string;
        Solution: string;
        SolvedAtUtc: any;
        StackTrace: string;
        Tags: string[];
        UpdatedAtUtc: any;
        WaitingUserCount: number;
    }

    export class GetIncidentStatistics {
        static TYPE_NAME = "GetIncidentStatistics";
        IncidentId: number;
        NumberOfDays: number;
        QueryId: string;
    }

    export class GetIncidentStatisticsResult {
        static TYPE_NAME = "GetIncidentStatisticsResult";
        Labels: string[];
        Values: number[];
    }

    export class ReportDay {
        static TYPE_NAME = "ReportDay";
        Date: any;
        Count: number;
    }

}

module OneTrueError.Core.Incidents.Events {
    export class IncidentIgnored {
        static TYPE_NAME = "IncidentIgnored";
        IncidentId: number;
        AccountId: number;
        UserName: string;
        EventId: string;

        public constructor(incidentId: number, accountId: number, userName: string) {
            this.IncidentId = incidentId;
            this.AccountId = accountId;
            this.UserName = userName;
        }
    }

    export class IncidentReOpened {
        static TYPE_NAME = "IncidentReOpened";
        ApplicationId: number;
        CreatedAtUtc: any;
        IncidentId: number;
        EventId: string;

        public constructor(applicationId: number, incidentId: number, createdAtUtc: any) {
            this.ApplicationId = applicationId;
            this.IncidentId = incidentId;
            this.CreatedAtUtc = createdAtUtc;
        }
    }

    export class ReportAddedToIncident {
        static TYPE_NAME = "ReportAddedToIncident";
        Incident: IncidentSummaryDTO;
        Report: Reports.ReportDTO;
        IsReOpened: boolean;
        EventId: string;

        public constructor(incident: IncidentSummaryDTO, report: Reports.ReportDTO, isReOpened: boolean) {
            this.Incident = incident;
            this.Report = report;
            this.IsReOpened = isReOpened;
        }
    }

}

module OneTrueError.Core.Incidents.Commands {
    export class CloseIncident {
        static TYPE_NAME = "CloseIncident";
        CanSendNotification: boolean;
        IncidentId: number;
        NotificationText: string;
        NotificationTitle: string;
        ShareSolution: boolean;
        Solution: string;
        UserId: number;
        CommandId: string;

        public constructor(solution: string, incidentId: number) {
            this.Solution = solution;
            this.IncidentId = incidentId;
        }
    }

    export class IgnoreIncident {
        static TYPE_NAME = "IgnoreIncident";
        IncidentId: number;
        UserId: number;
        CommandId: string;

        public constructor(incidentId: number) {
            this.IncidentId = incidentId;
        }
    }

}

module OneTrueError.Core.Feedback.Commands {
    export class SubmitFeedback {
        static TYPE_NAME = "SubmitFeedback";
        CreatedAtUtc: any;
        Feedback: string;
        RemoteAddress: string;
        ErrorId: string;
        ReportId: number;
        Email: string;
        CommandId: string;

        public constructor(errorId: string, remoteAddress: string) {
            this.ErrorId = errorId;
            this.RemoteAddress = remoteAddress;
        }
    }

}

module OneTrueError.Core.Applications {
    export class ApplicationListItem {
        static TYPE_NAME = "ApplicationListItem";
        Id: number;
        Name: string;

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

module OneTrueError.Core.Applications.Events {
    export class ApplicationCreated {
        static TYPE_NAME = "ApplicationCreated";
        ApplicationName: string;
        ApplicationId: number;
        CreatedById: number;
        AppKey: string;
        SharedSecret: string;
        EventId: string;

        public constructor(id: number, name: string, createdById: number, appKey: string, sharedSecret: string) {
            this.CreatedById = createdById;
            this.AppKey = appKey;
            this.SharedSecret = sharedSecret;
        }
    }

    export class UserInvitedToApplication {
        static TYPE_NAME = "UserInvitedToApplication";
        ApplicationId: number;
        EmailAddress: string;
        InvitedBy: string;
        EventId: string;

        public constructor(applicationId: number, emailAddress: string, invitedBy: string) {
            this.ApplicationId = applicationId;
            this.EmailAddress = emailAddress;
            this.InvitedBy = invitedBy;
        }
    }

}

module OneTrueError.Core.Applications.Queries {
    export class GetApplicationTeamResult {
        static TYPE_NAME = "GetApplicationTeamResult";
        Invited: GetApplicationTeamResultInvitation[];
        Members: GetApplicationTeamMember[];
    }

    export class OverviewStatSummary {
        static TYPE_NAME = "OverviewStatSummary";
        Followers: number;
        Incidents: number;
        Reports: number;
        UserFeedback: number;
    }

    export class GetApplicationIdByKey {
        static TYPE_NAME = "GetApplicationIdByKey";
        ApplicationKey: string;
        QueryId: string;

        public constructor(applicationKey: string) {
            this.ApplicationKey = applicationKey;
        }
    }

    export class GetApplicationIdByKeyResult {
        static TYPE_NAME = "GetApplicationIdByKeyResult";
        Id: number;
    }

    export class GetApplicationInfo {
        static TYPE_NAME = "GetApplicationInfo";
        AppKey: string;
        ApplicationId: number;
        QueryId: string;
    }

    export class GetApplicationInfoResult {
        static TYPE_NAME = "GetApplicationInfoResult";
        AppKey: string;
        ApplicationType: Applications.TypeOfApplication;
        Id: number;
        Name: string;
        SharedSecret: string;
        TotalIncidentCount: number;
    }

    export class GetApplicationList {
        static TYPE_NAME = "GetApplicationList";
        AccountId: number;
        QueryId: string;
    }

    export class GetApplicationOverviewResult {
        static TYPE_NAME = "GetApplicationOverviewResult";
        Days: number;
        ErrorReports: number[];
        Incidents: number[];
        StatSummary: OverviewStatSummary;
        TimeAxisLabels: string[];
    }

    export class GetApplicationTeam {
        static TYPE_NAME = "GetApplicationTeam";
        ApplicationId: number;
        QueryId: string;

        public constructor(applicationId: number) {
            this.ApplicationId = applicationId;
        }
    }

    export class GetApplicationTeamMember {
        static TYPE_NAME = "GetApplicationTeamMember";
        JoinedAtUtc: any;
        UserId: number;
        UserName: string;
    }

    export class GetApplicationTeamResultInvitation {
        static TYPE_NAME = "GetApplicationTeamResultInvitation";
        EmailAddress: string;
        InvitedAtUtc: any;
        InvitedByUserName: string;
    }

    export class GetApplicationOverview {
        static TYPE_NAME = "GetApplicationOverview";
        ApplicationId: number;
        NumberOfDays: number;
        QueryId: string;

        public constructor(applicationId: number) {
            this.ApplicationId = applicationId;
        }
    }

}

module OneTrueError.Core.Applications.Commands {
    export class CreateApplication {
        static TYPE_NAME = "CreateApplication";
        ApplicationKey: string;
        Name: string;
        TypeOfApplication: Applications.TypeOfApplication;
        UserId: number;
        CommandId: string;

        public constructor(name: string, typeOfApplication: Applications.TypeOfApplication) {
            this.Name = name;
            this.TypeOfApplication = typeOfApplication;
        }
    }

}

module OneTrueError.Core.Accounts {
    export class RegisterSimple {
        static TYPE_NAME = "RegisterSimple";
        EmailAddress: string;
        CommandId: string;

        public constructor(emailAddress: string) {
            this.EmailAddress = emailAddress;
        }
    }

}

module OneTrueError.Core.Accounts.Requests {
    export class AcceptInvitation {
        static TYPE_NAME = "AcceptInvitation";
        UserName: string;
        Password: string;
        InvitationKey: string;
        FirstName: string;
        LastName: string;
        Email: string;
        RequestId: string;

        public constructor(userName: string, password: string, invitationKey: string) {
            this.UserName = userName;
            this.Password = password;
            this.InvitationKey = invitationKey;
        }
    }

    export class AcceptInvitationReply {
        static TYPE_NAME = "AcceptInvitationReply";
        AccountId: number;
        UserName: string;

        public constructor(accountId: number, userName: string) {
            this.AccountId = accountId;
            this.UserName = userName;
        }
    }

    export class ActivateAccount {
        static TYPE_NAME = "ActivateAccount";
        ActivationKey: string;
        RequestId: string;

        public constructor(activationKey: string) {
            this.ActivationKey = activationKey;
        }
    }

    export class ActivateAccountReply {
        static TYPE_NAME = "ActivateAccountReply";
        UserName: string;
        AccountId: number;

        public constructor(accountId: number, userName: string) {
            this.AccountId = accountId;
            this.UserName = userName;
        }
    }

    export class ChangePassword {
        static TYPE_NAME = "ChangePassword";
        CurrentPassword: string;
        NewPassword: string;
        UserId: number;
        RequestId: string;

        public constructor(currentPassword: string, newPassword: string) {
            this.CurrentPassword = currentPassword;
            this.NewPassword = newPassword;
        }
    }

    export class ChangePasswordReply {
        static TYPE_NAME = "ChangePasswordReply";
        Success: boolean;
    }

    export class IgnoreFieldAttribute {
        static TYPE_NAME = "IgnoreFieldAttribute";
        TypeId: any;
    }

    export class Login {
        static TYPE_NAME = "Login";
        Password: string;
        UserName: string;
        RequestId: string;

        public constructor(userName: string, password: string) {
            this.UserName = userName;
            this.Password = password;
        }
    }

    export class LoginReply {
        static TYPE_NAME = "LoginReply";
        Result: LoginResult;
        AccountId: number;
        UserName: string;
    }

    export enum LoginResult {
        Locked = 0,
        IncorrectLogin = 1,
        Successful = 2,
    }

    export class ResetPassword {
        static TYPE_NAME = "ResetPassword";
        ActivationKey: string;
        NewPassword: string;
        RequestId: string;

        public constructor(activationKey: string, newPassword: string) {
            this.ActivationKey = activationKey;
            this.NewPassword = newPassword;
        }
    }

    export class ResetPasswordReply {
        static TYPE_NAME = "ResetPasswordReply";
        Success: boolean;
    }

    export class ValidateNewLogin {
        static TYPE_NAME = "ValidateNewLogin";
        Email: string;
        UserName: string;
        RequestId: string;
    }

    export class ValidateNewLoginReply {
        static TYPE_NAME = "ValidateNewLoginReply";
        EmailIsTaken: boolean;
        UserNameIsTaken: boolean;
    }

}

module OneTrueError.Core.Accounts.Queries {
    export class AccountDTO {
        static TYPE_NAME = "AccountDTO";
        Id: number;
        UserName: string;
        CreatedAtUtc: any;
        State: AccountState;
        UpdatedAtUtc: any;
        LastLoginAtUtc: any;
        Email: string;
    }

    export enum AccountState {
        VerificationRequired = 0,
        Active = 1,
        Locked = 2,
        ResetPassword = 3,
    }

    export class FindAccountByUserName {
        static TYPE_NAME = "FindAccountByUserName";
        UserName: string;
        QueryId: string;

        public constructor(userName: string) {
            this.UserName = userName;
        }
    }

    export class GetAccountById {
        static TYPE_NAME = "GetAccountById";
        AccountId: number;
        QueryId: string;

        public constructor(accountId: number) {
            this.AccountId = accountId;
        }
    }

    export class GetAccountEmailById {
        static TYPE_NAME = "GetAccountEmailById";
        AccountId: number;
        QueryId: string;

        public constructor(accountId: number) {
            this.AccountId = accountId;
        }
    }

    export class FindAccountByUserNameResult {
        static TYPE_NAME = "FindAccountByUserNameResult";
        AccountId: number;
        DisplayName: string;

        public constructor(accountId: number, displayName: string) {
            this.AccountId = accountId;
            this.DisplayName = displayName;
        }
    }

}

module OneTrueError.Core.Accounts.Events {
    export class AccountActivated {
        static TYPE_NAME = "AccountActivated";
        AccountId: number;
        EmailAddress: string;
        UserName: string;
        EventId: string;

        public constructor(accountId: number, userName: string) {
            this.AccountId = accountId;
            this.UserName = userName;
        }
    }

    export class AccountRegistered {
        static TYPE_NAME = "AccountRegistered";
        AccountId: number;
        UserName: string;
        EventId: string;

        public constructor(accountId: number, userName: string) {
            this.AccountId = accountId;
            this.UserName = userName;
        }
    }

    export class InvitationAccepted {
        static TYPE_NAME = "InvitationAccepted";
        AccountId: number;
        ApplicationIds: number[];
        InvitedByUserName: string;
        UserName: string;
        EmailAddress: string;
        EventId: string;

        public constructor(accountId: number, invitedByUserName: string, userName: string) {
            this.AccountId = accountId;
            this.InvitedByUserName = invitedByUserName;
            this.UserName = userName;
        }
    }

    export class LoginFailed {
        static TYPE_NAME = "LoginFailed";
        InvalidLogin: boolean;
        IsActivated: boolean;
        IsLocked: boolean;
        UserName: string;
        EventId: string;

        public constructor(userName: string) {
            this.UserName = userName;
        }
    }

}

module OneTrueError.Core.Accounts.Commands {
    export class DeclineInvitation {
        static TYPE_NAME = "DeclineInvitation";
        InvitationId: string;
        CommandId: string;

        public constructor(invitationId: string) {
            this.InvitationId = invitationId;
        }
    }

    export class RegisterAccount {
        static TYPE_NAME = "RegisterAccount";
        UserName: string;
        Password: string;
        Email: string;
        CommandId: string;

        public constructor(userName: string, password: string, email: string) {
            this.UserName = userName;
            this.Password = password;
            this.Email = email;
        }
    }

    export class RequestPasswordReset {
        static TYPE_NAME = "RequestPasswordReset";
        EmailAddress: string;
        CommandId: string;

        public constructor(emailAddress: string) {
            this.EmailAddress = emailAddress;
        }
    }

}