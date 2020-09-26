import { AppRoot } from '../../../services/AppRoot';
import {
    GetIncidentStateSummary, GetIncidentStateSummaryResult,
    GetIncidentsForStates, GetIncidentsForStatesResult, GetIncidentsForStatesResultItem
} from "../../../dto/Modules/History";
import Vue from "vue";
import { Component, Watch } from "vue-property-decorator";

@Component
export default class DeploymentSummaryComponent extends Vue {
    applicationId: number = 0;
    version: string;
    stats: GetIncidentStateSummaryResult | null;
    reOpenedIncidents: GetIncidentsForStatesResultItem[] = [];
    newIncidents: GetIncidentsForStatesResultItem[] = [];
    closedIncidents: GetIncidentsForStatesResultItem[] = [];

    created() {
        var appIdStr = this.$route.params.applicationId;
        this.applicationId = parseInt(appIdStr, 10);

        this.version = this.$route.params.version.replace(/\_/g, ".");

        var q1 = new GetIncidentStateSummary();
        q1.ApplicationId = this.applicationId;
        q1.ApplicationVersion = this.version;
        AppRoot.Instance.apiClient.query<GetIncidentStateSummaryResult>(q1)
            .then(x => {
                this.stats = x;
            });

        var q2 = new GetIncidentsForStates();
        q2.ApplicationVersion = this.version;
        q2.ApplicationId = this.applicationId;
        AppRoot.Instance.apiClient.query<GetIncidentsForStatesResult>(q2)
            .then(x => {
                x.Items.forEach(item => {
                    if (item.IsNew)
                        this.newIncidents.push(item);
                    if (item.IsClosed)
                        this.closedIncidents.push(item);
                    if (item.IsReopened)
                        this.reOpenedIncidents.push(item);
                });
            });

    }


    mounted() {
    }


    @Watch('$route.params.applicationId')
    onApplicationChanged(value: string, oldValue: string) {
        if (!value) {
            this.applicationId = null;
        } else {
            this.applicationId = parseInt(value, 10);
        }

        this.loadData(this.applicationId);
    }

    loadData(applicationId?: number) {

    }


}
