export class GetApplicationVersions
{
    public static TYPE_NAME: string = 'GetApplicationVersions';
    public ApplicationId: number;
}
export class GetApplicationVersionsResult
{
    public static TYPE_NAME: string = 'GetApplicationVersionsResult';
    public Items: GetApplicationVersionsResultItem[];
}
export class GetApplicationVersionsResultItem
{
    public static TYPE_NAME: string = 'GetApplicationVersionsResultItem';
    public FirstReportReceivedAtUtc: Date;
    public IncidentCount: number;
    public LastReportReceivedAtUtc: Date;
    public ReportCount: number;
    public Version: string;
}
export class GetVersionHistory
{
    public static TYPE_NAME: string = 'GetVersionHistory';
    public ApplicationId: number;
    public FromDate: Date;
    public ToDate: Date;
}
export class GetVersionHistoryResult
{
    public Dates: string[];
    public IncidentCounts: GetVersionHistorySeries[];
    public ReportCounts: GetVersionHistorySeries[];
}
export class GetVersionHistorySeries {
    Name: string;
    Values: number[];
}
