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
