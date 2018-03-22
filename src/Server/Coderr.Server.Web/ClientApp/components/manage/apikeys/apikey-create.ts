import { AppRoot } from "../../../services/AppRoot";
import { ApplicationSummary } from "../../../services/applications/ApplicationService";
import { CreateApiKey } from "../../../dto/Core/ApiKeys";
import Vue from "vue";
import { Component } from "vue-property-decorator";

class Guid {
    static newGuid() {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    }
}

@Component
export default class CreateApiKeyComponent extends Vue {
    apiKeyId: number = 0;

    key: string = '';
    sharedSecret: string = '';
    applications: number[] = [];
    availableApplications: ApplicationSummary[] = [];
    forApplicationName: string = '';
    accountId: number;

    created() {

        AppRoot.Instance.applicationService.list()
            .then(x => {
                this.availableApplications = x;
            });

        AppRoot.Instance.loadCurrentUser().then(x => this.accountId = x.id);
        this.key = Guid.newGuid();
        this.sharedSecret = Guid.newGuid();
    }


    mounted() {
        this.applications.push(parseInt(this.$route.params.applicationId, 10));
    }

    saveKey() {
        var cmd = new CreateApiKey();
        cmd.ApplicationName = this.forApplicationName;
        cmd.ApplicationIds = this.applications;
        cmd.AccountId = this.accountId;
        cmd.ApiKey = this.key;
        cmd.SharedSecret = this.sharedSecret;
        AppRoot.Instance.apiClient.command(cmd)
            .then(x => {
                AppRoot.notify('Key is being created..');
                this.$router.push({
                    name: 'manageApiKeys',
                    params: { applicationId: this.$route.params.applicationId }
                });
            });
    }
}
