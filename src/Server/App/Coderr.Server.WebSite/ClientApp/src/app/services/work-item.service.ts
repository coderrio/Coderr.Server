import { Injectable } from '@angular/core';
import * as WorkItems from "../../server-api/Common/WorkItems";
import { ApiClient } from "../utils/HttpClient";

export interface IWorkItem {
  incidentId: number;
  applicationId: number;
  name: string;
  url: string;
}

export interface IIntegration {
  title: string;
  name: string;
}
@Injectable({
  providedIn: 'root'
})
export class WorkItemService {

  constructor(private apiClient: ApiClient) {


  }

  createWorkItem(applicationId: number, incidentId: number) {
    var cmd = new WorkItems.CreateWorkItem();
    cmd.IncidentId = incidentId;
    cmd.ApplicationId = applicationId;
    this.apiClient.command(cmd);
  }

  async getWorkItem(incidentId: number): Promise<IWorkItem> {
    var dto = new WorkItems.FindWorkItem();
    dto.IncidentId = incidentId;
    var result = await this.apiClient.query<WorkItems.FindWorkItemResult>(dto);
    if (result == null)
      return null;

    var item = {
      incidentId,
      applicationId: result.ApplicationId,
      name: result.Name,
      url: result.Url
    };

    return item;
  }


  async findIntegration(applicationId: number): Promise<IIntegration> {
    var dto = new WorkItems.FindIntegration();
    dto.ApplicationId = applicationId;
    var result = await this.apiClient.query<WorkItems.FindIntegrationResult>(dto);
    if (!result.HaveIntegration) {
      return null;
    }

    return {
      title: result.Title,
      name: result.Name
    }
  }
}
