import * as MenuApi from "../../services/menu/MenuApi";
import Vue from 'vue';
import { Component, Watch } from 'vue-property-decorator';
import { MyIncidents, IMyIncident } from "./myincidents";

interface IRouteNavigation {
    routeName: string;
    url: string;
    setMenu(name: String): void;
}
type NavigationCallback = (context: IRouteNavigation) => void;

@Component
export default class AnalyzeMenuComponent extends Vue {
    childMenu: MenuApi.MenuItem[] = [];

    incidents: IMyIncident[] = [];
    title = '';
    incidentId: number | null = null;

    created() {
        MyIncidents.Instance.subscribeOnSelectedIncident(this.onIncidentSelected);
        MyIncidents.Instance.subscribeOnListChanges(x => {
            this.incidents = MyIncidents.Instance.myIncidents;
        });
        MyIncidents.Instance.ready()
            .then(x => {
                this.incidents = MyIncidents.Instance.myIncidents;
            });

        if (this.$route.params.incidentId) {
            this.incidentId = parseInt(this.$route.params.incidentId, 10);
            MyIncidents.Instance.switchIncident(this.incidentId);
        }
    }

    private onIncidentSelected(incident: IMyIncident | null) {
        if (incident == null) {
            this.title = '(Select an incident)';
            this.incidentId = null;
        } else {

            // update URL
            if (this.incidentId !== incident.incidentId) {
                this.$router.push({ name: 'analyzeIncident', params: { incidentId: incident.incidentId.toString() } });
            }

            this.title = incident.shortTitle;
            this.incidentId = incident.incidentId;
        }
    }

    @Watch('$route.params.incidentId')
    onIncidentRoute(value: string, oldValue: string) {
        if (this.$route.fullPath.indexOf('/analyze/') === -1) {
            return;
        }

        var newIncidentId = parseInt(value, 10);

        //ignore subroutes to same incident.
        if (this.incidentId === newIncidentId) {
            return;
        }

        this.incidentId = newIncidentId;
        MyIncidents.Instance.switchIncident(newIncidentId);
    }

    mounted() {
        MyIncidents.Instance.ready().then(() => {
            if (!this.$route.params.incidentId) {
                if (MyIncidents.Instance.myIncidents.length === 0) {
                    return;
                }

                var incident = MyIncidents.Instance.myIncidents[0];
                this.$router.push({ name: 'analyzeIncident', params: { incidentId: incident.incidentId.toString() } });
                return;
            }

            MyIncidents.Instance.switchIncident(this.incidentId);
        });
    }
}
