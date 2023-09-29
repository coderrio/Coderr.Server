import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AccountService, User } from "../../accounts/account.service";
import { ChartService } from "../../services/chart.service";
import { NavMenuService } from "../../nav-menu/nav-menu.service";
import { ApplicationService } from "../../applications/application.service";
import { IApplication } from "../application.model";

@Component({
  selector: 'app-details',
  templateUrl: './details.component.html',
  styleUrls: ['./details.component.scss']
})
export class ApplicationDetailsComponent implements OnInit, OnDestroy {
  application: IApplication;
  applicationId: number;
  users: User[] = [];


  private sub: any;

  constructor(
    private applicationService: ApplicationService,
    private route: ActivatedRoute,
    private accountService: AccountService,
    private menuService: NavMenuService,
  ) {
  }

  ngOnInit(): void {
    this.sub = this.route.params.subscribe(params => {
      this.applicationId = +params['applicationId'];
      this.loadEverything();
    });

    this.accountService.getAllButMe()
      .then(users => {
        this.users = users;
      });
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }


  private async loadEverything() {
    this.application = await this.applicationService.get(this.applicationId);
    this.menuService.updateNav([
      { title: this.application.name, route: ['application', this.applicationId] }
    ]
    );

  }
}
