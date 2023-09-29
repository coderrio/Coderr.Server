import * as WorkItems from "@/dto/Common/WorkItems";
import { AppRoot } from "@/services/AppRoot";

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

export class WorkItemService {
    
    createWorkItem(applicationId: number, incidentId: number) {
        var cmd = new WorkItems.CreateWorkItem();
        cmd.IncidentId = incidentId;
        cmd.ApplicationId = applicationId;
        AppRoot.Instance.apiClient.command(cmd);
    }

    async getWorkItem(incidentId: number): Promise<IWorkItem> {
        var dto = new WorkItems.FindWorkItem();
        dto.IncidentId = incidentId;
        var result = await AppRoot.Instance.apiClient.query<WorkItems.FindWorkItemResult>(dto);
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
        var result = await AppRoot.Instance.apiClient.query<WorkItems.FindIntegrationResult>(dto);
        if (!result.HaveIntegration) {
            return null;
        }

        return {
            title: result.Title,
            name: result.Name
        }
    }
}