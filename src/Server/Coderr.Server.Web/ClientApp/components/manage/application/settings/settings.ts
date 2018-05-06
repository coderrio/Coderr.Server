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
        console.log(value);
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

    updateApp() {
        AppRoot.Instance.applicationService.update(this.applicationId, this.applicationName);
        AppRoot.notify('Application settings have been saved.');
    }

}
