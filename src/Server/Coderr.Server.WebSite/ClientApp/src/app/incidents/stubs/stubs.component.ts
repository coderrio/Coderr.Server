import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { IncidentsService } from "../incidents.service";
import { SignalRService, ISubscriber, IHubEvent } from "../../services/signal-r.service";
import * as api from "../../../server-api/Core/Incidents";

export enum ItemType {
  Mine,
  Newest,
  Recommended
}

export class IncidentStub {
  id: number;
  name: string;
  applicationName: string;
  applicationId: number;
  assignedAtUtc: Date | null;
  createdAtUtc: Date;
  reportCount: number;
  lastReportReceivedAtUtc: Date;
  motivation?: string;
}

@Component({
  selector: 'incident-stubs',
  templateUrl: './stubs.component.html',
  styleUrls: ['./stubs.component.scss']
})
export class IncidentStubsComponent implements ISubscriber, OnInit, OnDestroy {
  private _applicationId = 0;
  private _itemType = ItemType.Mine;
  private allIncidents: IncidentStub[] = [];
  private sub: any;
  incidents: IncidentStub[] = [];
  gotApplicationId = false;


  constructor(
    private readonly incidentService: IncidentsService,
    signalR: SignalRService) {
    signalR.subscribe(x => {
      return x.typeName === "IncidentAssigned" || x.typeName === "IncidentCreated" || x.typeName === "IncidentClosed";
    }, this);
  }

  ngOnInit(): void {

  }

  ngOnDestroy(): void {

  }

  handle(event: IHubEvent) {
    this.handleAsync(event);
  }

  /**
   * Should be one of them ItemType enum values.
   */
  @Input()
  get itemType(): string { return this._itemType.toString(); }
  set itemType(itemType: string) {
    this._itemType = ItemType[itemType];
    this.load(this._itemType);
  }

  @Input()
  get applicationId(): number { return this._applicationId; }
  set applicationId(applicationId: number) {
    this._applicationId = applicationId;
    this.gotApplicationId = applicationId > 0;
    if (this.gotApplicationId) {
      this.incidents = this.allIncidents.filter(x => x.applicationId === this._applicationId);
    } else {
      this.incidents = this.allIncidents;
    }
  }

  private async load(itemType: ItemType) {
    var log = false;

    switch (itemType) {

      case ItemType.Mine:
        await this.loadMine();
        break;

      case ItemType.Recommended:
        await this.loadRecommended(this._applicationId);
        break;

      default:
        await this.loadNewest(this._applicationId);
        break;
    }

    if (this.gotApplicationId) {
      this.incidents = this.allIncidents.filter(x => x.applicationId === this._applicationId);
    } else {
      this.incidents = this.allIncidents;
    }
  }

  private async loadMine() {
    await this.incidentService.initialize();
    this.allIncidents = this.incidentService.myIncidents;
  }

  private async loadRecommended(applicationId?: number) {
    var result = await this.incidentService.getRecommendations(applicationId);

    // Not supported in OSS version.
    if (!result) {
      this.allIncidents = [];
      return;
    }

    var stubs: IncidentStub[] = [];
    result.forEach(x => {
      var stub = new IncidentStub();
      stub.id = x.id;
      stub.applicationId = x.applicationId;
      stub.applicationName = x.applicationName;
      stub.createdAtUtc = x.createdAtUtc;
      stub.lastReportReceivedAtUtc = x.lastReportReceivedAtUtc;
      stub.name = x.name;
      stub.reportCount = x.reportCount;
      stub.motivation = x.motivation;
      stubs.push(stub);
    });
    this.allIncidents = stubs;
  }

  private async loadNewest(applicationId?: number) {
    this.allIncidents = await this.incidentService.getLatest(applicationId, 5);
  }



  private async handleAsync(event: IHubEvent): Promise<any> {
    switch (event.typeName) {
      case api.IncidentAssigned.TYPE_NAME:
        setTimeout(() => this.load(this._itemType), 1000);
        break;

      case api.IncidentCreated.TYPE_NAME:
        //timeout so that everything can be processed in the background before we load it.
        setTimeout(() => this.load(this._itemType), 5000);
        break;

      default:
        break;
    }
  }

}
