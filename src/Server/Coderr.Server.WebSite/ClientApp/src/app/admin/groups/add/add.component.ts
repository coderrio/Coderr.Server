import { Component, OnInit, OnDestroy, Output, EventEmitter } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ApplicationService } from "../../../applications/application.service";
import { ApplicationGroupService } from "../application-groups.service";
import { NavMenuService } from "../../../nav-menu/nav-menu.service";

@Component({
  selector: 'app-group-add',
  templateUrl: './add.component.html',
  styleUrls: ['./add.component.scss']
})
export class GroupAddComponent implements OnInit, OnDestroy {
  createForm = this.formBuilder.group({
    name: '',
  });
  errorMessage = '';
  returnUrl = '';
  disabled = false;
  @Output() closed = new EventEmitter();

  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private service: ApplicationGroupService,
    navMenuService: NavMenuService) {

    // can be included in other pages.
    if (this.router.url.includes('/groups/new')) {
      navMenuService.updateNav([
        { title: 'System Administration', route: ['/admin'] },
        { title: 'Application Groups', route: ['/admin/groups'] },
        { title: 'New', route: ['/admin/groups/new'] }
      ]);

    }
  }

  ngOnInit() {
  }

  ngOnDestroy() {
  }

  cancel() {
    this.closed.emit({ success: false, message: "Canceled" });
  }

  onSubmit(): void {
    this.disabled = true;
    this.service.create(this.createForm.value.name)
      .then(x => {
        this.createForm.reset();
        this.closed.emit({ success: true, group: x });
      }).catch(e => {
        this.closed.emit({ success: false, error: e });
        this.errorMessage = e.message.replace(/\n/g, "<br>\n");
      });
  }
}
