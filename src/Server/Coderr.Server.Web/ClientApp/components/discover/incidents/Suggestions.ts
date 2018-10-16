import { AppRoot } from "../../../services/AppRoot";
import { FindIncidents, FindIncidentsResult, FindIncidentsResultItem, IncidentOrder } from "../../../dto/Core/Incidents";
import { ListMyIncidents, ListMyIncidentsResult } from "../../../dto/Common/Mine";
import Vue from "vue";
import { Component } from "vue-property-decorator";

interface ISuggestion {
    applicationId: number;
    applicationName: string;
    incidentId: number;
    incidentName: string;
    reason: string;
    exceptionType: string;
}

@Component
export default class SuggestionsComponent extends Vue {
    private static activeBtnTheme$: string = 'btn-dark';

    //chartData: [['Jan', 44], ['Feb', 27], ['Mar', 60], ['Apr', 55], ['May', 37], ['Jun', 40], ['Jul', 69], ['Aug', 33], ['Sept', 76], ['Oct', 90], ['Nov', 34], ['Dec', 22]];

    applicationId: number | null = null;
    suggestions: ISuggestion[] = [];
    showEmpty = false;

    created() {
        if (this.$route.params.applicationId) {
            this.applicationId = parseInt(this.$route.params.applicationId, 10);
        }

        var query = new ListMyIncidents();
        if (this.applicationId) {
            query.ApplicationId = this.applicationId;
        }

        AppRoot.Instance.apiClient.query<ListMyIncidentsResult>(query)
            .then(result => {
                result.Suggestions.forEach(suggestion => {
                    this.suggestions.push({
                        applicationId: suggestion.ApplicationId,
                        applicationName: suggestion.ApplicationName,
                        exceptionType: suggestion.ExceptionTypeName,
                        incidentId: suggestion.Id,
                        incidentName: suggestion.Name,
                        reason: suggestion.Motivation.replace(/\r\n/g, ", ")
                    });
                });
                this.showEmpty = this.suggestions.length === 0;

            });

    }

    mounted() {
    }

    assignToMe(incidentId: number) {
        if (!incidentId) {
            throw new Error("IncidentId was not specified.");
        }

        AppRoot.Instance.incidentService.assignToMe(incidentId)
            .then(result => {
                this.$router.push({ name: 'analyzeIncident', params: { incidentId: incidentId.toString() } });
            });
    }
}
