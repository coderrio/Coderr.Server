import * as MenuApi from "../../services/menu/MenuApi";
import { AppRoot } from "@/services/AppRoot";
import { Component, Watch, Vue, Mixins } from "vue-property-decorator";
import { MyIncidents, IMyIncident } from "./myincidents";
import { AppAware } from "@/AppMixins";

interface IRouteNavigation {
    routeName: string;
    url: string;
    setMenu(name: String): void;
}

@Component
export default class AnalyzeMenuComponent extends Mixins(AppAware) {
    childMenu: MenuApi.MenuItem[] = [];

    incidents: IMyIncident[] = [];
    title = "";
    incidentId: number | null = null;
    toggleMenu = false;
    applicationId: number | null = null;
    loadedIncident: number = 0;

    created() {
        this.applicationId = AppRoot.Instance.currentApplicationId;
        this.onApplicationChanged(this.onApplication);

        if (this.$route.params.incidentId) {
            this.incidentId = parseInt(this.$route.params.incidentId, 10);
            this.loadedIncident = this.incidentId;
            MyIncidents.Instance.switchIncident(this.incidentId);
        }

        MyIncidents.Instance.subscribeOnSelectedIncident(this.onIncidentSelected);
        MyIncidents.Instance.subscribeOnListChanges(this.onListChanged);
    }

    mounted() {
        MyIncidents.Instance.ready()
            .then(() => {
                if (MyIncidents.Instance.incident) {
                    this.incidentId = MyIncidents.Instance.incident.incidentId;
                    this.title = MyIncidents.Instance.incident.title;
                }
                this.incidents = MyIncidents.Instance.myIncidents;
            });

    }

    destroyed() {
        MyIncidents.Instance.unsubscribe(this.onIncidentSelected);
        MyIncidents.Instance.unsubscribe(this.onListChanged);
    }

    toggleIncidentMenu() {
        this.toggleMenu = !this.toggleMenu;

    }

    private onApplication(applicationId: number) {
        this.applicationId = applicationId;
        if (applicationId === 0) {
            this.incidents = MyIncidents.Instance.myIncidents;
        } else {
            this.incidents = MyIncidents.Instance.myIncidents.filter(x => x.applicationId === applicationId);
        }
    }

    private onListChanged(args: any) {
        this.incidents = MyIncidents.Instance.myIncidents;
        if (!this.incidentId && this.incidents.length > 0) {
            this.incidentId = this.incidents[0].incidentId;
            MyIncidents.Instance.switchIncident(this.incidentId);
        }
    }

    private onIncidentSelected(incident: IMyIncident | null) {

        if (incident == null) {
            this.title = "(Select an incident)";
            this.incidentId = null;
        } else {
            var routeIncident = parseInt(this.$route.params.incidentId);
            if (routeIncident !== incident.incidentId) {
                this.$router.push({ name: "analyzeIncident", params: { incidentId: incident.incidentId.toString() } });
            }
            this.title = incident.shortTitle;
            this.incidentId = incident.incidentId;
        }
    }

    @Watch("$route.params.incidentId")
    onIncidentRoute(value: string, oldValue: string) {
        if (this.$route.fullPath.indexOf("/analyze/") === -1) {
            return;
        }
        if (!value) {
            this.incidentId = null;
            return;
        } else {
            var newIncidentId = parseInt(value, 10);
            if (this.incidentId === newIncidentId) {
                return;
            }
            this.incidentId = newIncidentId;
        }

        MyIncidents.Instance.switchIncident(this.incidentId);
    }

}