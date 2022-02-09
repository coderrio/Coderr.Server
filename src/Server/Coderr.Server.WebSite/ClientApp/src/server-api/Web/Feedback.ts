export class GetFeedbackForApplicationPage
{
    public static TYPE_NAME: string = 'GetFeedbackForApplicationPage';
    public applicationId: number;
}
export class GetFeedbackForApplicationPageResult
{
    public static TYPE_NAME: string = 'GetFeedbackForApplicationPageResult';
    public emails: string[];
    public items: GetFeedbackForApplicationPageResultItem[];
    public totalCount: number;
}
export class GetFeedbackForApplicationPageResultItem
{
    public static TYPE_NAME: string = 'GetFeedbackForApplicationPageResultItem';
    public emailAddress: string;
    public incidentId: number;
    public incidentName: string;
    public message: string;
    public writtenAtUtc: Date;
}
export class GetFeedbackForDashboardPage
{
    public static TYPE_NAME: string = 'GetFeedbackForDashboardPage';
}
export class GetFeedbackForDashboardPageResult
{
    public static TYPE_NAME: string = 'GetFeedbackForDashboardPageResult';
    public emails: string[];
    public items: GetFeedbackForDashboardPageResultItem[];
    public totalCount: number;
}
export class GetFeedbackForDashboardPageResultItem
{
    public static TYPE_NAME: string = 'GetFeedbackForDashboardPageResultItem';
    public applicationId: number;
    public applicationName: string;
    public emailAddress: string;
    public message: string;
    public writtenAtUtc: Date;
}
export class GetIncidentFeedback
{
    public static TYPE_NAME: string = 'GetIncidentFeedback';
    public incidentId: number;
}
export class GetIncidentFeedbackResult
{
    public static TYPE_NAME: string = 'GetIncidentFeedbackResult';
    public emails: string[];
    public items: GetIncidentFeedbackResultItem[];
}
export class GetIncidentFeedbackResultItem
{
    public static TYPE_NAME: string = 'GetIncidentFeedbackResultItem';
    public emailAddress: string;
    public message: string;
    public writtenAtUtc: Date;
}
