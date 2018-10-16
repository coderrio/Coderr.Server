export class GetFeedbackForApplicationPage
{
    public static TYPE_NAME: string = 'GetFeedbackForApplicationPage';
    public ApplicationId: number;
}
export class GetFeedbackForApplicationPageResult
{
    public static TYPE_NAME: string = 'GetFeedbackForApplicationPageResult';
    public Emails: string[];
    public Items: GetFeedbackForApplicationPageResultItem[];
    public TotalCount: number;
}
export class GetFeedbackForApplicationPageResultItem
{
    public static TYPE_NAME: string = 'GetFeedbackForApplicationPageResultItem';
    public EmailAddress: string;
    public IncidentId: number;
    public IncidentName: string;
    public Message: string;
    public WrittenAtUtc: Date;
}
export class GetFeedbackForDashboardPage
{
    public static TYPE_NAME: string = 'GetFeedbackForDashboardPage';
}
export class GetFeedbackForDashboardPageResult
{
    public static TYPE_NAME: string = 'GetFeedbackForDashboardPageResult';
    public Emails: string[];
    public Items: GetFeedbackForDashboardPageResultItem[];
    public TotalCount: number;
}
export class GetFeedbackForDashboardPageResultItem
{
    public static TYPE_NAME: string = 'GetFeedbackForDashboardPageResultItem';
    public ApplicationId: number;
    public ApplicationName: string;
    public EmailAddress: string;
    public Message: string;
    public WrittenAtUtc: Date;
}
export class GetIncidentFeedback
{
    public static TYPE_NAME: string = 'GetIncidentFeedback';
    public IncidentId: number;
}
export class GetIncidentFeedbackResult
{
    public static TYPE_NAME: string = 'GetIncidentFeedbackResult';
    public Emails: string[];
    public Items: GetIncidentFeedbackResultItem[];
}
export class GetIncidentFeedbackResultItem
{
    public static TYPE_NAME: string = 'GetIncidentFeedbackResultItem';
    public EmailAddress: string;
    public Message: string;
    public WrittenAtUtc: Date;
}
