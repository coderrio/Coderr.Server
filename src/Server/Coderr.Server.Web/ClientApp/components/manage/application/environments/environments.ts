import { AppRoot } from '../../../../services/AppRoot';
import { GetEnvironments, GetEnvironmentsResult,ResetEnvironment } from "../../../../dto/Core/Environments";
import Vue from "vue";
import { Component, Watch } from "vue-property-decorator";

interface IEnvironment {
    name: string;
    id: number;
}

@Component
export default class ManageEnvironmentsComponent extends Vue {
    applicationId: number = 0;
    applicationName: string = '';
    environments: IEnvironment[] = [];
    selectedResetEnvironment: number = 0;

    mounted() {
        this.load();
    }

    @Watch('$route.params.applicationId')
    onApplicationChanged(value: string, oldValue: string) {
        this.applicationId = parseInt(value);
        this.load();
    }

    resetEnvironment(environmentId: number) {
        var cmd = new ResetEnvironment();
        cmd.ApplicationId = this.applicationId;
        cmd.EnvironmentId = environmentId;
        AppRoot.Instance.apiClient.command(cmd)
            .then(result => {
                AppRoot.notify('All incidents were deleted in that environment.');
            });
    }


    private load() {
        var appIdStr = this.$route.params.applicationId;
        this.applicationId = parseInt(appIdStr, 10);

        AppRoot.Instance.applicationService.get(this.applicationId)
            .then(appInfo => {
                this.applicationName = appInfo.name;
            });

        var q = new GetEnvironments();
        q.ApplicationId = this.applicationId;
        AppRoot.Instance.apiClient.query<GetEnvironmentsResult>(q)
            .then(result => {
                result.Items.forEach(item => this.environments.push({ id: item.Id, name: item.Name }));
            });
    }
}
