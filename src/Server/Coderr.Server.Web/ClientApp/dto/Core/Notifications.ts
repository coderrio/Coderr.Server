export class AddNotification
{
    public static TYPE_NAME: string = 'AddNotification';
    public AccountId: number|null;
    public HoldbackInterval: string|null;
    public Message: string;
    public NotificationType: string;
    public RoleName: string;
}
