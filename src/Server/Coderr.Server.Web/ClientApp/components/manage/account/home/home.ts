import { AppRoot } from "@/services/AppRoot";
import * as dto from "@/dto/Core/Users";
import { Component, Vue } from "vue-property-decorator";
//import JQuery from "jquery";
//let $ = JQuery;

@Component
export default class ManageHomeComponent extends Vue {
    applicationId: number | null = null;
    appNotice: string;
    private workerUrl = '/js/pushworker.js';
    notifyOnNewIncidents = dto.NotificationState.Disabled;
    notifyOnPeaks = dto.NotificationState.Disabled;
    notifyOnReOpenedIncident = dto.NotificationState.Disabled;
    notifyOnUserFeedback = dto.NotificationState.Disabled;

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
                this.notifyOnNewIncidents = x.Notifications.NotifyOnNewIncidents;
                this.notifyOnPeaks = x.Notifications.NotifyOnPeaks;
                this.notifyOnUserFeedback = x.Notifications.NotifyOnUserFeedback;
                this.notifyOnReOpenedIncident = x.Notifications.NotifyOnReOpenedIncident;
            });

    }

    save() {
        var cmd = new dto.UpdateNotifications();
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
            this.notifyOnReOpenedIncident === value ||
            this.notifyOnUserFeedback === value) {
            if (!this.isNotificationsSupported()) {
                AppRoot.notify("Your browser do not support notifications");
                return;
            }

            this.storeSubscription();
        } else {
            this.deleteSubscription();
        }
    }

    private async storeSubscription() {
        await this.askForPermission();
        var subscription = await this.subscribeUserToPush();
        this.storeBrowserSubscription(subscription);
    }

    private async deleteSubscription() {
        var sw = await this.getRegistration();
        const reg = await sw.pushManager.getSubscription();
        if (reg == null) {
            return;
        }

        reg.unsubscribe();

        var cmd = new dto.DeleteBrowserSubscription();
        cmd.Endpoint = reg.endpoint;
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

    async getRegistration(): Promise<ServiceWorkerRegistration> {
        let registration = await navigator.serviceWorker.getRegistration(this.workerUrl);
        console.log('regurl', registration);
        if (registration == null) {
            registration = await navigator.serviceWorker.register(this.workerUrl);
            console.log('regurl2', registration);
        }

        return registration;
    }

    async askForPermission() {
        var result = await Notification.requestPermission();
        if (result !== 'granted') {
            throw new Error('We weren\'t granted permission.');
        }
    }

    async subscribeUserToPush() {
        const key = await this.getPublicKey();
        console.log('publicKey', key);
        const subscribeOptions = {
            userVisibleOnly: true,
            applicationServerKey: this.urlBase64ToUint8Array(key)
        };

        var registration = await this.getRegistration();
        const subscription = await registration.pushManager.getSubscription();
        if (subscription !== null) {
            return subscription;
        }

        return await registration.pushManager.subscribe(subscribeOptions);
    }

    private async storeBrowserSubscription(subscription: PushSubscription) {
        var cmd = new dto.StoreBrowserSubscription();
        cmd.UserId=AppRoot.Instance.currentUser.id;
        cmd.Endpoint = subscription.endpoint;
        cmd.ExpirationTime = subscription.expirationTime;

        // Know a better way which isn't allocate a lot of objects?
        console.log(JSON.stringify(subscription));
        var obj = JSON.parse(JSON.stringify(subscription));

        cmd.PublicKey = obj.keys.p256dh;
        cmd.AuthenticationSecret = obj.keys.auth;

        console.log(JSON.stringify(cmd));
        AppRoot.Instance.apiClient.command(cmd);
    }

    private async getPublicKey() {
        var response = await fetch('./push/vapidpublickey');
        return await response.json();
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

    private arrayBufferToBase64(buffer: ArrayBuffer) {
        var binary = '';
        var bytes = new Uint8Array(buffer);
        var len = bytes.byteLength;
        for (var i = 0; i < len; i++) {
            binary += String.fromCharCode(bytes[i]);
        }
        return window.btoa(binary);
    }
}
