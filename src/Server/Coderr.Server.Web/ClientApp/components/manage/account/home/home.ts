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

        navigator.serviceWorker.ready.then(registration => {
            registration.pushManager.getSubscription().then(subscription => console.log(JSON.stringify(subscription)));
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

            console.log('regging')
            this.registerPushSubscription();
        } else {
            this.deleteSubscription();
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

    private async registerPushSubscription(): Promise<any> {
        console.log('register')
        navigator.serviceWorker.register(this.workerUrl);
        console.log('waiting ready', navigator.serviceWorker.ready)
        const registration = await navigator.serviceWorker.ready;
        console.log('getsub')
        let subscription = await registration.pushManager.getSubscription();
        if (subscription) {
            return subscription;
        }

        const key = await this.getPublicKey();
        subscription = await registration.pushManager.subscribe({
            userVisibleOnly: true,
            applicationServerKey: this.urlBase64ToUint8Array(key)
        });

        console.log('Got ', subscription);
        this.storeBrowserSubscription(subscription);
    }

    private async storeBrowserSubscription(subscription: PushSubscription) {
        var cmd = new dto.StoreBrowserSubscription();
        cmd.UserId = AppRoot.Instance.currentUser.id;
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

}
