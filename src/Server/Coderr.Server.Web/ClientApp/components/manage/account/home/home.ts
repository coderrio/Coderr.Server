import { AppRoot } from "@/services/AppRoot";
import * as dto from "@/dto/Core/Users";
import { Component, Vue } from "vue-property-decorator";
//import JQuery from "jquery";
//let $ = JQuery;

@Component
export default class ManageHomeComponent extends Vue {
    applicationId: number | null = null;
    appNotice: string;
    private workerUrl = '/service-worker.js';
    notifyOnNewIncidents = dto.NotificationState.Disabled;
    notifyOnCriticalIncidents = dto.NotificationState.Disabled;
    notifyOnImportantIncidents = dto.NotificationState.Disabled;
    notifyOnPeaks = dto.NotificationState.Disabled;
    notifyOnReOpenedIncident = dto.NotificationState.Disabled;
    notifyOnUserFeedback = dto.NotificationState.Disabled;
    emailAddress = '';
    workEmail = '';
    missingKeys = false;
    generatedPublicKey = '';
    generatedPrivateKey = '';
    deniedPushNotification = false;
    pushNotificationRequest = false;

    created() {
        if (this.$route.params.applicationId) {
            this.$router.push({
                name: 'manageAccountSettings',
                params: { applicationId: this.$route.params.applicationId }
            });
        }
    }


    mounted() {
        var query = new dto.GetUserSettings();

        AppRoot.Instance.apiClient.query<dto.GetUserSettingsResult>(query)
            .then(x => {
                this.notifyOnImportantIncidents = x.Notifications.NotifyOnImportantIncidents;
                this.notifyOnCriticalIncidents = x.Notifications.NotifyOnCriticalIncidents;
                this.notifyOnNewIncidents = x.Notifications.NotifyOnNewIncidents;
                this.notifyOnPeaks = x.Notifications.NotifyOnPeaks;
                this.notifyOnUserFeedback = x.Notifications.NotifyOnUserFeedback;
                this.notifyOnReOpenedIncident = x.Notifications.NotifyOnReOpenedIncident;
                this.emailAddress = x.EmailAddress;
                this.workEmail = x.EmailAddress;
            });

        navigator.serviceWorker.ready.then(registration => {
            registration.pushManager.getSubscription().then(subscription => console.log('subs', JSON.stringify(subscription)));
        });

        this.getPublicKey().then(key => {
            if (key) {
                return;
            }
        });
    }

    save() {
        var cmd = new dto.UpdateNotifications();
        cmd.NotifyOnCriticalIncidents = this.notifyOnCriticalIncidents;
        cmd.NotifyOnImportantIncidents = this.notifyOnImportantIncidents;
        cmd.NotifyOnNewIncidents = this.notifyOnNewIncidents;
        cmd.NotifyOnPeaks = this.notifyOnPeaks;
        cmd.NotifyOnReOpenedIncident = this.notifyOnReOpenedIncident;
        cmd.NotifyOnUserFeedback = this.notifyOnUserFeedback;
        cmd.ApplicationId = this.applicationId;
        cmd.UserId = AppRoot.Instance.currentUser.id;
        AppRoot.Instance.apiClient.command(cmd);

        var value = dto.NotificationState.BrowserNotification;
        if (this.notifyOnNewIncidents === value ||
            this.notifyOnPeaks === value ||
            this.notifyOnCriticalIncidents === value ||
            this.notifyOnImportantIncidents === value ||
            this.notifyOnReOpenedIncident === value ||
            this.notifyOnUserFeedback === value) {
            if (!this.isNotificationsSupported()) {
                AppRoot.notify("Your browser do not support notifications");
                return;
            }

            this.registerPushSubscription().then(result => {
                if (result) {
                    AppRoot.notify('Saved OK');
                } 
            });
        } else {
            this.deleteSubscription();
            AppRoot.notify('Saved OK');
        }

        
    }

    saveSettings() {
        var cmd = new dto.UpdatePersonalSettings();
        cmd.EmailAddress = this.workEmail;
        AppRoot.Instance.apiClient.command(cmd);
        AppRoot.notify('Saved OK');

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
        cmd.Endpoint = subscription.endpoint;
        cmd.UserId = AppRoot.Instance.currentUser.id;
        AppRoot.Instance.apiClient.command(cmd);
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
        console.log('ley', key);

        subscription = await registration.pushManager.subscribe({
            userVisibleOnly: true,
            applicationServerKey: key
        });

        this.storeBrowserSubscription(subscription);
        return true;
    }

    private async storeBrowserSubscription(subscription: PushSubscription) {
        var cmd = new dto.StoreBrowserSubscription();
        cmd.UserId = AppRoot.Instance.currentUser.id;
        cmd.Endpoint = subscription.endpoint;
        cmd.ExpirationTime = subscription.expirationTime;

        // Know a better way which isn't allocate a lot of objects?
        var obj = JSON.parse(JSON.stringify(subscription));

        cmd.PublicKey = obj.keys.p256dh;
        cmd.AuthenticationSecret = obj.keys.auth;
        AppRoot.Instance.apiClient.command(cmd);
    }

    private async getPublicKey(): Promise<Uint8Array> {
        const response = await fetch('./push/vapidpublickey');
        if (response.status === 204) {
            return null;
        }

        const json = await response.json();
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
