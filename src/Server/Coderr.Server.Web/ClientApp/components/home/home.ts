import { AppRoot } from "../../services/AppRoot";
import { FindIncidents, FindIncidentsResult } from "../../dto/Core/Incidents";
import Vue from "vue";
import { Component } from "vue-property-decorator";
import * as Onboarding from "@/dto/Modules/Onboarding";

@Component
export default class HomeHomeComponent extends Vue {
    destroyed$ = false;
    muteOnboarding = false;
    noApps = false;
    appId = '1';
    showOnboarding = false;

    created() {

        if (this.$route.query['from'] === 'invited') {
            this.mute();
            return;
        }

        AppRoot.Instance.loadState("MainHome", this)
            .then(gotState => {
                
                if (gotState) {

                    if (this.muteOnboarding) {
                        this.$router.push({ name: "discover" });
                    }
                    return;
                }

                var query = new Onboarding.GetOnboardingState();
                AppRoot.Instance.apiClient.query<Onboarding.GetOnboardingStateResult>(query)
                    .then(result => {
                        if (this.destroyed$) {
                            return;
                        }

                        if (result.IsComplete) {
                            this.mute();
                            return;
                        }

                        var q = new FindIncidents();
                        q.PageNumber = 1;
                        q.ItemsPerPage = 1;
                        AppRoot.Instance.apiClient.query<FindIncidentsResult>(q)
                            .then(result => {
                                if (this.destroyed$) {
                                    return;
                                }
                                if (result.TotalCount > 0) {
                                    this.mute();
                                } else {
                                    this.start();
                                }
                            });
                    });
            });
    }

    destroyed() {
        this.destroyed$ = true;
    }

    start() {
        this.$router.push({ name: 'onboardStart', params: { applicationId: this.appId } });
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
                    this.showOnboarding = false;
                    return;
                }
                this.$router.push({ name: "discover", params: { applicationId: apps[0].id.toString() } });
            });
    }

}
