export class EmailAddress {
  public static TYPE_NAME: string = 'EmailAddress';
  public address: string;
  public name: string;
}
export class EmailMessage {
  public static TYPE_NAME: string = 'EmailMessage';
  public htmlBody: string;
  public recipients: EmailAddress[];
  public replyTo: EmailAddress;
  public resources: EmailResource[];
  public subject: string;
  public textBody: string;
}
export class EmailResource {
  public static TYPE_NAME: string = 'EmailResource';
  public content: string /*base64 encoded data*/;
  public name: string;
}
export class SendEmail {
  public static TYPE_NAME: string = 'SendEmail';
  public emailMessage: EmailMessage;
}
export class SendSms {
  public static TYPE_NAME: string = 'SendSms';
  public message: string;
  public phoneNumber: string;
}
