import { AppRoot } from "../../../services/AppRoot";
import { GetOverview, GetOverviewResult } from "../../../dto/Web/Overview"
import { FindIncidentsResultItem } from "../../../dto/Core/Incidents"
import { GetApplicationOverview, GetApplicationOverviewResult } from "../../../dto/Core/Applications"
import * as Mine from "../../../dto/Common/Mine"
import Vue from "vue";
import { Component, Watch } from "vue-property-decorator";
import Chartist from "chartist";
import "chartist/dist/chartist.css";
import * as moment from "moment";

interface ISeries {
    name?: string;
    data: number[];
}
interface ILegend {
    className: string;
    name: string;
}

@Component
export default class DiscoverComponent extends Vue {
    private static activeBtnTheme: string = 'btn-dark';

    applicationId: number = 0;

    // summary, changes when time window changes
    reportCount: number = 0;
    incidentCount: number = 0;
    feedbackCount: number = 0;
    followers: number = 0;

    myIncidents: FindIncidentsResultItem[] = [];
    myBestSuggestion: Mine.ListMySuggestedItem | null = null;

    legend: ILegend[] = [];

    @Watch('$route.params.applicationId')
    onApplicationChanged(value: string, oldValue: string) {
        if (!value) {
            this.applicationId = 0;
            this.loadGenericOverview();
            return;
        }

        if (this.$route.fullPath.indexOf('/discover/') === -1) {
            return;
        }

        var applicationId = parseInt(value);
        this.loadApplication(applicationId);
    }

    created() {
        if (this.$route.params.applicationId && this.$route.params.applicationId !== '0') {
            this.applicationId = parseInt(this.$route.params.applicationId, 10);
            this.loadApplication(this.applicationId);
        } else {
            this.loadGenericOverview();
        }
    }

    mounted() {
    }

    assignBestToMe() {

    }

    private loadApplication(applicationId: number) {
        var q = new GetApplicationOverview();
        q.ApplicationId = applicationId;
        q.NumberOfDays = 30;
        AppRoot.Instance.apiClient.query<GetApplicationOverviewResult>(q)
            .then(result => {
                this.incidentCount = result.StatSummary.Incidents;
                this.reportCount = result.StatSummary.Reports;
                this.feedbackCount = result.StatSummary.UserFeedback;
                this.followers = result.StatSummary.Followers;
                this.displayChartForApplication(result);
            });

        AppRoot.Instance.incidentService.getMine(applicationId)
            .then(result => {
                this.myIncidents = result;
            });
    }

    private loadGenericOverview() {
        var q = new GetOverview();
        q.NumberOfDays = 30;
        AppRoot.Instance.apiClient.query<GetOverviewResult>(q)
            .then(result => {
                this.incidentCount = result.StatSummary.Incidents;
                this.reportCount = result.StatSummary.Reports;
                this.feedbackCount = result.StatSummary.UserFeedback;
                this.followers = result.StatSummary.Followers;
                this.displayChart(result);
            });

        AppRoot.Instance.incidentService.getMine()
            .then(result => {
                this.myIncidents = result;
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
            axisY: {
                onlyInteger: true,
                offset: 0
            },
            axisX: {
                labelInterpolationFnc(value: any, index: number, labels: any) {
                    if (index % 3 !== 0) {
                        return '';
                    }
                    return moment(value).format('MMM D');
                }
            }
        };
        var data = {
            labels: labels,
            series: series
        };
        console.log(data);
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
            data: stats.Incidents
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
                onlyInteger: true,
                offset: 0
            },
            axisX: {
                labelInterpolationFnc(value: any, index: number, labels: any) {
                    if (index % 3 !== 0) {
                        return '';
                    }
                    return moment(value).format('MMM D');
                }
            }
        };
        var data = {
            labels: labels,
            series: series
        };
        console.log(data);
        new Chartist.Line('.ct-chart', data, options);
    }

}
