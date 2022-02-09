import { AppRoot } from '../../../../services/AppRoot';
import * as dto from "@/dto/Core/Applications";
import { ApplicationGroup } from "@/services/applications/ApplicationService";
import Vue from "vue";
import { Component, Watch } from "vue-property-decorator";


@Component
export default class ManageEnvironmentsComponent extends Vue {
    groups: ApplicationGroup[] = [];
    groupToDelete: number = -1;
    groupToMoveAppsTo: number = -1;

    mounted() {
        this.load();
    }

    deleteGroup() {
        var cmd = new dto.DeleteApplicationGroup();
        cmd.GroupId = this.groupToDelete;
        cmd.MoveAppsToGroupId = this.groupToMoveAppsTo;
        AppRoot.Instance.apiClient.command(cmd)
            .then(result => {
                AppRoot.notify('Group have been deleted.');
            });

        var index = this.groups.findIndex(x => x.id === this.groupToDelete);
        this.$delete(this.groups, index);
        this.groupToDelete = -1;
        this.groupToMoveAppsTo = -1;
    }


    private load() {
        AppRoot.Instance.applicationService.getGroups()
            .then(groups => {
                this.groups = groups;
            });

    }

    createGroup() {
        AppRoot.modal({
            contentId: "newGroupModal",
            showFooter: true
        }).then(result => {

            // Fetch last since they modal is duplicated
            var nodes = document.querySelectorAll('[name="newGroupName"]');
            var value = (<HTMLInputElement>nodes[nodes.length - 1]).value;

            AppRoot.Instance.applicationService.createGroup(value);
        });
    }

}
