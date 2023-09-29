import { Component, OnInit, OnDestroy } from '@angular/core';
import { ApplicationService } from "../../../application.service";
import { ActivatedRoute, Router } from "@angular/router";
import { NavMenuService } from "../../../../nav-menu/nav-menu.service";
import { EnvironmentService, Environment } from "../environment.service";

@Component({
  selector: 'app-edit',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.scss']
})
export class EditComponent implements OnInit, OnDestroy {
  environment: Environment = new Environment(-1, '');

  private id = 0;
  private applicationId = 0;
  private sub: any;

  constructor(private appService: ApplicationService,
    private service: EnvironmentService,
    private route: ActivatedRoute,
    private menuService: NavMenuService,
    private router: Router) { }

  ngOnInit(): void {
    this.sub = this.route.params.subscribe(params => {
      this.applicationId = +params['applicationId'];
      this.id = +params['id'];

      this.appService.get(this.applicationId).then(x => {

        this.menuService.updateNav([
          { title: x.name, route: ['application', x.id] },
          { title: "Administration", route: ['application', x.id, 'admin'] },
          { title: "Edit Environment", route: ['application', x.id, 'admin', 'environments/', this.id] }
        ]);

      });

      this.load();
    });
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  async save(): Promise<void> {
    console.log('storing ', this.environment);
    await this.service.update(this.environment);
    this.router.navigate(['/application', this.applicationId, 'admin']);
  }

  cancel() {
    this.router.navigate(['/application', this.applicationId, 'admin']);
  }

  private async load(): Promise<void> {
    this.environment = await this.service.get(this.applicationId, this.id);
  }
}
