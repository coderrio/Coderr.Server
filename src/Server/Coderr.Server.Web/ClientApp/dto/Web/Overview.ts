export class GetOverview
{
    public static TYPE_NAME: string = 'GetOverview';
    public NumberOfDays: number;
}
export class GetOverviewApplicationResult
{
    public static TYPE_NAME: string = 'GetOverviewApplicationResult';
    public Label: string;
    public Values: number[];
}
export class GetOverviewResult
{
    public static TYPE_NAME: string = 'GetOverviewResult';
    public Days: number;
    public IncidentsPerApplication: GetOverviewApplicationResult[];
    public StatSummary: OverviewStatSummary;
    public TimeAxisLabels: string[];
}
export class OverviewStatSummary
{
    public static TYPE_NAME: string = 'OverviewStatSummary';
    public Followers: number;
    public Incidents: number;
    public Reports: number;
    public UserFeedback: number;
}
