import { PubSubService, MessageContext } from "../../services/PubSub";
import * as MenuApi from "../../services/menu/MenuApi";
import Vue from "vue";
import { AppRoot } from '../../services/AppRoot';
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
    toggleMenu = false;
    applicationId: number | null = null;

    created() {
        if (this.$route.params.incidentId) {
            this.incidentId = parseInt(this.$route.params.incidentId, 10);
        }
        this.applicationId = AppRoot.Instance.currentApplicationId;
        PubSubService.Instance.subscribe(MenuApi.MessagingTopics.ApplicationChanged, this.onApplicationChangedInNavMenu);
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
        PubSubService.Instance.unsubscribe(MenuApi.MessagingTopics.ApplicationChanged, this.onApplicationChangedInNavMenu);
    }

    toggleIncidentMenu() {
        this.toggleMenu = !this.toggleMenu;

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
            this.title = '(Select an incident)';
            this.incidentId = null;
        } else {
            this.$router.push({ name: 'analyzeIncident', params: { incidentId: incident.incidentId.toString() } });
            this.title = incident.shortTitle;
            this.incidentId = incident.incidentId;
        }
    }

    @Watch('$route.params.applicationId')
    onAppRoute(value: string, oldValue: string) {
        if (!value) {
            this.applicationId = null;
            return;
        } else {
            this.applicationId = parseInt(value, 10);
        }
    }
    private onApplicationChangedInNavMenu(ctx: MessageContext) {
        var body = <MenuApi.ApplicationChanged>ctx.message.body;
        this.applicationId = body.applicationId;
    }

    @Watch('$route.params.incidentId')
    onIncidentRoute(value: string, oldValue: string) {
        if (this.$route.fullPath.indexOf('/analyze/') === -1) {
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