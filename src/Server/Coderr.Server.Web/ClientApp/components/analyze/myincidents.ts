import { AppRoot } from "@/services/AppRoot";
import { AppEvents, ApplicationChanged } from "@/services/applications/ApplicationService";
import { PubSubService, MessageContext } from "@/services/PubSub";
import { IncidentTopcis, IncidentAssigned, IncidentClosed, IncidentIgnored } from "@/services/incidents/IncidentService";
import * as incidents from "@/dto/Core/Incidents";

/**
 * null if the user do not have any assigned incidents (for the selected application)
 */
export interface incidentSelectedCallback { (selected: IMyIncident | null): void }

export interface IIncidentListChangedContext {
    incidentId?: number;
    added?: boolean;
}
/**
 * An incident have been added or removed from the my incident list
 */
export interface incidentListChanged {
    (ctx: IIncidentListChangedContext): void
}
export interface IMyIncident {
    incidentId: number;
    applicationId: number;
    applicationName: string;
    createdAtUtc: Date;
    assignedAtUtc: Date;
    title: string;

    /**
     * Some incidents have long titles (exception message). This field 
     */
    shortTitle: string;
}

export class MyIncidents {
    public static Instance = new MyIncidents();
    private allMyIncidents$: IMyIncident[] = [];
    private selectedCallbacks$: incidentSelectedCallback[] = [];
    private listChangedCallback$: incidentListChanged[] = [];
    private loadPromise$: Promise<any>;

    myIncidents: IMyIncident[] = [];
    selectedIncident: IMyIncident | null;
    menuTitle = '';

    selectedApplicationId: number | null = null;

    constructor() {
        this.loadPromise$ = this.loadMyIncidentsFromBackend();

        PubSubService.Instance.subscribe(AppEvents.Selected, x => {
            var msg = <ApplicationChanged>x.message.body;
            this.switchApplication(msg.applicationId);
        });

        PubSubService.Instance.subscribe(IncidentTopcis.Assigned, x => this.onIncidentAssigned(x));
        PubSubService.Instance.subscribe(IncidentTopcis.Closed, x => {
            var msg = <IncidentClosed>x.message.body;
            this.removeIncident(msg.incidentId);
        });
        PubSubService.Instance.subscribe(IncidentTopcis.Ignored, x => {
            var msg = <IncidentIgnored>x.message.body;
            this.removeIncident(msg.incidentId);
        });
    }

    public get incident(): IMyIncident {
        return this.selectedIncident;
    }


    async ready() {
        await this.loadPromise$;
    }

    /**
     * 
     * @param applicationId Can be 0 for all applications
     */
    async switchApplication(applicationId: number) {
        await this.loadPromise$;

        if (applicationId === 0) {
            this.selectedApplicationId = null;
        } else {
            this.selectedApplicationId = applicationId;
        }

        this.filterMyIncidents();
        this.triggerIncidentListCallbacks();

        if (this.selectedIncident) {
            if (this.selectedIncident.applicationId === applicationId) {
                return;
            }
        }

        this.setNextIncident();
    }

    async switchIncident(incidentId: number) {
        if (this.selectedIncident != null && this.selectedIncident.incidentId === incidentId) {
            return;
        }

        if (this.loadPromise$) {
            await this.loadPromise$;
        }

        var incident = await this.getIncident(incidentId);
        if (incident == null) {
            this.setNextIncident();
        } else {
            this.selectedIncident = incident;
            this.triggerIncidentChangedCallbacks();
        }
    }

    /**
     * 
     * @param callback
     * @returns The incident that was selected when the subscribe method was invoked (null if none was selected)
     */
    public subscribeOnSelectedIncident(callback: incidentSelectedCallback): IMyIncident {
        this.selectedCallbacks$.push(callback);
        return this.selectedIncident;
    }

    /**
     * 
     * @param callback
     * @returns The incident that was selected when the subscribe method was invoked (null if none was selected)
     */
    public subscribeOnListChanges(callback: incidentListChanged): IMyIncident {
        this.listChangedCallback$.push(callback);
        return this.selectedIncident;
    }

    unsubscribe(callback: any) {
        this.selectedCallbacks$ = this.selectedCallbacks$.filter(x => x !== callback);
        this.listChangedCallback$ = this.listChangedCallback$.filter(x => x !== callback);
    }


    private onIncidentAssigned(msgContext: MessageContext) {
        var msg = <IncidentAssigned>msgContext.message.body;
        AppRoot.Instance.incidentService.get(msg.incidentId)
            .then(assignedIncident => {
                var index = this.allMyIncidents$.findIndex(menuItem => menuItem.incidentId === assignedIncident.Id);
                var item: IMyIncident;
                if (index === -1) {
                    item = this.createItem(
                        assignedIncident.Id,
                        assignedIncident.ApplicationId,
                        '',
                        assignedIncident.CreatedAtUtc,
                        assignedIncident.AssignedAtUtc,
                        assignedIncident.Description
                    );
                    AppRoot.Instance.applicationService.get(assignedIncident.ApplicationId)
                        .then(x => item.applicationName = x.name);
                    this.allMyIncidents$.push(item);
                } else {
                    item = this.allMyIncidents$[index];
                }

                this.filterMyIncidents();
                this.triggerIncidentListCallbacks(item.incidentId, true);

                // always switch to correct incident upon assign
                this.switchIncident(msg.incidentId);
            });
    }

    private triggerIncidentChangedCallbacks() {
        this.selectedCallbacks$.forEach(x => {
            x(this.selectedIncident);
        });
    }

    private triggerIncidentListCallbacks(incidentId?: number, added?: boolean) {
        this.listChangedCallback$.forEach(x => {
            x({ incidentId, added });
        });
    }

    private async loadMyIncidentsFromBackend() {
        if (this.loadPromise$) {
            await this.loadPromise$;
            return;
        }

        var mine = await AppRoot.Instance.incidentService.getMine();
        if (mine.length === 0) {
            return;
        }

        mine.forEach(dto => {
            if (!this.allMyIncidents$.find(item => item.incidentId === dto.Id)) {
                var item = this.createItem2(dto);
                this.allMyIncidents$.push(item);
            }
        });
        this.filterMyIncidents();
        this.triggerIncidentListCallbacks();
    }

    private filterMyIncidents() {

        this.myIncidents = this.allMyIncidents$.filter(item => {
            if (this.selectedApplicationId > 0) {
                return item.applicationId === this.selectedApplicationId;
            }

            return true;
        });
    }

    private createItem(incidentId: number, applicationId: number, applicationName: string, createdAtUtc: Date, assignedAtUtc: Date, title: string): IMyIncident {
        let shortTitle = title;
        if (shortTitle.length > 50) {
            shortTitle = title.substr(0, 45) + '[...]';
        }

        var item: IMyIncident = {
            title: title,
            shortTitle: shortTitle,
            incidentId: incidentId,
            createdAtUtc: createdAtUtc,
            applicationId: applicationId,
            applicationName: applicationName,
            assignedAtUtc: assignedAtUtc
        };
        return item;
    }

    private createItem2(incident: incidents.FindIncidentsResultItem): IMyIncident {
        let shortTitle = incident.Name;
        if (shortTitle.length > 50) {
            shortTitle = incident.Name.substr(0, 45) + '[...]';
        }

        var item: IMyIncident = {
            title: incident.Name,
            shortTitle: shortTitle,
            incidentId: incident.Id,
            createdAtUtc: incident.CreatedAtUtc,
            applicationId: incident.ApplicationId,
            applicationName: incident.ApplicationName,
            assignedAtUtc: incident.AssignedAtUtc
        };
        return item;
    }

    private async getIncident(incidentId: number): Promise<IMyIncident> {
        for (var i = 0; i < this.allMyIncidents$.length; i++) {
            if (this.allMyIncidents$[i].incidentId === incidentId) {
                return this.allMyIncidents$[i];
            }
        }

        // Can happen when we just assigned a new incident to ourselves.
        var allMine = await AppRoot.Instance.incidentService.getMine();
        var foundItem: IMyIncident = null;
        allMine.forEach(myIncident => {
            if (myIncident.Id === incidentId) {
                var item = this.createItem2(myIncident);
                this.allMyIncidents$.push(item);
                foundItem = item;
            }
        });
        if (foundItem != null) {
            return foundItem;
        }

        return null;
    }


    private removeIncident(incidentId: number) {
        if (!incidentId) {
            throw new Error("We do not own " + incidentId);
        }

        for (var i = 0; i < this.allMyIncidents$.length; i++) {
            if (this.allMyIncidents$[i].incidentId !== incidentId) {
                continue;
            }

            var incident = this.allMyIncidents$[i];
            this.allMyIncidents$.splice(i, 1);
            this.filterMyIncidents();
            if (this.selectedIncident != null && this.selectedIncident.incidentId === incident.incidentId) {
                this.selectedIncident = null;
            }

            this.triggerIncidentListCallbacks(incident.incidentId, false);
            break;
        }



        if (this.selectedIncident == null || incidentId === this.selectedIncident.incidentId) {
            this.setNextIncident();
        }
    }

    private setNextIncident() {
        if (this.myIncidents.length > 0) {
            this.selectedIncident = this.myIncidents[0];
        } else {
            this.selectedIncident = null;
        }

        this.triggerIncidentChangedCallbacks();
    }

}