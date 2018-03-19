import { AppRoot } from '../../../services/AppRoot';
import {GetApplicationTeam, GetApplicationTeamResult, GetApplicationTeamResultInvitation, GetApplicationTeamMember} from "../../../dto/Core/Applications";
import Vue from "vue";
import { Component } from "vue-property-decorator";


@Component
export default class ManageHomeComponent extends Vue {
    applicationId: number = 0;
    applicationName: string = "";

    members: GetApplicationTeamMember[] = [];
    invites: GetApplicationTeamResultInvitation[] = [];

    created() {
        var appIdStr = this.$route.params.applicationId;
        this.applicationId = parseInt(appIdStr, 10);

        AppRoot.Instance.applicationService.get(this.applicationId)
            .then(appInfo => {
                this.applicationName = appInfo.name;
            });

        var q = new GetApplicationTeam();
        q.ApplicationId = this.applicationId;
        AppRoot.Instance.apiClient.query<GetApplicationTeamResult>(q)
            .then(result => {
                for (let i = 0; i < result.Invited.length; i++) {
                    this.invites.push(result.Invited[i]);
                }
                for (let i = 0; i < result.Members.length; i++) {
                    this.members.push(result.Members[i]);
                }
                console.log(this.members);
            });
    }

    
    mounted() {
    }
    


}
