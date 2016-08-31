/// <reference path="../../Scripts/Promise.ts" />
/// <reference path="../../Scripts/Griffin.WebApp.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />
/// <reference path="../ChartViewModel.ts" />
module OneTrueError.Application {
    import CqsClient = Griffin.Cqs.CqsClient;
    import IncidentOrder = Core.Incidents.IncidentOrder;
    import PagerSubscriber = Griffin.WebApp.IPagerSubscriber;
    import Pager = Griffin.WebApp.Pager;
    import FindIncidents = Core.Incidents.Queries.FindIncidents;
    import ActivationContext = Griffin.Yo.Spa.ViewModels.IActivationContext;
    import Yo = Griffin.Yo;

    export class DetailsViewModel implements Griffin.Yo.Spa.ViewModels.IViewModel, PagerSubscriber {
        private pager: Pager;
        private applicationId: number;
        private applicationName: string;
        private lineChart: LineChart;
        private static UP = "glyphicon-chevron-up";
        private static DOWN = "glyphicon-chevron-down";
        private _sortType = IncidentOrder.Newest;
        private _sortAscending = false;
        private _incidentType = "active";
        private _ctx: ActivationContext;

        constructor() {
        }

        getTitle(): string {
            $("#appTitle").text(this.applicationName);
            return this.applicationName;
        }

        activate(ctx: Griffin.Yo.Spa.ViewModels.IActivationContext) {
            this._ctx = ctx;
            this.applicationId = ctx.routeData["applicationId"];
            this.pager = new Griffin.WebApp.Pager(0, 0, 0);
            this.pager.subscribe(this);
            this.pager.draw(ctx.select.one("#pager"));
            var self = this;

            var firstIsRun = false;
            var chartResult: Core.Applications.Queries.GetApplicationOverviewResult = null;
            var chartRendering = (result: Core.Applications.Queries.GetApplicationOverviewResult) => {
                //chart must be rendered after the element being attached and visible.
                ctx.resolve();
                self.lineChart = new LineChart(ctx.select.one("#myChart"));
                self.renderChart(result);
            };

            var appQuery = new Core.Applications.Queries.GetApplicationInfo();
            appQuery.ApplicationId = ctx.routeData["applicationId"];
            CqsClient.query<Core.Applications.Queries.GetApplicationInfoResult>(appQuery)
                .done(info => {

                    Yo.GlobalConfig.applicationScope["application"] = info;
                    this.applicationName = info.Name;
                    if (chartResult != null) {
                        chartRendering(chartResult);
                    }
                    firstIsRun = true;
                    this.renderInfo(info);
                });
            var query = new Core.Applications.Queries.GetApplicationOverview(this.applicationId);
            CqsClient.query<Core.Applications.Queries.GetApplicationOverviewResult>(query)
                .done(response => {

                    if (!firstIsRun) {
                        chartResult = response;
                    } else {
                        chartRendering(response);
                    }
                });
            this.getIncidentsFromServer(1);
            ctx.handle.click("#btnClosed", e => this.onBtnClosed(e));
            ctx.handle.click("#btnActive", e => this.onBtnActive(e));
            ctx.handle.click("#btnIgnored", e => this.onBtnIgnored(e));
            ctx.handle.click("#LastReportCol", e => this.onLastReportCol(e));
            ctx.handle.click("#CountCol", e => this.onCountCol(e));
            ctx.handle.change('[name="range"]', e => this.onRange(e));
        }

        deactivate() {

        }

        onRange(e: Event) {
            var elem = <HTMLInputElement>e.target;
            var days = parseInt(elem.value, 10);
            var query = new Core.Applications.Queries.GetApplicationOverview(this.applicationId);
            query.NumberOfDays = days;
            CqsClient.query<Core.Applications.Queries.GetApplicationOverviewResult>(query)
                .done(response => {
                    this.renderChart(response);
                });
        }

        private onBtnActive(e: Event): void {
            e.preventDefault();
            this._incidentType = "active";
            this.pager.reset();
            this.getIncidentsFromServer(-1);
        }

        private onBtnClosed(e: Event): void {
            e.preventDefault();
            this._incidentType = "closed";
            this.pager.reset();
            this.getIncidentsFromServer(-1);
        }

        private onBtnIgnored(e: Event): void {
            e.preventDefault();
            this._incidentType = "ignored";
            this.pager.reset();
            this.getIncidentsFromServer(-1);
        }

        onPager(pager: Pager): void {
            this.getIncidentsFromServer(pager.currentPage);
            var table = this._ctx.select.one("#incidentTable");
            //.scrollIntoView(true);
            var y = this.findYPosition(table);
            window.scrollTo(null, y - 50); //navbar 
        }

        private findYPosition(element: HTMLElement): number {
            var curtop = 0;
            if (element.offsetParent) {
                do {
                    curtop += element.offsetTop;
                    element = <HTMLElement>(element.offsetParent);
                } while (element);
                return curtop;
            }
        }


        onCountCol(args: MouseEvent): void {
            if (this._sortType !== IncidentOrder.MostReports) {
                this._sortType = IncidentOrder.MostReports;
                this._sortAscending = true; //will be changed below
            }
            this.updateSortingUI(args.target);
            this.getIncidentsFromServer(this.pager.currentPage);
        }

        onLastReportCol(args: MouseEvent): void {
            if (this._sortType !== IncidentOrder.Newest) {
                this._sortType = IncidentOrder.Newest;
                this._sortAscending = true; //will be changed below
            }
            this.updateSortingUI(args.target);
            this.getIncidentsFromServer(this.pager.currentPage);
        }


        private renderTable(pageNumber: number, data: Core.Incidents.Queries.FindIncidentResult) {
            var directives = {
                Items: {
                    IncidentName: {
                        text(data) {
                            return data;
                        },
                        href(data, dto) {
                            return `#/application/${dto.ApplicationId}/incident/${dto.Id}`;
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
                        href(value, dto) {
                            return `#/application/${dto.ApplicationId}`;
                        }
                    }
                }
            };

            //workaround for root dto name rendering over our collection name
            data.Items.forEach(item => {
                (<any>item).IncidentName = item.Name;
            });
            this._ctx.renderPartial("#incidentTable", data, directives);
        }

        private renderInfo(dto: Core.Applications.Queries.GetApplicationInfoResult) {
            var directives = {
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
                }

            };
            dto["StatSummary"] = {
                AppKey: dto.AppKey,
                SharedSecret: dto.SharedSecret
            };
            this._ctx.render(dto, directives);
        }


        private updateSortingUI(parentElement: any) {
            this._sortAscending = !this._sortAscending;
            var icon = DetailsViewModel.UP;
            if (!this._sortAscending) {
                icon = DetailsViewModel.DOWN;
            }
            $("#ApplicationView thead th span")
                .removeClass("glyphicon-chevron-down")
                .addClass(`glyphicon ${icon}`)
                .css("visibility", "hidden");
            $("span", parentElement)
                .attr("class", `glyphicon ${icon}`)
                .css("visibility", "inherit");
        }

        private getIncidentsFromServer(pageNumber: number): void {
            if (pageNumber === -1) {
                pageNumber = this.pager.currentPage;
            }
            var query = new FindIncidents();
            query.SortType = this._sortType;
            query.SortAscending = this._sortAscending;
            query.PageNumber = pageNumber;
            query.ItemsPerPage = 10;
            query.ApplicationId = this.applicationId;
            if (this._incidentType === "closed") {
                query.Closed = true;
                query.Open = false;
            } else if (this._incidentType === "ignored") {
                query.Closed = false;
                query.Open = false;
                query.ReOpened = false;
                query.Ignored = true;
            }


            CqsClient.query<Core.Incidents.Queries.FindIncidentResult>(query)
                .done(response => {
                    if (this.pager.pageCount === 0) {
                        this.pager.update(response.PageNumber, response.PageSize, response.TotalCount);
                    }

                    this.renderTable(pageNumber, response);
                });
        }

        renderChart(result: Core.Applications.Queries.GetApplicationOverviewResult) {

            var legend = [
                {
                    color: LineChart.LineThemes[0].strokeColor,
                    label: "Incidents"
                }, {
                    color: LineChart.LineThemes[1].strokeColor,
                    label: "Reports"
                }
            ];

            var incidentsDataset = new Dataset();
            incidentsDataset.label = "Incidents";
            incidentsDataset.data = result.Incidents;

            var reportDataset = new Dataset();
            reportDataset.label = "Reports";
            reportDataset.data = result.ErrorReports;


            var directives = {
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
            //$('#chart-legend').render(legend, directives);

            var data = new LineData();
            data.datasets = [incidentsDataset, reportDataset];
            data.labels = result.TimeAxisLabels;
            this.lineChart.render(data);
            this._ctx.renderPartial('[data-name="StatSummary"]', result.StatSummary);
        }
    }
}