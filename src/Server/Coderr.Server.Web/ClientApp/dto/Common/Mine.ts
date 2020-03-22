export class ListMyIncidents
{
    public static TYPE_NAME: string = 'ListMyIncidents';
    public ApplicationId: number|null;
}
export class ListMyIncidentsResult
{
    public static TYPE_NAME: string = 'ListMyIncidentsResult';
    public Items: ListMyIncidentsResultItem[];
    public Suggestions: ListMySuggestedItem[];
    public Comment: string;
}
export class ListMyIncidentsResultItem
{
    public static TYPE_NAME: string = 'ListMyIncidentsResultItem';
    public ApplicationId: number;
    public ApplicationName: string;
    public AssignedAtUtc: Date;
    public CreatedAtUtc: Date;
    public Id: number;
    public LastReportAtUtc: Date;
    public Name: string;
    public ReportCount: number;
}
export class ListMySuggestedItem
{
    public static TYPE_NAME: string = 'ListMySuggestedItem';
    public ApplicationId: number;
    public ApplicationName: string;
    public CreatedAtUtc: Date;
    public ExceptionTypeName: string;
    public Id: number;
    public LastReportAtUtc: Date;
    public Name: string;
    public Weight: number;
    public ReportCount: number;
    public StackTrace: string;
    public Motivation: string;
}
