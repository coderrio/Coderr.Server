import { PubSubService } from "../../../services/PubSub";
import { ApiClient } from '../../../services/ApiClient';
import { AppRoot } from '../../../services/AppRoot';
import { GetIncidentResult, GetIncidentStatistics, GetIncidentStatisticsResult } from "../../../dto/Core/Incidents";
import { GetReportList, GetReportListResult, GetReportListResultItem, GetReport, GetReportResult, GetReportResultContextCollection } from "../../../dto/Core/Reports";
import Vue from "vue";
import { Component, Watch } from "vue-property-decorator";



@Component
export default class AnalyzeIncidentComponent extends Vue {
    private static activeBtnTheme: string = 'btn-dark';
    private apiClient: ApiClient = new ApiClient('http://localhost:50473/cqs/');
    private static readonly selectCollectionTitle: string = '(select collection)';

    name = '';
    incident = new GetIncidentResult();
    isEmpty: boolean = false;
    reports: GetReportListResultItem[] = [];
    currentReport = new GetReportResult();
    currentCollection = new GetReportResultContextCollection();

    currentReportName = '(select report)';
    currentCollectionName: string = '';

    created() {
        if (!this.$route.params.incidentId) {
            AppRoot.Instance.incidentService.getMine().then(incidents => {
                if (incidents.length === 0) {
                    this.isEmpty = false;
                    return;
                } else {
                    let incidentId = incidents[0].Id;
                    this.loadIncident(incidentId);
                }
            });

        } else {
            let incidentId = parseInt(this.$route.params.incidentId, 10);
            this.loadIncident(incidentId);
        }
        
    }

    @Watch('$route.params.incidentId')
    onIncidentSelected(value: string, oldValue: string) {
        console.log('changed', value);
        var incidentId = parseInt(value, 10);
        this.loadIncident(incidentId);
    }

    reAssign() {

    }

    closeIncident() {

    }

    addToTfs() {

    }

    private loadIncident(id: number) {
        AppRoot.Instance.incidentService.get(id)
            .then(incident => {
                this.incident = incident;
                //incident.ContextCollections;
                //incident.
                //this.incident.Tags
            });

        var q = new GetReportList();
        q.IncidentId = id;
        AppRoot.Instance.apiClient.query<GetReportListResult>(q)
            .then(list => {
                this.reports = list.Items;
                if (this.reports.length > 0) {
                    this.loadReport(this.reports[0].Id);
                }
            });
    }


    private loadReport(reportId: number) {
        var q = new GetReport();
        q.ReportId = reportId;
        AppRoot.Instance.apiClient.query<GetReportResult>(q)
            .then(report => {
                this.currentReport = report;
                this.currentReportName = new Date(report.CreatedAtUtc).toLocaleString();
                //(<HTMLButtonElement>document.getElementById('reportChooser')).removeAttribute('disabled');
                if (report.ContextCollections.length > 0) {
                    this.loadCollection(this.currentCollectionName);
                } else {
                    this.currentCollection = new GetReportResultContextCollection();
                    this.currentCollectionName = AnalyzeIncidentComponent.selectCollectionTitle;
                }
            });
    }

    private loadCollection(name: string) {
        if (name === AnalyzeIncidentComponent.selectCollectionTitle || name === '') {
            name = this.currentReport.ContextCollections[0].Name;
        }

        for (var i = 0; i < this.currentReport.ContextCollections.length; i++) {
            var col = this.currentReport.ContextCollections[i];
            if (col.Name === name) {
                this.currentCollection = col;
                this.currentCollectionName = name;
                return;
            }
        }

    }


}
