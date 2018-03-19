import { PubSubService } from "../../../services/PubSub";
import { ApiClient } from '../../../services/ApiClient';
import { AppRoot } from '../../../services/AppRoot';
import { FindIncidents, FindIncidentsResult, FindIncidentsResultItem } from '../../../dto/Core/Incidents'
import * as Mine from '../../../dto/Common/Mine'
import Vue from "vue";
import { Component } from "vue-property-decorator";

@Component
export default class AnalyzeHomeComponent extends Vue {

    applicationId: number|null = null;
    incidentId: number|null = null;
    noIncidents: boolean = false;

    created() {
        if (this.$route.params.applicationId) {
            this.applicationId = parseInt(this.$route.params.applicationId, 10);
        }
        if (this.$route.params.incidentId) {
            this.incidentId = parseInt(this.$route.params.incidentId, 10);
        }
        

        var q3 = new FindIncidents();
        q3.AssignedToId = 1;
        AppRoot.Instance.apiClient.query<FindIncidentsResult>(q3)
            .then(result => {
                if (result.Items.length > 0) {
                    this.$router.push({ name: 'analyzeIncident', params: { incidentId: result.Items[0].Id.toString() } });
                }
            });
    }

    mounted() {
    }
    

}
