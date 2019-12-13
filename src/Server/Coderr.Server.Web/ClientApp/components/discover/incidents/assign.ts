import { AppRoot } from '../../../services/AppRoot';
import { Component, Vue } from "vue-property-decorator";


@Component
export default class AssignIncidentComponent extends Vue {
    created() {
        const incidentId = parseInt(this.$route.params.incidentId, 10);
        AppRoot.Instance.incidentService.assignToMe(incidentId);
        setTimeout(() => {
                this.$router.push({ name: 'analyzeIncident', params: { 'incidentId': incidentId.toString() } });
            },
            1000);
    }
}
