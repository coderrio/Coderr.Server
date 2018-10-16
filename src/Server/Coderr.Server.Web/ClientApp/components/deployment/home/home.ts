import { AppRoot } from '../../../services/AppRoot';
import {
    GetVersionHistory, GetVersionHistoryResult, GetVersionHistorySeries,
    GetApplicationVersions, GetApplicationVersionsResult, GetApplicationVersionsResultItem
} from "../../../dto/Modules/Versions";
import Vue from "vue";
import { Component, Watch } from "vue-property-decorator";
import Chartist from "chartist";
import moment from "moment";


interface ISeries {
    name?: string;
    data: number[];
}
interface ILegend {
    className: string;
    name: string;
}

interface IVersion {
    version: string;
    versionForRoutes: string;
    reportCount: number;
    reportSign?: "fa-arrow-up text-danger" | "fa-arrow-down text-success";
    reportPercentage?: number;
    incidentCount: number;
    incidentSign?: "fa-arrow-up text-danger" | "fa-arrow-down text-success";
    incidentPercentage?: number;
}

@Component
export default class DeploymentHomeComponent extends Vue {
    applicationId: number | null = null;
    history: GetVersionHistoryResult | null = null;
    versions: IVersion[] = [];
    legend: ILegend[] = [];

    created() {
        var appIdStr = this.$route.params.applicationId;
        if (appIdStr) {
            this.applicationId = parseInt(appIdStr, 10);
            this.loadData(this.applicationId);
        }
    }


    mounted() {
    }


    @Watch('$route.params.applicationId')
    onApplicationChanged(value: string, oldValue: string) {
        if (!value) {
            this.applicationId = null;
        } else {
            this.applicationId = parseInt(value, 10);
        }

        this.loadData(this.applicationId);
    }

    loadData(applicationId?: number) {
        var q = new GetVersionHistory();
        q.ApplicationId = applicationId;
        AppRoot.Instance.apiClient.query<GetVersionHistoryResult>(q)
            .then(x => {
                this.history = x;
                this.displayChart(x);
            });

        var q2 = new GetApplicationVersions();
        q2.ApplicationId = applicationId;
        AppRoot.Instance.apiClient.query<GetApplicationVersionsResult>(q2)
            .then(x => {
                for (var i = 0; i < x.Items.length; i++) {
                    let dto = x.Items[i];
                    
                    var v: IVersion = {
                        version: dto.Version,
                        versionForRoutes: dto.Version.replace(/\./g, "_"),
                        reportCount: dto.ReportCount,
                        incidentCount: dto.IncidentCount
                    };

                    if (i < x.Items.length - 1) {
                        var previousVersion = x.Items[i + 1];

                        v.incidentPercentage = this.calculatePercentage (previousVersion.IncidentCount, dto.IncidentCount);
                        if (v.incidentPercentage < 0) {
                            v.incidentSign = "fa-arrow-down text-success";
                        } else if (v.incidentPercentage > 0) {
                            if (v.incidentPercentage === Infinity) {
                                v.incidentPercentage = 0;
                            }
                            v.incidentSign = "fa-arrow-up text-danger";
                        }
                        v.reportPercentage = this.calculatePercentage (previousVersion.ReportCount, dto.ReportCount);
                        if (v.reportPercentage < 0) {
                            v.reportSign = "fa-arrow-down text-success";
                        } else if (v.reportPercentage > 0) {
                            if (v.reportPercentage === Infinity) {
                                v.reportPercentage = 0;
                            }
                            v.reportSign = "fa-arrow-up text-danger";
                        }
                    }

                    this.versions.push(v);
                }
            });
    }

    private calculatePercentage(before: number, after: number): number {
        //number of reports increased
        if (before < after) {
            let diff = after - before;
            return Math.round(diff / before * 100);
        } else {
            let diff = before - after;
            return Math.round(-(diff / before * 100));
        }
    }

    private displayChart(stats: GetVersionHistoryResult) {
        var labels: Date[] = [];
        this.legend.length = 0;
        var series: ISeries[] = [];
        var charStart = 97;//'a'


        stats.IncidentCounts.forEach(data => {
            var aSeries: ISeries = {
                name: data.Name,
                data: data.Values,
            };
            series.push(aSeries);

            this.legend.push({
                name: data.Name,
                className: 'ct-series-' + String.fromCharCode(charStart++)
            });
        });

        for (var i = 0; i < stats.Dates.length; i++) {
            var value = new Date(stats.Dates[i]);
            labels.push(value);
        }

        var options = {
            axisY: {
                onlyInteger: true,
                offset: 0
            },
            axisX: {
                labelInterpolationFnc(value: any, index: number, labels: any) {
                    //if (index % 3 !== 0) {
                    //    return '';
                    //}
                    return moment(value).format('MMM D');
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
