import { AppRoot } from '../../../services/AppRoot';
import Vue from "vue";
import { Component, Watch } from "vue-property-decorator";


@Component
export default class ManageHomeComponent extends Vue {
    applicationId: number = 0;
    applicationName: string = "";
    appKey: string = "";
    sharedSecret: string = "";


    created() {
        var appIdStr = this.$route.params.applicationId;
        this.applicationId = parseInt(appIdStr, 10);
        AppRoot.Instance.applicationService.get(this.applicationId)
            .then(appInfo => {
                this.applicationName = appInfo.name;
                this.sharedSecret = appInfo.sharedSecret;
                this.appKey = appInfo.appKey;
            });
    }

    
    mounted() {
    }
    

    @Watch('$route.params.applicationId')
    onApplicationChanged(value: string, oldValue: string) {
        this.applicationId = parseInt(value, 10);
        AppRoot.Instance.applicationService.get(this.applicationId)
            .then(appInfo => {
                this.applicationName = appInfo.name;
                this.sharedSecret = appInfo.sharedSecret;
                this.appKey = appInfo.appKey;
            });
    }


}
