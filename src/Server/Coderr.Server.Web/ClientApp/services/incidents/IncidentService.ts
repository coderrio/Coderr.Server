import { ApiClient } from '../ApiClient';
import { AppRoot } from '../AppRoot';
import { PubSubService } from '../PubSub';
import {
    GetIncident,
    GetIncidentResult,
    AssignIncident,
    IgnoreIncident,
    CloseIncident,
    DeleteIncident,
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
export interface IncidentClosed {
    incidentId: number;
    userId: number;
}
export interface IncidentDeleted {
    incidentId: number;
    userId: number;
}
export interface IncidentIgnored {
    incidentId: number;
    userId: number;
}


export class IncidentTopcis {
    static readonly Assigned = "/incidents/assigned";
    static readonly Closed = "/incidents/closed";
    static readonly Ignored = "/incidents/ignored";
    static readonly Deleted = "/incidents/deleted";
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

    async find(applicationId?: number): Promise<FindIncidentsResult> {
        var q = new FindIncidents();
        if (applicationId) {
            q.ApplicationIds = [applicationId];
        }
        return await AppRoot.Instance.apiClient.query<FindIncidentsResult>(q);
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

        await this.addMyIncident(incidentId, "me", currentUser.id);
        var msg: IncidentAssigned = {
            incidentId: incidentId,
            userId: currentUser.id
        };
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

        await this.addMyIncident(incidentId, userName, userId);
        var msg: IncidentAssigned = {
            incidentId: incidentId,
            userId: userId
        };
        PubSubService.Instance.publish(IncidentTopcis.Assigned, msg);
    }

    async delete(incidentId: number, areYouSure: string) {
        if (!incidentId) {
            throw new Error("incidentId is required.");
        }
        if (areYouSure !== "yes") {
            throw new Error("Please be sure!");
        }

        const cmd = new DeleteIncident();
        cmd.IncidentId = incidentId;
        cmd.AreYouSure = "yes";
        await this.apiClient.command(cmd);

        var msg: IncidentDeleted = {
            incidentId: incidentId,
            userId: -1
        };
        PubSubService.Instance.publish(IncidentTopcis.Deleted, msg);
    }

    /**
     * 
     * @param reason
     * @returns Number of users subscribing on status updates
     */
    async close(incidentId: number, reason: string, fixedInVersion?: string): Promise<number> {
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
        item.SolvedAtUtc = new Date();

        const current = await AppRoot.Instance.loadCurrentUser();
        const closeCmd = new CloseIncident();
        closeCmd.IncidentId = incidentId;
        closeCmd.Solution = reason;
        closeCmd.UserId = current.id;
        closeCmd.ApplicationVersion = fixedInVersion;
        await this.apiClient.command(closeCmd);

        for (var i = 0; i < this.myIncidents.length; i++) {
            if (this.myIncidents[i].Id === incidentId) {
                this.myIncidents.splice(i, 1);
                break;
            }
        }
        var msg: IncidentClosed = {
            incidentId: incidentId,
            userId: current.id
        };
        PubSubService.Instance.publish(IncidentTopcis.Closed, msg);

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
            submitButtonText: 'Close incident',
            cancelButtonText: 'Cancel'
        });

        var modal = <HTMLDivElement>document.getElementById(modalResult.modalId);
        var area = <HTMLTextAreaElement>modal.querySelector('textarea');
        var versionBox = <HTMLInputElement>modal.querySelector('input[name="version"]');
        var version = versionBox.value === '' ? null : versionBox.value;
        var reason = area.value;

        var numberOfFollowers = await this.close(incidentId, reason, version);
        AppRoot.notify('Incident have been closed');

        if (numberOfFollowers <= 0) {
            return {
                requiresStatusUpdate: false
            };
        }

        modalResult = await AppRoot.modal({
            title: 'Incident have followers',
            htmlContent:
                'Incident have users following it. We strongly recommend that you use Coderr to send them a status update saying that the error have been corrected.',
            submitButtonText: 'Notify users',
        });

        if (modalResult.pressedButtonName === 'submit') {
            return {
                requiresStatusUpdate: true
            };
        } else {
            return {
                requiresStatusUpdate: false
            };
        }

        //var ops = this.$router.push(
        //    {
        //        name: 'incidentStatusUpdate',
        //        params: {
        //            'incidentId': incidentId.toString(),
        //            'closed': 'true'
        //        }
        //    });
    }

    private pendingIncidents: any[] = [];
    async get(id: number): Promise<GetIncidentResult> {
        if (id === 0) {
            throw new Error("Expected an incidentId");
        }

        var cached = this.getFromCache(id);
        if (cached) {
            return cached;
        }

        var pending = this.pendingIncidents.find(x => x.incidentId === id);
        if (pending) {
            return await pending.promise;
        }

        var q = new GetIncident();
        q.IncidentId = id;
        var promise = this.apiClient.query<GetIncidentResult>(q);
        var index = this.pendingIncidents.push({ incidentId: id, promise: promise });

        var result = await promise;
        this.pushToCache(result);
        this.pendingIncidents.splice(index, 1);
        return result;
    }


    /**
     * Fetch all my incidents
     * @param applicationId
     * @param ignoreId Ignore this incident (we just closed or ignored it and the server might not have processed the command yet)
     */
    async getMine(applicationId?: number, ignoreId?: number): Promise<FindIncidentsResultItem[]> {
        if (this.myIncidents.length > 0 && this.haveFetchedMine) {
            if (applicationId) {
                return this.myIncidents.filter(x => x.ApplicationId === applicationId);
            }
            return this.myIncidents;
        }

        this.haveFetchedMine = true;
        var current = await AppRoot.Instance.loadCurrentUser();
        var query = new FindIncidents();
        query.AssignedToId = current.id;
        query.IsClosed = false;
        query.IsAssigned = true;
        if (applicationId) {
            query.ApplicationIds = [applicationId];
        }

        var result = await this.apiClient.query<FindIncidentsResult>(query);
        result.Items.forEach(x => {
            var ignoreIncident = x.Id === ignoreId;

            if (!ignoreIncident) {
                this.myIncidents.forEach(cached => {
                    if (cached.Id === x.Id) {
                        ignoreIncident = true;
                        return;
                    }
                });
            }

            if (!ignoreIncident) {
                this.myIncidents.push(x);
            }
        });

        return this.myIncidents;
    }

    private getFromCache(id: number): GetIncidentResult | null {
        if (!id) {
            throw new Error("id is required");
        }

        for (var i = 0; i < this.cachedIncidents.length; i++) {
            if (this.cachedIncidents[i].Id === id) {
                return this.cachedIncidents[i];
            }

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


        for (var i = 0; i < this.myIncidents.length; i++) {
            if (this.myIncidents[i].Id === incidentId) {
                this.myIncidents.splice(i, 1);
                break;
            }
        }

        var msg: IncidentIgnored = {
            incidentId: incidentId,
            userId: current.id
        };
        PubSubService.Instance.publish(IncidentTopcis.Ignored, msg);

        var item = this.getFromCache(incidentId);
        if (item) {
            item.IsIgnored = true;
        }
    }

    private async addMyIncident(incidentId: number, assignedTo: string, assignedToId: number): Promise<null> {
        const item = this.getFromCache(incidentId);
        if (item) {
            item.AssignedTo = assignedTo;
            item.AssignedAtUtc = new Date();
            item.AssignedToId = assignedToId;

            let myItem = new FindIncidentsResultItem();
            myItem.Id = item.Id;
            myItem.ApplicationId = item.ApplicationId;
            myItem.CreatedAtUtc = item.CreatedAtUtc;
            myItem.ReportCount = item.ReportCount;
            myItem.LastReportReceivedAtUtc = item.LastReportReceivedAtUtc;
            myItem.IsReOpened = item.IsReOpened;
            myItem.Name = item.Description;
            this.myIncidents.push(myItem);
        } else {
            var incident = await this.get(incidentId);
            let myItem = new FindIncidentsResultItem();
            myItem.Id = incident.Id;
            myItem.ApplicationId = incident.ApplicationId;
            myItem.CreatedAtUtc = incident.CreatedAtUtc;
            myItem.ReportCount = incident.ReportCount;
            myItem.LastReportReceivedAtUtc = incident.LastReportReceivedAtUtc;
            myItem.IsReOpened = incident.IsReOpened;
            myItem.Name = incident.Description;
            this.myIncidents.push(myItem);
        }
        return null;
    }

}