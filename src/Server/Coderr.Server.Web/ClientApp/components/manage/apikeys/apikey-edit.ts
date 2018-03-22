import { AppRoot } from "../../../services/AppRoot";
import {
    GetApiKey, GetApiKeyResult, GetApiKeyResultApplication,
    EditApiKey, DeleteApiKey
} from "../../../dto/Core/ApiKeys";
import { ApplicationSummary } from "../../../services/applications/ApplicationService";
import Vue from "vue";
import { Component } from "vue-property-decorator";


@Component
export default class EditApiKeyComponent extends Vue {
    apiKeyId: number = 0;

    key: string = '';
    sharedSecret: string = '';

    applications: number[] = [];
    availableApplications: ApplicationSummary[] = [];
    forApplicationName: string = 'n/a';

    created() {
        var value = this.$route.params.apiKey;
        this.apiKeyId = parseInt(value, 10);

        var q = new GetApiKey();
        q.Id = this.apiKeyId;
        AppRoot.Instance.apiClient.query<GetApiKeyResult>(q)
            .then(x => {
                x.AllowedApplications.forEach(app => {
                    this.applications.push(app.ApplicationId);
                });
                this.forApplicationName = x.ApplicationName;

                this.key = x.GeneratedKey;
                this.sharedSecret = x.SharedSecret;
            });

        AppRoot.Instance.applicationService.list()
            .then(x => {
                this.availableApplications = x;
            });
    }


    mounted() {
    }

    isChecked(applicationId: number) {
        for (var i = 0; i < this.applications.length; i++) {
            if (this.applications[i] === applicationId)
                return true;
        }
        return false;
    }

    saveKey() {
        var cmd = new EditApiKey();
        cmd.Id = this.apiKeyId;
        cmd.ApplicationName = this.forApplicationName;
        cmd.ApplicationIds = this.applications;
        AppRoot.Instance.apiClient.command(cmd)
            .then(x => {
                AppRoot.notify('App key is scheduled for an update.');
                this.$router.push({
                    name: 'manageApiKeys',
                    params: { applicationId: this.$route.params.applicationId }
                });
            });
    }

    deleteKey() {
        AppRoot.modal({
            cancelButtonText: 'No',
            submitButtonText: 'Yes',
            htmlContent: 'Do you really want to delete the ApiKey?'
        }).then(x => {
            var cmd = new DeleteApiKey();
            cmd.Id = this.apiKeyId;
            AppRoot.Instance.apiClient.command(cmd)
                .then(y => {
                    this.$router.push({
                        name: 'manageApiKeys',
                        params: { applicationId: this.$route.params.applicationId }
                    });
                });
            AppRoot.notify('Key is scheduled for deletion.');
        });
    }
}
