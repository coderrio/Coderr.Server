export class NotificationSettings {
    public static TYPE_NAME: string = 'NotificationSettings';
    public NotifyOnCriticalIncidents: NotificationState;
    public NotifyOnImportantIncidents: NotificationState;
    public NotifyOnNewIncidents: NotificationState;
    public NotifyOnPeaks: NotificationState;
    public NotifyOnReOpenedIncident: NotificationState;
    public NotifyOnUserFeedback: NotificationState;
}
export enum NotificationState {
    UseGlobalSetting = 1,
    Disabled = 2,
    Cellphone = 3,
    Email = 4,
    BrowserNotification = 5
}
export class GetUserSettings {
    public static TYPE_NAME: string = 'GetUserSettings';
    public ApplicationId: number;
}
export class GetUserSettingsResult {
    public static TYPE_NAME: string = 'GetUserSettingsResult';
    public EmailAddress: string;
    public FirstName: string;
    public LastName: string;
    public MobileNumber: string;
    public Notifications: NotificationSettings;
}
export class UpdateNotifications {
    public static TYPE_NAME: string = 'UpdateNotifications';
    public ApplicationId: number;
    public NotifyOnCriticalIncidents: NotificationState;
    public NotifyOnImportantIncidents: NotificationState;
    public NotifyOnNewIncidents: NotificationState;
    public NotifyOnPeaks: NotificationState;
    public NotifyOnReOpenedIncident: NotificationState;
    public NotifyOnUserFeedback: NotificationState;
    public UserId: number;
}
export class UpdatePersonalSettings {
    public static TYPE_NAME: string = 'UpdatePersonalSettings';
    public EmailAddress: string;
    public FirstName: string;
    public LastName: string;
    public MobileNumber: string;
}

export class StoreBrowserSubscription {
    public static TYPE_NAME: string = 'StoreBrowserSubscription';
    public UserId: number | null;
    public Endpoint?: string;
    public ExpirationTime?: number;
    public PublicKey: string;
    public AuthenticationSecret: string;
}

export class DeleteBrowserSubscription {
    public static TYPE_NAME: string = 'DeleteBrowserSubscription';
    public UserId: number | null;
    public Endpoint: string;
}
