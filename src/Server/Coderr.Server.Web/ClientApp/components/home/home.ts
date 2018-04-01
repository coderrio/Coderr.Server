import { AppRoot } from "../../services/AppRoot";
import Vue from "vue";
import { Component } from "vue-property-decorator";

@Component
export default class HomeHomeComponent extends Vue {
    muteOnboarding = false;
    noApps = false;
    created() {
        AppRoot.Instance.loadState("MainHome", this)
            .then(x => {
                if (this.muteOnboarding) {
                    this.$router.push({ name: "discover" });
                }
            });
    }

    start() {
        // since we can be users who do not have access to application with id 1
        AppRoot.Instance.applicationService.list()
            .then(apps => {
                if (apps.length === 0) {
                    this.noApps = true;
                    return;
                }
                this.$router.push({ name: 'onboardApp', params: { applicationId: apps[0].id.toString() } });
            });
    }

    mute() {
        this.muteOnboarding = true;
        AppRoot.Instance.storeState({
            component: this,
            name: "MainHome"
        });

        // since we can be users who do not have access to application with id 1
        AppRoot.Instance.applicationService.list()
            .then(apps => {
                if (apps.length === 0) {
                    this.noApps = true;
                    return;
                }
                this.$router.push({ name: "discover", params: { applicationId: apps[0].id.toString() } });
            });
    }

}
