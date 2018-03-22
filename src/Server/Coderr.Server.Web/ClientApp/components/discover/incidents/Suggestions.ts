import { AppRoot } from "../../../services/AppRoot";
import { FindIncidents, FindIncidentsResult, FindIncidentsResultItem, IncidentOrder } from "../../../dto/Core/Incidents";
import Vue from "vue";
import { Component } from "vue-property-decorator";

interface ISuggestion {
    applicationId: string;
    applicationName: string;
    incidentId: number;
    incidentName: string;
    reason: string;
}

@Component
export default class SuggestionsComponent extends Vue {
    private static activeBtnTheme$: string = 'btn-dark';

    //chartData: [['Jan', 44], ['Feb', 27], ['Mar', 60], ['Apr', 55], ['May', 37], ['Jun', 40], ['Jul', 69], ['Aug', 33], ['Sept', 76], ['Oct', 90], ['Nov', 34], ['Dec', 22]];

    applicationId: number | null = 0;
    suggestions: ISuggestion[] = [];
    showEmpty = false;

    created() {
        if (this.$route.params.applicationId) {
            this.applicationId = parseInt(this.$route.params.applicationId, 10);
        }


        var query = new FindIncidents();
        query.IsNew = true;
        query.IsAssigned = false;
        query.IsIgnored = false;
        if (this.applicationId) {
            query.ApplicationIds = [this.applicationId];
        }

        // get oldest
        query.ItemsPerPage = 1;
        query.PageNumber = 1;
        query.SortAscending = false;

        AppRoot.Instance.apiClient.query<FindIncidentsResult>(query)
            .then(x => {
                if (x.Items.length > 0) {
                    var item = x.Items[0];
                    var suggestion: ISuggestion = {
                        incidentId: item.Id,
                        applicationId: item.ApplicationId,
                        applicationName: item.ApplicationName,
                        incidentName: item.Name,
                        reason: 'Oldest incident (' + item.CreatedAtUtc.toLocaleDateString() + ")"
                    };

                    this.suggestions.push(suggestion);
                }
                this.showEmpty = x.Items.length === 0;
            });

        query.SortAscending = true;
        AppRoot.Instance.apiClient.query<FindIncidentsResult>(query)
            .then(x => {
                if (x.Items.length > 0) {
                    var item = x.Items[0];
                    var suggestion: ISuggestion = {
                        incidentId: item.Id,
                        applicationId: item.ApplicationId,
                        applicationName: item.ApplicationName,
                        incidentName: item.Name,
                        reason: 'Newest incident (' + new Date(item.CreatedAtUtc).toLocaleString() + ")"
                    };

                    this.suggestions.push(suggestion);
                }
            });

        query.SortAscending = false;
        query.SortType = IncidentOrder.MostReports;
        AppRoot.Instance.apiClient.query<FindIncidentsResult>(query)
            .then(x => {
                if (x.Items.length > 0) {
                    var item = x.Items[0];
                    var suggestion: ISuggestion = {
                        incidentId: item.Id,
                        applicationId: item.ApplicationId,
                        applicationName: item.ApplicationName,
                        incidentName: item.Name,
                        reason: 'Most reports (' + item.ReportCount + " reports)"
                    };

                    this.suggestions.push(suggestion);
                }
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
                console.log('assign result', result);
                this.$router.push({ name: 'analyzeIncident', params: { incidentId: incidentId.toString() } });
            });
    }

}
