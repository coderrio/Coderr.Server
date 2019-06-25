import { ApiClient } from "../../../services/ApiClient";
import * as http from '../../../services/HttpClient';
import { AppRoot } from '../../../services/AppRoot';
import { FindIncidents, FindIncidentsResult, FindIncidentsResultItem } from '../../../dto/Core/Incidents'
import * as Mine from '../../../dto/Common/Mine'
import Vue from "vue";
import { Component, Watch } from "vue-property-decorator";

interface ILibrarySummary {
    clientFolderName: string;
    description: string;
    id: string;
    libraryName: string;
}
@Component
export default class ConfigureClientComponent extends Vue {
    private lastLib: string;
    libraries: ILibrarySummary[] = [];
    instruction: string | null = null;

    applicationId = 0;
    appKey = "";
    sharedSecret = "";
    reportUrl = "";

    noConnection = false;
    weAreInTrouble = false;

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
        AppRoot.Instance.applicationService.get(this.applicationId)
            .then(app => {
                this.sharedSecret = app.sharedSecret;
                this.appKey = app.appKey;
            });
    }

    @Watch('$route.params.applicationId')
    onApplicationChanged(value: string, oldValue: string) {
        if (!value) {
            return;
        }

        this.applicationId = parseInt(value);
        AppRoot.Instance.applicationService.get(this.applicationId)
            .then(app => {
                this.sharedSecret = app.sharedSecret;
                this.appKey = app.appKey;
                this.select(this.lastLib);
            });
    }

    goToSupport() {
        this.$router.push({
            name: "support",
            params: { type: "configuration" }
        });
    }

    select(libName: string) {
        this.lastLib = libName;
        var client = new http.HttpClient();
        client.get(ApiClient.ApiUrl + 'onboarding/library/' + libName + "/?appKey=" + this.appKey)
            .then(response => {
                this.instruction = response.body
                    .replace('yourAppKey', this.appKey)
                    .replace('yourSharedSecret', this.sharedSecret);
            });
        var buttons = document.querySelectorAll('.buttons button');
        for (var i = 0; i < buttons.length; i++) {
            if (buttons[i].className.indexOf('btn-red') !== -1) {
                buttons[i].classList.remove('btn-red');
                buttons[i].classList.add('btn-light');
            }

        }
        var button = <HTMLElement>document.querySelector(`.buttons [data-lib="${libName}"]`);
        button.classList.add('btn-red');
    }

    completedConfiguration() {
        var q = new FindIncidents();
        q.PageNumber = 1;
        q.ItemsPerPage = 1;
        q.ApplicationIds = [this.applicationId];
        AppRoot.Instance.apiClient.query<FindIncidentsResult>(q)
            .then(result => {
                if (result.TotalCount === 0) {
                    this.weAreInTrouble = true;
                    return;
                }

                this.$router.push({
                    name: "discover",
                    params: { applicationId: this.applicationId.toString() }
                });
            });

    }


}
