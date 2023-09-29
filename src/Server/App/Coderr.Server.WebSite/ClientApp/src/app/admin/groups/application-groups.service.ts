import { Injectable, OnDestroy } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { BehaviorSubjectList } from "../../utils/SubjectList";
import * as model from "./group.model";
import { ApiClient } from "../../utils/HttpClient";
import { SignalRService } from "../../services/signal-r.service";
import { AuthorizeService, IUser } from "../../../api-authorization/authorize.service";
import * as api from "../../../server-api/Core/Applications";
import IGroup = model.IGroup;
import Group = model.Group;
import { PromiseWrapper } from "../../PromiseWrapper";

@Injectable({
  providedIn: 'root'
})
export class ApplicationGroupService implements OnDestroy {
  private grps = new BehaviorSubjectList<model.IGroupListItem>((a, b) => a.name.localeCompare(b.name));
  private allGroups: Group[] = [];
  private loadPromise: PromiseWrapper<void> = new PromiseWrapper();
  private userSub: any;

  constructor(
    private client: ApiClient,
    private authService: AuthorizeService,
    private signalHub: SignalRService
  ) {
    this.userSub = this.authService.userEvents.subscribe(x => this.onAuth(x));
  }

  ngOnDestroy(): void {
    this.userSub.unsubscribe();
  }

  get groups(): BehaviorSubject<model.IGroupListItem[]> {
    return this.grps.subject;
  }

  async get(id: number): Promise<IGroup> {
    if (!id) {
      throw new Error("Id must be specified!");
    }

    await this.loadPromise.promise;

    if (this.allGroups.length === 0) {
      await this.loadPromise.promise;
    }

    var group = this.allGroups.find(x => x.id === id);
    if (!group) {
      throw new Error("Failed to find group " + id);
    }

    return group;
  }

  async remove(groupId: number): Promise<void> {
    var cmd = new api.DeleteApplicationGroup();
    cmd.groupId = groupId;
    await this.client.command(cmd);
  }

  async list(): Promise<IGroup[]> {
    await this.loadPromise.promise;
    return this.allGroups;
  }

  async getGroupsForApplication(applicationId: number): Promise<number[]> {
    await this.loadPromise.promise;

    return this.allGroups.filter(x => x.applications.includes(applicationId)).map(x => x.id);
  }

  async create(name: string): Promise<model.IGroupListItem> {
    if (!name) {
      throw new Error("Name must be specified!");
    }

    var waitPromise = this.signalHub.wait(x =>
      x.typeName === "ApplicationGroupCreated" &&
      x.body.createdById === this.authService.user.accountId);

    var cmd = new api.CreateApplicationGroup();
    cmd.name = name;
    await this.client.command(cmd);

    var event = await waitPromise;

    var group: model.IGroupListItem = {
      id: event.body.id,
      name: name
    };
    this.grps.add(group);
    return group;
  }

  async update(group: IGroup): Promise<void> {
    if (!group) {
      throw new Error("Group must be specified!");
    }

    var cmd = new api.RenameApplicationGroup();
    cmd.newName = group.name;
    cmd.groupId = group.id;
    await this.client.command(cmd);

    var cmd2 = new api.MapApplicationsToGroup();
    cmd2.groupId = group.id;
    cmd2.applicationIds = group.applications;
    await this.client.command(cmd2);

    var entity = this.allGroups.find(x => x.id === group.id);
    entity.name = group.name;
    entity.applications = group.applications;
  }

  private async loadGroups2(): Promise<void> {
    const query = new api.GetApplicationGroups();
    const result = await this.client.query<api.GetApplicationGroupsResult>(query);
    if (!result) {
      return;
    }

    var allGroups: model.IGroupListItem[] = [];
    result.items.forEach(group => {
      allGroups.push({
        id: group.id,
        name: group.name
      });
    });

    this.allGroups = allGroups.map(x => { return { id: x.id, name: x.name, applications: [], teams: [] } });

    const query2 = new api.GetApplicationGroupMap();
    const result2 = await this.client.query<api.GetApplicationGroupMapResult>(query2);
    if (!result2) {
      return;
    }

    result2.items.forEach(map => {
      var group = this.allGroups.find(y => map.groupId === y.id);
      group.applications.push(map.applicationId);
    });

    this.grps.addAll(allGroups);
    this.loadPromise.accept(null);
  }

  private onAuth(user: IUser): void {
    if (user) {
      this.loadGroups2();
    }
  }
}
