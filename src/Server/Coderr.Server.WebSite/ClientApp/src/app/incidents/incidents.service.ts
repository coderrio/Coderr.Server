import { Injectable, OnDestroy } from '@angular/core';
import { AuthorizeService } from "../../api-authorization/authorize.service";
import * as api from "../../server-api/Core/Incidents";
import { IHubEvent, ISubscriber, SignalRService } from "../services/signal-r.service";
import { ApiClient } from "../utils/HttpClient";
import { Incident, IncidentRecommendation, IncidentSummary, IncidentState } from "./incident.model";
import { IncidentLoader } from "./IncidentConverter";
import { ToastrService } from "ngx-toastr";
import SortOrder = api.SortOrder;

interface IIncidentMap {
  item: Incident | null;
  id: number;
  refreshedAt: Date;
  loadPromise: Promise<Incident>;

}

@Injectable({
  providedIn: 'root'
})
export class IncidentsService implements ISubscriber, OnDestroy {
  private cachedIncidents: IIncidentMap[] = [];
  private myIncidentsList: IncidentSummary[] = [];
  private incidentLoader: IncidentLoader;
  private initPromise: Promise<any>;

  constructor(
    private readonly apiClient: ApiClient,
    private readonly signalR: SignalRService,
    private readonly authService: AuthorizeService,
    private readonly toastrService: ToastrService) {

    this.incidentLoader = new IncidentLoader(apiClient);

    this.initPromise = new Promise((accept, reject) => {
      this.incidentLoader.findForUser(authService.user.accountId)
        .then(result => {
          this.myIncidentsList = result;
          accept(null);
        }).catch(e => {
          reject(e);
        });
    });

    signalR.subscribe(x => {
      return x.typeName === "IncidentAssigned" || x.typeName === "IncidentCreated" || x.typeName === "IncidentClosed";
    }, this);
    this.incidentLoader = new IncidentLoader(apiClient);
  }
  

  /**
   * 
   * @param incidentId
   * @param accountId -1 = logged om user
   */
  async assign(incidentId: number, accountId: number): Promise<object> {

    if (!incidentId) {
      throw new Error("incidentId must be specified");
    }

    if (accountId === -1) {
      accountId = this.authService.user.accountId;
    }

    if (!accountId) {
      throw new Error("accountId must be specified");
    }

    const cmd = new api.AssignIncident();
    cmd.incidentId = incidentId;
    cmd.assignedTo = accountId;
    await this.apiClient.command(cmd);

    var incident = await this.get(incidentId);
    this.incidentLoader.refreshIncident(incident);

    if (accountId === this.authService.user.accountId) {
      this.incidentLoader.findForUser(accountId)
        .then(x => {
          var newIncident = x.find(y => y.id === incidentId);
          if (newIncident) {
            this.myIncidentsList.push(newIncident);
          }
        });
    }

    return null;
  }

  /**
   * 
   * @param incidentId
   * @param version
   * @param reason
   * @return Number of incident followers.
   */
  async close(incidentId: number, version: string, reason: string): Promise<number> {
    var cmd = new api.CloseIncident();
    cmd.incidentId = incidentId;
    cmd.applicationVersion = version;
    cmd.solution = reason;
    await this.apiClient.command(cmd);

    const q = new api.GetIncidentForClosePage();
    q.incidentId = incidentId;
    var result = await this.apiClient.query<api.GetIncidentForClosePageResult>(q);
    //return result.SubscriberCount;
    return 1;
  }

  get myIncidents(): IncidentSummary[] {
    return this.myIncidentsList;
  }

  initialize(): Promise<any> {
    return this.initPromise;
  }

  async getRecommendations(applicationId?: number): Promise<IncidentRecommendation[]> {
    return await this.incidentLoader.getRecommendations(applicationId);
  }

  async getLatest(applicationId?: number, count: number = 3): Promise<IncidentSummary[]> {
    var q = new api.FindIncidents();
    if (applicationId > 0) {
      q.applicationIds = [applicationId];
    }
    q.sortType = SortOrder.Newest;
    q.isNew = true;
    q.pageNumber = 1;
    q.itemsPerPage = 3;


    var result: IncidentSummary[] = [];
    var reply = await this.apiClient.query<api.FindIncidentsResult>(q);
    reply.items.forEach(x => {
      var item = new IncidentSummary();
      item.applicationId = x.applicationId;
      item.applicationName = x.applicationName;
      item.createdAtUtc = new Date(x.createdAtUtc + "Z");
      item.id = x.id;
      item.lastReportReceivedAtUtc = new Date(x.lastReportReceivedAtUtc + "Z");
      item.name = x.name;
      item.reportCount = x.reportCount;
      result.push(item);
    });

    return result;
  }

  async get(incidentId: number): Promise<Incident> {
    if (!incidentId) {
      throw new Error("incidentId is required.");
    }

    let wrapper = this.cachedIncidents.find(x => x.item && x.id === incidentId);
    if (wrapper) {
      if (wrapper.item === null) {
        await wrapper.loadPromise;
      }

      //TODO: Check refreshTime
      return wrapper.item;
    }

    wrapper = {
      id: incidentId,
      item: null,
      loadPromise: null,
      refreshedAt: new Date()
    };
    this.cachedIncidents.push(wrapper);

    const incident = new Incident();
    incident.id = incidentId;
    wrapper.loadPromise = this.incidentLoader.refreshIncident(incident, () => {
      wrapper.item = incident;
    });

    //TODO: Push it to our list.
    //if (incident.assignedToId === this.authService.user.accountId) {
    //  this.myIncidentsList.push(incident);
    //}

    return await wrapper.loadPromise;
  }

  ngOnDestroy(): void {
    this.signalR.unsubscribe(this);
  }

  handle(event: IHubEvent) {
    this.handleAsync(event);
  }

  private async handleAsync(event: IHubEvent): Promise<any> {
    switch (event.typeName) {
      case api.IncidentAssigned.TYPE_NAME:
        const incidentAssigned = <api.IncidentAssigned>event.body;
        const incident = await this.findIncident(incidentAssigned.incidentId);
        if (incident) {
          incident.assignedAtUtc = new Date();
          incident.assignedToId = incidentAssigned.assignedToId;
          incident.isAssigned = true;
          incident.assignedTo = "apa"; //TODO: Lookup

        } else if (incidentAssigned.assignedToId === this.authService.user.accountId) {
          this.get(incidentAssigned.incidentId);
        }

      case api.IncidentIgnored.TYPE_NAME:
        const incidentIgnored = <api.IncidentIgnored>event.body;
        this.cachedIncidents = this.cachedIncidents.filter(x => x.id !== incidentIgnored.incidentId);
        break;

      case api.IncidentCreated.TYPE_NAME:
        const incidentCreated = <api.IncidentCreated>event.body;
        this.get(incidentCreated.incidentId);
        const link = `/application/${incidentCreated.applicationId}/error/${incidentCreated.incidentId}`;
        this.toastrService.info(`A new error '<a href="${link}">${incidentCreated.incidentName}</a> was received.`, "New error", { enableHtml: true });

      default:
        break;
    }
  }


  private async findIncident(incidentId: number): Promise<Incident> {
    var incident = this.cachedIncidents.find(x => x.id === incidentId);
    if (!incident) {
      return null;
    }

    await incident.loadPromise;
    return incident.item;
  }


  ignore(incidentId: number) {
    var cmd = new api.IgnoreIncident();
    cmd.incidentId = incidentId;
    this.apiClient.command(cmd);
  }
}
