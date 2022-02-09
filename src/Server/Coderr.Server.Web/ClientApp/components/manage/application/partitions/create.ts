import { AppRoot } from '../../../../services/AppRoot';
import * as Partitions from "../../../../dto/Common/Partitions";
import Vue from "vue";
import { Component } from "vue-property-decorator";


@Component
export default class CreatePartitionComponent extends Vue {

    //new partition
    title = '';
    partitionKey = '';
    numberOfItems: number | null = null;
    importantThreshold: number | null = null;
    criticalThreshold: number | null = null;
    weight = 1;

    applicationId: number = 0;

    created() {
        var appIdStr = this.$route.params.applicationId;
        this.applicationId = parseInt(appIdStr, 10);

    }


    mounted() {
    }


    createPartition() {
        var cmd = new Partitions.CreatePartition();
        cmd.Name = this.title;
        cmd.ApplicationId = this.applicationId;
        cmd.PartitionKey = this.partitionKey;
        cmd.Weight = this.weight;
        if (this.numberOfItems !== 0 && this.numberOfItems != null) {
            cmd.NumberOfItems = this.numberOfItems;
        }
        if (this.importantThreshold !== 0 && this.importantThreshold != null) {
            cmd.ImportantThreshold = this.importantThreshold;
        }
        if (this.criticalThreshold !== 0 && this.criticalThreshold != null) {
            cmd.CriticalThreshold = this.criticalThreshold;
        }

        AppRoot.Instance.apiClient.command(cmd)
            .then(x => {
                this.$router.push(
                    { name: "managePartitions", params: { applicationId: this.applicationId.toString() } });

                AppRoot.notify('Partition is scheduled to be created.');
            });
    }

}
