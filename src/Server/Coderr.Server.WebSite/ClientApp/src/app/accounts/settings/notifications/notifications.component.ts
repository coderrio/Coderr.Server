import { Component, OnInit } from '@angular/core';
import { ApiClient, HttpClient } from "../../../utils/HttpClient";
import * as dto from "../../../../server-api/Core/Users";
import { ToastrService } from "ngx-toastr";
import { AuthorizeService } from "../../../../api-authorization/authorize.service";

@Component({
  selector: 'app-notifications',
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.scss']
})
export class NotificationsComponent implements OnInit {
  applicationId: number | null = null;
  appNotice: string;
  private workerUrl = '/service-worker.js';
  settings: dto.UpdateNotifications = new dto.UpdateNotifications();
  emailAddress = '';
  workEmail = '';
  missingKeys = false;
  generatedPublicKey = '';
  generatedPrivateKey = '';
  deniedPushNotification = false;
  pushNotificationRequest = false;

  constructor(private apiClient: ApiClient,
    private httpClient: HttpClient,
    private authService: AuthorizeService,
    private toastrService: ToastrService) {
    this.settings.notifyOnNewIncidents = dto.NotificationState.Disabled;
    this.settings.notifyOnCriticalIncidents = dto.NotificationState.Disabled;
    this.settings.notifyOnImportantIncidents = dto.NotificationState.Disabled;
    this.settings.notifyOnPeaks = dto.NotificationState.Disabled;
    this.settings.notifyOnReOpenedIncident = dto.NotificationState.Disabled;
    this.settings.notifyOnUserFeedback = dto.NotificationState.Disabled;
  }

  ngOnInit(): void {
    this.loadSettings();
  }


  private async loadSettings(): Promise<object> {
    var query = new dto.GetUserSettings();

    var x = await this.apiClient.query<dto.GetUserSettingsResult>(query)
    this.settings.notifyOnImportantIncidents = x.notifications.notifyOnImportantIncidents;
    this.settings.notifyOnCriticalIncidents = x.notifications.notifyOnCriticalIncidents;
    this.settings.notifyOnNewIncidents = x.notifications.notifyOnNewIncidents;
    this.settings.notifyOnPeaks = x.notifications.notifyOnPeaks;
    this.settings.notifyOnUserFeedback = x.notifications.notifyOnUserFeedback;
    this.settings.notifyOnReOpenedIncident = x.notifications.notifyOnReOpenedIncident;
    this.emailAddress = x.emailAddress;
    this.workEmail = x.emailAddress;

    navigator.serviceWorker.ready.then(registration => {
      registration.pushManager.getSubscription();
    });

    this.getPublicKey().then(key => {
      if (key) {
        return null;
      }
    });

    return null;
  }

  async save() {
    var cmd = new dto.UpdateNotifications();
    cmd.notifyOnCriticalIncidents = +this.settings.notifyOnCriticalIncidents;
    cmd.notifyOnImportantIncidents = +this.settings.notifyOnImportantIncidents;
    cmd.notifyOnNewIncidents = +this.settings.notifyOnNewIncidents;
    cmd.notifyOnPeaks = +this.settings.notifyOnPeaks;
    cmd.notifyOnReOpenedIncident = +this.settings.notifyOnReOpenedIncident;
    cmd.notifyOnUserFeedback = +this.settings.notifyOnUserFeedback;
    cmd.applicationId = this.applicationId;
    cmd.userId = this.authService.user.accountId;
    await this.apiClient.command(cmd);

    var browserNotificationState = dto.NotificationState.BrowserNotification;
    var shouldGenerateSubscription = cmd.notifyOnNewIncidents === browserNotificationState ||
      cmd.notifyOnPeaks === browserNotificationState ||
      cmd.notifyOnCriticalIncidents === browserNotificationState ||
      cmd.notifyOnImportantIncidents === browserNotificationState ||
      cmd.notifyOnReOpenedIncident === browserNotificationState ||
      cmd.notifyOnUserFeedback === browserNotificationState;

    if (shouldGenerateSubscription) {
      if (!this.isNotificationsSupported()) {
        this.toastrService.warning("Your browser do not support notifications");
        return null;
      }

      this.registerPushSubscription().then(result => {
        if (result) {
          this.toastrService.success('Saved OK');
        }
      });
    } else {
      this.deleteSubscription();
      this.toastrService.success('Saved OK');
    }


  }

  private async deleteSubscription() {
    if (!navigator.serviceWorker)
      return;

    const registration = await navigator.serviceWorker.ready;
    const subscription = await registration.pushManager.getSubscription();
    if (!subscription) {
      return;
    }

    subscription.unsubscribe();

    var cmd = new dto.DeleteBrowserSubscription();
    cmd.endpoint = subscription.endpoint;
    cmd.userId = this.authService.user.accountId;
    this.apiClient.command(cmd);
  }


  isNotificationsSupported() {
    if (!('serviceWorker' in navigator)) {
      return false;
    }

    if (!('PushManager' in window)) {
      return false;
    }

    return true;
  }

  private async registerPushSubscription(): Promise<boolean> {
    if (Notification.permission !== 'granted') {
      try {
        this.pushNotificationRequest = true;
        const permission = await Notification.requestPermission();
        this.pushNotificationRequest = false;
        if (permission !== 'granted') {
          this.deniedPushNotification = true;
          return false;
        }
      } catch (e) {
        this.pushNotificationRequest = false;
        this.deniedPushNotification = true;
        console.log(e);
        return false;
      }
    }

    await navigator.serviceWorker.register(this.workerUrl);
    const registration = await navigator.serviceWorker.ready;
    let subscription = await registration.pushManager.getSubscription();
    if (subscription) {
      this.storeBrowserSubscription(subscription);
      return true;
    }

    const key = await this.getPublicKey();

    subscription = await registration.pushManager.subscribe({
      userVisibleOnly: true,
      applicationServerKey: key
    });

    this.storeBrowserSubscription(subscription);
    return true;
  }

  private async storeBrowserSubscription(subscription: PushSubscription) {

    var mightHave = <any>subscription;
    var cmd = new dto.StoreBrowserSubscription();
    cmd.userId = this.authService.user.accountId;
    cmd.endpoint = subscription.endpoint;
    cmd.expirationTime = mightHave.expirationTime;

    // Know a better way which isn't allocate a lot of objects?
    var obj = JSON.parse(JSON.stringify(subscription));

    cmd.publicKey = obj.keys.p256dh;
    cmd.authenticationSecret = obj.keys.auth;
    this.apiClient.command(cmd);
  }

  private async getPublicKey(): Promise<Uint8Array> {

    const response = await this.httpClient.get('./push/vapidpublickey');
    if (response.statusCode === 204) {
      return null;
    }

    const json = await response.body;
    var value = this.urlBase64ToUint8Array(json);
    return value;
  }

  private urlBase64ToUint8Array(base64String: string) {
    var padding = '='.repeat((4 - base64String.length % 4) % 4);
    var base64 = (base64String + padding)
      .replace(/\-/g, '+')
      .replace(/_/g, '/');

    var rawData = window.atob(base64);
    var outputArray = new Uint8Array(rawData.length);

    for (var i = 0; i < rawData.length; ++i) {
      outputArray[i] = rawData.charCodeAt(i);
    }

    return outputArray;
  }

}
