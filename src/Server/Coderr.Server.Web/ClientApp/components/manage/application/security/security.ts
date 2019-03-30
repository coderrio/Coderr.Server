import { AppRoot } from '../../../../services/AppRoot';
import {
    GetApplicationTeam, GetApplicationTeamResult, GetApplicationTeamResultInvitation,
    RemoveTeamMember, UpdateRoles
} from "../../../../dto/Core/Applications";
import { InviteUser, DeleteInvitation } from "../../../../dto/Core/Invitations";
import Vue from "vue";
import { Component, Watch } from "vue-property-decorator";

interface IUser {
    name: string;
    id: number;
}

@Component
export default class ManageHomeComponent extends Vue {
    applicationId: number = 0;
    applicationName: string = "";

    members: IUser[] = [];
    admins: IUser[] = [];
    invites: GetApplicationTeamResultInvitation[] = [];

    inviteEmail = '';

    mounted() {
        this.load();
    }

    @Watch('$route.params.applicationId')
    onApplicationChanged(value: string, oldValue: string) {
        this.members = [];
        this.admins = [];
        this.invites = [];
        this.load();
    }

    removeAdmin(userId: number) {
        for (var i = 0; i < this.admins.length; i++) {
            if (this.admins[i].id === userId) {
                this.members.push(this.admins[i]);
                this.admins.splice(i, 1);

                var cmd = new UpdateRoles();
                cmd.UserToUpdate = userId;
                cmd.Roles = ["Member"];
                cmd.ApplicationId = this.applicationId;
                AppRoot.Instance.apiClient.command(cmd);
            }
        }
    }



    addAdmin(userId: number) {
        for (var i = 0; i < this.members.length; i++) {
            if (this.members[i].id === userId) {
                this.admins.push(this.members[i]);
                this.members.splice(i, 1);

                var cmd = new UpdateRoles();
                cmd.UserToUpdate = userId;
                cmd.Roles = ["Admin", "Member"];
                cmd.ApplicationId = this.applicationId;
                AppRoot.Instance.apiClient.command(cmd);
            }
        }
    }

    removeMember(userId: number) {
        for (var i = 0; i < this.members.length; i++) {
            if (this.members[i].id === userId) {
                this.members.splice(i, 1);

                var cmd = new RemoveTeamMember();
                cmd.UserToRemove = userId;
                cmd.ApplicationId = this.applicationId;
                AppRoot.Instance.apiClient.command(cmd);
            }
        }
    }

    removeInvitation(emailAddress: string) {
        var cmd = new DeleteInvitation();
        cmd.ApplicationId = this.applicationId;
        cmd.InvitedEmailAddress = emailAddress;
        AppRoot.Instance.apiClient.command(cmd);

        for (var i = 0; i < this.invites.length; i++) {
            if (this.invites[i].EmailAddress === emailAddress) {
                this.invites.splice(i, 1);
            }
        }
    }

    inviteUser() {
        var cmd = new InviteUser();
        cmd.ApplicationId = this.applicationId;
        cmd.EmailAddress = this.inviteEmail;
        AppRoot.Instance.apiClient.command(cmd);
        AppRoot.notify('Invitation have been sent.');
        var dto = new GetApplicationTeamResultInvitation();
        dto.EmailAddress = cmd.EmailAddress;
        this.invites.push(dto);
        this.inviteEmail = '';
    }


    private load() {
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
                    let member = result.Members[i];
                    if (member.IsAdmin) {
                        this.admins.push({ name: member.UserName, id: member.UserId });
                    } else {
                        this.members.push({ name: member.UserName, id: member.UserId });
                    }

                }
            });
    }

}
