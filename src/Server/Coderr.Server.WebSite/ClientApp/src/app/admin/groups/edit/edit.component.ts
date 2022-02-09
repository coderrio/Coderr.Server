import { Component, OnInit, OnDestroy } from '@angular/core';
import { ApplicationService } from "../../../applications/application.service";
import { ActivatedRoute, Router } from "@angular/router";
import { NavMenuService } from "../../../nav-menu/nav-menu.service";
import { ApplicationGroupService } from "../application-groups.service";
import { IGroup, Group } from "../group.model";
import { IApplication } from "../../../applications/application.model";

export interface IApplicationSelection {
  selected: boolean;
  name: string;
  id: number;
}

@Component({
  selector: 'app-edit',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.scss']
})
export class GroupEditComponent implements OnInit, OnDestroy {
  applications: IApplicationSelection[] = [];
  group: IGroup;
  name = '';
  disabled = false;
  private allApps: IApplication[] = [];
  private dbGroupApps: number[] = [];
  private id: number;
  private routeSub: any;
  private appSub: any;

  constructor(appService: ApplicationService,
    private service: ApplicationGroupService,
    private route: ActivatedRoute,
    private router: Router,
    private navMenuService: NavMenuService) {
    this.appSub = appService.applications.subscribe(x => {
      this.allApps = x;
      this.updateApplications();
    });
  }

  ngOnInit(): void {
    this.routeSub = this.route.params.subscribe(params => {
      this.id = +params['id'];
      this.load(this.id);
    });
  }

  ngOnDestroy(): void {
    this.routeSub.unsubscribe();
    this.appSub.unsubscribe();
  }

  save() {
    this.disabled = true;
    var group = new Group(this.group.id, this.name);
    group.applications = this.applications.filter(x => x.selected).map(x => x.id);
    this.service.update(group);
    this.router.navigate(['..'], { relativeTo: this.route });
  }

  cancel() {
    this.disabled = false;
    this.router.navigate(['../'], { relativeTo: this.route });
  }

  private async load(id: number): Promise<void> {
    this.group = await this.service.get(id);
    this.name = this.group.name;
    this.dbGroupApps = this.group.applications;
    if (this.dbGroupApps.length > 0) {
      this.updateApplications();
    }

    this.navMenuService.updateNav([
      { title: 'System Administration', route: ['admin'] },
      { title: 'Application Groups', route: ['admin/groups'] },
      { title: this.name, route: ['admin/groups/', id] }
    ]);

  }

  private updateApplications() {
    if (this.allApps.length === 0) {
      return;
    }

    var selectedApps = this.applications.filter(x => x.selected).map(x => x.id);
    if (selectedApps.length === 0) {
      selectedApps = this.dbGroupApps;
      this.dbGroupApps = [];
    }

    var ourApps = this.allApps.map<IApplicationSelection>(x => {
      var map: IApplicationSelection = {
        id: x.id,
        name: x.name,
        selected: selectedApps.includes(x.id) || x.groupIds.includes(this.id)
      };

      return map;
    });

    this.applications = ourApps;
  }
}
