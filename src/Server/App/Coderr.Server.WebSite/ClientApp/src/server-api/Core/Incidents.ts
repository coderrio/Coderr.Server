import { ReportDTO } from './Reports'
// ReSharper disable InconsistentNaming

export enum SortOrder {
  Newest = 0,
  LatestReport = 1,
  ReportCount = 2,
}
export class IncidentSummaryDTO {
  public static TYPE_NAME: string = 'IncidentSummaryDTO';
  public applicationId: number;
  public applicationName: string;
  public createdAtUtc: Date;
  public id: number;
  public isReOpened: boolean;
  public assignedToUserId: number | null;
  public lastUpdateAtUtc: Date;
  public name: string;
  public reportCount: number;
}
export class FindIncidents {
  public static TYPE_NAME: string = 'FindIncidents';
  public applicationIds: number[];
  public freeText: string;
  public isAssigned: boolean;
  public isClosed: boolean;
  public isIgnored: boolean;
  public isNew: boolean;
  public itemsPerPage: number;
  public maxDate: Date;
  public minDate: Date;
  public pageNumber: number;
  public reOpened: boolean;
  public sortAscending: boolean;
  public sortType: SortOrder;
  public version: string;
  public tags: string[];
  public assignedToId: number;
  public contextCollectionName: string;
  public contextCollectionPropertyName: string;
  public contextCollectionPropertyValue: string;
  public environmentIds: number[];
}
export class FindIncidentsResult {
  public static TYPE_NAME: string = 'FindIncidentsResult';
  public items: FindIncidentsResultItem[];
  public pageNumber: number;
  public pageSize: number;
  public totalCount: number;
}
export class FindIncidentsResultItem {
  public static TYPE_NAME: string = 'FindIncidentsResultItem';
  public applicationId: number;
  public applicationName: string;
  public assignedAtUtc: Date | null;
  public createdAtUtc: Date;
  public id: number;
  public isReOpened: boolean;
  public lastUpdateAtUtc: Date;
  public name: string;
  public reportCount: number;
  public lastReportReceivedAtUtc: Date;
}
export class GetIncident {
  public static TYPE_NAME: string = 'GetIncident';
  public incidentId: number;
}
export class GetIncidentForClosePage {
  public static TYPE_NAME: string = 'GetIncidentForClosePage';
  public incidentId: number;
}
export class GetIncidentForClosePageResult {
  public static TYPE_NAME: string = 'GetIncidentForClosePageResult';
  public description: string;
  public subscriberCount: number;
}
export class GetIncidentResult {
  public static TYPE_NAME: string = 'GetIncidentResult';
  public applicationId: number;
  public assignedAtUtc: Date | null;
  public assignedTo: string;
  public assignedToId: number | null;
  public contextCollections: string[];
  public createdAtUtc: Date;
  public dayStatistics: ReportDay[];
  public description: string;
  public facts: QuickFact[];
  public fullName: string;
  public hashCodeIdentifier: string;
  public id: number;
  public incidentState: number;
  public isIgnored: boolean;
  public isReOpened: boolean;
  public isSolutionShared: boolean;
  public isSolved: boolean;
  public lastReportReceivedAtUtc: Date;
  public previousSolutionAtUtc: Date;
  public reOpenedAtUtc: Date;
  public reportCount: number;
  public reportHashCode: string;
  public solution: string;
  public solvedAtUtc: Date;
  public stackTrace: string;
  public tags: string[];
  public updatedAtUtc: Date;
  public suggestedSolutions: SuggestedIncidentSolution[];
  public highlightedContextData: HighlightedContextData[];
}
export class GetIncidentStatistics {
  public static TYPE_NAME: string = 'GetIncidentStatistics';
  public incidentId: number;
  public numberOfDays: number;
}
export class GetIncidentStatisticsResult {
  public static TYPE_NAME: string = 'GetIncidentStatisticsResult';
  public labels: string[];
  public values: number[];
}
export class HighlightedContextData {
  public static TYPE_NAME: string = 'HighlightedContextData';
  public description: string;
  public name: string;
  public url: string;
  public value: string[];
}
export class QuickFact {
  public static TYPE_NAME: string = 'QuickFact';
  public description: string;
  public title: string;
  public url: string;
  public value: string;
}
export class ReportDay {
  public static TYPE_NAME: string = 'ReportDay';
  public count: number;
  public date: string;
}
export class SuggestedIncidentSolution {
  public static TYPE_NAME: string = 'SuggestedIncidentSolution';
  public reason: string;
  public suggestedSolution: string;
}
export class IncidentCreated {
  public static TYPE_NAME: string = 'IncidentCreated';
  public applicationId: number;
  public applicationVersion: string;
  public createdAtUtc: string;
  public exceptionTypeName: string;
  public incidentId: number;
  public incidentName: string;
}
export class IncidentAssigned {
  public static TYPE_NAME: string = 'IncidentAssigned';
  public assignedById: number;
  public assignedToId: number;
  public incidentId: number;
}
export class IncidentIgnored {
  public static TYPE_NAME: string = 'IncidentIgnored';
  applicationId: number;
  public accountId: number;
  public incidentId: number;
  public userName: string;
}
export class IncidentReOpened {
  public static TYPE_NAME: string = 'IncidentReOpened';
  public applicationId: number;
  public createdAtUtc: Date;
  public incidentId: number;
}
export class IncidentClosed {
  public static TYPE_NAME: string = 'IncidentClosed';
  applicationId: number;
  public closedById: number;
  public incidentId: number;
  public solution: string;
  public applicationVersion: string;
  public closedAtUtc: Date;
}
export class IncidentEscalated {
  public static TYPE_NAME: string = 'IncidentEscalated';
  public applicationId: number;
  public incidentId: number;
  public isCritical: boolean;
  public isImportant: boolean;
}
export class ReportAddedToIncident {
  public static TYPE_NAME: string = 'ReportAddedToIncident';
  public incident: IncidentSummaryDTO;
  public isReOpened: boolean;
  public report: ReportDTO;
}
export class AssignIncident {
  public static TYPE_NAME: string = 'AssignIncident';
  public assignedBy: number;
  public assignedTo: number;
  public incidentId: number;
}
export class CloseIncident {
  public static TYPE_NAME: string = 'CloseIncident';
  public canSendNotification: boolean;
  public incidentId: number;
  public notificationText: string;
  public notificationTitle: string;
  public shareSolution: boolean;
  public solution: string;
  public userId: number;
  public applicationVersion: string;
}
export class DeleteIncident {
  public static TYPE_NAME: string = 'DeleteIncident';
  public areYouSure: string;
  public incidentId: number;
}
export class IgnoreIncident {
  public static TYPE_NAME: string = 'IgnoreIncident';
  public incidentId: number;
  public userId: number;
}
export class ReOpenIncident {
  public static TYPE_NAME: string = 'ReOpenIncident';
  public incidentId: number;
  public userId: number;
}

export class NotifySubscribers {
  public static TYPE_NAME: string = 'NotifySubscribers';
  public incidentId: number;
  public body: string;
  public title: string;
}
