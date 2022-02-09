import { AppRoot } from '../../../../services/AppRoot';
import { GetEnvironments, GetEnvironmentsResult, ResetEnvironment, CreateEnvironment, UpdateEnvironment } from "../../../../dto/Core/Environments";
import Vue from "vue";
import { Component, Watch } from "vue-property-decorator";

interface IEnvironment {
    name: string;
    id: number;
    deleteIncidents: boolean;
    toggleText: string
}

@Component
export default class ManageEnvironmentsComponent extends Vue {
    private serverEnvironments: GetEnvironmentsResult;

    applicationId: number = 0;
    applicationName: string = '';
    environments: IEnvironment[] = [];
    selectedResetEnvironment: number = 0;
    deleteIncidents: boolean;

    newName: string = '';
    newDeleteIncidents = false;

    mounted() {
        this.load();
    }

    @Watch('$route.params.applicationId')
    onApplicationChanged(value: string, oldValue: string) {
        this.applicationId = parseInt(value);
        this.load();
    }

    resetEnvironment() {
        var cmd = new ResetEnvironment();
        cmd.ApplicationId = this.applicationId;
        cmd.EnvironmentId = this.selectedResetEnvironment;
        AppRoot.Instance.apiClient.command(cmd)
            .then(result => {
                AppRoot.notify('All incidents were deleted in that environment.');
                this.load();
            });
    }

    createEnvironment() {
        var cmd = new CreateEnvironment();
        cmd.ApplicationId = this.applicationId;
        cmd.Name = this.newName;
        cmd.DeleteIncidents = this.newDeleteIncidents;
        AppRoot.Instance.apiClient.command(cmd)
            .then(result => {
                AppRoot.notify('Environment have been created.');

            });
    }

    toggleEnvironment(environment: IEnvironment) {
        console.log(environment);
        environment.deleteIncidents = !environment.deleteIncidents;
        environment.toggleText = environment.deleteIncidents ? 'Process received reports' : 'Ignore received reports';

        var cmd = new UpdateEnvironment();
        cmd.ApplicationId = this.applicationId;
        cmd.EnvironmentId = environment.id;
        cmd.DeleteIncidents = environment.deleteIncidents;
        AppRoot.Instance.apiClient.command(cmd);

        AppRoot.notify('Environment have been updated.');
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
                this.environments = [];
                this.serverEnvironments = result;
                result.Items.forEach(item => this.environments.push({
                    id: item.Id,
                    name: item.Name,
                    deleteIncidents: item.DeleteIncidents,
                    toggleText: (item.DeleteIncidents ? 'Process received reports' : 'Ignore received reports')
                }));
            });
    }
}
