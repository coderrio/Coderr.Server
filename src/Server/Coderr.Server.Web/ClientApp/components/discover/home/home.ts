import { PubSubService } from "@/services/PubSub";
import * as MenuApi from "@/services/menu/MenuApi";
import { AppRoot } from "@/services/AppRoot";
import * as Mine from "@/dto/Common/Mine"
import { GetOverview, GetOverviewResult } from "@/dto/Web/Overview"
import { GetApplicationOverview, GetApplicationOverviewResult, GetApplicationList, ApplicationListItem } from "@/dto/Core/Applications"
import { Component, Mixins } from "vue-property-decorator";
import { AppAware } from "@/AppMixins";
import Chartist from "chartist";
import { FindIncidentsResultItem } from "@/dto/Core/Incidents";
import * as helpers from "@/helpers";

interface ISeries {
    name?: string;
    data: number[];
}
interface ILegend {
    className: string;
    name: string;
}

@Component
export default class DiscoverHomeComponent extends Mixins(AppAware) {
    private static activeBtnTheme: string = 'btn-dark';

    applicationId: number = 0;
    firstApplicationId = 0;
    destroyed$ = false;

    // summary, changes when time window changes
    reportCount: number = -1;
    incidentCount: number = 0;
    feedbackCount: number = 0;
    followers: number = 0;

    myIncidents: FindIncidentsResultItem[] = [];
    myBestSuggestion: Mine.ListMySuggestedItem | null = null;

    comment: string = "";
    legend: ILegend[] = [];

    created() {
        if (this.$route.params.applicationId && this.$route.params.applicationId !== '0') {
            this.applicationId = parseInt(this.$route.params.applicationId, 10);
            this.loadApplication(this.applicationId);
            this.firstApplicationId = this.applicationId;
        } else {
            this.loadGenericOverview();
            this.firstApplicationId = 0;
        }

        this.onApplicationChanged(applicationId => {
            if (this.applicationId === applicationId) {
                return;
            }
            if (applicationId === 0) {
                this.applicationId = 0;
                this.firstApplicationId = 0;
                this.loadGenericOverview();
                this.getBestNewSuggestion();
                return;
            }
            this.applicationId = applicationId;
            this.firstApplicationId = 0;
            if (this.$route.fullPath.indexOf('/discover') === -1) {
                return;
            }

            this.loadApplication(this.applicationId);
            this.getBestNewSuggestion();
        });

        AppRoot.Instance.apiClient.query<ApplicationListItem[]>(new GetApplicationList())
            .then(result => {
                if (this.destroyed$) {
                    return;
                }

                // invited without access
                if (result.length > 0) {
                    this.firstApplicationId = result[0].Id;
                }
            });

        this.getBestNewSuggestion();
    }

    mounted() {
    }

    beforeDestroy() {
        this.destroyed$ = true;
    }
    assignBestToMe() {
        AppRoot.Instance.incidentService.assignToMe(this.myBestSuggestion.Id)
            .then(x => {
                this.$router.push({ name: 'analyzeIncident', params: { 'incidentId': this.myBestSuggestion.Id.toString() } });
            });
    }

    private loadApplication(applicationId: number) {
        var route = this.$route;
        if (this.applicationId !== applicationId) {
            this.$router.push({ name: route.name, params: { applicationId: applicationId.toString() } });
        }

        var q = new GetApplicationOverview();
        q.ApplicationId = applicationId;
        q.NumberOfDays = 30;
        AppRoot.Instance.apiClient.query<GetApplicationOverviewResult>(q)
            .then(result => {
                if (this.destroyed$) {
                    return;
                }

                this.incidentCount = result.StatSummary.Incidents;
                this.reportCount = result.StatSummary.Reports;
                this.feedbackCount = result.StatSummary.UserFeedback;
                this.followers = result.StatSummary.Followers;
                this.displayChartForApplication(result);
            });

        this.getBestNewSuggestion();
    }

    private getBestNewSuggestion() {
        var query = new Mine.ListMyIncidents();
        if (this.applicationId > 0) {
            query.ApplicationId = this.applicationId;
        }
        AppRoot.Instance.apiClient.query<Mine.ListMyIncidentsResult>(query)
            .then(result => {
                if (result.Suggestions.length > 0) {
                    this.myBestSuggestion = result.Suggestions[0];
                    this.myBestSuggestion.Motivation = this.myBestSuggestion.Motivation.replace(/\r\n/g, ', ');
                } else {
                    this.myBestSuggestion = null;
                }
                this.comment = result.Comment;
            });
    }

    private loadGenericOverview() {
        var q = new GetOverview();
        q.NumberOfDays = 30;
        AppRoot.Instance.apiClient.query<GetOverviewResult>(q)
            .then(result => {
                if (this.destroyed$) {
                    return;
                }

                this.incidentCount = result.StatSummary.Incidents;
                this.reportCount = result.StatSummary.Reports;
                this.feedbackCount = result.StatSummary.UserFeedback;
                this.followers = result.StatSummary.Followers;
                this.displayChart(result);

                PubSubService.Instance.publish(MenuApi.MessagingTopics.IgnoredReportCountUpdated, result.MissedReports);
            });
    }

    private displayChart(stats: GetOverviewResult) {
        var labels: Date[] = [];
        this.legend.length = 0;
        var series: ISeries[] = [];
        var charStart = 97;//'a'
        stats.IncidentsPerApplication.forEach(x => {
            series.push({
                name: x.Label,
                data: x.Values
            });
            this.legend.push({
                name: x.Label,
                className: 'ct-series-' + String.fromCharCode(charStart++)
            });
        });

        for (var i = 0; i < stats.TimeAxisLabels.length; i++) {
            var value = new Date(stats.TimeAxisLabels[i]);
            labels.push(value);
        }


        var options = {
            fullWidth: true,
            chartPadding: {
                right: 40
            },
            axisY: {
                onlyInteger: true,
                offset: 0
            },
            axisX: {
                labelInterpolationFnc(value: any, index: number, labels: any): string {
                    if (index % 3 !== 0) {
                        return "";
                    }

                    return helpers.monthDay(value);
                }
            }
        };
        var data = {
            labels: labels,
            series: series
        };
        new Chartist.Line('.ct-chart', data, options);
    }

    private displayChartForApplication(stats: GetApplicationOverviewResult) {
        var labels: Date[] = [];
        this.legend.length = 0;
        var series: ISeries[] = [];
        var charStart = 97;//'a'
        series.push({
            name: 'Incidents',
            data: stats.Incidents
        });
        this.legend.push({
            name: 'Incidents',
            className: 'ct-series-a'
        });
        series.push({
            name: 'Reports',
            data: stats.ErrorReports
        });
        this.legend.push({
            name: 'Reports',
            className: 'ct-series-b'
        });

        for (var i = 0; i < stats.TimeAxisLabels.length; i++) {
            var value = new Date(stats.TimeAxisLabels[i]);
            labels.push(value);
        }

        var options = {
            axisY: {
                type: Chartist.AutoScaleAxis,
                onlyInteger: true,
                low: 0
            },
            axisX: {
                labelInterpolationFnc(value: Date, index: number, labels: any) {
                    if (index % 3 !== 0) {
                        return '';
                    }
                    return helpers.monthDay(value);
                }
            }
        };
        var data = {
            labels: labels,
            series: series
        };
        new Chartist.Line('.ct-chart', data, options);
    }

}
