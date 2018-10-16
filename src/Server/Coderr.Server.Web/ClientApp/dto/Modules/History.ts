export class GetIncidentStateSummary {
    static TYPE_NAME: string = "GetIncidentStateSummary";
    ApplicationId: number;
    ApplicationVersion: string;
}

export class GetIncidentStateSummaryResult {
    ReOpenedCount: number;
    NewCount: number;
    ClosedCount: number;
}

export class GetIncidentsForStates {
    static TYPE_NAME: string = "GetIncidentsForStates";
    ApplicationId: number;
    ApplicationVersion: string;
}

export class GetIncidentsForStatesResult {
    Items: GetIncidentsForStatesResultItem[];
}
export class GetIncidentsForStatesResultItem {
    IncidentId: number;
    IncidentName: string;
    CreatedAtUtc: Date;
    IsClosed: boolean;
    IsNew: boolean;
    IsReopened: boolean;
}
