import { AppRoot } from "../../../services/AppRoot";
import { ListMyIncidents, ListMyIncidentsResult } from "@/dto/Common/Mine";
import { Component, Mixins } from "vue-property-decorator";
import { AppAware } from "@/AppMixins";

interface ISuggestion {
    applicationId: number;
    applicationName: string;
    incidentId: number;
    incidentName: string;
    reason: string;
    exceptionType: string;
}

@Component
export default class SuggestionsComponent extends Mixins(AppAware) {
    applicationId: number | null = null;
    suggestions: ISuggestion[] = [];
    showEmpty = false;

    created() {
        this.applicationId = AppRoot.Instance.currentApplicationId;
        this.onApplicationChanged(applicationId => {
            this.applicationId = applicationId;
            this.loadSuggestions(applicationId);
        })

        this.loadSuggestions(this.applicationId);
    }

    private loadSuggestions(applicationId?: number) {
        var query = new ListMyIncidents();
        if (applicationId) {
            query.ApplicationId = applicationId;
        }
        this.suggestions.length = 0;
        AppRoot.Instance.apiClient.query<ListMyIncidentsResult>(query)
            .then(result => {
                this.suggestions.length = 0;
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
