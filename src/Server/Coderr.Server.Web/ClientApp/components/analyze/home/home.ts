import { MyIncidents, IMyIncident } from "../myincidents";
import Vue from "vue";
import { Component } from "vue-property-decorator";

@Component
export default class AnalyzeHomeComponent extends Vue {
    showWelcome: boolean = false;

    created() {
        MyIncidents.Instance.subscribeOnSelectedIncident(x => {
            this.showWelcome = x == null;
        });
        MyIncidents.Instance.subscribeOnListChanges(x => {
            this.showWelcome = MyIncidents.Instance.myIncidents.length === 0;
        });
        MyIncidents.Instance.ready()
            .then(x => {
                this.showWelcome = MyIncidents.Instance.myIncidents.length === 0;
            });
    }
}
