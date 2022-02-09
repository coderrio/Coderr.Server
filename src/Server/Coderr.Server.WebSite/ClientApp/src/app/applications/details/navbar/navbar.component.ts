import { Component, OnInit, Input } from '@angular/core';
import { AccountService, User } from "../../../accounts/account.service";
import { EmptyApplication, IApplication } from "../../application.model";
import { ApplicationService } from "../../application.service";
import { SignalRService, ISubscriber, IHubEvent } from "../../../services/signal-r.service";
import * as api from "../../../../server-api/Core/Incidents";

@Component({
  selector: 'application-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements ISubscriber, OnInit {
  application: IApplication = new EmptyApplication();
  myApplicationId: number;
  users: User[] = [];
  incidentCount: number;
  latestIncidentDate: Date;

  constructor(
    private applicationService: ApplicationService,
    private accountService: AccountService,
    signalR: SignalRService) {
    //signalR.subscribe(x => {
    //  return x.typeName === "IncidentCreated" || x.typeName === "IncidentClosed";
    //}, this);
  }

  ngOnInit(): void {
  }

  handle(event: IHubEvent) {
    switch (event.typeName) {
      case api.IncidentClosed.TYPE_NAME:
        this.incidentCount--;
        break;

      case api.IncidentCreated.TYPE_NAME:
        var info = <api.IncidentCreated>event.body;
        this.incidentCount++;
        this.latestIncidentDate = new Date(Date.parse(info.createdAtUtc));
        break;
    }
  }

  @Input()
  get applicationId(): number { return this.myApplicationId; }
  set applicationId(applicationId: number) {
    this.myApplicationId = applicationId;
    if (this.myApplicationId > 0) {
      this.loadEverything();
    }
  }


  private async loadEverything() {
    this.application = await this.applicationService.get(this.myApplicationId);
    this.users = await this.accountService.getAll();
  }

}
