/// <reference path="../../Scripts/Griffin.WebApp.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />

module OneTrueError.Application {
    import CqsClient = Griffin.Cqs.CqsClient;
    import Pager = Griffin.WebApp.Pager;
    import ActivationContext = Griffin.Yo.Spa.ViewModels.IActivationContext;
    import Yo = Griffin.Yo;

    export class DetailsViewModel implements Griffin.Yo.Spa.ViewModels.IViewModel {
        private incidentTable: IncidentTableViewModel;
        private pager: Pager;
        private applicationId: number;
        private applicationName: string;
        private ctx: ActivationContext;
        private chartOptions: morris.ILineOptions;
        private chart: morris.GridChart;
        private chartDays = 30;
        private filterVersion: string;

        constructor() {
        }

        getTitle(): string {
            const bc: Applications.IBreadcrumb[] = [
                { href: `/application/${this.applicationId}/`, title: this.applicationName }
            ];
            Applications.Navigation.breadcrumbs(bc);
            Applications.Navigation.pageTitle = this.applicationName;
            return this.applicationName;
        }

        totalIncidentCount = 0;

        activate(ctx: Griffin.Yo.Spa.ViewModels.IActivationContext) {
            this.incidentTable = new IncidentTableViewModel(ctx);
            this.ctx = ctx;
            this.applicationId = ctx.routeData["applicationId"];
            var self = this;

            var firstIsRun = false;
            var chartResult: Core.Applications.Queries.GetApplicationOverviewResult = null;
            var chartRendering = (result: Core.Applications.Queries.GetApplicationOverviewResult) => {
                //chart must be rendered after the element being attached and visible.
                ctx.resolve();
                self.renderChart(result);
            };

            const appQuery = new Core.Applications.Queries.GetApplicationInfo();
            appQuery.ApplicationId = ctx.routeData["applicationId"];
            appQuery.Version = this.filterVersion;
            CqsClient.query<Core.Applications.Queries.GetApplicationInfoResult>(appQuery)
                .done(info => {
                    Yo.GlobalConfig.applicationScope["application"] = info;

                    this.totalIncidentCount = info.TotalIncidentCount;
                    this.applicationName = info.Name;
                    if (chartResult != null) {
                        chartRendering(chartResult);
                    }
                    firstIsRun = true;
                    this.renderInfo(info);
                    ctx.handle.click(".version",
                        e => {
                            var version = (e.target as HTMLElement).getAttribute("data-value");
                            this.filterVersion = version;
                            ctx.render({ ActiveVersion: 'Filter: v' + version });
                            this.updateAppInfo();
                            this.incidentTable.load(this.applicationId, this.filterVersion);
                        });
                });
            const query = new Core.Applications.Queries.GetApplicationOverview(this.applicationId);
            query.Version = this.filterVersion;
            CqsClient.query<Core.Applications.Queries.GetApplicationOverviewResult>(query)
                .done(response => {

                    if (!firstIsRun) {
                        chartResult = response;
                    } else {
                        chartRendering(response);
                    }
                });

            this.incidentTable.load(this.applicationId);
            ctx.handle.change('[name="range"]', e => this.onRange(e));
        }

        deactivate() {

        }

        onRange(e: Event) {
            const elem = e.target as HTMLInputElement;
            const days = parseInt(elem.value, 10);
            this.chartDays = days;
            this.updateAppInfo();
        }

        private updateAppInfo() {
            const query = new Core.Applications.Queries.GetApplicationOverview(this.applicationId);
            query.NumberOfDays = this.chartDays;
            query.Version = this.filterVersion;
            CqsClient.query<Core.Applications.Queries.GetApplicationOverviewResult>(query)
                .done(response => {
                    console.log('overview', response);
                    this.updateChart(response);
                    this.ctx.render(response);
                });
        }

        private renderInfo(dto: Core.Applications.Queries.GetApplicationInfoResult) {
            const directives = {
                CreatedAtUtc: {
                    text(value) {
                        const date = new Date(value);
                        return moment(date).fromNow();
                    }
                },
                UpdatedAtUtc: {
                    text(value) {
                        const date = new Date(value);
                        return moment(date).fromNow();
                    }
                },
                SolvedAtUtc: {
                    text(value) {
                        const date = new Date(value);
                        return moment(date).fromNow();
                    }
                },
                Versions: {
                    text(value, dto) {
                        return "v" +dto;
                    },
                    "data-value"(value, dto) {
                        return dto;
                    }
                }
            };
            this.ctx.render(dto, directives);
        }


        renderChart(result: Core.Applications.Queries.GetApplicationOverviewResult) {
            console.log(result);
            const data = [];
            const labels = [{ name: "Reports", color: "#0094DA" }, { name: "Incidents", color: "#2B4141" }];
            for (let i = 0; i < result.ErrorReports.length; ++i) {
                const dataItem = {
                    date: result.TimeAxisLabels[i],
                    Reports: result.ErrorReports[i],
                    Incidents: result.Incidents[i]
                };
                data.push(dataItem);
            }

            $("#myChart").html("");
            this.chartOptions = {
                element: "myChart",
                data: data,
                xkey: "date",
                ykeys: ["Reports", "Incidents"],
                xLabels: result.Days === 1 ? "hour" as morris.Interval : "day",
                labels: ["Reports", "Incidents"],
                lineColors: ["#0094DA", "#2B4141"]
            };
            this.chart = Morris.Line(this.chartOptions);
            const directives = {
                labels: {
                    color: {
                        style(value) {
                            return `color: ${value}`;
                        },
                        html(value) {
                            return "";
                        }
                    },
                    name: {
                        html(value) {
                            return value;
                        }
                    }
                }
            };
            this.ctx.renderPartial("legend", { labels: labels }, directives);
            this.ctx.renderPartial('[data-name="StatSummary"]', result.StatSummary);
        }

        updateChart(result: Core.Applications.Queries.GetApplicationOverviewResult) {
            const data = [];
            let lastHour = -1;
            let date = new Date();
            date.setDate(date.getDate() - 1);
            for (let i = 0; i < result.ErrorReports.length; ++i) {
                const dataItem = {
                    date: result.TimeAxisLabels[i],
                    Reports: result.ErrorReports[i],
                    Incidents: result.Incidents[i]
                };

                if (this.chartDays === 1) {
                    const hour = parseInt(dataItem.date.substr(0, 2), 10);
                    if (hour < lastHour && lastHour !== -1) {
                        date = new Date();
                    }
                    lastHour = hour;
                    const minute = parseInt(dataItem.date.substr(3, 2), 10);
                    date.setHours(hour, minute);
                    dataItem.date = date.toISOString();
                }

                data.push(dataItem);
            }

            this.chartOptions.xLabelFormat = null;
            if (this.chartDays === 7) {
                this.chartOptions.xLabelFormat = (xDate: Date): string => {
                    return moment(xDate).format("dd");
                };
            } else if (this.chartDays === 1) {
                this.chartOptions.xLabels = "hour";
            } else {
                this.chartOptions.xLabels = "day";
            }
            this.chart.setData(data, true);
        }

    }
}