import { PubSubService } from "@/services/PubSub";
import { AppRoot } from '@/services/AppRoot';
import { Component, Vue } from "vue-property-decorator";
import * as dto from "@/dto/Common/Partitions"

@Component
export default class IncidentImpactComponent extends Vue {
    solution = "";
    partitions: dto.GetPartitionsResultItem[] = [];
    values: dto.GetPartitionValuesResultItem[] = [];
    incidentId: number = 0;
    partitionId: number = 0;

    mounted() {
        this.incidentId = parseInt(this.$route.params.incidentId, 10);
        AppRoot.Instance.incidentService.get(this.incidentId)
            .then(incident => {
                var query = new dto.GetPartitions();
                query.ApplicationId = incident.ApplicationId;
                AppRoot.Instance.apiClient.query<dto.GetPartitionsResult>(query)
                    .then(x => {
                        this.partitions = x.Items;
                    });
            });
    }

    onPartitionChanged() {
        var query = new dto.GetPartitionValues();
        query.IncidentId = this.incidentId;
        query.PartitionId = this.partitionId;
        AppRoot.Instance.apiClient.query<dto.GetPartitionValuesResult>(query)
            .then(result => {
                this.values = result.Items;
            });

    }
}
