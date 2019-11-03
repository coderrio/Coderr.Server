import {ReportDTO} from './Reports'
// ReSharper disable InconsistentNaming

export enum IncidentOrder
{
    Newest = 0,
    MostReports = 1,
    MostFeedback = 2,
}
export class IncidentSummaryDTO
{
    public static TYPE_NAME: string = 'IncidentSummaryDTO';
    public ApplicationId: number;
    public ApplicationName: string;
    public CreatedAtUtc: Date;
    public Id: number;
    public IsReOpened: boolean;
    public AssignedToUserId: number|null;
    public LastUpdateAtUtc: Date;
    public Name: string;
    public ReportCount: number;
}
export class FindIncidents
{
    public static TYPE_NAME: string = 'FindIncidents';
    public ApplicationIds: number[];
    public FreeText: string;
    public IsAssigned: boolean;
    public IsClosed: boolean;
    public IsIgnored: boolean;
    public IsNew: boolean;
    public ItemsPerPage: number;
    public MaxDate: Date;
    public MinDate: Date;
    public PageNumber: number;
    public ReOpened: boolean;
    public SortAscending: boolean;
    public SortType: IncidentOrder;
    public Version: string;
    public Tags: string[];
    public AssignedToId: number;
    public ContextCollectionName: string;
    public ContextCollectionPropertyName: string;
    public ContextCollectionPropertyValue: string;
    public EnvironmentIds: number[];
}
export class FindIncidentsResult
{
    public static TYPE_NAME: string = 'FindIncidentsResult';
    public Items: FindIncidentsResultItem[];
    public PageNumber: number;
    public PageSize: number;
    public TotalCount: number;
}
export class FindIncidentsResultItem
{
    public static TYPE_NAME: string = 'FindIncidentsResultItem';
    public ApplicationId: number;
    public ApplicationName: string;
    public AssignedAtUtc: Date|null;
    public CreatedAtUtc: Date;
    public Id: number;
    public IsReOpened: boolean;
    public LastUpdateAtUtc: Date;
    public Name: string;
    public ReportCount: number;
    public LastReportReceivedAtUtc: Date;
}
export class GetIncident
{
    public static TYPE_NAME: string = 'GetIncident';
    public IncidentId: number;
}
export class GetIncidentForClosePage
{
    public static TYPE_NAME: string = 'GetIncidentForClosePage';
    public IncidentId: number;
}
export class GetIncidentForClosePageResult
{
    public static TYPE_NAME: string = 'GetIncidentForClosePageResult';
    public Description: string;
    public SubscriberCount: number;
}
export class GetIncidentResult
{
    public static TYPE_NAME: string = 'GetIncidentResult';
    public ApplicationId: number;
    public AssignedAtUtc: Date|null;
    public AssignedTo: string;
    public AssignedToId: number|null;
    public ContextCollections: string[];
    public CreatedAtUtc: Date;
    public DayStatistics: ReportDay[];
    public Description: string;
    public Facts: QuickFact[];
    public FullName: string;
    public HashCodeIdentifier: string;
    public Id: number;
    public IncidentState: number;
    public IsIgnored: boolean;
    public IsReOpened: boolean;
    public IsSolutionShared: boolean;
    public IsSolved: boolean;
    public LastReportReceivedAtUtc: Date;
    public PreviousSolutionAtUtc: Date;
    public ReOpenedAtUtc: Date;
    public ReportCount: number;
    public ReportHashCode: string;
    public Solution: string;
    public SolvedAtUtc: Date;
    public StackTrace: string;
    public Tags: string[];
    public UpdatedAtUtc: Date;
    public SuggestedSolutions: SuggestedIncidentSolution[];
    public HighlightedContextData: HighlightedContextData[];
}
export class GetIncidentStatistics
{
    public static TYPE_NAME: string = 'GetIncidentStatistics';
    public IncidentId: number;
    public NumberOfDays: number;
}
export class GetIncidentStatisticsResult
{
    public static TYPE_NAME: string = 'GetIncidentStatisticsResult';
    public Labels: string[];
    public Values: number[];
}
export class HighlightedContextData
{
    public static TYPE_NAME: string = 'HighlightedContextData';
    public Description: string;
    public Name: string;
    public Url: string;
    public Value: string[];
}
export class QuickFact
{
    public static TYPE_NAME: string = 'QuickFact';
    public Description: string;
    public Title: string;
    public Url: string;
    public Value: string;
}
export class ReportDay
{
    public static TYPE_NAME: string = 'ReportDay';
    public Count: number;
    public Date: Date;
}
export class SuggestedIncidentSolution
{
    public static TYPE_NAME: string = 'SuggestedIncidentSolution';
    public Reason: string;
    public SuggestedSolution: string;
}
export class IncidentAssigned
{
    public static TYPE_NAME: string = 'IncidentAssigned';
    public AssignedById: number;
    public AssignedToId: number;
    public IncidentId: number;
}
export class IncidentIgnored
{
    public static TYPE_NAME: string = 'IncidentIgnored';
    public AccountId: number;
    public IncidentId: number;
    public UserName: string;
}
export class IncidentReOpened
{
    public static TYPE_NAME: string = 'IncidentReOpened';
    public ApplicationId: number;
    public CreatedAtUtc: Date;
    public IncidentId: number;
}
export class ReportAddedToIncident
{
    public static TYPE_NAME: string = 'ReportAddedToIncident';
    public Incident: IncidentSummaryDTO;
    public IsReOpened: boolean;
    public Report: ReportDTO;
}
export class AssignIncident
{
    public static TYPE_NAME: string = 'AssignIncident';
    public AssignedBy: number;
    public AssignedTo: number;
    public IncidentId: number;
}
export class CloseIncident
{
    public static TYPE_NAME: string = 'CloseIncident';
    public CanSendNotification: boolean;
    public IncidentId: number;
    public NotificationText: string;
    public NotificationTitle: string;
    public ShareSolution: boolean;
    public Solution: string;
    public UserId: number;
    public ApplicationVersion: string;
}
export class DeleteIncident {
    public static TYPE_NAME: string = 'DeleteIncident';
    public AreYouSure: string;
    public IncidentId: number;
}
export class IgnoreIncident
{
    public static TYPE_NAME: string = 'IgnoreIncident';
    public IncidentId: number;
    public UserId: number;
}
export class ReOpenIncident
{
    public static TYPE_NAME: string = 'ReOpenIncident';
    public IncidentId: number;
    public UserId: number;
}

export class NotifySubscribers
{
    public static TYPE_NAME: string = 'NotifySubscribers';
    public IncidentId: number;
    public Body: string;
    public Title: string;
}
