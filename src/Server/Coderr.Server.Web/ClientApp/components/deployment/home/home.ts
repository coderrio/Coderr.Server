import { AppRoot } from '@/services/AppRoot';
import {
    GetVersionHistory, GetVersionHistoryResult, GetVersionHistorySeries,
    GetApplicationVersions, GetApplicationVersionsResult, GetApplicationVersionsResultItem
} from "@/dto/Modules/Versions";
import * as insights from "@/dto/Common/Insights";
import { Component, Mixins } from "vue-property-decorator";
import { AppAware } from "@/AppMixins";
import Chartist from "chartist";
import * as helpers from "@/helpers";

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

export interface IChartData {
    labels: Date[],
    series: ISeries[];
}

export interface IIndicatorWrapper {
    indicator: insights.GetInsightResultIndicator;
    alternatives: insights.GetInsightResultIndicator[];
}

export interface IIndicatorChart {
    legend: ILegend[];
    name: string;
    title: string;
    description: string;
    comment: string;
    data: IChartData;
    visible: boolean;
    chart: Chartist.IChartistLineChart;
    indicator: insights.GetInsightResultIndicator;
    toplistVisible: boolean;

    /**
     * Used to attach alternatives (like Highest amount of incidents)
     */
    alternatives: IIndicatorChart[];
}

@Component
export default class DeploymentHomeComponent extends Mixins(AppAware) {
    applicationId: number | null = null;
    history: GetVersionHistoryResult | null = null;
    versions: IVersion[] = [];
    showNoData = true;
    charts: IIndicatorChart[] = [];
    insightResult: insights.GetInsightsResult = null;
    insights: IIndicatorWrapper[] = [];
    showMore = true;

    created() {
        this.onApplicationChanged(this.onAppSelected);
        var appIdStr = this.$route.params.applicationId;
        if (appIdStr) {
            this.applicationId = parseInt(appIdStr, 10);
            this.loadData(this.applicationId);
        }
        else {
            this.loadInsights(0);
        }
    }


    mounted() {
    }

    loadData(applicationId?: number) {
        if (!applicationId) {
            return;
        }

        var q2 = new GetApplicationVersions();
        q2.ApplicationId = applicationId;
        AppRoot.Instance.apiClient.query<GetApplicationVersionsResult>(q2)
            .then(x => {
                if (x.Items.length === 0) {
                    return;
                }

                this.showNoData = false;
                this.versions = [];
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

                        v.incidentPercentage = this.calculatePercentage(previousVersion.IncidentCount, dto.IncidentCount);
                        if (v.incidentPercentage < 0) {
                            v.incidentSign = "fa-arrow-down text-success";
                        } else if (v.incidentPercentage > 0) {
                            if (v.incidentPercentage === Infinity) {
                                v.incidentPercentage = 0;
                            }
                            v.incidentSign = "fa-arrow-up text-danger";
                        }
                        v.reportPercentage = this.calculatePercentage(previousVersion.ReportCount, dto.ReportCount);
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

        this.loadInsights(this.applicationId);
    }

    showGraph(graph: IIndicatorChart, parent: IIndicatorChart) {
        parent.visible = false;
        parent.alternatives.forEach(x => {
            x.visible = false;
        });
        graph.visible = true;
        this.$nextTick(() => {
            graph.chart.update(null);
        });
        graph.toplistVisible = false;

    }

    showTopList(graph: IIndicatorChart) {
        graph.visible = false;
        graph.alternatives.forEach(x => {
            x.visible = false;
        });
        graph.toplistVisible = true;
    }

    private onAppSelected(applicationId: number) {
        if (!applicationId) {
            this.applicationId = null;
            this.showInsights(0);
        } else {
            this.applicationId = applicationId;
            this.loadData(this.applicationId);
        }
    }

    private loadInsights(applicationId: number) {
        if (!this.insightResult) {
            var q3 = new insights.GetInsights();
            q3.ApplicationId = applicationId;
            AppRoot.Instance.apiClient.query<insights.GetInsightsResult>(q3)
                .then(x => {
                    this.insightResult = x;
                    if (this.showNoData) {
                        this.showNoData = this.insightResult.Indicators.length === 0;
                    }
                    this.showInsights(applicationId);
                });
        } else {
            this.showInsights(applicationId);
        }

    }

    private showInsights(applicationId: number) {
        this.showMore = true;
        this.insights.length = 0;
        if (!applicationId) {
            let allItems: IIndicatorWrapper[] = [];
            this.insightResult.Indicators.forEach(x => {
                if (x.IsAlternative) {
                    var root = allItems.find(y => y.indicator.Name === x.Name);
                    if (root) {
                        x.Name = `${x.Name}-alt${root.alternatives.length}`;
                        root.alternatives.push(x);
                    }
                } else {
                    allItems.push({
                        indicator: x,
                        alternatives: []
                    });
                }
            });
            this.insights = allItems;
        } else {
            let indicators = this.insightResult
                .ApplicationInsights
                .find(x => x.Id === applicationId)
                .Indicators;
            let allItems: IIndicatorWrapper[] = [];
            indicators.forEach(x => {
                if (x.IsAlternative) {
                    var root = allItems.find(y => y.indicator.Name === x.Name);
                    if (root) {
                        x.Name = `${x.Name}-alt${root.alternatives.length}`;
                        root.alternatives.push(x);
                    }
                } else {
                    allItems.push({
                        indicator: x,
                        alternatives: []
                    });
                }
            });
            this.insights = allItems;

        }
        this.charts.length = 0;
        this.insights.forEach(x => {
            var chart = this.buildInsightChart(x.indicator, this.insightResult.TrendDates, applicationId === 0);
            if (chart) {
                this.charts.push(chart);
            }

            x.alternatives.forEach(child => {
                var alt = this.buildInsightChart(child, this.insightResult.TrendDates, applicationId === 0);
                if (alt) {
                    alt.visible = false;
                    chart.alternatives.push(alt);
                }
            });
        });

        this.$nextTick(() => {
            this.renderCharts();
        });
    }

    private renderCharts() {
        this.charts.forEach(x => {
            x.chart = this.createGraphElement(x.name, x.data);
            x.alternatives.forEach(child => {
                child.chart = this.createGraphElement(`${child.name}`, child.data);
            });
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

    createGraphElement(insightName: string, data: IChartData): Chartist.IChartistLineChart {

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
                    return helpers.monthDay(value);
                }
            },
            height: '300px'
        };
        const node = document.getElementById(`indicator-chart-${insightName}`);
        if (!node) {
            return null;
        }
        return new Chartist.Line(`#indicator-chart-${insightName}`, data, options);
    }

    private getValues(trendValues: insights.TrendValue[]): any[] {
        var result: any[] = [];
        trendValues.forEach(x => {
            result.push(x.Value);
        });
        return result;
    }

    private getNormalizedValues(trendValues: insights.TrendValue[]): any[] {
        var result: any[] = [];
        trendValues.forEach(x => {
            result.push(x.Normalized);
        });
        return result;
    }

    private buildInsightChart(indicator: insights.GetInsightResultIndicator, dates: string[], isSystemIndicator: boolean): IIndicatorChart | null {
        if (!indicator) {
            throw new Error("No indicator");
        }
        if (!indicator.TrendLines || !this.insightResult.TrendDates) {
            throw new Error(`No trend lines for indicator ${indicator.Name}`);
        }

        var labels: Date[] = [];
        var legend: ILegend[] = [];
        var series: ISeries[] = [];
        var charStart = 97;//'a'

        dates.forEach(item => {
            labels.push(new Date(item));
        });

        var allZero = true;

        indicator.TrendLines.forEach(x => {
            var total = 0;
            x.TrendValues.forEach(y => {
                total += y;
            });
            if (total < 1) {
                return;
            }

            var hasNormalized = true;
            x.TrendValues.forEach(x => {
                if (isNaN(x.Normalized)) {
                    hasNormalized = false;
                }
            });
            allZero = false;
            var aSeries: ISeries = {
                name: x.DisplayName,
                data: hasNormalized
                    ? this.getNormalizedValues(x.TrendValues)
                    : this.getValues(x.TrendValues)
            };
            if (isSystemIndicator) {
                console.log(indicator.Name, hasNormalized);
            }
            
            series.push(aSeries);
            legend.push({
                name: x.DisplayName,
                className: 'ct-series-' + String.fromCharCode(charStart++)
            });

            if (indicator.CanBeNormalized) {
                //var aSeries2: ISeries = {
                //    name: x.DisplayName,
                //    data: this.getNormalized(x.TrendValues)
                //};
                //console.log('normalized', JSON.stringify(this.getNormalized(x.TrendValues)));
                //series.push(aSeries2);

                //legend.push({
                //    name: "Application average (per developer)",
                //    className: 'ct-series-' + String.fromCharCode(charStart++)
                //});
            }
        });
        if (allZero) {
            return null;
        }
        var systemIndicator = this.insightResult.Indicators.find(x => x.Name === indicator.Name);
        if (systemIndicator != null) {
            //console.log('systemindicator', systemIndicator);
            if (indicator.TrendLines && indicator.TrendLines.length > 0 && false) {
                var aSeries2: ISeries = {
                    name: indicator.TrendLines[0].DisplayName,
                    data: this.getValues(systemIndicator.TrendLines[0].TrendValues),

                };
                series.push(aSeries2);

                legend.push({
                    name: systemIndicator.TrendLines[0].DisplayName,
                    className: 'company-average ct-series-' + String.fromCharCode(charStart++)
                });
            }
        }

        return {
            name: indicator.Name,
            title: indicator.Title,
            comment: indicator.Comment,
            description: indicator.Description,
            legend: legend,
            visible: true,
            chart: null,
            indicator: indicator,
            toplistVisible: false,
            data: {
                labels: labels,
                series: series
            },
            alternatives: []
        };
    }

    //private displayChart(stats: GetVersionHistoryResult) {
    //    var labels: Date[] = [];
    //    this.legend.length = 0;
    //    var series: ISeries[] = [];
    //    var charStart = 97;//'a'


    //    stats.IncidentCounts.forEach(data => {
    //        var aSeries: ISeries = {
    //            name: data.Name,
    //            data: data.Values,
    //        };
    //        series.push(aSeries);

    //        this.legend.push({
    //            name: data.Name,
    //            className: 'ct-series-' + String.fromCharCode(charStart++)
    //        });
    //    });

    //    for (var i = 0; i < stats.Dates.length; i++) {
    //        var value = new Date(stats.Dates[i]);
    //        labels.push(value);
    //    }

    //    var options = {
    //        axisY: {
    //            onlyInteger: true,
    //            offset: 0
    //        },
    //        axisX: {
    //            labelInterpolationFnc(value: any, index: number, labels: any) {
    //                //if (index % 3 !== 0) {
    //                //    return '';
    //                //}
    //                return moment(value).format('MMM D');
    //            }
    //        }
    //    };
    //    var data = {
    //        labels: labels,
    //        series: series
    //    };
    //    if (!document.getElementById('.ct-chart')) {
    //        setTimeout(() => {
    //            new Chartist.Line('.ct-chart', data, options);
    //        },
    //            500);
    //    }

    //}

}
