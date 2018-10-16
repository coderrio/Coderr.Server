export class ContextCollectionDTO
{
    public static TYPE_NAME: string = 'ContextCollectionDTO';
    public Name: string;
    public Properties: string[];
}
export class ReportDTO
{
    public static TYPE_NAME: string = 'ReportDTO';
    public ApplicationId: number;
    public ContextCollections: ContextCollectionDTO[];
    public CreatedAtUtc: Date;
    public Exception: ReportExeptionDTO;
    public Id: number;
    public IncidentId: number;
    public RemoteAddress: string;
    public ReportId: string;
    public ReportVersion: string;
}
export class ReportExeptionDTO
{
    public static TYPE_NAME: string = 'ReportExeptionDTO';
    public AssemblyName: string;
    public BaseClasses: string[];
    public Everything: string;
    public FullName: string;
    public InnerException: ReportExeptionDTO;
    public Message: string;
    public Name: string;
    public Namespace: string;
    public Properties: string[];
    public StackTrace: string;
}
export class GetReport
{
    public static TYPE_NAME: string = 'GetReport';
    public ReportId: number;
}
export class GetReportException
{
    public static TYPE_NAME: string = 'GetReportException';
    public AssemblyName: string;
    public BaseClasses: string[];
    public Everything: string;
    public FullName: string;
    public InnerException: GetReportException;
    public Message: string;
    public Name: string;
    public Namespace: string;
    public StackTrace: string;
}
export class GetReportList
{
    public static TYPE_NAME: string = 'GetReportList';
    public IncidentId: number;
    public PageNumber: number;
    public PageSize: number;
}
export class GetReportListResult
{
    public static TYPE_NAME: string = 'GetReportListResult';
    public Items: GetReportListResultItem[];
    public PageNumber: number;
    public PageSize: number;
    public TotalCount: number;
}
export class GetReportListResultItem
{
    public static TYPE_NAME: string = 'GetReportListResultItem';
    public CreatedAtUtc: Date;
    public Id: number;
    public Message: string;
    public RemoteAddress: string;
}
export class GetReportResult
{
    public static TYPE_NAME: string = 'GetReportResult';
    public ContextCollections: GetReportResultContextCollection[];
    public CreatedAtUtc: Date;
    public EmailAddress: string;
    public ErrorId: string;
    public Exception: GetReportException;
    public Id: string;
    public IncidentId: string;
    public Message: string;
    public StackTrace: string;
    public UserFeedback: string;
}
export class GetReportResultContextCollection
{
    public static TYPE_NAME: string = 'GetReportResultContextCollection';
    public Name: string;
    public Properties: KeyValuePair[];
}
export class KeyValuePair
{
    public static TYPE_NAME: string = 'KeyValuePair';
    public Key: string;
    public Value: string;
}
