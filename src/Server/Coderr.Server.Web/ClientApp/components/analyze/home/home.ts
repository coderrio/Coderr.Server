import { MyIncidents, IMyIncident, IIncidentListChangedContext } from "../myincidents";
import Vue from "vue";
import { Component } from "vue-property-decorator";


@Component
export default class AnalyzeHomeComponent extends Vue {
    showWelcome: boolean = false;

    created() {
        MyIncidents.Instance.subscribeOnSelectedIncident(x => this.onIncidentSelected(x));
        MyIncidents.Instance.subscribeOnListChanges(x => this.onIncidentListChanged(x));
        MyIncidents.Instance.ready()
            .then(() => this.onReady());
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
