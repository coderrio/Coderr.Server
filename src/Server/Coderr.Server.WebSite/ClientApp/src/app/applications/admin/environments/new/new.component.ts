import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from "@angular/router";

import { Environment, EnvironmentService } from "../environment.service";
import { NavMenuService } from "../../../../nav-menu/nav-menu.service";
import { ApplicationService } from "../../../application.service";

@Component({
  selector: 'app-new',
  templateUrl: './new.component.html',
  styleUrls: ['./new.component.scss']
})
export class NewComponent implements OnInit {
  ignoreErrorReports = false;
  name = "";
  private applicationId = 0;
  private sub: any;

  constructor(private service: EnvironmentService,
    private route: ActivatedRoute,
    private router: Router,
    private appService: ApplicationService,
    private menuService: NavMenuService) {

  }

  ngOnInit(): void {
    this.sub = this.route.params.subscribe(params => {
      this.applicationId = +params['applicationId'];

      this.appService.get(this.applicationId).then(x => {

        this.menuService.updateNav([
          { title: x.name, route: ['application', x.id] },
          { title: "Administration", route: ['application', x.id, 'admin'] },
          { title: "New environment", route: ['application', x.id, 'admin', 'environments/'] }
        ]);

      });

    });
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  async save(): Promise<void> {
    var env = new Environment(this.applicationId, this.name);
    env.ignoreErrorReports = this.ignoreErrorReports;

    await this.service.create(env);
    this.router.navigate(['/application', this.applicationId, 'admin']);
  }

  async cancel(): Promise<void> {
    this.router.navigate(['/application', this.applicationId, 'admin']);
  }


}
