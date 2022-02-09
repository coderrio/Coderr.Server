export class NotificationSettings {
  public static TYPE_NAME: string = 'NotificationSettings';
  public notifyOnCriticalIncidents: NotificationState;
  public notifyOnImportantIncidents: NotificationState;
  public notifyOnNewIncidents: NotificationState;
  public notifyOnPeaks: NotificationState;
  public notifyOnReOpenedIncident: NotificationState;
  public notifyOnUserFeedback: NotificationState;
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
  public applicationId: number;
}
export class GetUserSettingsResult {
  public static TYPE_NAME: string = 'GetUserSettingsResult';
  public emailAddress: string;
  public firstName: string;
  public lastName: string;
  public mobileNumber: string;
  public notifications: NotificationSettings;
}
export class UpdateNotifications {
  public static TYPE_NAME: string = 'UpdateNotifications';
  public applicationId: number;
  public notifyOnCriticalIncidents: NotificationState;
  public notifyOnImportantIncidents: NotificationState;
  public notifyOnNewIncidents: NotificationState;
  public notifyOnPeaks: NotificationState;
  public notifyOnReOpenedIncident: NotificationState;
  public notifyOnUserFeedback: NotificationState;
  public userId: number;
}
export class UpdatePersonalSettings {
  public static TYPE_NAME: string = 'UpdatePersonalSettings';
  public emailAddress: string;
  public firstName: string;
  public lastName: string;
  public mobileNumber: string;
}

export class StoreBrowserSubscription {
  public static TYPE_NAME: string = 'StoreBrowserSubscription';
  public userId: number | null;
  public endpoint?: string;
  public expirationTime?: number;
  public publicKey: string;
  public authenticationSecret: string;
}

export class DeleteBrowserSubscription {
  public static TYPE_NAME: string = 'DeleteBrowserSubscription';
  public userId: number | null;
  public endpoint: string;
}
export class GetAccountSetting {
  public static TYPE_NAME: string = 'GetAccountSetting';
  public accountId: number;
  public name: string;
}
export class GetAccountSettingResult {
  public value: string;
}
export class GetAccountSettings {
  public static TYPE_NAME: string = 'GetAccountSettings';
  public accountId: number;
}
interface IDictionary<T> {
  [key: string]: T;
}
export class GetAccountSettingsResult {
  public values: IDictionary<string>;
}
export class SaveAccountSetting {
  public static TYPE_NAME: string = 'SaveAccountSetting';
  public accountId: number;
  public name: string;
  public value: string;
}
