import * as MenuApi from "../../services/menu/MenuApi";
import { AppRoot } from "../../services/AppRoot";
import { IncidentTopcis, IncidentAssigned, IncidentClosed, IncidentIgnored } from "../../services/incidents/IncidentService";
import { PubSubService } from "../../services/PubSub";
import Vue from 'vue';
import { Component, Watch } from 'vue-property-decorator';
import { Location } from "vue-router";

interface IRouteNavigation {
    routeName: string;
    url: string;
    setMenu(name: String): void;
}
type NavigationCallback = (context: IRouteNavigation) => void;

@Component
export default class AnalyzeMenuComponent extends Vue {
    private callbacks: NavigationCallback[] = [];
    private myIncidentsPromise: Promise<any>;

    childMenu: MenuApi.MenuItem[] = [];
    myIncidents: MenuApi.MenuItem[] = [];
    currentIncidentName: string = '(choose incident)';
    currentIncidentId: number | null = null;

    @Watch('$route.params.incidentId')
    onIncidentSelected(value: string, oldValue: string) {
        if (this.$route.fullPath.indexOf('/analyze/') === -1) {
            return;
        }

        //ignore subroutes to same incident.
        if (this.currentIncidentId === parseInt(value, 10)) {
            return;
        }

        var incidentId = 0;
        if (!value) {
            //Todo, we can arrive here when assigning the first incident.
            if (this.myIncidents.length === 0) {
                return;
            }
            incidentId = <number>this.myIncidents[0].tag;
        } else {
            incidentId = parseInt(value, 10);
        }
        
        this.updateIncidentMenu(incidentId);
    }

    mounted() {
        PubSubService.Instance.subscribe(IncidentTopcis.Assigned, x => {
            var msg = <IncidentAssigned>x.message.body;
            AppRoot.Instance.incidentService.get(msg.incidentId)
                .then(assignedIncident => {
                    if (this.myIncidents.findIndex(menuItem => menuItem.tag === assignedIncident.Id) === -1) {
                        var item = this.createMenuItem(assignedIncident.Id, assignedIncident.Description);
                        this.myIncidents.push(item);
                    }
                });
        });
        PubSubService.Instance.subscribe(IncidentTopcis.Closed, x => {
            var msg = <IncidentClosed>x.message.body;
            this.removeIncident(msg.incidentId);
        });
        PubSubService.Instance.subscribe(IncidentTopcis.Ignored, x => {
            var msg = <IncidentIgnored>x.message.body;
            this.removeIncident(msg.incidentId);
        });

        if (this.$route.params.incidentId) {
            this.currentIncidentId = parseInt(this.$route.params.incidentId, 10);
        }

        this.myIncidentsPromise = new Promise((resolve, reject) => {
            AppRoot.Instance.incidentService.getMine()
                .then(mine => {
                    if (mine.length === 0) {
                        this.currentIncidentName = '(choose incident)';
                        this.currentIncidentId = 0;
                        resolve();
                        return;
                    }

                    mine.forEach(myIncident => {
                        if (!this.myIncidents.find(menuItem => menuItem.tag === myIncident.Id)) {
                            var item = this.createMenuItem(myIncident.Id, myIncident.Name);
                            this.myIncidents.push(item);
                        }
                    });

                    if (!this.$route.params.incidentId) {
                        this.navigateToIncident(mine[0].Id);
                        resolve();
                        return;
                    }

                    this.updateIncidentMenu(this.currentIncidentId);
                    resolve();
                });
        });

    }

    navigateToIncident(incidentId: number) {
        if (!incidentId) {
            throw new Error('Expected an incidentId, got: ' + incidentId);
        }

        this.myIncidentsPromise
            .then(x => {
                let routeLocation: Location =
                    { name: 'analyzeIncident', params: { 'incidentId': incidentId.toString() } };
                this.$router.push(routeLocation);
                this.updateIncidentMenu(incidentId);
            });
    }

    private createMenuItem(incidentId: number, title: string): MenuApi.MenuItem {
        const routeLocation: Location =
            { name: 'analyzeIncident', params: { 'incidentId': incidentId.toString() } };
        var route = <string>this.$router.resolve(routeLocation).href;
        var mnuItem: MenuApi.MenuItem = {
            title: title,
            url: route,
            tag: incidentId
        };
        return mnuItem;
    }

    private async updateIncidentMenu(incidentId: number): Promise<null> {
        await this.myIncidentsPromise;
        let menuItem = await this.getIncident(incidentId);
        this.currentIncidentId = incidentId;
        let title = menuItem.title;
        if (title.length > 20) {
            title = title.substr(0, 15) + '[...]';
        }
        this.currentIncidentName = title;

        // We've switched incident and need to update the URL
        if (this.$route.params.incidentId !== menuItem.tag.toString()) {
            this.$router.push({ name: 'analyzeIncident', params: { 'incidentId': menuItem.tag.toString() } });
        }

        return null;
    }

    private removeIncident(incidentId: number) {
        if (!incidentId) {
            throw new Error("We do not own " + incidentId);
        }

        for (var i = 0; i < this.myIncidents.length; i++) {
            if (this.myIncidents[i].tag === incidentId) {
                this.$delete(this.myIncidents, i);
                //this.myIncidents.splice(i, 1);
                break;
            }
        }
        console.log('new list without ' + incidentId, this.myIncidents);
    }

    private async getIncident(incidentId: number): Promise<MenuApi.MenuItem> {
        for (var i = 0; i < this.myIncidents.length; i++) {
            if (this.myIncidents[i].url.indexOf(`/incident/${incidentId}`) !== -1) {
                return this.myIncidents[i];
            }
        }

        // Can happen when we just assigned a new incident to ourselves.
        var allMine = await AppRoot.Instance.incidentService.getMine();
        var foundItem: MenuApi.MenuItem = null;
        allMine.forEach(myIncident => {
            if (myIncident.Id === incidentId) {
                var item = this.createMenuItem(myIncident.Id, myIncident.Name);
                this.myIncidents.push(item);
                foundItem = item;
            }
        });
        if (foundItem != null) {
            return foundItem;
        }

        throw new Error('Failed to find myIncident ' + incidentId);
    }

}
