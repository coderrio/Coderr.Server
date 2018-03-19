export class EmailAddress
{
    public static TYPE_NAME: string = 'EmailAddress';
    public Address: string;
    public Name: string;
}
export class EmailMessage
{
    public static TYPE_NAME: string = 'EmailMessage';
    public HtmlBody: string;
    public Recipients: EmailAddress[];
    public ReplyTo: EmailAddress;
    public Resources: EmailResource[];
    public Subject: string;
    public TextBody: string;
}
export class EmailResource
{
    public static TYPE_NAME: string = 'EmailResource';
    public Content: string /*base64 encoded data*/;
    public Name: string;
}
export class SendEmail
{
    public static TYPE_NAME: string = 'SendEmail';
    public EmailMessage: EmailMessage;
}
export class SendSms
{
    public static TYPE_NAME: string = 'SendSms';
    public Message: string;
    public PhoneNumber: string;
}
