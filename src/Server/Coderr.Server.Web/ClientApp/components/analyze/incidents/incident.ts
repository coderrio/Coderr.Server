import { MyIncidents, IMyIncident } from "../myincidents";
import { ApplicationMember } from "../../../services/applications/ApplicationService";
import { ApiClient } from '../../../services/ApiClient';
import { AppRoot } from '../../../services/AppRoot';
import { GetIncidentResult, GetIncidentStatistics, GetIncidentStatisticsResult } from "../../../dto/Core/Incidents";
import { GetReportList, GetReportListResult, GetReportListResultItem, GetReport, GetReportResult, GetReportResultContextCollection } from "../../../dto/Core/Reports";
import Vue from "vue";
import { Component, Watch } from "vue-property-decorator";



@Component
export default class AnalyzeIncidentComponent extends Vue {
    private static activeBtnTheme: string = 'btn-dark';
    private apiClient: ApiClient = AppRoot.Instance.apiClient;
    private static readonly selectCollectionTitle: string = '(select collection)';
    private team: ApplicationMember[] = [];

    name = '';
    incidentId = 0;
    incident = new GetIncidentResult();
    reports: GetReportListResultItem[] = [];
    currentReport = new GetReportResult();
    currentCollection = new GetReportResultContextCollection();

    currentReportName = '(select report)';
    currentCollectionName: string = '';

    created() {
        MyIncidents.Instance.subscribeOnSelectedIncident(x => {
            this.selectIncident(x);
        });
    }

    mounted() {
        var incidentId = parseInt(this.$route.params.incidentId, 10);
        this.loadIncident(incidentId);
    }

    destroyed() {
        MyIncidents.Instance.unsubscribe(this.selectIncident);
    }

    reAssign() {
        AppRoot.modal({
            contentId: 'assignToModal',
            showFooter: false
        }).then(result => {
            var value = result.pressedButton.value;
            var accountId = parseInt(value, 10);
            var member = <ApplicationMember>this.team.find((x, index) => x.id === accountId);
            AppRoot.Instance.incidentService
                .assign(this.incident.Id, accountId, member.name)
                .then(x => AppRoot.notify('Incident have been assigned', 'fa-check'));
        });
    }

    closeIncident() {
        AppRoot.Instance.incidentService.showClose(this.incident.Id, "CloseBody")
            .then(x => {
                if (x.requiresStatusUpdate) {
                    this.$router.push({
                        name: 'notifyUsers',
                        params: { incidentId: this.incident.Id.toString() }
                    });
                    return;
                }
            });
    }

    addToTfs() {

    }

    private loadIncident(id: number) {
        if (!id) {
            throw new Error("Expected an incidentId.");
        }

        this.incidentId = id;
        AppRoot.Instance.incidentService.get(id)
            .then(incident => {
                this.incident = incident;
                AppRoot.Instance.applicationService.getTeam(incident.ApplicationId)
                    .then(x => {
                        this.team = x;
                    });
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

    private embedScreenshots() {

    }

    private loadCollection(name: string) {
        if (name === AnalyzeIncidentComponent.selectCollectionTitle || name === '') {
            name = null;
            var namesToFind = ['ContextData', 'ViewModel', 'HttpRequest', 'ExceptionProperties'];
            this.currentReport.ContextCollections.forEach(x => {
                if (namesToFind.indexOf(x.Name) !== -1 && name == null) {
                    name = x.Name;
                }
            });

            if (name == null) {
                name = this.currentReport.ContextCollections[0].Name;
            }
        }

        for (var i = 0; i < this.currentReport.ContextCollections.length; i++) {
            var col = this.currentReport.ContextCollections[i];
            for (var j = 0; j < col.Properties.length; j++) {
                var prop = col.Properties[j];
                prop.Value = prop.Value.replace(/;;/g, "\r\n</br>");
            }

            if (col.Name === name) {
                this.currentCollection = col;
                this.currentCollectionName = name;
                if (name === "Screenshots") {
                    for (var j = 0; j < this.currentCollection.Properties.length; j++) {
                        var kvp = this.currentCollection.Properties[j];
                        if (kvp.Value.substr(0, 1) !== '<') {
                            kvp.Value = '<img src="data:image/png;base64, ' + kvp.Value + '" />';
                        }
                    }
                }
                return;
            }


        }

    }


    selectIncident(myIncident: IMyIncident | null): void {
        if (myIncident == null) {
            this.$router.push({ name: 'analyzeHome' });
        } else {
            this.loadIncident(myIncident.incidentId);
        }
    }
}
