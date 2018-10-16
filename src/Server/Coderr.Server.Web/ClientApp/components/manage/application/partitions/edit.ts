import { AppRoot } from '../../../../services/AppRoot';
import * as Partitions from "../../../../dto/Common/Partitions";
import Vue from "vue";
import { Component } from "vue-property-decorator";


@Component
export default class EditPartitionComponent extends Vue {
    id = 0;

    title = "";
    partitionKey = "";
    numberOfItems: number | null = null;
    weight = 1;

    applicationId: number = 0;

    created() {
        var appIdStr = this.$route.params.applicationId;
        this.applicationId = parseInt(appIdStr, 10);
        this.id = parseInt(this.$route.params.partitionId, 10);
        this.getPartition();
    }


    mounted() {
    }


    getPartition() {
        var query = new Partitions.GetPartition();
        query.Id = this.id;
        AppRoot.Instance.apiClient.query<Partitions.GetPartitionResult>(query)
            .then(dto => {
                this.title = dto.Name;
                this.partitionKey = dto.PartitionKey;
                this.weight = dto.Weight;
                this.numberOfItems = dto.NumberOfItems;
            });
    }

    updatePartition() {
        var cmd = new Partitions.UpdatePartition();
        cmd.Id = this.id;
        cmd.Name = this.title;
        cmd.Weight = this.weight;
        cmd.NumberOfItems = this.numberOfItems;
        if (this.numberOfItems !== 0 && this.numberOfItems != null) {
            cmd.NumberOfItems = this.numberOfItems;
        }
        AppRoot.Instance.apiClient.command(cmd)
            .then(x => {
                this.$router.push(
                    { name: "managePartitions", params: { applicationId: this.applicationId.toString() } });

                AppRoot.notify('Partition is scheduled to be updated.');
            });
    }

}
