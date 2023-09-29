export class GetInsights {
    public static TYPE_NAME: string = 'GetInsights';
    public ApplicationId: number | null;
}
export class GetInsightsResult {
    public static TYPE_NAME: string = 'GetInsightsResult';
    public ApplicationInsights: GetInsightResultApplication[];
    public Indicators: GetInsightResultIndicator[];
    public TrendDates: string[];
}
export class GetInsightResultApplication {
    public Id: number;
    public Name: string;
    public NumberOfDevelopers: number;
    public Indicators: GetInsightResultIndicator[];
}
export class GetInsightResultIndicator {
    public CanBeNormalized: boolean;
    public PeriodValueName: string;
    public Name: string;
    public Title: string;
    public IsAlternative: boolean;
    public Description: string;
    public Comment: string;
    public Value: any;
    public ValueName: string;
    public PeriodValue: any;
    public TrendLines: TrendLine[];
    public Toplist: ToplistItem[];
    public HigherValueIsBetter: boolean | any;
    public ValueUnit: null|"days"|"users"|"incidents"|"versions";
}
export class TrendLine {
    public DisplayName: string;
    public TrendValues: any[];
}
export class TrendValue {
    public Value: any;
    public Normalized: any;
}
export class ToplistItem {
    public Title: string;
    public Value: string;
    public Comment: string;
}