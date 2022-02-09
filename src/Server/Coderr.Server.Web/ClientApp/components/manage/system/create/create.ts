import { AppRoot } from '../../../../services/AppRoot';
import { ApplicationGroup } from "@/services/applications/ApplicationService";
import { GetApplicationIdByKey, GetApplicationIdByKeyResult } from "../../../../dto/Core/Applications"
import { Component, Vue } from "vue-property-decorator";
import * as dto from "@/dto/Core/Applications";

@Component
export default class ManageCreateApplicationComponent extends Vue {
    private timer$: any;
    groupId = 1;
    applicationGroups: ApplicationGroup[] = [];
    applicationName = "";
    groupName = "";
    numberOfDevelopers?: number = null;
    estimatedNumberOfErrors?: number = null;
    disableButton = false;
    retentionDays: number = 60;

    created() {
    }


    mounted() {
        AppRoot.Instance.applicationService.getGroups().then(result => {
            this.applicationGroups = result;
            this.groupId = result[0].id;
        });
    }

    createApplication() {
        AppRoot.Instance.applicationService.create(this.groupId, this.applicationName, this.numberOfDevelopers, this.estimatedNumberOfErrors, this.retentionDays)
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

        var baseUrl = document.getElementsByTagName('base')[0].href;
        var url = baseUrl + "/account/update/session/?ReturnUrl=" + encodeURI(route.href);
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
                    AppRoot.Instance.applicationService.setGroup(result.Id, this.groupId);
                }
            });
    }
}
