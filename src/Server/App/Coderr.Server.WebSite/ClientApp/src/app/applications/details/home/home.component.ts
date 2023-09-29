import { Component, OnInit, OnDestroy } from '@angular/core';
import { ApplicationService } from "../../application.service";
import { ActivatedRoute } from "@angular/router";
import { IApplication, EmptyApplication } from "../../application.model";
import { NavMenuService } from "../../../nav-menu/nav-menu.service";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class ApplicationHomeComponent implements OnInit, OnDestroy {
  app: IApplication = new EmptyApplication();
  applicationId: number = 0;
  sub: any;

  constructor(
    private readonly appService: ApplicationService,
    private readonly navService: NavMenuService,
    private readonly route: ActivatedRoute) {

  }

  ngOnInit(): void {
    this.sub = this.route.parent.params.subscribe(params => {
      this.applicationId = +params['applicationId'];
      this.appService.get(this.applicationId)
        .then(x => {
          this.app = x;
          this.navService.updateNav([{
            title: x.name,
            route: ['application', x.id]
          }]);
        });
    });

  }
  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
