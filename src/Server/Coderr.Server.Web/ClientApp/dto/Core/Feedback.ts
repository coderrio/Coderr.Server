export class FeedbackAttachedToIncident
{
    public static TYPE_NAME: string = 'FeedbackAttachedToIncident';
    public IncidentId: number;
    public Message: string;
    public UserEmailAddress: string;
}
export class SubmitFeedback
{
    public static TYPE_NAME: string = 'SubmitFeedback';
    public CreatedAtUtc: Date;
    public Email: string;
    public ErrorId: string;
    public Feedback: string;
    public RemoteAddress: string;
    public ReportId: number;
}
