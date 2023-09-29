export interface IState {
  id: number;
  name: string;
  isSettable: boolean;
}

export var states: IState[] = [
  { id: 0, name: "New", isSettable: false },
  { id: 1, name: "Assigned", isSettable: true  },
  { id: 2, name: "Ignored", isSettable: true  },
  { id: 3, name: "Closed", isSettable: true  },
  { id: 4, name: "Re-opened", isSettable: false  }
];

export class Incident {
  applicationId: number;
  assignedAtUtc: Date | null;
  assignedTo: string;
  assignedToId: number | null;
  contextCollections: string[];
  createdAtUtc: Date;
  description: string;
  facts: IQuickFact[];
  fullName: string;
  id: number;
  state: number;
  isIgnored: boolean;
  isReOpened: boolean;
  isSolved: boolean;
  isAssigned: boolean;
  lastReportReceivedAtUtc: Date;
  previousSolutionAtUtc: Date;
  reOpenedAtUtc: Date;
  reportCount: number;
  solution: string;
  solvedAtUtc: Date;
  stackTrace: string;
  tags: string[];
  updatedAtUtc: Date;
  latestRefresh: Date;
  suggestedSolutions: ISuggestedSolution[];
  highlightedContextData: IHighlightedData[];
  monthReports: IncidentMonthStats;
}

/**
 *
 */
export class IncidentMonthStats {
  days: string[];
  values: number[];
}

export class IncidentRecommendation {
  applicationId: number;
  applicationName: string;
  createdAtUtc: Date;
  id: number;
  name: string;
  reportCount: number;
  lastReportReceivedAtUtc: Date;
  weight: number;
  exceptionTypeName: string;
  motivation: string;
}

export class IncidentSummary {
  applicationId: number;
  applicationName: string;
  assignedAtUtc: Date | null;
  createdAtUtc: Date;
  id: number;
  isReOpened: boolean;
  name: string;
  reportCount: number;
  lastReportReceivedAtUtc: Date;

  isIgnored: boolean;
  isSolved: boolean;
  isAssigned: boolean;
  assignedTo: string;
  assignedToId: number | null;
}

export interface ISuggestedSolution {
  reason: string;
  suggestedSolution: string;
}

export interface IHighlightedData {
  description: string;
  name: string;
  url: string;
  value: string[];
}

export interface IQuickFact {
  description: string;
  title: string;
  url: string;
  value: string;
}

export enum IncidentState {
  /**
    * Incident have arrived but have not yet been categorized.
    */
  New = 0,

  /**
    * Incident should be fixed (assigned)
    */
  Active = 1,


  /**
    * Ignore all reports for this incident
    */
  Ignored = 2,

  /**
   * Incident have been corrected.
   */
  Closed = 3,

  /**
  * We received a new error report on a closed incident (used in history table)
  */
  ReOpened = 4,
}
