export class GetIncidentStateSummary {
    static TYPE_NAME: string = "GetIncidentStateSummary";
    applicationId: number;
    applicationVersion: string;
}

export class GetIncidentStateSummaryResult {
    reOpenedCount: number;
    newCount: number;
    closedCount: number;
}

export class GetIncidentsForStates {
    static TYPE_NAME: string = "GetIncidentsForStates";
    applicationId: number;
    applicationVersion: string;
}

export class GetIncidentsForStatesResult {
    items: GetIncidentsForStatesResultItem[];
}
export class GetIncidentsForStatesResultItem {
    incidentId: number;
    incidentName: string;
    createdAtUtc: Date;
    isClosed: boolean;
    isNew: boolean;
    isReopened: boolean;
}
