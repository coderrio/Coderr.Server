export class FeedbackAttachedToIncident {
  public static TYPE_NAME: string = 'FeedbackAttachedToIncident';
  public incidentId: number;
  public message: string;
  public userEmailAddress: string;
}
export class SubmitFeedback {
  public static TYPE_NAME: string = 'SubmitFeedback';
  public createdAtUtc: Date;
  public email: string;
  public errorId: string;
  public feedback: string;
  public remoteAddress: string;
  public reportId: number;
}
