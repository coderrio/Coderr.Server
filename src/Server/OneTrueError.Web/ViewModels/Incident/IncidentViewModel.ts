/// <reference path="../../Scripts/Promise.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />
/// <reference path="../ChartViewModel.ts" />
module OneTrueError.Incident {
    import CqsClient = Griffin.Cqs.CqsClient;
    import PagerSubscriber = Griffin.WebApp.IPagerSubscriber;
    import Pager = Griffin.WebApp.Pager;
    import ReportDay = Core.Incidents.Queries.ReportDay;

    export class IncidentViewModel implements PagerSubscriber, Griffin.Yo.Spa.ViewModels.IViewModel {
        private static UP = "glyphicon-chevron-up";
        private static DOWN = "glyphicon-chevron-down";
        private pager: Pager;
        private lineChart: LineChart;
        private applicationId: number;
        private name: string;
        private ctx: Griffin.Yo.Spa.ViewModels.IActivationContext;
        id: number;

        constructor(appScope) {
        }


        getTitle(): string {
            return `Incident ${this.name}`;
        }

        isIgnored = false;

        activate(ctx: Griffin.Yo.Spa.ViewModels.IActivationContext) {
            const query = new Core.Incidents.Queries.GetIncident(ctx.routeData["incidentId"]);
            this.ctx = ctx;
            CqsClient.query<Core.Incidents.Queries.GetIncidentResult>(query)
                .done(response => {
                    this.isIgnored = response.IsIgnored;
                    this.name = response.Description;
                    this.id = response.Id;
                    this.applicationId = response.ApplicationId;
                    this.pager = new Pager(0, 0, 0);
                    this.pager.subscribe(this);
                    this.pager.draw(ctx.select.one("#pager"));
                    this.renderInfo(response);
                    var query = new Core.Reports.Queries.GetReportList(this.id);
                    query.PageNumber = 1;
                    query.PageSize = 20;
                    CqsClient.query<Core.Reports.Queries.GetReportListResult>(query)
                        .done(result => {
                            this.pager.update(result.PageNumber, result.PageSize, result.TotalCount);
                            this.renderTable(1, result);
                        });
                    var elem = ctx.select.one("#myChart") as HTMLCanvasElement;
                    ctx.resolve();
                    this.renderInitialChart(elem, response.DayStatistics);

                });

            ctx.handle.change('[name="range"]', e => this.onRange(e));

        }

        deactivate() {

        }


        onPager(pager: Pager): void {
            const query = new Core.Reports.Queries.GetReportList(this.id);
            query.PageNumber = pager.currentPage;
            query.PageSize = 20;
            CqsClient.query<Core.Reports.Queries.GetReportListResult>(query)
                .done(result => {
                    this.renderTable(1, result);
                });
        }

        onRange(e: Event) {
            const elem = e.target as HTMLInputElement;
            const days = parseInt(elem.value, 10);
            this.loadChartInfo(days);
        }

        private renderInitialChart(chartElement: HTMLCanvasElement, stats: ReportDay[]) {
            var labels: string[] = new Array();
            var dataset = new Dataset();
            dataset.label = "Error reports";
            dataset.data = new Array();
            stats.forEach(item => {
                labels.push(new Date(item.Date).toLocaleDateString());
                dataset.data.push(item.Count);
            });
            const data = new LineData();
            data.datasets = [dataset];
            data.labels = labels;
            this.lineChart = new LineChart(chartElement);
            this.lineChart.render(data);
            //this.loadChartInfo(30);
        }

        private renderTable(pageNumber: number, data: any) {
            var self = this;
            const directives = {
                Items: {
                    CreatedAtUtc: {
                        text(value, dto) {
                            return new Date(value).toLocaleString();
                        }
                    },
                    Message: {
                        text(value, dto) {
                            if (!value) {
                                return "(No exception message)";
                            }
                            return dto.Message;
                        },
                        href(value, dto) {
                            return `#/application/${self.applicationId}/incident/${self.id}/report/${dto.Id}`;
                        }
                    }
                }
            };
            this.ctx.renderPartial("#reportsTable", data, directives);
        }

        private renderInfo(dto: Core.Incidents.Queries.GetIncidentResult) {
            const self = this;
            if (dto.IsSolved) {
                dto.Tags.push("solved");
            }
            const directives = {
                CreatedAtUtc: {
                    text(value) {
                        return new Date(value).toLocaleString();
                    }
                },
                UpdatedAtUtc: {
                    text(value) {
                        return new Date(value).toLocaleString();
                    }
                },
                SolvedAtUtc: {
                    text(value) {
                        return new Date(value).toLocaleString();
                    }
                },
                Tags: {
                    "class"(value) {
                        if (value === "solved")
                            return "tag bg-danger";
                        return "tag nephritis";
                    },
                    text(value) {
                        return value;
                    },
                    href(value) {
                        return `http://stackoverflow.com/search?q=%5B${value}%5D+${dto.Description}`;
                    }
                }

            };
            this.ctx.render(dto, directives);
            if (dto.IsSolved) {
                this.ctx.select.one("#actionButtons").style.display = "none";
                this.ctx.select.one('[data-name="Description"]').style.textDecoration = "line-through";
            }
        }

        private loadChartInfo(days: number) {
            const query = new Core.Incidents.Queries.GetIncidentStatistics();
            query.IncidentId = this.id;
            query.NumberOfDays = days;
            CqsClient.query<Core.Incidents.Queries.GetIncidentStatisticsResult>(query)
                .done(response => {
                    var dataset = new Dataset();
                    dataset.label = "Error reports";
                    dataset.data = response.Values;
                    var data = new LineData();
                    data.datasets = [dataset];
                    data.labels = response.Labels;
                    this.lineChart.render(data);
                });
        }

    }
}