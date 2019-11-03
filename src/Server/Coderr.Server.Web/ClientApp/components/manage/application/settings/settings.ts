import { AppRoot } from '../../../../services/AppRoot';
import Vue from "vue";
import { Component, Watch } from "vue-property-decorator";


@Component
export default class ManageAppSettingsComponent extends Vue {
    applicationId: number = 0;
    applicationName: string = "";
    appKey: string = "";
    sharedSecret: string = "";


    created() {
        this.load();
    }

    
    mounted() {
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
            });
    }

    deleteApp() {
        var result = confirm("Do you really want to delete '" + this.applicationName + "'.");
        if (result) {
            AppRoot.Instance.applicationService.delete(this.applicationId);
            AppRoot.notify("Application have been queued for deletion. Might take time depending on the number of incidents.", "fa-info", "success");
            this.$router.push('manageHome');
        }
    }
    updateApp() {
        AppRoot.Instance.applicationService.update(this.applicationId, this.applicationName);
        AppRoot.notify('Application settings have been saved.');
    }

}
