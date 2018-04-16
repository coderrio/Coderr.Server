import { AppRoot } from '../../../../services/AppRoot';
import { ApplicationToCreate } from "../../../../services/applications/ApplicationService";
import { GetApplicationIdByKey, GetApplicationIdByKeyResult } from "../../../../dto/Core/Applications"
import Vue from "vue";
import { Component, Watch } from "vue-property-decorator";


@Component
export default class ManageCreateApplicationComponent extends Vue {
    private timer$: any;
    applicationName = "";
    disableButton = false;

    created() {
    }


    mounted() {
    }

    createApplication() {
        AppRoot.Instance.applicationService.create(this.applicationName)
            .then(appKey => {
                this.timer$ = setInterval(() => {
                    this.checkIfApplicationIsCreated(appKey);
                }, 1000);
            });
        this.disableButton = true;

    }

    private updateSession(applicationId: number) {
        var route = this.$router.resolve({
            name: 'configureApplication',
            params: { applicationId: applicationId.toString() }
        });
        var url = "/account/update/session/?ReturnUrl=" + encodeURI(route.href);
        window.location.href = url;
    }

    private checkIfApplicationIsCreated(appKey: string) {
        var query = new GetApplicationIdByKey();
        query.ApplicationKey = appKey;
        AppRoot.Instance.apiClient.query<GetApplicationIdByKeyResult>(query)
            .then(result => {
                if (result) {
                    clearInterval(this.timer$);
                    this.updateSession(result.Id);
                }
            });
    }
}
