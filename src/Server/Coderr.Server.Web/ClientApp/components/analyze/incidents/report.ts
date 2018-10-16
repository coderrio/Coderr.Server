import { PubSubService } from "../../../services/PubSub";
import { ApiClient } from '../../../services/ApiClient';
import { AppRoot } from '../../../services/AppRoot';
import { GetReportList, GetReportListResult, GetReportListResultItem, GetReport, GetReportResult, GetReportResultContextCollection } from "../../../dto/Core/Reports";
import Vue from "vue";
import { Component, Watch } from "vue-property-decorator";



@Component
export default class AnalyzeReportComponent extends Vue {
    private apiClient: ApiClient = AppRoot.Instance.apiClient;
    private static readonly selectCollectionTitle: string = '(select collection)';
    private currentIndex: number = 0;
    private currentPage: number = 1;
    private findStartItem: boolean = true;
    private pageSize = 50;

    indexInTotalSet = 0;
    totalCount = 0;

    reportedAt = new Date();
    stackTrace = '';
    reportId: number;
    incidentId: number;

    userFeedback: string|null = null;
    userOrEmail = 'user';

    reports: GetReportListResultItem[] = [];
    showNextButton= true;
    showPrevButton=false;

    contextCollections: GetReportResultContextCollection[] = [];
    currentCollectionName = '';
    currentCollection = new GetReportResultContextCollection();


    created() {
        this.incidentId = parseInt(this.$route.params.incidentId, 10);
        this.loadReports(this.incidentId, 1, true);

        // can be undefined when user requested 
        // to view reports in general (a specific one was not requested).
        // The loadReports method will select first report.
        if (this.$route.params.reportId) {
            this.reportId = parseInt(this.$route.params.reportId, 10);
            this.loadReport(this.reportId);
        }
    }

    prevReport() {
        this.currentIndex--;
        this.indexInTotalSet--;
        this.checkNavigationLinks();

        if (this.currentIndex < 0) {
            this.currentPage--;
            this.currentIndex = 0;
            this.loadReports(this.incidentId, this.currentPage, false);
        } else {
            this.loadReport(this.reports[this.currentIndex].Id);
        }
    }
    
    nextReport() {
        this.currentIndex++;
        this.indexInTotalSet++;
        this.checkNavigationLinks();

        if (this.currentIndex >= this.pageSize) {
            this.currentPage++;
            this.currentIndex = 0;
            this.loadReports(this.incidentId, this.currentPage, false);
        } else {
            this.loadReport(this.reports[this.currentIndex].Id);
        }

        
    }

    private checkNavigationLinks() {
        this.showPrevButton = this.indexInTotalSet > 0;

        // -1 since we are comparing a zero based index with a one based count
        this.showNextButton = this.indexInTotalSet < (this.totalCount - 1);
    }

    @Watch('$route.params.reportId')
    onNavigationChanged(value: string, oldValue: string) {
    }

    private loadReport(reportId: number) {
        if (!reportId) {
            throw new Error("Expected a reportId");
        }

        if (reportId !== this.reportId) {
            this.reportId = reportId;
            this.$router.push({ name: 'analyzeReport', params: { reportId: reportId.toString(), incidentId: this.incidentId.toString() } });
        }

        var q = new GetReport();
        q.ReportId = reportId;
        AppRoot.Instance.apiClient.query<GetReportResult>(q)
            .then(report => {
                this.stackTrace = report.StackTrace;
                this.reportedAt = report.CreatedAtUtc;
                this.contextCollections = report.ContextCollections;
                this.userFeedback = report.UserFeedback;
                if (report.EmailAddress != null) {
                    this.userOrEmail = report.EmailAddress;
                }

                if (report.ContextCollections.length > 0) {
                    var isFound = false;
                    report.ContextCollections.forEach(col => {
                        if (col.Name === this.currentCollectionName) {
                            isFound = true;
                        }
                    });
                    if (!isFound) {
                        this.currentCollectionName = report.ContextCollections[0].Name;
                    }
                    
                    this.loadCollection(this.currentCollectionName);
                } else {
                    this.currentCollection = new GetReportResultContextCollection();
                    this.currentCollectionName = AnalyzeReportComponent.selectCollectionTitle;
                }
            });
    }

    /**
     * Used to fetch more items since we are using paging.
     * @param incidentId
     * @param pageNumber
     * @param isEntryLoadForIncident
     */
    private loadReports(incidentId: number, pageNumber: number, isEntryLoadForIncident: boolean) {
        var q = new GetReportList();
        q.IncidentId = incidentId;
        q.PageNumber = pageNumber;
        q.PageSize = this.pageSize;
        if (pageNumber) {
            q.PageNumber = pageNumber;
            
        }

        AppRoot.Instance.apiClient.query<GetReportListResult>(q)
            .then(result => {
                this.reports = result.Items;
                this.currentIndex = 0;
                this.currentPage = result.PageNumber;
                this.totalCount = result.TotalCount;
                this.showNextButton = result.TotalCount > 1;
                // Used when we navigate out of the current page
                if (!isEntryLoadForIncident || this.reportId == null) {
                    if (result.Items.length > 0) {
                        this.loadReport(result.Items[0].Id);
                    }
                } else {
                    // when we have navigated directly to a specific report
                    // we need to find it so that prev/next links works correctly
                    for (var i = 0; i < this.reports.length; i++) {
                        if (result.Items[i].Id === this.reportId) {
                            this.currentIndex = i;
                            this.indexInTotalSet = i;
                            break;
                        }
                    }

                    this.checkNavigationLinks();
                }
                
            });
    }

    private loadCollection(name: string) {
        if (name === AnalyzeReportComponent.selectCollectionTitle || name === '') {
            name = this.contextCollections[0].Name;
        }

        for (var i = 0; i < this.contextCollections.length; i++) {
            var col = this.contextCollections[i];
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
