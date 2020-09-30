import { MyIncidents, IMyIncident, IIncidentListChangedContext } from "../myincidents";
import Vue from "vue";
import { Component } from "vue-property-decorator";


@Component
export default class AnalyzeHomeComponent extends Vue {
    showWelcome: boolean = false;

    created() {
        MyIncidents.Instance.subscribeOnSelectedIncident(this.onIncidentSelected);
        MyIncidents.Instance.subscribeOnListChanges(this.onIncidentListChanged);
        MyIncidents.Instance.ready()
            .then(() => this.onReady());

    }

    mounted() {
        if (this.$route.params.incidentId) {
            var incidentId = parseInt(this.$route.params.incidentId, 10);
            MyIncidents.Instance.switchIncident(incidentId);
        } else {
            MyIncidents.Instance.switchIncident(0);
        }
    }

    destroyed() {
        MyIncidents.Instance.unsubscribe(this.onIncidentSelected);
        MyIncidents.Instance.unsubscribe(this.onIncidentListChanged);
    }

    private onIncidentSelected(incident?: IMyIncident) {
        this.showWelcome = incident == null;
    }

    private onIncidentListChanged(incident?: IIncidentListChangedContext) {
        this.showWelcome = MyIncidents.Instance.myIncidents.length === 0;
    }

    private onReady() {
        this.showWelcome = MyIncidents.Instance.myIncidents.length === 0;
    }
}
