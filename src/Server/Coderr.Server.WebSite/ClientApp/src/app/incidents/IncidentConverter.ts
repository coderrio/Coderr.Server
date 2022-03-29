import * as model from "./incident.model";
import * as api from "../../server-api/Core/Incidents";
import * as mine from "../../server-api/Common/Mine";
import { ApiClient, HttpError } from "../utils/HttpClient";
import { AuthorizeService } from "../../api-authorization/authorize.service";

export interface IIncidentMap {
  item: model.Incident | null;
  id: number;
  refreshedAt: Date;
  loadPromise: Promise<model.Incident>;
}

export interface ISearchQuery {

}

/**
 * Loads incidents from the backend and caches them.
 * Used by all services so that they can share incidents and enjoy updates.
 */
export class IncidentLoader {
  constructor(
    private readonly apiClient: ApiClient) {

  }


  async getRecommendations(applicationId?: number): Promise<model.IncidentRecommendation[]> {
    var recommendations: model.IncidentRecommendation[] = [];


    var query = new mine.ListMyIncidents();
    if (applicationId > 0) {
      query.applicationId = applicationId;
    }

    var result = await this.apiClient.query<mine.ListMyIncidentsResult>(query);

    // not supported in OSS.
    if (!result) {
      return null;
    }

    result.suggestions.forEach(x => {
      var item = new model.IncidentRecommendation();
      item.applicationId = x.applicationId;
      item.applicationName = x.applicationName;
      item.name = x.name;
      item.id = x.id;
      item.createdAtUtc = new Date(x.createdAtUtc + "Z");
      item.lastReportReceivedAtUtc = new Date(x.lastReportAtUtc + "Z");
      item.reportCount = x.reportCount;
      item.exceptionTypeName = x.exceptionTypeName;
      item.motivation = x.motivation.replace(/\r\n/g, "; ");
      item.weight = x.weight;
      recommendations.push(item);
    });

    return recommendations.sort((a, b) => a.weight - b.weight);
  }

  async findForUser(accountId: number): Promise<model.IncidentSummary[]> {
    var foundIncidents: model.IncidentSummary[] = [];


    var query = new mine.ListMyIncidents();
    var result = await this.apiClient.query<mine.ListMyIncidentsResult>(query);
    if (!result) {
      return [];
    }

    result.items.forEach(x => {
      var item = new model.IncidentSummary();
      item.applicationId = x.applicationId;
      item.applicationName = x.applicationName;
      item.name = x.name;
      item.id = x.id;
      item.assignedAtUtc = new Date(x.assignedAtUtc + "Z");
      item.createdAtUtc = new Date(x.createdAtUtc + "Z");
      item.lastReportReceivedAtUtc = new Date(x.lastReportAtUtc + "Z");
      item.reportCount = x.reportCount;
      foundIncidents.push(item);
    });

    return foundIncidents;
  }

  async search(text?: string, applicationId?: number, collectionName?: string, propertyName?: string, propertyValue?: string,): Promise<model.IncidentSummary[]> {
    var foundIncidents: model.IncidentSummary[] = [];

    var query = new api.FindIncidents();
    if (text) {
      query.freeText = text;
    }
    if (applicationId) {
      query.applicationIds = [applicationId];
    }
    if (collectionName) {
      query.contextCollectionName = collectionName;
    }
    if (propertyName) {
      query.contextCollectionPropertyName = propertyName;
    }
    if (propertyValue) {
      query.contextCollectionPropertyValue = propertyValue;
    }
    var result = await this.apiClient.query<api.FindIncidentsResult>(query);
    result.items.forEach(x => {
      var item = new model.IncidentSummary();
      item.applicationId = x.applicationId;
      item.applicationName = x.applicationName;
      item.name = x.name;
      item.id = x.id;
      item.assignedAtUtc = x.assignedAtUtc;
      item.createdAtUtc = x.createdAtUtc;
      item.lastReportReceivedAtUtc = x.lastReportReceivedAtUtc;
      item.assignedAtUtc = x.assignedAtUtc;
      foundIncidents.push(item);
    });

    return foundIncidents;
  }

  public async refreshIncident(incident: model.Incident, callback?: () => void): Promise<model.Incident> {
    const query = new api.GetIncident();
    query.incidentId = incident.id;
    const result = await this.apiClient.query<api.GetIncidentResult>(query);
    this.convertIncidentResult(result, incident);
    if (callback) {
      callback();
    }

    return incident;
  }

  private convertIncidentResult(result: api.GetIncidentResult, incident: model.Incident) {
    incident.applicationId = result.applicationId;
    incident.assignedAtUtc = result.assignedAtUtc;
    incident.assignedTo = result.assignedTo;
    incident.assignedToId = result.assignedToId;
    incident.contextCollections = result.contextCollections;
    incident.createdAtUtc = result.createdAtUtc;
    incident.description = result.description;
    incident.facts = this.convertFacts(result.facts);
    incident.state = result.incidentState;
    incident.fullName = result.fullName;
    incident.highlightedContextData = this.convertData(result.highlightedContextData);
    incident.isIgnored = result.isIgnored;
    incident.isReOpened = result.isReOpened;
    incident.isSolved = result.isSolved;
    incident.lastReportReceivedAtUtc = result.lastReportReceivedAtUtc;
    incident.reOpenedAtUtc = result.reOpenedAtUtc;
    incident.reportCount = result.reportCount;
    incident.solution = result.solution;
    incident.solvedAtUtc = result.solvedAtUtc;
    incident.stackTrace = result.stackTrace;
    incident.suggestedSolutions = this.convertSolutions(result.suggestedSolutions);
    incident.tags = result.tags;
    incident.updatedAtUtc = result.updatedAtUtc;

    var stats = new model.IncidentMonthStats();
    stats.days = [];
    stats.values = [];
    result.dayStatistics.map(x => {
      stats.days.push(x.date);
      stats.values.push(x.count);
    });
    incident.monthReports = stats;
  }

  private convertFacts(highlightedContextData: api.QuickFact[]): model.IQuickFact[] {
    var output: model.IQuickFact[] = [];
    highlightedContextData.forEach(x => {
      output.push({
        description: x.description,
        title: x.title,
        value: x.value,
        url: x.url
      });
    });
    return output;
  }

  private convertData(highlightedContextData: api.HighlightedContextData[]): model.IHighlightedData[] {
    var output: model.IHighlightedData[] = [];
    highlightedContextData.forEach(x => {
      output.push({
        name: x.name,
        value: x.value,
        description: x.description,
        url: x.url
      });
    });
    return output;
  }

  private convertSolutions(highlightedContextData: api.SuggestedIncidentSolution[]): model.ISuggestedSolution[] {
    var output: model.ISuggestedSolution[] = [];
    highlightedContextData.forEach(x => {
      output.push({
        reason: x.reason,
        suggestedSolution: x.suggestedSolution,
      });
    });
    return output;
  }

  private convertState(state: number): string {
    switch (state) {
      case model.IncidentState.New:
        return "New";
      case model.IncidentState.Active:
        return "Active";
      case model.IncidentState.Ignored:
        return "Ignored";
      case model.IncidentState.Closed:
        return "Closed";
      case model.IncidentState.ReOpened:
        return "ReOpened";
      default:
        return "Unknown";
    }
  }
}
