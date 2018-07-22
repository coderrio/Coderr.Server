import { AppRoot } from "../../services/AppRoot";
import { FindIncidents, FindIncidentsResult } from "../../dto/Core/Incidents";
import Vue from "vue";
import { Component } from "vue-property-decorator";

@Component
export default class HomeHomeComponent extends Vue {
    muteOnboarding = false;
    noApps = false;
    appId = '1';
    showOnboarding = false;

    created() {
        AppRoot.Instance.loadState("MainHome", this)
            .then(x => {
                if (this.muteOnboarding) {
                    this.$router.push({ name: "discover" });
                }
            });

        var q = new FindIncidents();
        q.PageNumber = 1;
        q.ItemsPerPage = 1;
        AppRoot.Instance.apiClient.query<FindIncidentsResult>(q)
            .then(result => {
                this.showOnboarding = true;
                if (result.TotalCount > 0) {
                    this.mute();
                }
            });

        AppRoot.Instance.applicationService.list()
            .then(apps => {
                this.noApps = apps.length === 0;
                if (apps.length > 0) {
                    this.appId = apps[0].id.toString();
                }
            });
    }

    start() {
        this.$router.push({ name: 'onboardApp', params: { applicationId: this.appId } });
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
