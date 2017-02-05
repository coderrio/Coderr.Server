/// <reference path="../../Scripts/Promise.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />
/// <reference path="../ChartViewModel.ts" />
module OneTrueError.Overview {
    import CqsClient = Griffin.Cqs.CqsClient;
    import IncidentOrder = Core.Incidents.IncidentOrder;
    import PagerSubscriber = Griffin.WebApp.IPagerSubscriber;
    import Pager = Griffin.WebApp.Pager;
    import FindIncidents = Core.Incidents.Queries.FindIncidents;

    export class OverviewViewModel implements PagerSubscriber, Griffin.Yo.Spa.ViewModels.IViewModel {
        private pager: Pager;
        private lineChart: LineChart;
        private static UP = "glyphicon-chevron-up";
        private static DOWN = "glyphicon-chevron-down";
        private _sortType = IncidentOrder.Newest;
        private _sortAscending = false;
        private _incidentType = "active";
        private _ctx: Griffin.Yo.Spa.ViewModels.IActivationContext;

        constructor() {
            this.pager = new Pager(0, 0, 0);
            this.pager.subscribe(this);
        }

        getTitle(): string {
            return "Overview";
        }

        activate(ctx: Griffin.Yo.Spa.ViewModels.IActivationContext) {
            this._ctx = ctx;

            const query = new Web.Overview.Queries.GetOverview();
            CqsClient.query<Web.Overview.Queries.GetOverviewResult>(query)
                .done(response => {
                    this.renderInfo(response);
                    ctx.resolve();
                    this.lineChart = new LineChart(ctx.select.one("#myChart"));
                    this.renderChart(response);

                });

            const pagerElement = ctx.select.one("pager");
            this.pager.draw(pagerElement);
            this.getIncidentsFromServer(1);
            ctx.handle.change('[name="range"]', e => this.OnRange(e));
            ctx.handle.click("#btnClosed", e => this.onBtnClosed(e));
            ctx.handle.click("#btnActive", e => this.onBtnActive(e));
            ctx.handle.click("#btnIgnored", e => this.onBtnIgnored(e));
            ctx.handle.click("#LastReportCol", e => this.onLastReportCol(e));
            ctx.handle.click("#CountCol", e => this.onCountCol(e));
        }

        deactivate() {

        }


        onPager(pager: Pager): void {
            this.getIncidentsFromServer(pager.currentPage);
            const table = this._ctx.select.one("#incidentTable");
            //.scrollIntoView(true);
            const y = this.findYPosition(table);
            window.scrollTo(null, y - 50); //navbar 
        }

        private findYPosition(element: HTMLElement): number {
            if (element.offsetParent) {
                let curtop = 0;
                do {
                    curtop += element.offsetTop;
                    element = ((element.offsetParent) as HTMLElement);
                } while (element);
                return curtop;
            }
        }

        OnRange(e: Event) {
            const query = new Web.Overview.Queries.GetOverview();
            const elem = e.target as HTMLInputElement;
            query.NumberOfDays = parseInt(elem.value, 10);
            CqsClient.query<Web.Overview.Queries.GetOverviewResult>(query)
                .done(response => {
                    this.renderChart(response);
                });
        }

        private onBtnClosed(e: Event) {
            e.preventDefault();
            this._incidentType = "closed";
            this.pager.reset();
            this.getIncidentsFromServer(0);
        }

        private onBtnActive(e: Event) {
            e.preventDefault();
            this._incidentType = "active";
            this.pager.reset();
            this.getIncidentsFromServer(0);
        }

        private onBtnIgnored(e: Event) {
            e.preventDefault();
            this._incidentType = "ignored";
            this.pager.reset();
            this.getIncidentsFromServer(0);
        }

        onCountCol(e: Event): void {
            if (this._sortType !== IncidentOrder.MostReports) {
                this._sortType = IncidentOrder.MostReports;
                this._sortAscending = true; //will be changed below
            }
            this.updateSortingUI(e.target);
            this.getIncidentsFromServer(this.pager.currentPage);
        }

        onLastReportCol(e: Event): void {
            if (this._sortType !== IncidentOrder.Newest) {
                this._sortType = IncidentOrder.Newest;
                this._sortAscending = true; //will be changed below
            }
            this.updateSortingUI(e.target);
            this.getIncidentsFromServer(this.pager.currentPage);
        }

        private renderTable(pageNumber: number, data: Core.Incidents.Queries.FindIncidentResult) {
            const self = this;
            const directives = {
                Items: {
                    Name: {
                        text(value) {
                            return value;
                        },
                        href(data, parentDto) {
                            return `#/application/${parentDto.ApplicationId}/incident/${parentDto.Id}`;
                        }
                    },
                    CreatedAtUtc: {
                        text(value) {
                            return new Date(value).toLocaleString();
                        }
                    },
                    LastUpdateAtUtc: {
                        text(value) {
                            return new Date(value).toLocaleString();
                        }
                    },
                    ApplicationName: {
                        text(value) {
                            return value;
                        },
                        href(params, parentDto) {
                            return `#/application/${parentDto.ApplicationId}`;
                        }
                    }
                }
            };

            if (data.TotalCount === 0) {
                this._ctx.select.one("getting-started").style.display = "";
            }
            this._ctx.renderPartial("#incidentTable", data, directives);
        }

        private renderInfo(dto: any) {
            const self = this;
            const directives = {
                CreatedAtUtc: {
                    text: function(params) {
                        return new Date(this.CreatedAtUtc).toLocaleString();
                    }
                },
                UpdatedAtUtc: {
                    text: function(params) {
                        return new Date(this.UpdatedAtUtc).toLocaleString();
                    }
                },
                SolvedAtUtc: {
                    text: function(params) {
                        return new Date(this.SolvedAtUtc).toLocaleString();
                    }
                },

            };

            this._ctx.render(dto, directives);
        }


        private updateSortingUI(parentElement: any) {
            this._sortAscending = !this._sortAscending;
            let icon = OverviewViewModel.UP;
            if (!this._sortAscending) {
                icon = OverviewViewModel.DOWN;
            }
            $("#OverviewView thead th span")
                .removeClass("glyphicon-chevron-down")
                .addClass(`glyphicon ${icon}`)
                .css("visibility", "hidden");
            $("span", parentElement)
                .attr("class", `glyphicon ${icon}`)
                .css("visibility", "inherit");
        }

        private getIncidentsFromServer(pageNumber: number): void {
            const query = new FindIncidents();
            query.SortType = this._sortType;
            query.SortAscending = this._sortAscending;
            query.PageNumber = pageNumber;
            query.ItemsPerPage = 10;
            if (this._incidentType === "closed") {
                query.Closed = true;
                query.Open = false;
            }

            //else if (this._incidentType === '')


            CqsClient.query<Core.Incidents.Queries.FindIncidentResult>(query)
                .done(response => {
                    if (this.pager.pageCount === 0) {
                        this.pager.update(response.PageNumber, response.PageSize, response.TotalCount);
                    }
                    this.renderTable(pageNumber, response);
                });
        }

        renderChart(result: Web.Overview.Queries.GetOverviewResult) {
            var data = new LineData();
            data.datasets = [];

            var legend = [];
            var found = false;
            var index = 0;
            result.IncidentsPerApplication.forEach(function (app) {
                var sum = app.Values.reduce(function (a, b) { return a + b; }, 0);
                if (sum === 0) {
                    return;
                }

                const ds = new Dataset();
                ds.label = app.Label;
                ds.data = app.Values;
                data.datasets.push(ds);

                //not enough color themes to display all.
                if (index > LineChart.LineThemes.length) {
                    return;
                }

                const l = {
                    label: app.Label,
                    color: LineChart.LineThemes[index++].strokeColor
                };
                legend.push(l);
                found = true;
            });
            const directives = {
                color: {
                    text() {
                        return "";
                    },
                    style(value, dto) {
                        return `display: inline; font-weight: bold; color: ${dto.color}`;
                    }
                },
                label: {
                    style(value, dto) {
                        return `font-weight: bold; color: ${dto.color}`;
                    }
                }
            };
            this._ctx.renderPartial("#chart-legend", legend, directives);
            data.labels = result.TimeAxisLabels;
            //if (data.)
            this.lineChart.render(data);
            this._ctx.renderPartial("StatSummary", result.StatSummary);
        }
    }
}