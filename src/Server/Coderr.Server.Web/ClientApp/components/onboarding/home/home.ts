import { PubSubService } from "../../../services/PubSub";
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
export default class OnboardingHomeComponent extends Vue {
    libraries: ILibrarySummary[] = [];
    instruction: string | null = null;
    weAreInTrouble = false;

    created() {
        var client = new http.HttpClient();
        client.get('/api/onboarding/libraries/')
            .then(response => {
                var data = <ILibrarySummary[]>response.body;
                data.forEach(lib => {
                    this.libraries.push(lib);
                });
            });
    }

    select(libName: string) {
        var appInfo = AppRoot.Instance.currentUser.applications[0];
        AppRoot.Instance.applicationService.get(appInfo.id)
            .then(app => {
                var client = new http.HttpClient();
                client.get('/api/onboarding/library/' + libName + "/?appKey=" + app.appKey)
                    .then(response => {
                        this.instruction = response.body;
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

    mounted() {
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
