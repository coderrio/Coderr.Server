import { MyIncidents, IMyIncident } from "../myincidents";
import { ApplicationMember } from "@/services/applications/ApplicationService";
import { IHighlight, IncidentService } from "@/services/incidents/IncidentService";
import { ApiClient } from "@/services/ApiClient";
import { AppRoot } from "@/services/AppRoot";
import { GetIncidentResult } from "@/dto/Core/Incidents";
import { GetApplicationVersions, GetApplicationVersionsResult } from "@/dto/Core/Applications";
import { GetReportList, GetReportListResult, GetReportListResultItem, GetReport, GetReportResult, GetReportResultContextCollection } from "@/dto/Core/Reports";
import { Component, Vue } from "vue-property-decorator";
import * as workItems from "@/common/services/WorkItemService";

declare global {
    interface Window {
        gtag(type: string, type2: string, data: any): void;
    }
}

@Component
export default class AnalyzeIncidentComponent extends Vue {
    private static activeBtnTheme: string = "btn-dark";
    private apiClient: ApiClient = AppRoot.Instance.apiClient;
    private static readonly selectCollectionTitle: string = "(select collection)";
    private team: ApplicationMember[] = [];

    applicationName = "";
    name = "";
    incidentId = 0;
    incident = new GetIncidentResult();
    reports: GetReportListResultItem[] = [];
    currentReport = new GetReportResult();
    currentCollection = new GetReportResultContextCollection();

    currentReportName = "(select report)";
    currentCollectionName: string = "";

    closeVersion = '';

    highlights: IHighlight[] = [];

    workItem: workItems.IWorkItem = null;
    workItemIntegration: workItems.IIntegration = { title:'', name:'' };
    haveWorkItem: boolean | null = null;
    haveWorkItemIntegration: boolean | null = null;
    showCreateWorkItemButton: boolean = false;

    created() {
        // required for contextnavigator
        this.incidentId = parseInt(this.$route.params.incidentId, 10);
    }

    mounted() {
        this.loadIncident();
        MyIncidents.Instance.subscribeOnSelectedIncident(this.selectIncident);
    }

    destroyed() {
        MyIncidents.Instance.unsubscribe(this.selectIncident);
    }

    reAssign() {
        AppRoot.modal({
            contentId: "assignToModal",
            showFooter: false
        }).then(result => {
            var value = result.pressedButton.value;
            var accountId = parseInt(value, 10);
            var member = <ApplicationMember>this.team.find((x, index) => x.id === accountId);
            AppRoot.Instance.incidentService
                .assign(this.incident.Id, accountId, member.name)
                .then(x => AppRoot.notify("Incident have been assigned", "fa-check"));
        });
    }

    closeIncident() {
        AppRoot.Instance.incidentService.showClose(this.incident.Id, "CloseBody")
            .then(x => {
                if (x.requiresStatusUpdate) {
                    this.$router.push({
                        name: "notifyUsers",
                        params: { incidentId: this.incident.Id.toString() }
                    });
                    return;
                }
            });
    }

    addToTfs() {

    }

    private trimVersion(version: string): string {
        let parts = version.split(".");
        if (parts.length > 3 && parts[parts.length - 1] === "0") {
            parts.splice(parts.length - 1, 1);
        }
        return parts.join('.');
    }

    private bumpVersion(version: string): string {
        const parts = version.split(".");
        var value = parseInt(parts[parts.length - 1]) + 1;
        parts[parts.length - 1] = value.toString();
        return parts.join('.');
    }

    private loadIncident() {
        AppRoot.Instance.incidentService.get(this.incidentId)
            .then(incident => {
                this.incident = incident;
                if (this.incident.Tags.indexOf("demo") === -1 && window.gtag) {
                    setTimeout(() => {
                            window.gtag('event', 'conversion', { 'send_to': 'AW-1029996736/49DdCJf0_vABEMCBkusD' });
                        },
                        1000);
                }

                AppRoot.Instance.applicationService.getTeam(incident.ApplicationId)
                    .then(x => {
                        this.team = x;
                    });

                AppRoot.Instance.incidentService.collectHighlightedData(incident)
                    .then(data => {
                        this.highlights = data;
                    });

                AppRoot.Instance.applicationService.get(incident.ApplicationId)
                    .then(x => {
                        this.applicationName = x.name;
                    });

                var query = new GetApplicationVersions();
                query.ApplicationId = this.incident.ApplicationId;
                this.apiClient.query<GetApplicationVersionsResult>(query).then(x => {
                    var version = x.Items[0].Version;
                    version = this.trimVersion(version);
                    this.closeVersion = this.bumpVersion(version);

                    // Couldn't get binding to work.
                    // fix this if you know Vue better than me :)
                    document.querySelector("[name='version']").setAttribute('value', this.closeVersion);
                });

                this.findIntegration(incident.ApplicationId);
                this.loadWorkItem();
            });

        var q = new GetReportList();
        q.IncidentId = this.incidentId;
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
        if (name === AnalyzeIncidentComponent.selectCollectionTitle || name === "") {
            name = null;
            var namesToFind = ["ContextData", "ViewModel", "HttpRequest", "ExceptionProperties"];
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
                        if (kvp.Value.substr(0, 1) !== "<") {
                            kvp.Value = '<img src="data:image/png;base64, ' + kvp.Value + '" />';
                        }
                    }
                }
                return;
            }


        }

    }

    createWorkItem() {
        const service = new workItems.WorkItemService();
        service.createWorkItem(this.incident.ApplicationId, this.incidentId);
        this.showCreateWorkItemButton = false;
        AppRoot.notify("Work item have been created.");
    }

    private async loadWorkItem() {
        const service = new workItems.WorkItemService();
        this.workItem = await service.getWorkItem(this.incidentId);
        this.haveWorkItem = this.workItem != null;
        this.toggleWorkItemUi();
    }


    private async findIntegration(applicationId: number) {
        const service = new workItems.WorkItemService();
        this.workItemIntegration = await service.findIntegration(applicationId);
        this.haveWorkItemIntegration = this.workItemIntegration != null;
        this.toggleWorkItemUi();
    }

    private toggleWorkItemUi() {
        this.showCreateWorkItemButton = this.haveWorkItemIntegration && this.haveWorkItem === false;
    }


    selectIncident(myIncident: IMyIncident | null): void {
        if (myIncident == null) {
            this.$router.push({ name: "analyzeHome" });
        } else {
            this.incidentId = myIncident.incidentId;
            this.loadIncident();
        }
    }
}
