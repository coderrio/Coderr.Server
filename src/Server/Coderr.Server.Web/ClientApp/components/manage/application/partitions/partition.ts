import { AppRoot } from '../../../../services/AppRoot';
import * as Partitions from "../../../../dto/Common/Partitions";
import Vue from "vue";
import { Component } from "vue-property-decorator";


@Component
export default class ManagePartitionsComponent extends Vue {

    applicationId: number = 0;
    partitions: Partitions.GetPartitionsResultItem[] = [];

    created() {
        var appIdStr = this.$route.params.applicationId;
        this.applicationId = parseInt(appIdStr, 10);

        var q = new Partitions.GetPartitions();
        q.ApplicationId = this.applicationId;
        AppRoot.Instance.apiClient.query<Partitions.GetPartitionsResult>(q)
            .then(result => {
                for (let i = 0; i < result.Items.length; i++) {
                    this.partitions.push(result.Items[i]);
                }
            });
    }


    mounted() {
    }

    deletePartition(id: number) {
        if (!id) {
            throw new Error("Expected partition id, got: " + id);
        }

        var itemIndex = -1;
        for (var i = 0; i < this.partitions.length; i++) {
            if (this.partitions[i].Id === id) {
                itemIndex = i;
            }
        }
        if (itemIndex === -1) {
            return;
        }

        AppRoot.modal({
            title: "Delete partition",
            htmlContent: "Do you really want to delete partition '" + this.partitions[itemIndex].Name + "'?",
            cancelButtonText: "No",
            submitButtonText: "Yes"
        }).then(result => {
            var cmd = new Partitions.DeletePartition();
            cmd.Id = id;
            AppRoot.Instance.apiClient.command(cmd);

            this.partitions.splice(itemIndex, 1);
        });

    }

}
