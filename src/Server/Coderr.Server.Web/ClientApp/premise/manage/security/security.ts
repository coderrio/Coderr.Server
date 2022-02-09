import { AppRoot } from '../../../services/AppRoot';
import {
    GetApplicationTeam,
    GetApplicationTeamResult,
    GetApplicationTeamResultInvitation,
    RemoveTeamMember,
    UpdateRoles
} from "../../../dto/Core/Applications";
import { InviteUser, DeleteInvitation } from "../../../dto/Core/Invitations";
import * as Ad from "../../../dto/Premise/ActiveDirectory";

import Vue from "vue";
import { Component, Watch } from "vue-property-decorator";

interface IUser {
    name: string;
    id: string;
    type: string; //"user", "aduser", "adgroup"
}

interface ISearchAd {
    findGroups: boolean;
    findUsers: boolean;
    text: string;
}

@Component
export default class ManageHomeComponent extends Vue {
    applicationId: number = 0;
    applicationName: string = "";

    members: IUser[] = [];
    admins: IUser[] = [];
    invites: GetApplicationTeamResultInvitation[] = [];

    adSearch: ISearchAd = {
        text: "",
        findGroups: true,
        findUsers: true
    };
    adSearchResult: Ad.SearchAdResultItem[] = [];

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
            if (this.admins[i].id === userId.toString()) {
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

    findAd() {
        var q = new Ad.SearchAd();
        if ((<any>this).premisePlus) {
            q.FindGroups = this.adSearch.findGroups;
            q.FindUsers = this.adSearch.findUsers;
        } else {
            q.FindUsers = true;
            q.FindGroups = false;
        }
        q.Text = this.adSearch.text;
        AppRoot.Instance.apiClient.query<Ad.SearchAdResult>(q)
            .then(result => {
                this.adSearchResult = result.Items;
                result.Items.forEach(item => {

                });
            });
    }

    addAdItem(item: Ad.SearchAdResultItem) {
        var cmd: any;
        if (item.Type === "Group") {
            cmd = new Ad.AddAdGroupToTeam();
            cmd.ApplicationId = this.applicationId;
            cmd.Sid = item.Sid;
            AppRoot.notify('Group have been added to team.');
        } else {
            cmd = new Ad.AddAdUserToTeam();
            cmd.ApplicationId = this.applicationId;
            cmd.Sid = item.Sid;
            AppRoot.notify('User have been added to team.');
        }
        AppRoot.Instance.apiClient.command(cmd);
        var member: IUser = {
            id: item.Sid,
            name: item.Name,
            type: "ad" + item.Type.toLocaleLowerCase()
        };
        this.members.push(member);
    }


    addAdmin(userId: string) {
        for (var i = 0; i < this.members.length; i++) {
            var member = this.members[i];
            if (member.id === userId.toString()) {
                this.admins.push(member);
                this.members.splice(i, 1);

                //AD
                if (member.type !== "user") {
                    let cmd2 = new Ad.ChangeAdTeamRole();
                    cmd2.ApplicationId = this.applicationId;
                    cmd2.Sid = member.id;
                    cmd2.IsAdmin = true;
                    AppRoot.Instance.apiClient.command(cmd2);
                    return;
                }

                var cmd = new UpdateRoles();
                cmd.UserToUpdate = parseInt(userId, 10);
                cmd.Roles = ["Admin", "Member"];
                cmd.ApplicationId = this.applicationId;
                AppRoot.Instance.apiClient.command(cmd);
            }
        }
    }

    /**
     * 
     * @param userId User id or SID
     */
    removeMember(userId: string) {
        for (var i = 0; i < this.members.length; i++) {
            var member = this.members[i];
            if (member.id === userId.toString()) {
                this.members.splice(i, 1);

                if (member.type === "user") {
                    const cmd = new RemoveTeamMember();
                    cmd.UserToRemove = parseInt(userId, 10);
                    cmd.ApplicationId = this.applicationId;
                    AppRoot.Instance.apiClient.command(cmd);
                } else {
                    const cmd = new Ad.RemoveAdTeamMember();
                    cmd.Sid = userId;
                    cmd.ApplicationId = this.applicationId;
                    AppRoot.Instance.apiClient.command(cmd);
                }
            }
        }
    }

    removeInvitation(emailAddress: string) {
        const cmd = new DeleteInvitation();
        cmd.ApplicationId = this.applicationId;
        cmd.InvitedEmailAddress = emailAddress;
        AppRoot.Instance.apiClient.command(cmd);

        for (let i = 0; i < this.invites.length; i++) {
            if (this.invites[i].EmailAddress === emailAddress) {
                this.invites.splice(i, 1);
            }
        }
    }

    inviteUser(evt: Event) {
        var cmd = new InviteUser();
        cmd.ApplicationId = this.applicationId;
        cmd.EmailAddress = this.inviteEmail;
        AppRoot.Instance.apiClient.command(cmd);
        AppRoot.notify('Invitation have been sent.');
        var form = <HTMLFormElement>evt.target;
        form.reset();
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
                        this.admins.push({ name: member.UserName, id: member.UserId.toString(), type: "user" });
                    } else {
                        this.members.push({ name: member.UserName, id: member.UserId.toString(), type: "user" });
                    }

                }
            });

        var q2 = new Ad.GetAdTeamMembers();
        q2.ApplicationId = this.applicationId;
        AppRoot.Instance.apiClient.query<Ad.GetAdTeamMembersResult>(q2)
            .then(result => {
                for (let i = 0; i < result.Items.length; i++) {
                    let member = result.Items[i];
                    let type = member.IsGroup ? "adgroup" : "aduser";
                    if (member.IsAdmin) {
                        this.admins.push({ name: member.Name, id: member.Sid, type: type });
                    } else {
                        this.members.push({ name: member.Name, id: member.Sid, type: type });
                    }
                }
            });

    }

}