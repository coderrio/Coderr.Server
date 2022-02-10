import { Component, OnInit, OnDestroy } from '@angular/core';
import { IGroupListItem } from "../group.model";
import { ApplicationGroupService } from "../application-groups.service";
import { NavMenuService } from "../../../nav-menu/nav-menu.service";
import { ModalService } from "../../../_controls/modal/modal.service";
import { IGroupCreated } from "../add/add.component";

@Component({
  selector: 'app-groups',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class GroupListComponent implements OnInit, OnDestroy {
  groups: IGroupListItem[];
  private sub: any;

  constructor(
    private groupService: ApplicationGroupService,
    private modalService: ModalService,
    navMenuService: NavMenuService) {

    navMenuService.updateNav([
      { title: 'System Administration', route: ['/admin'] },
      { title: 'Application Groups', route: ['/admin/groups'] }
    ]);
  }

  ngOnInit(): void {
    this.sub = this.groupService.groups.subscribe(groups => {
      this.groups = groups;
    });
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  createGroup() {
    this.modalService.open('createNewGroupModal');
  }

  onGroupCreated(e: IGroupCreated) {
    console.log(e);
    if (e.success) {
      this.groups.push(e.group);
    }
    
    this.modalService.close('createNewGroupModal');
  }


}
