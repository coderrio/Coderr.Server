export class HasLogs {
    public static TYPE_NAME: string = 'HasLogs';
    public incidentId: number;
    public reportId: number | null;
}
export class HasLogsResult {
    public hasLogs: boolean;
}

export class GetLogs {
    public static TYPE_NAME: string = 'GetLogs';
    public incidentId: number;
    public reportId: number | null;
}
export class GetLogsResult {
    public entries: GetLogsResultEntry[];
}
export class GetLogsResultEntry {
    public message: string;
    public level: number;
    public timeStampUtc: Date;
    public exception: string;
}
