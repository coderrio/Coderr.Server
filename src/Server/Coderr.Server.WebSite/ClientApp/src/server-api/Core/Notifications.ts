export class AddNotification {
  public static TYPE_NAME: string = 'AddNotification';
  public accountId: number | null;
  public holdbackInterval: string | null;
  public message: string;
  public notificationType: string;
  public roleName: string;
}
