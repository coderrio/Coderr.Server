export class GetOverview {
  public static TYPE_NAME: string = 'GetOverview';
  public NumberOfDays: number;
  public IncludeChartData: boolean = true;
  public IncludePartitions: boolean;
}
export class GetOverviewApplicationResult {
  public static TYPE_NAME: string = 'GetOverviewApplicationResult';
  public Label: string;
  public Values: number[];
}
export class GetOverviewResult {
  public static TYPE_NAME: string = 'GetOverviewResult';
  public Days: number;
  public IncidentsPerApplication: GetOverviewApplicationResult[];
  public StatSummary: OverviewStatSummary;
  public TimeAxisLabels: string[];
  public MissedReports?: number;
}
export class OverviewStatSummary {
  public static TYPE_NAME: string = 'OverviewStatSummary';
  public Followers: number;
  public Incidents: number;
  public NewestIncidentReceivedAtUtc?: Date;
  public Reports: number;
  public NewestReportReceivedAtUtc?: Date;
  public UserFeedback: number;
  public Partitions: PartitionOverview[];
}
export class PartitionOverview {
  public Name: string;
  public DisplayName: string;
  public Value: number;
}
