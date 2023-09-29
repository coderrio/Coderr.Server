import { Component, OnDestroy } from "@angular/core";
import { Router } from "@angular/router";
import { ToastrService } from "ngx-toastr";

import { ApplicationService } from "../applications/application.service";
import { IApplication } from "../applications/application.model";
import { INavPill, NavMenuService } from "./nav-menu.service";
import { GuideService } from "../_controls/guide/guide.service";
import * as api from "../../server-api/Core/Support";
import { ModalService } from "../_controls/modal/modal.service";
import { ApiClient } from "../utils/HttpClient";
import { AuthorizeService, IUser } from "../../api-authorization/authorize.service";

interface IApplicationMenuItem {
  id: number,
  title: string;
  groupIds: number[];
  hasIncidents: boolean;
}

interface IApplicationMenuGroupItem {
  id: number;
  title: string;
}

@Component({
  selector: "app-nav-menu",
  templateUrl: "./nav-menu.component.html",
  styleUrls: ["./nav-menu.component.scss"]
})
export class NavMenuComponent implements OnDestroy {
  allApplications: IApplicationMenuItem[] = [];
  applications: IApplicationMenuItem[];
  groups: IApplicationMenuGroupItem[] = [];
  selected: IApplicationMenuItem;
  showAsOnboarding = false;
  showAppMenu = false;
  navItems: INavPill[] = [];
  gotGuides: boolean;

  supportMessage = '';
  supportSubject = '';

  isAuthenticated = false;

  private navSub: any;
  private guideSub: any;
  private accountSub: any;

  showConfigure = false;
  configureAppId: number;


  private emptyApp = { id: 0, title: "(All applications)", groupIds: [], hasIncidents: false };
  private selectedGroupId: number;

  constructor(
    navService: NavMenuService,
    private appService: ApplicationService,
    private modalService: ModalService,
    private guideService: GuideService,
    private router: Router,
    private apiClient: ApiClient,
    private authService: AuthorizeService,
    private toastrService: ToastrService) {
    appService.selected.subscribe(x => this.onApplicationChanged(x));
    appService.applications.subscribe(x => this.onApplications(x));
    this.navSub = navService.navItems.subscribe(items => this.navItems = items);
    this.guideSub = this.guideService.guidesAvailable.subscribe(x => this.onGuidesAvailable(x));
    this.accountSub = this.authService.userEvents.subscribe(x => this.onAccountChange(x));
  }

  toggleAppMenu() {
    this.showAppMenu = !this.showAppMenu;
    console.log('toggled to ', this.showAppMenu);
  }

  selectApplication(applicationId: number) {
    console.log('toggle false' + applicationId);
    this.showAppMenu = false;
    this.appService.selectApplication(applicationId);

    if (!applicationId) {
      this.configureAppId = applicationId;
    } else if (this.allApplications.length > 0) {
      this.configureAppId = this.allApplications[0].id;
    } else {
      this.configureAppId = 0;
    }

    var app = this.allApplications.find(x => x.id === this.configureAppId);
    console.log('incident count', app ? app.hasIncidents : 0);
    this.showConfigure = app && !app.hasIncidents;
  }

  selectGroup(groupId: number) {
    this.selectedGroupId = groupId;
    this.filterApplications(groupId);
  }

  showWizard() {
    this.guideService.showNextGuide();
  }

  showSupport() {
    this.modalService.open("chatModal");
  }

  sendSupport() {
    var cmd = new api.SendSupportRequest();
    cmd.message = this.supportMessage;
    cmd.subject = this.supportSubject;
    cmd.url = this.router.url;
    this.apiClient.command(cmd);
    this.modalService.close("chatModal");
    this.toastrService.success("Message has been sent");
  }

  ngOnDestroy(): void {
    this.appService.selected.unsubscribe();
    this.appService.applications.unsubscribe();
    this.navSub.unsubscribe();
    this.guideSub.unsubscribe();
  }

  private onAccountChange(user: IUser) {
    this.isAuthenticated = user != null;
  }

  private onApplicationChanged(application: IApplication) {
    console.log('Nav menu app application', application);
    if (application == null) {
      this.selected = this.emptyApp;
      return;
    }

    var menuItem = this.toMenuItem(application);
    this.showConfigure = !menuItem.hasIncidents;
    this.selected = menuItem;
  }

  private onApplications(applications: IApplication[]) {
    var newApps: IApplicationMenuItem[] = [];
    applications.forEach(app => {
      newApps.push(this.toMenuItem(app));
    });
    this.allApplications = newApps;

    if (this.allApplications.length > 0 && !this.configureAppId) {
      this.configureAppId = this.allApplications[0].id;
    }

    var app = this.allApplications.find(x => x.id === this.configureAppId);
    this.showConfigure = app && !app.hasIncidents;

    if (this.selectedGroupId > 0) {
      this.filterApplications(this.selectedGroupId);
    }
  }

  private filterApplications(groupId: number) {
    var apps: IApplicationMenuItem[];
    if (!groupId) {
      apps = this.allApplications;
    } else {
      apps = this.allApplications.filter(x => x.groupIds.includes(groupId));
    }
    this.applications = apps;

    if (apps.indexOf(this.selected) === -1 && apps.length > 0) {
      this.selectApplication(apps[0].id);
    }
  }

  private toMenuItem(app: IApplication): IApplicationMenuItem {
    console.log('converting', app.totalIncidentCount > 0, app.totalIncidentCount);
    return { id: app.id, title: app.name, groupIds: app.groupIds, hasIncidents: app.totalIncidentCount > 0 };
  }

  private onGuidesAvailable(isAvailable: boolean) {
    this.gotGuides = isAvailable;
  }

}
