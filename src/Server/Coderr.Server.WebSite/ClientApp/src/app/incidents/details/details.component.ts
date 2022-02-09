import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { IncidentsService } from "../incidents.service";
import { Incident } from "../incident.model";
import { AccountService, User } from "../../accounts/account.service";
import { NavMenuService } from "../../nav-menu/nav-menu.service";
import { ApplicationService } from "../../applications/application.service";

@Component({
  selector: 'incident-details',
  templateUrl: './details.component.html',
  styleUrls: ['./details.component.scss']
})
export class DetailsComponent implements OnInit, OnDestroy {
  incident = new Incident();
  incidentId: number;
  applicationId: number;
  users: User[] = [];


  private sub: any;

  constructor(
    private incidentService: IncidentsService,
    private route: ActivatedRoute,
    private applicationService: ApplicationService,
    private accountService: AccountService,
    private menuService: NavMenuService,
    ) {
    this.incident.description = "Loading";
    this.incident.id = 0;
  }

  ngOnInit(): void {
    this.sub = this.route.params.subscribe(params => {
      this.incidentId = +params['incidentId'];
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
    this.incident = await this.incidentService.get(this.incidentId);
    this.applicationId = this.incident.applicationId;
    const app = await this.applicationService.get(this.incident.applicationId);
    this.menuService.updateNav([
      { title: app.name, route: ['application', app.id] },
      { title: 'Errors', route: ['application', app.id, 'incidents'] }
    ]
    );

  }
}
