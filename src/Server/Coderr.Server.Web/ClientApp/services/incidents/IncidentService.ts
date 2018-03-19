import { ApiClient } from '../ApiClient';
import { AppRoot } from '../AppRoot';
import { PubSubService } from '../PubSub';
import {
    GetIncident,
    GetIncidentResult,
    AssignIncident,
    IgnoreIncident,
    CloseIncident,
    GetIncidentForClosePage, GetIncidentForClosePageResult,
    FindIncidents, FindIncidentsResult, FindIncidentsResultItem,
} from "../../dto/Core/Incidents";

export interface ICloseResult {
    requiresStatusUpdate: boolean
}

export interface IncidentAssigned {
    incidentId: number;
    userId: number;
}

export class IncidentTopcis {
    static readonly Assigned = "/incidents/assigned";
}

export class IncidentService {
    private myIncidents: FindIncidentsResultItem[] = [];
    private haveFetchedMine = false;

    // Cache of the 10 last fetched incidents.
    // Rolling access.
    private cachedIncidents: GetIncidentResult[] = [];

    constructor(private apiClient: ApiClient) {
        if (!apiClient) {
            throw new Error("apiClient is required.");
        }
    }

    async assignToMe(incidentId: number) {
        if (!incidentId) {
            throw new Error("incidentId is required.");
        }

        var currentUser = await AppRoot.Instance.loadCurrentUser();
        const cmd = new AssignIncident();
        cmd.IncidentId = incidentId;
        cmd.AssignedTo = currentUser.id;
        cmd.AssignedBy = currentUser.id;
        await this.apiClient.command(cmd);

        const item = this.getFromCache(incidentId);
        if (item) {
            item.AssignedTo = "me";
            item.AssignedAtUtc = new Date();
            item.AssignedToId = currentUser.id;

            console.log('Got cached one');
            let myItem = new FindIncidentsResultItem();
            myItem.Id = item.Id;
            myItem.ApplicationId = item.ApplicationId.toString();
            myItem.CreatedAtUtc = item.CreatedAtUtc;
            myItem.ReportCount = item.ReportCount;
            myItem.LastReportReceivedAtUtc = item.LastReportReceivedAtUtc;
            myItem.IsReOpened = item.IsReOpened;
            myItem.Name = item.Description;
            this.myIncidents.push(myItem);
        } else {
            console.log('fetchig incident..');
            var incident = await this.get(incidentId);
            console.log('got', incident);
            let myItem = new FindIncidentsResultItem();
            myItem.Id = incident.Id;
            myItem.ApplicationId = incident.ApplicationId.toString();
            myItem.CreatedAtUtc = incident.CreatedAtUtc;
            myItem.ReportCount = incident.ReportCount;
            myItem.LastReportReceivedAtUtc = incident.LastReportReceivedAtUtc;
            myItem.IsReOpened = incident.IsReOpened;
            myItem.Name = incident.Description;
            this.myIncidents.push(myItem);
        }

        var msg: IncidentAssigned = {
            incidentId: incidentId,
            userId: currentUser.id
        };
        
        console.log('publsing assigned event')
        PubSubService.Instance.publish(IncidentTopcis.Assigned, msg);
    }

    async assign(incidentId: number, userId: number, userName: string) {
        if (!incidentId) {
            throw new Error("incidentId is required.");
        }

        
        const cmd = new AssignIncident();
        cmd.IncidentId = incidentId;
        cmd.AssignedTo = userId;
        var current = await AppRoot.Instance.loadCurrentUser();
        cmd.AssignedBy = current.id;
        await this.apiClient.command(cmd);

        var item = this.getFromCache(incidentId);
        if (item) {
            item.AssignedTo = userName;
            item.AssignedAtUtc = new Date();
            item.AssignedToId = userId;
        }

        var msg: IncidentAssigned = {
            incidentId: incidentId,
            userId: userId
        };
        PubSubService.Instance.publish(IncidentTopcis.Assigned, msg);
    }

    /**
     * 
     * @param reason
     * @returns Number of users subscribing on status updates
     */
    async close(incidentId: number, reason: string): Promise<number> {
        if (!incidentId) {
            throw new Error("incidentId is required.");
        }
        var item = this.getFromCache(incidentId);
        if (!item) {
            item = await this.get(incidentId);
        }
        if (item == null) {
            throw new Error('Failed to find incident ' + incidentId);
        }

        item.IsSolved = true;
        item.Solution = reason;

        const current = await AppRoot.Instance.loadCurrentUser();
        const closeCmd = new CloseIncident();
        closeCmd.IncidentId = incidentId;
        closeCmd.Solution = reason;
        closeCmd.UserId = current.id;
        await this.apiClient.command(closeCmd);

        const q = new GetIncidentForClosePage();
        q.IncidentId = incidentId;
        var result = await this.apiClient.query<GetIncidentForClosePageResult>(q);
        return result.SubscriberCount;
    }

    /**
     * 
     * @param reason
     * @returns Number of users subscribing on status updates
     */
    async showClose(incidentId: number, closeModelBodyId: string): Promise<ICloseResult> {
        if (!incidentId) {
            throw new Error("incidentId is required.");
        }
        var modalResult = await AppRoot.modal({
            title: 'Close incident',
            contentId: '#CloseBody',
        });

        var modal = <HTMLDivElement>document.getElementById(modalResult.modalId);
        var area = <HTMLTextAreaElement>modal.querySelector('textarea');
        var reason = area.value;

        var numberOfFollowers = await this.close(incidentId, reason);
        AppRoot.notify('Incident have been closed');

        if (numberOfFollowers <= 0) {
            return {
                requiresStatusUpdate: false
            };
        }

        modalResult = await AppRoot.modal({
            title: 'Incident have followers',
            htmlContent:
            'Incident have users following it. We strongly recommend that you use Coderr to send them a status update saying that the error is corrected.',
            submitButtonText: 'Draft an update'
        });

        return {
            requiresStatusUpdate: true
        };
        //var ops = this.$router.push(
        //    {
        //        name: 'incidentStatusUpdate',
        //        params: {
        //            'incidentId': incidentId.toString(),
        //            'closed': 'true'
        //        }
        //    });
    }


    async get(id: number): Promise<GetIncidentResult> {
        if (id === 0) {
            throw new Error("Expected an incidentId");
        }

        var cached = this.getFromCache(id);
        if (cached) {
            return cached;
        }

        var q = new GetIncident();
        q.IncidentId = id;
        var result = await this.apiClient.query<GetIncidentResult>(q);
        this.pushToCache(result);
        return result;
    }


    async getMine(): Promise<FindIncidentsResultItem[]> {
        if (this.myIncidents.length > 0 && this.haveFetchedMine) {
            console.log('returning cached mine.');
            return this.myIncidents;
        }
            

        console.log('loading mine.');
        this.haveFetchedMine = true;
        var current = await AppRoot.Instance.loadCurrentUser();
        var query = new FindIncidents();
        query.AssignedToId = current.id;

        var result = await this.apiClient.query<FindIncidentsResult>(query);
        result.Items.forEach(x => {
            this.myIncidents.push(x);
        });

        return result.Items;
    }

    private getFromCache(id: number): GetIncidentResult | null {
        if (!id) {
            throw new Error("id is required");
        }

        for (var i = 0; i < this.cachedIncidents.length; i++) {
            if (this.cachedIncidents[i].Id === id)
                return this.cachedIncidents[i];
        }

        return null;
    }

    private pushToCache(incident: GetIncidentResult) {
        if (this.cachedIncidents.length === 10) {
            this.cachedIncidents.shift();
        }

        this.cachedIncidents.push(incident);
    }

    async ignore(incidentId: number): Promise<any> {
        if (!incidentId) {
            throw new Error("incidentId is required");
        }
        
        var current = await AppRoot.Instance.loadCurrentUser();
        var cmd = new IgnoreIncident();
        cmd.IncidentId = incidentId;
        cmd.UserId = current.id;
        await AppRoot.Instance.apiClient.command(cmd);

        var item = this.getFromCache(incidentId);
        if (item) {
            item.IsIgnored = true;
        }
    }
}