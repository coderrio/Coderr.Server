import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApplicationService } from "../../../application.service";
import { AccountService, User } from "../../../../accounts/account.service";
import { IApplicationMember } from "../../../application.model";
import { ModalService } from "../../../../_controls/modal/modal.service";
import { ToastrService } from "ngx-toastr";
import { NavMenuService } from "../../../../nav-menu/nav-menu.service";

@Component({
  selector: 'app-team',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class TeamListComponent implements OnInit {
  applicationId: number;
  members: IApplicationMember[] = [];
  users: User[] = [];

  //invitations:
  selectedAccountId: number = -1;
  inviteEmail: string = "";

  constructor(private service: ApplicationService,
    private accountService: AccountService,
    route: ActivatedRoute,
    private modalService: ModalService,
    private noticeService: ToastrService,
    private menuService: NavMenuService) {
    this.applicationId = +route.snapshot.params.applicationId;
    this.load();
  }

  ngOnInit(): void {
  }

  showAdd() {
    this.modalService.open("AddUserModel");
  }

  hideShowAdd() {
    this.modalService.close("AddUserModel");
  }

  promote(user: IApplicationMember) {
    (<any>user).isAdmin = true;
    this.service.makeAdmin(this.applicationId, user.accountId);
  }

  demote(user: IApplicationMember) {
    (<any>user).isAdmin = false;
    this.service.removeAdmin(this.applicationId, user.accountId);
  }

  async addUser(): Promise<void> {
    this.hideShowAdd();

    if (this.inviteEmail.length > 0) {
      await this.service.inviteUser(this.applicationId, this.inviteEmail);
      this.members.push({ accountId: -1, isAdmin: false, userName: this.inviteEmail, isInvited: true });
      this.inviteEmail = '';

    } else if (this.selectedAccountId > 0) {
      await this.service.addMember(this.applicationId, this.selectedAccountId, false);
      this.members.push({
        accountId: this.selectedAccountId,
        isAdmin: false,
        userName: this.users.find(x => x.id === this.selectedAccountId).userName,
        isInvited: false
      });
    }
  }

  private async load(): Promise<void> {
    this.users = await this.accountService.getAll();
    this.members = await this.service.getMembers(this.applicationId);

    var app = await this.service.get(this.applicationId);
    this.menuService.updateNav([
      { title: app.name, route: ['application', this.applicationId] },
      { title: "Administration", route: ['application', this.applicationId, 'admin'] },
      { title: "Team", route: ['application', this.applicationId, 'admin', 'team'] }
    ]);
  }
}
