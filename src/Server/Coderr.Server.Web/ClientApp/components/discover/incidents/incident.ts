import { IHighlight } from "@/services/incidents/IncidentService";
import { AppRoot } from '@/services/AppRoot';
import { ApplicationMember } from "@/services/applications/ApplicationService";
import { GetIncidentResult, ReportDay, QuickFact } from "@/dto/Core/Incidents";
import { Component, Vue } from "vue-property-decorator";
import Chartist from "chartist";
import * as Reports from "../../../dto/Core/Reports";
import * as workItems from "@/common/services/WorkItemService";
import { DateTime } from 'luxon';


declare global {
    interface Window {
        gtag(type: string, type2: string, data: any): void;
    }
}

@Component
export default class IncidentComponent extends Vue {
    incidentId: number;
    incident: GetIncidentResult = new GetIncidentResult;
    isIgnored: boolean = false;
    isClosed = false;
    highlights: IHighlight[] = [];
    team: ApplicationMember[] = [];

    workItem: workItems.IWorkItem = null;
    workItemIntegration: workItems.IIntegration = { title: '', name: '' };
    haveWorkItem: boolean | null = null;
    haveWorkItemIntegration: boolean | null = null;
    showCreateWorkItemButton: boolean = false;

    created() {
        this.incident.Tags = [];
        this.incidentId = parseInt(this.$route.params.incidentId, 10);
        AppRoot.Instance.incidentService.get(this.incidentId)
            .then(result => {
                this.incident = result;
                this.isIgnored = result.IsIgnored;
                this.isClosed = result.IsSolved;
                result.Facts = result.Facts.filter(v => v.Value !== '0');
                if (this.incident.Tags.indexOf("demo") === -1 && window.gtag) {
                    setTimeout(() => {
                            window.gtag('event', 'conversion', { 'send_to': 'AW-1029996736/49DdCJf0_vABEMCBkusD' });
                        },
                        1000);
                }

                if (result.AssignedToId > 0) {
                    var fact = new QuickFact();
                    console.log(result.AssignedAtUtc, typeof result.AssignedAtUtc);
                    fact.Description = 'Assigned at ' + this.$options.filters.niceTime(result.AssignedAtUtc);
                    fact.Title = "Assigned to";
                    fact.Value = result.AssignedTo;
                    result.Facts.push(fact);
                }

                this.setHighlights(result);

                this.$nextTick(() => {
                    this.displayChart(result.DayStatistics);
                });

                AppRoot.Instance.applicationService.getTeam(result.ApplicationId)
                    .then(x => {
                        this.team = x;
                    });

                AppRoot.Instance.incidentService.collectHighlightedData(result)
                    .then(data => {
                        //this.highlights = data;
                    });

                this.findIntegration(this.incident.ApplicationId);
                this.loadWorkItem();
            });
    }

    ignore() {
        AppRoot.modal({
            title: "Ignore incident",
            htmlContent:
                "<p>This feature will hide incident from the search result and throw away all future error reports without analyzing them.</p>",
            submitButtonText: "Ignore reports",
            footerHint: "<a href=\"https://coderr.io/documentation/features/incident/close/\">Learn more about this feature</a>"
        }).then(x => {
            AppRoot.Instance.incidentService.ignore(this.incidentId)
                .then(x => {
                    AppRoot.notify('Incident marked as ignored.');
                    this.isIgnored = true;
                });
        });
    }

    assignToMe() {
        AppRoot.Instance.incidentService.assignToMe(this.incidentId)
            .then(x => {
                this.$router.push({ name: 'analyzeIncident', params: { incidentId: this.incidentId.toString() } });
            });
    }

    assignToSomeoneElse() {
        AppRoot.modal({
            contentId: 'assignToModal',
            showFooter: false
        }).then(result => {
            var value = result.pressedButton.value;
            var accountId = parseInt(value, 10);
            var member = <ApplicationMember>this.team.find((x, index) => x.id === accountId);
            AppRoot.Instance.incidentService
                .assign(this.incidentId, accountId, member.name)
                .then(x => AppRoot.notify('Incident have been assigned', 'fa-check'));
        });
    }


    mounted() {

    }

    close() {
        AppRoot.Instance.incidentService.showClose(this.incidentId, "CloseDialog")
            .then(result => {
                this.isClosed = true;
                if (result.requiresStatusUpdate) {
                    this.$router.push({
                        name: 'notifyUsers',
                        params: { incidentId: this.incident.Id.toString() }
                    });
                    return;
                }
            });
    }
    
    deleteIncident() {
        AppRoot.Instance.incidentService.delete(this.incidentId, "yes");
        AppRoot.notify("Incident have been scheduled for deletion.", 'fa-info', 'info');
        this.$router.push({ name: 'suggest'});
    }

    private setHighlights(incident: GetIncidentResult) {
        this.highlights.length = 0;
        incident.HighlightedContextData.forEach(x => {
            this.highlights.push({ name: x.Name, value: x.Value[0] });
        });

        var q = new Reports.GetReportList();
        q.IncidentId = incident.Id;
        q.PageNumber = 1;
        q.PageSize = 1;
        AppRoot.Instance.apiClient.query<Reports.GetReportListResult>(q)
            .then(list => {
                if (list.Items.length === 0) {
                    return;
                }

                var q = new Reports.GetReport();
                q.ReportId = list.Items[0].Id;
                AppRoot.Instance.apiClient.query<Reports.GetReportResult>(q)
                    .then(report => {
                        let collectionsToGet: string[] = [];
                        report.ContextCollections.forEach(x => {
                            if (x.Name !== "CoderrData") {
                                return;
                            }

                            x.Properties.forEach(y => {
                                if (y.Key === "HighlightCollections") {
                                    collectionsToGet = y.Value.split(",");
                                }
                                //if (y.Key === "HighlightProperties") {
                                //    collectionsToGet = y.Value.split(",");
                                //}

                            });
                        });
                        report.ContextCollections.forEach(x => {
                            var match = collectionsToGet.find(y => y === x.Name);
                            if (match) {
                                x.Properties.forEach(z => {
                                    this.highlights.push({ name: `${x.Name}.${z.Key}`, value: z.Value });
                                });
                            }
                        });

                    });

            });
    }

    private displayChart(days: ReportDay[]) {
        var labels: Date[] = [];
        var series: number[] = [];
        for (var i = 0; i < days.length; i++) {

            var value = new Date(days[i].Date);
            labels.push(value);
            series.push(days[i].Count);
        }

        var options = {
            axisY: {
                onlyInteger: true,
                offset: 0
            },
            axisX: {
                labelInterpolationFnc(value: Date, index: number, labels: any) {
                    if (index % 3 !== 0) {
                        return '';
                    }
                    return DateTime.fromJSDate(value).toFormat('LLL dd');
                }
            }
        };
        var data = {
            labels: labels,
            series: [{ data: series }],
        };
        new Chartist.Line('.ct-chart', data, options);
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
        console.log(this.workItemIntegration);
        this.toggleWorkItemUi();
    }

    private toggleWorkItemUi() {
        this.showCreateWorkItemButton = this.haveWorkItemIntegration && this.haveWorkItem === false;
    }
}
