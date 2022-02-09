import { AppRoot } from '../../../../services/AppRoot';
import { ApplicationGroup } from "@/services/applications/ApplicationService";
import { Component, Watch, Vue } from "vue-property-decorator";
import * as dto from "@/dto/Core/Applications";

@Component
export default class ManageAppSettingsComponent extends Vue {
    applicationId: number = 0;
    applicationName: string = "";
    appKey: string = "";
    sharedSecret: string = "";
    groupId: number = 0;
    applicationGroups: ApplicationGroup[] = [];
    newGroupName: string = "";
    retentionDays: number = 90;

    created() {
        this.load();
    }


    mounted() {
        AppRoot.Instance.applicationService.getGroups().then(result => {
            this.applicationGroups = result;
        });
    }


    @Watch('$route.params.applicationId')
    onApplicationChanged(value: string, oldValue: string) {
        this.load();
    }

    private load() {
        this.applicationId = parseInt(this.$route.params.applicationId, 10);
        AppRoot.Instance.applicationService.get(this.applicationId)
            .then(appInfo => {
                this.applicationName = appInfo.name;
                this.sharedSecret = appInfo.sharedSecret;
                this.appKey = appInfo.appKey;
                this.groupId = appInfo.groupId;
                this.retentionDays = appInfo.retentionDays;
            });
    }

    deleteApp() {
        var result = confirm("Do you really want to delete '" + this.applicationName + "'.");
        if (result) {
            AppRoot.Instance.applicationService.delete(this.applicationId);
            AppRoot.notify("Application have been queued for deletion. Might take time depending on the number of incidents.", "fa-info", "success");
            this.$router.push({ name: 'manageHome' });
        }
    }
    updateApp() {
        AppRoot.Instance.applicationService.update(this.applicationId, this.applicationName, this.retentionDays);
        AppRoot.Instance.applicationService.setGroup(this.applicationId, this.groupId);
        AppRoot.notify('Application settings have been saved.');
    }
}
