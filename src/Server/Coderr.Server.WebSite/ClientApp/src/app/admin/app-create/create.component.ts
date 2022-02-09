import { Component, OnInit, OnDestroy, Output, EventEmitter } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { ApplicationService } from "../../applications/application.service";
import { IGroupListItem } from "../groups/group.model";
import { ApplicationGroupService } from "../groups/application-groups.service";

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.scss']
})
export class CreateApplicationComponent implements OnInit, OnDestroy {
  createForm = this.formBuilder.group({
    name: '',
    groupId: 0,
    trackStats: false
  });
  errorMessage = '';
  returnUrl = '';
  groups: IGroupListItem[] = [];
  showGroups = false;
  disabled=false;
  @Output() closed = new EventEmitter();

  private sub: any;

  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    groupService: ApplicationGroupService,
    private applicationService: ApplicationService) {
    this.sub = groupService.groups.subscribe(groups => {
      this.groups = groups;
      this.showGroups = this.groups.length > 0;
    });

  }

  ngOnInit() {
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }

  cancel() {
    this.closed.emit({ success: false, message: "Canceled" });
  }

  onSubmit(): void {
    var groupId: number | null = null;
    if (this.createForm.value.groupId !== "") {
      groupId = parseInt(this.createForm.value.groupId, 10);
    }

    this.disabled = true;
    this.applicationService.create(this.createForm.value.name, groupId, null)
      .then(x => {
        this.createForm.reset();
        this.closed.emit({ success: true, application: x });
        this.router.navigate(['/']);
      }).catch(e => {
        this.closed.emit({ success: false, error: e });
        console.log('got error ', e, e.description);
        this.errorMessage = e.message.replace(/\n/g, "<br>\n");
      });

  }
}
