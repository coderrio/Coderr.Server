import { Component, OnInit } from '@angular/core';
import { EmailAddress } from "../../../../server-api/Core/Messaging";
import {
  GetApplicationTeamResultInvitation, UpdateRoles, RemoveTeamMember, GetApplicationTeam, GetApplicationTeamResult
} from "../../../../server-api/Core/Applications";
import { DeleteInvitation, InviteUser } from "../../../../server-api/Core/Invitations";
import * as Ad from "../../../../server-api/Premise/ActiveDirectory";
import { ApiClient } from "../../../utils/HttpClient";
import { ToastrService } from "ngx-toastr";
import { ApplicationService } from "../../../applications/application.service";

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

@Component({
  selector: 'app-add',
  templateUrl: './add.component.html',
  styleUrls: ['./add.component.scss']
})
export default class ManageHomeComponent implements OnInit {
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

  constructor(private apiClient: ApiClient,
    private toastrService: ToastrService,
    private applicationService: ApplicationService) {

  }

  ngOnInit() {
    this.members = [];
    this.admins = [];
    this.invites = [];
    this.load();
    this.load();
  }

  removeAdmin(userId: number) {
    for (var i = 0; i < this.admins.length; i++) {
      if (this.admins[i].id === userId.toString()) {
        this.members.push(this.admins[i]);
        this.admins.splice(i, 1);

        var cmd = new UpdateRoles();
        cmd.userToUpdate = userId;
        cmd.roles = ["Member"];
        cmd.applicationId = this.applicationId;
        this.apiClient.command(cmd);
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
    this.apiClient.query<Ad.SearchAdResult>(q)
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
      this.toastrService.info('Group have been added to team.');
    } else {
      cmd = new Ad.AddAdUserToTeam();
      cmd.ApplicationId = this.applicationId;
      cmd.Sid = item.Sid;
      this.toastrService.info('User have been added to team.');
    }
    this.apiClient.command(cmd);
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
          this.apiClient.command(cmd2);
          return;
        }

        var cmd = new UpdateRoles();
        cmd.userToUpdate = parseInt(userId, 10);
        cmd.roles = ["Admin", "Member"];
        cmd.applicationId = this.applicationId;
        this.apiClient.command(cmd);
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
          cmd.userToRemove = parseInt(userId, 10);
          cmd.applicationId = this.applicationId;
          this.apiClient.command(cmd);
        } else {
          const cmd = new Ad.RemoveAdTeamMember();
          cmd.Sid = userId;
          cmd.ApplicationId = this.applicationId;
          this.apiClient.command(cmd);
        }
      }
    }
  }

  removeInvitation(emailAddress: string) {
    const cmd = new DeleteInvitation();
    cmd.applicationId = this.applicationId;
    cmd.invitedEmailAddress = emailAddress;
    this.apiClient.command(cmd);

    for (let i = 0; i < this.invites.length; i++) {
      if (this.invites[i].emailAddress === emailAddress) {
        this.invites.splice(i, 1);
      }
    }
  }

  inviteUser(evt: Event) {
    var cmd = new InviteUser();
    cmd.applicationId = this.applicationId;
    cmd.emailAddress = this.inviteEmail;
    this.apiClient.command(cmd);
    this.toastrService.info('Invitation have been sent.');
    var form = <HTMLFormElement>evt.target;
    form.reset();
  }


  private load() {
    var appIdStr = this.$route.params.applicationId;
    this.applicationId = parseInt(appIdStr, 10);

    this.applicationService.get(this.applicationId)
      .then(appInfo => {
        this.applicationName = appInfo.name;
      });

    var q = new GetApplicationTeam();
    q.applicationId = this.applicationId;
    this.apiClient.query<GetApplicationTeamResult>(q)
      .then(result => {
        for (let i = 0; i < result.invited.length; i++) {
          this.invites.push(result.invited[i]);
        }
        for (let i = 0; i < result.members.length; i++) {
          let member = result.members[i];
          if (member.isAdmin) {
            this.admins.push({ name: member.userName, id: member.userId.toString(), type: "user" });
          } else {
            this.members.push({ name: member.userName, id: member.userId.toString(), type: "user" });
          }

        }
      });

    var q2 = new Ad.GetAdTeamMembers();
    q2.ApplicationId = this.applicationId;
    this.apiClient.query<Ad.GetAdTeamMembersResult>(q2)
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
