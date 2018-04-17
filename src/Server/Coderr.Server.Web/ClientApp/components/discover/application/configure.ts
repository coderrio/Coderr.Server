import { ApiClient } from "../../../services/ApiClient";
import * as http from '../../../services/HttpClient';
import { AppRoot } from '../../../services/AppRoot';
import { FindIncidents, FindIncidentsResult, FindIncidentsResultItem } from '../../../dto/Core/Incidents'
import * as Mine from '../../../dto/Common/Mine'
import Vue from "vue";
import { Component } from "vue-property-decorator";

interface ILibrarySummary {
    clientFolderName: string;
    description: string;
    id: string;
    libraryName: string;
}
@Component
export default class ConfigureClientComponent extends Vue {
    libraries: ILibrarySummary[] = [];
    instruction: string | null = null;
    weAreInTrouble = false;

    applicationId = 0;
    appKey = "";
    sharedSecret = "";
    reportUrl = "";

    noConnection = false;
    gotNoIncidents = false;

    created() {
        this.applicationId = parseInt(this.$route.params.applicationId, 10);

        var client = new http.HttpClient();
        client.get(ApiClient.ApiUrl + 'onboarding/libraries/')
            .then(response => {
                var data = <ILibrarySummary[]>response.body;
                data.forEach(lib => {
                    this.libraries.push(lib);
                });
            }).catch(x => {
                this.noConnection = true;
            });

        var pos = window.location.toString().indexOf('/discover/');
        this.reportUrl = window.location.toString().substr(0, pos + 1);
    }

    mounted() {
        this.applicationId = parseInt(this.$route.params.applicationId, 10);
        AppRoot.Instance.applicationService.get(this.applicationId)
            .then(app => {
                this.sharedSecret = app.sharedSecret;
                this.appKey = app.appKey;
            });
    }

    goToSupport() {
        this.$router.push({
            name: "support",
            params: { type: "configuration" }
        });
    }

    onExitGuide() {
        AppRoot.Instance.incidentService.find(this.applicationId)
            .then(x => {
                if (x.Items.length === 0) {
                    this.gotNoIncidents = true;
                } else {
                    this.$router.push({
                        name: "discover",
                        params: { applicationId: this.applicationId.toString() }
                    });
                }
            });
    }

    select(libName: string) {
        var appInfo = AppRoot.Instance.currentUser.applications[0];
        AppRoot.Instance.applicationService.get(appInfo.id)
            .then(app => {
                var client = new http.HttpClient();
                client.get(ApiClient.ApiUrl + 'onboarding/library/' + libName + "/?appKey=" + app.appKey)
                    .then(response => {
                        this.instruction = response.body
                            .replace('yourAppKey', this.appKey)
                            .replace('yourSharedSecret', this.sharedSecret);
                    });
            });
        var buttons = document.querySelectorAll('.buttons button');
        for (var i = 0; i < buttons.length; i++) {
            if (buttons[i].className.indexOf('btn-dark') !== -1) {
                buttons[i].classList.remove('btn-dark');
                buttons[i].classList.remove('btn-light');
            }

        }
        var button = <HTMLElement>document.querySelector(`.buttons [data-lib="${libName}"]`);
        button.classList.remove('btn-light');
        button.classList.add('btn-dark');
    }

    completedConfiguration() {
        var q = new FindIncidents();
        q.PageNumber = 1;
        q.ItemsPerPage = 1;
        AppRoot.Instance.apiClient.query<FindIncidentsResult>(q)
            .then(result => {
                if (result.TotalCount === 0) {
                    this.weAreInTrouble = true;
                    return;
                }

                this.$router.push({ name: "root" });
            });

    }


}
