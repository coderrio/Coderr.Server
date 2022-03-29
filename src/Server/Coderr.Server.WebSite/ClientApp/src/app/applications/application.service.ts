import { Injectable, OnDestroy } from '@angular/core';
import { BehaviorSubject, Subject } from 'rxjs';
import * as model from "./application.model";
import * as api from "../../server-api/Core/Applications";
import * as inviteApi from "../../server-api/Core/Invitations";
import * as incidentApi from "../../server-api/Core/Incidents";
import { AuthorizeService, IUser } from "../../api-authorization/authorize.service";
import { BehaviorSubjectList } from "../utils/SubjectList";
import { ApiClient } from "../utils/HttpClient";
import { SignalRService, ISubscriber, IHubEvent } from "../services/signal-r.service";
import { ApplicationGroupService } from "../admin/groups/application-groups.service";
import { PromiseWrapper } from "../PromiseWrapper";
import Roles = model.Roles;

@Injectable({
  providedIn: 'root'
})
export class ApplicationService implements ISubscriber, OnDestroy {
  private apps = new BehaviorSubjectList<model.IApplication>((a, b) => a.name.localeCompare(b.name));
  selected: BehaviorSubject<model.IApplication | null>;
  updated: BehaviorSubject<model.IApplication>;
  private timers: any[] = [];
  private destroyed = false;
  private selectedId: number = -1;
  private loadPromise: PromiseWrapper<void> = new PromiseWrapper<void>();
  private userSub: any;
  private isLoadingApps = false;
  private isLoggedIn = false;

  constructor(
    private client: ApiClient,
    private authService: AuthorizeService,
    private groupService: ApplicationGroupService,
    private signalHub: SignalRService) {

    this.userSub = this.authService.userEvents.subscribe(user => this.onAuthenticated(user));

    this.selected = new BehaviorSubject<model.IApplication>(null);

    signalHub.subscribe(x => {
      return x.typeName === "IncidentCreated" || x.typeName === "IncidentClosed" || x.typeName === "IncidentIgnored";
    }, this);


  }

  get applications(): Subject<model.IApplication[]> {
    return this.apps.subject;
  }


  async create(name: string, groupId?: number, appKey?: string): Promise<model.IApplication> {
    var existingApp = this.apps.current.find(x => x.name === name);
    if (existingApp) {
      return existingApp;
    }

    var cmd = new api.CreateApplication();
    cmd.name = name;
    if (appKey) {
      cmd.applicationKey = appKey;
    } else {
      cmd.applicationKey = model.Guid.newGuid().replace(/\-/g, '');
    }
    if (groupId) {
      cmd.groupId = groupId;
    }
    await this.client.command(cmd);

    var event = await this.signalHub.wait(x =>
      x.typeName === "ApplicationCreated" &&
      x.body.createdById === this.authService.user.accountId);

    // Double check so it wasn't added by another thread
    existingApp = this.apps.current.find(x => x.name === name);
    if (existingApp) {
      return existingApp;
    }

    var members: model.IApplicationMember[] = [
      {
        accountId: this.authService.user.accountId,
        userName: this.authService.user.userName,
        isAdmin: true,
        isInvited: false
      }
    ];

    var groups = cmd.groupId ? [cmd.groupId] : [];
    var app = new Application(event.body.applicationId, cmd.name, groups, true, members);

    this.apps.add(app);
    return app;
  }

  async selectApplication(applicationId: number) {
    if (applicationId == null || applicationId < 0)
      throw new Error("Must supply an application id");
    if (this.selectedId === applicationId) {
      return;
    }

    if (applicationId === 0) {
      this.selected.next(null);
    } else {
      const app = await this.get(applicationId);
      this.selected.next(app);
    }
  }

  async get(applicationId: number): Promise<model.IApplication> {
    if (!applicationId) {
      throw new Error("ApplicationId must be specified.");
    }
    if (!this.isLoggedIn) {
      return null;
    }

    // guard against strings :/
    applicationId = +applicationId;

    await this.loadPromise;

    let existingApp = this.apps.find(x => x.id === applicationId);
    if (existingApp != null && existingApp.latestIncidentDate != null) {
      return existingApp;
    }

    const query = new api.GetApplicationInfo();
    query.applicationId = applicationId;
    const result = await this.client.query<api.GetApplicationInfoResult>(query);

    let loadedApp: Application;
    if (existingApp == null) {
      var groups = await this.groupService.getGroupsForApplication(applicationId);
      loadedApp = new Application(result.id, result.name, groups, false);
    } else {
      loadedApp = <Application>existingApp;
    }

    loadedApp.sharedSecret = result.sharedSecret;
    loadedApp.appKey = result.appKey;

    loadedApp.totalIncidentCount = result.totalIncidentCount;
    loadedApp.latestIncidentDate = result.lastIncidentAtUtc;
    loadedApp.versions = result.versions;

    if (existingApp == null) {
      existingApp = this.apps.find(x => x.id === applicationId);
      if (existingApp) {
        return existingApp;
      }

      this.apps.add(loadedApp);
    }

    return loadedApp;
  }

  async makeAdmin(applicationId: number, userId: number): Promise<void> {
    var cmd = new api.UpdateRoles();
    cmd.userToUpdate = userId;
    cmd.roles = [Roles.Admin, Roles.Member];
    cmd.applicationId = applicationId;
    await this.client.command(cmd);
  }

  async removeAdmin(applicationId: number, userId: number): Promise<void> {
    var cmd = new api.UpdateRoles();
    cmd.userToUpdate = userId;
    cmd.roles = [Roles.Member];
    cmd.applicationId = applicationId;
    await this.client.command(cmd);
  }

  async getMembers(applicationId: number): Promise<model.IApplicationMember[]> {
    if (!applicationId) {
      throw new Error("ApplicationId must be specified.");
    }

    const query = new api.GetApplicationTeam();
    query.applicationId = applicationId;
    const result = await this.client.query<api.GetApplicationTeamResult>(query);
    var members = result.members.map<model.IApplicationMember>(x => {
      return {
        accountId: x.userId,
        userName: x.userName,
        isAdmin: x.isAdmin,
        isInvited: false
      }
    });

    result.invited.forEach(x => {
      members.push({
        accountId: -1,
        isAdmin: false,
        isInvited: true,
        userName: x.emailAddress
      });
    });

    return members;
  }

  async inviteUser(applicationId: number, email: string, message?: string): Promise<void> {
    if (!applicationId) {
      throw new Error("ApplicationId must be specified.");
    }

    if (!email) {
      throw new Error("email must be specified.");
    }

    var cmd = new inviteApi.InviteUser();
    cmd.applicationId = applicationId;
    cmd.emailAddress = email;
    cmd.text = message;
    await this.client.command(cmd);
  }

  async addMember(applicationId: number, accountId: number, isAdmin: boolean): Promise<void> {
    if (!applicationId) {
      throw new Error("ApplicationId must be specified.");
    }

    if (!accountId) {
      throw new Error("accountId must be specified.");
    }

    var cmd = new api.AddTeamMember();
    cmd.applicationId = applicationId;
    cmd.userToAdd = accountId;
    await this.client.command(cmd);
  }


  async removeMember(applicationId: number, accountId: number): Promise<void> {
    if (!applicationId) {
      throw new Error("ApplicationId must be specified.");
    }

    if (!accountId) {
      throw new Error("accountId must be specified.");
    }

    var cmd = new api.RemoveTeamMember();
    cmd.applicationId = applicationId;
    cmd.userToRemove = accountId;
    await this.client.command(cmd);
  }


  async list(): Promise<model.IApplication[]> {
    if (!this.isLoggedIn) {
      return [];
    }

    await this.loadPromise.promise;
    return this.apps.current;
  }

  ngOnDestroy() {
    this.timers.forEach(timer => {
      clearInterval(timer);
    });
    this.signalHub.unsubscribe(this);
    this.userSub.unsubscribe(this);
  }

  handle(event: IHubEvent) {
    this.handleAsync(event);
  }

  private async handleAsync(event: IHubEvent): Promise<any> {
    switch (event.typeName) {
      case incidentApi.IncidentCreated.TYPE_NAME:
        const incidentCreated = <incidentApi.IncidentCreated>event.body;
        var app1 = this.apps.find(x => x.id === incidentCreated.applicationId);
        if (app1 != null) {
          (<Application>app1).totalIncidentCount++;
        }
        break;

      case incidentApi.IncidentClosed.TYPE_NAME:
        const incidentClosed = <incidentApi.IncidentClosed>event.body;
        var app2 = this.apps.find(x => x.id === incidentClosed.applicationId);
        if (app2 != null) {
          (<Application>app2).totalIncidentCount--;
        }
        break;

      case incidentApi.IncidentIgnored.TYPE_NAME:
        const incidentIgnored = <incidentApi.IncidentIgnored>event.body;
        var app3 = this.apps.find(x => x.id === incidentIgnored.applicationId);
        if (app3 != null) {
          (<Application>app3).totalIncidentCount--;
        }
        break;
    }

  }

  // Should only be invoked when the user is published.
  private async loadApplicationsOnAuth(): Promise<any> {
    try {

      const query = new api.GetApplicationList();
      const result = await this.client.query<api.ApplicationListItem[]>(query);

      this.apps.clear();

      var apps: Application[] = [];

      var groups = await this.groupService.list();

      result.forEach(dto => {
        var appGroups = groups.filter(x => x.applications.includes(dto.id)).map(x => x.id);
        const app = new Application(dto.id, dto.name, appGroups, dto.isAdmin);
        apps.push(app);
      });
      this.apps.addAll(apps);

      // Have access to one or more applications.
      // Have zero applications when registering manually (instead of invites).
      if (result.length > 0) {
        this.selectApplication(result[0].id);
      }


      this.loadPromise.accept(null);
      this.isLoadingApps = false;
      return this.apps;

    } catch (error) {
      this.loadPromise.reject(error);
      this.isLoadingApps = false;
      return null;
    }
  }


  private onAuthenticated(user: IUser) {
    if (!user || this.isLoadingApps) {
      if (!user) {
        this.isLoggedIn = false;
        this.loadPromise = new PromiseWrapper<void>();
      }
      return;
    }

    this.isLoggedIn = true;
    this.isLoadingApps = true;
    this.loadApplicationsOnAuth();
  };
}



class Application implements model.IApplication {
  private _members: model.IApplicationMember[];
  private _name: string;
  private _groupIds = [];
  private _totalIncidentCount = 0;
  private _latestIncidentDate?: Date = null;
  private _versions: string[] = [];
  private _sharedSecret = '';
  private _appKey = '';

  constructor(private _id: number, name: string, groupIds: number[], isAdmin?: boolean, members?: model.IApplicationMember[]) {
    if (!_id) {
      throw new Error("Id must be defined");
    }

    if (!name) {
      throw new Error("name must be defined");
    }

    this._name = name;
    if (members) {
      this._members = members;
    } else {
      this._members = [];
    }
    this._groupIds = groupIds;
  }

  get groupIds(): number[] {
    return this._groupIds;
  }
  get id(): number {
    return this._id;
  }
  get name(): string {
    return this._name;
  }
  get sharedSecret(): string {
    return this._sharedSecret;
  }
  set sharedSecret(value: string) {
    this._sharedSecret = value;
  }
  get appKey(): string {
    return this._appKey;
  }
  set appKey(value: string) {
    this._appKey = value;
  }
  get members(): model.IApplicationMember[] {
    return this._members;
  }
  get totalIncidentCount(): number {
    return this._totalIncidentCount;
  }
  set totalIncidentCount(count: number) {
    this._totalIncidentCount = count;
  }
  get latestIncidentDate(): Date {
    return this._latestIncidentDate;
  }
  set latestIncidentDate(count: Date) {
    this._latestIncidentDate = count;
  }
  get versions(): string[] {
    return this._versions;
  }
  set versions(values: string[]) {
    this._versions = values;
  }


  addMember(newMember: model.IApplicationMember) {
    this._members.push(newMember);
  }

}
