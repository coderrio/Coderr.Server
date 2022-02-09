import { Component, OnInit, OnDestroy, Output, EventEmitter } from '@angular/core';
import { IGroupListItem } from "../../admin/groups/group.model";
import { ApplicationGroupService } from "../../admin/groups/application-groups.service";

@Component({
  selector: 'app-group-selector',
  templateUrl: './selector.component.html',
  styleUrls: ['./selector.component.scss']
})
export class GroupSelectorComponent implements OnInit, OnDestroy {
  private sub: any;
  groups: IGroupListItem[] = [];

  @Output() selected = new EventEmitter<IGroupListItem>();


  constructor(private readonly service: ApplicationGroupService) { }

  select(id: number) {
    if (id === -1) {
      this.selected.emit(null);
    }

    const groups = this.groups.filter(x => x.id === id);
    if (groups.length !== 1) {
      throw new Error("Found multiple groups: " + JSON.stringify(groups));
    }

    this.selected.emit(groups[0]);
  }

  ngOnInit(): void {
    this.sub = this.service.groups.subscribe(groups => {
      this.groups = groups;
    });
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
