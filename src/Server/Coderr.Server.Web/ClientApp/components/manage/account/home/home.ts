import { AppRoot } from "@/services/AppRoot";
import * as dto from "@/dto/Core/Users";
import { Component, Vue } from "vue-property-decorator";
//import JQuery from "jquery";
//let $ = JQuery;

@Component
export default class ManageHomeComponent extends Vue {
    applicationId: number | null = null;
    appNotice: string;
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
        console.log(value, this.notifyOnReOpenedIncident);
        //console.log(this.notifyOnNewIncidents, this.notifyOnNewIncidents === dto.NotificationState.BrowserNotification.toString());
        if (this.notifyOnNewIncidents === value ||
            this.notifyOnPeaks === value ||
            this.notifyOnReOpenedIncident === value ||
            this.notifyOnUserFeedback === value) {
            console.log('here here');
            if (!this.isNotificationsSupported()) {
                AppRoot.notify("Your browser do not support notifications");
                return;
            }

            this.storeSubscription();
        }
    }

    private async storeSubscription() {
        await this.askForPermission();
        await this.registerServiceWorker();
        var subscription = await this.subscribeUserToPush();
        this.storeBrowserSubscription(subscription);
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

    async registerServiceWorker() {
        await navigator.serviceWorker.register('/js/pushworker.js');
    }

    async askForPermission() {
        var result = await Notification.requestPermission();
        if (result !== 'granted') {
            throw new Error('We weren\'t granted permission.');
        }
    }

    async subscribeUserToPush() {
        var key = await this.getPublicKey();
        var registration = await navigator.serviceWorker.register('/js/pushworker.js');
        const subscribeOptions = {
            userVisibleOnly: true,
            applicationServerKey: this.urlBase64ToUint8Array(key)
        };

        return await registration.pushManager.subscribe(subscribeOptions);
    }

    private async storeBrowserSubscription(subscription: PushSubscription) {
        var cmd = new dto.StoreBrowserSubscription();
        cmd.UserId=AppRoot.Instance.currentUser.id;
        cmd.Endpoint = subscription.endpoint;
        //cmd.ExpirationTime = new Date(new Date() + subscription.expirationTime);
        console.log(JSON.stringify({
            subscription: subscription
        }));
    }

    private async getPublicKey() {
        var response = await fetch('./cqs/vapidpublickey');
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
