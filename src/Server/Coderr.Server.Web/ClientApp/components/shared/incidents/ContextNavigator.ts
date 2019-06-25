import { ApiClient } from '../../../services/ApiClient';
import { AppRoot } from '../../../services/AppRoot';
import { GetReport, GetReportList, GetReportListResult, GetReportListResultItem, GetReportResult, GetReportResultContextCollection } from "../../../dto/Core/Reports";
import Vue from "vue";
import { Component, Prop, Watch } from "vue-property-decorator";

@Component
export default class ContextNavigatorComponent extends Vue {
    private apiClient: ApiClient = AppRoot.Instance.apiClient;
    private static readonly selectCollectionTitle: string = '(select collection)';
    reports: GetReportListResultItem[] = [];
    currentCollection = new GetReportResultContextCollection();
    currentCollectionName: string = '';
    currentReport = new GetReportResult();
    currentReportName = '';
    @Prop() incidentId: number;
    @Prop() showAnalyzeFooter: boolean;

    mounted() {
    }

    @Watch('incidentId')
    onPropertyChanged(value: string, oldValue: string) {
        var incidentId = parseInt(value, 10);
        this.loadReports(incidentId);
    }

    loadReports(incidentId: number) {
        var q = new GetReportList();
        q.IncidentId = incidentId;
        q.PageNumber = 1;
        q.PageSize = 50;
        AppRoot.Instance.apiClient.query<GetReportListResult>(q)
            .then(list => {
                this.reports = list.Items;
                if (this.reports.length > 0) {
                    this.loadReport(this.reports[0].Id);
                }
            });

    }
    loadReport(reportId: number) {
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
                    this.currentCollectionName = ContextNavigatorComponent.selectCollectionTitle;
                }
            });
    }

    private loadCollection(name: string) {
        if (name === ContextNavigatorComponent.selectCollectionTitle || name === '') {
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

}