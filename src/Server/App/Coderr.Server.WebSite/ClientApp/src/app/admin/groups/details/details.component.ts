import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApplicationGroupService } from "../application-groups.service";
import { IGroup, Group } from "../group.model";
import { NavMenuService } from "../../../nav-menu/nav-menu.service";
import { ApplicationService } from "../../../applications/application.service";
import { IApplicationListItem } from "../../whitelist/whitelist.model";
import { ModalService } from "../../../_controls/modal/modal.service";

@Component({
  selector: 'group-details',
  templateUrl: './details.component.html',
  styleUrls: ['./details.component.scss']
})
export class GroupDetailsComponent implements OnInit, OnDestroy {
  group: IGroup = new Group(0, '');
  apps: IApplicationListItem[] = [];
  id: number;
  private routeSub: any;

  constructor(
    private groupService: ApplicationGroupService,
    private appService: ApplicationService,
    private modalService: ModalService,
    private route: ActivatedRoute,
    private router: Router,
    private navMenuService: NavMenuService) {
  }

  ngOnInit(): void {
    this.routeSub = this.route.params.subscribe(params => {
      this.id = +params['id'];
      this.load(this.id);
    });
  }

  ngOnDestroy(): void {
    this.routeSub.unsubscribe();
  }

  verifyRemove() {
    this.modalService.open('verifyRemoveModal');
  }

  async removeEntry(): Promise<void> {
    await this.groupService.remove(this.id);
    this.modalService.close('verifyRemoveModal');
    this.router.navigate(['../'], { relativeTo: this.route });
  }

  cancelRemove() {
    this.modalService.close('verifyRemoveModal');
  }

  private async load(id: number): Promise<void> {
    this.group = await this.groupService.get(id);
    var apps = await this.appService.list();
    this.apps = this.group.applications.map(x => {
      return {
        id: x,
        name: apps.find(y => y.id === x).name
      }
    });

    this.navMenuService.updateNav([
      { title: 'System Administration', route: ['admin'] },
      { title: 'Application Groups', route: ['admin/groups'] },
      { title: this.group.name, route: ['admin/groups/', id] }
    ]);

  }
}
