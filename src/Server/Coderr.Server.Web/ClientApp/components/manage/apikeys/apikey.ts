import { AppRoot } from "../../../services/AppRoot";
import {
    GetApiKey, GetApiKeyResult, GetApiKeyResultApplication,
} from "../../../dto/Core/ApiKeys";
import { GetApplicationTeam, GetApplicationTeamResult, GetApplicationTeamResultInvitation, GetApplicationTeamMember } from "../../../dto/Core/Applications";
import Vue from "vue";
import { Component } from "vue-property-decorator";


@Component
export default class ManageApiKeyComponent extends Vue {
    apiKeyId: number = 0;
    applicationId: number = 0;

    key: string = '';
    sharedSecret: string = '';
    
    applications: GetApiKeyResultApplication[] = [];
    forApplicationName: string = 'n/a';

    created() {
        this.applicationId = parseInt(this.$route.params.applicationId, 10);

        var q = new GetApiKey();
        var value = this.$route.params.apiKey;
        if (isNaN(<any>value)) {
            q.ApiKey = value;
        } else {
            q.Id = parseInt(value, 10);
        }

        AppRoot.Instance.apiClient.query<GetApiKeyResult>(q)
            .then(x => {
                this.apiKeyId = x.Id;
                
                this.applications = x.AllowedApplications;
                this.forApplicationName = x.ApplicationName;

                this.key = x.GeneratedKey;
                this.sharedSecret = x.SharedSecret;
            });

    }


    mounted() {
    }



}
