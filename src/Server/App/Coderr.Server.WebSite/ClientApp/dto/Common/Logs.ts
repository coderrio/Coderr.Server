export class HasLogs {
    public static TYPE_NAME: string = 'HasLogs';
    public IncidentId: number;
    public ReportId: number | null;
}
export class HasLogsResult {
    public HasLogs: boolean;
}

export class GetLogs {
    public static TYPE_NAME: string = 'GetLogs';
    public IncidentId: number;
    public ReportId: number | null;
}
export class GetLogsResult {
    public Entries: GetLogsResultEntry[];
}
export class GetLogsResultEntry {
    public Message: string;
    public Level: number;
    public TimeStampUtc: Date;
    public Exception: string;
}
