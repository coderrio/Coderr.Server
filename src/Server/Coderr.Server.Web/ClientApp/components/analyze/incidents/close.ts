import { PubSubService } from "../../../services/PubSub";
import { AppRoot } from '../../../services/AppRoot';
import * as SimpleMDE from "SimpleMDE";
import 'simplemde/dist/simplemde.min.css';
import Vue from "vue";
import { Component, Watch } from "vue-property-decorator";


@Component
export default class CloseIncidentComponent extends Vue {
    private simpleMde$: SimpleMDE;
    solution = "";

    incidentId: number = 0;

    mounted() {
        this.incidentId = parseInt(this.$route.params.incidentId, 10);
        this.simpleMde$ = new SimpleMDE({ element: document.getElementById("solution") });
    }
    
    close() {
        AppRoot.Instance.incidentService.close(this.incidentId, this.solution)
            .then(x => {
                this.$router.push({ name: 'analyzeHome' });
            });
    }

}
