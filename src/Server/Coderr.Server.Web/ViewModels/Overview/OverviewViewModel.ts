/// <reference path="../../Scripts/Promise.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />
/// <reference path="../ChartViewModel.ts" />
module codeRR.Overview {
    import CqsClient = Griffin.Cqs.CqsClient;
    import IncidentOrder = Core.Incidents.IncidentOrder;
    import PagerSubscriber = Griffin.WebApp.IPagerSubscriber;
    import Pager = Griffin.WebApp.Pager;
    import FindIncidents = Core.Incidents.Queries.FindIncidents;
    import GetApplicationList = codeRR.Core.Applications.Queries.GetApplicationList;
    import ApplicationListItem = codeRR.Core.Applications.ApplicationListItem;

    export class OverviewViewModel implements PagerSubscriber, Griffin.Yo.Spa.ViewModels.IViewModel {
        private pager: Pager;
        private static UP = "fa-chevron-up";
        private static DOWN = "fa-chevron-down";
        private _sortType: IncidentOrder = IncidentOrder.Newest;
        private _sortAscending = false;
        private _incidentType = "new";
        private _ctx: Griffin.Yo.Spa.ViewModels.IActivationContext;
        private chartOptions: morris.ILineOptions;
        private chart: morris.GridChart;

        constructor() {
            this.pager = new Pager(0, 0, 0);
            this.pager.subscribe(this);
        }

        getTitle(): string {
            Applications.Navigation.breadcrumbs([]);
            Applications.Navigation.pageTitle = 'Dashboard <em class="small">(summary for all applications)</em>';
            return "Overview";
        }

        activate(ctx: Griffin.Yo.Spa.ViewModels.IActivationContext) {
            this._ctx = ctx;

            const query = new Web.Overview.Queries.GetOverview();
            CqsClient.query<Web.Overview.Queries.GetOverviewResult>(query)
                .done(response => {
                    ctx.render(response);
                    ctx.resolve();
                    this.renderChart(response, 30);

                });
            var apps = new GetApplicationList();
            CqsClient.query<ApplicationListItem[]>(apps)
                .done(reply => {
                    if (reply.length === 0) {
                        ctx.select.one('config-help-guru').style.display = '';
                    }

                });
            const pagerElement = ctx.select.one("pager");
            this.pager.draw(pagerElement);
            this.getIncidentsFromServer(1);
            ctx.handle.change('[name="range"]', e => this.OnRange(e));
            ctx.handle.click("#btnNew", e => this.onBtnNew(e));
            ctx.handle.click("#btnAssigned", e => this.onBtnAssigned(e));
            ctx.handle.click("#btnIgnored", e => this.onBtnIgnored(e));
            ctx.handle.click("#btnClosed", e => this.onBtnClosed(e));
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
                    this.updateChart(response, query.NumberOfDays);
                });
        }

        private onBtnClosed(e: Event) {
            e.preventDefault();
            this._incidentType = "closed";
            this.pager.reset();
            $(e.target).parent().find('label').removeClass('active');
            $(e.target).addClass('active');
            this.getIncidentsFromServer(0);
        }

        private onBtnNew(e: Event) {
            e.preventDefault();
            this._incidentType = "new";
            this.pager.reset();
            $(e.target).parent().find('label').removeClass('active');
            $(e.target).addClass('active');
            this.getIncidentsFromServer(0);
        }

        private onBtnAssigned(e: Event) {
            e.preventDefault();
            this._incidentType = "assigned";
            this.pager.reset();
            $(e.target).parent().find('label').removeClass('active');
            $(e.target).addClass('active');
            this.getIncidentsFromServer(0);
        }

        private onBtnIgnored(e: Event) {
            e.preventDefault();
            this._incidentType = "ignored";
            this.pager.reset();
            $(e.target).parent().find('label').removeClass('active');
            $(e.target).addClass('active');
            this.getIncidentsFromServer(0);
        }

        onCountCol(e: Event): void {
            if (this._sortType !== IncidentOrder.MostReports) {
                this._sortType = IncidentOrder.MostReports ;
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

        private renderTable(pageNumber: number, data: codeRR.Core.Incidents.Queries.FindIncidentsResult) {
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
                            return momentsAgo(value);
                        }
                    },
                    LastReportReceivedAtUtc: {
                        text(value) {
                            return momentsAgo(value);
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
                //this._ctx.select.one("getting-started").style.display = "";
            }
            this._ctx.renderPartial("#incidentTable", data, directives);
        }

        private updateSortingUI(parentElement: any) {
            this._sortAscending = !this._sortAscending;
            let icon = OverviewViewModel.UP;
            if (!this._sortAscending) {
                icon = OverviewViewModel.DOWN;
            }
            $("#OverviewView thead th span")
                .removeClass("fa-chevron-down")
                .addClass(`fa ${icon}`)
                .css("visibility", "hidden");
            $("span", parentElement)
                .attr("class", `fa ${icon}`)
                .css("visibility", "inherit");
        }

        private getIncidentsFromServer(pageNumber: number): void {
            const query = new FindIncidents();
            query.SortType = this._sortType;
            query.SortAscending = this._sortAscending;
            query.PageNumber = pageNumber;
            query.ItemsPerPage = 10;
            if (this._incidentType === "closed") {
                query.IsClosed = true;
            }
            if (this._incidentType === "new") {
                query.IsNew = true;
            }
            if (this._incidentType === "assigned") {
                query.IsAssigned = true;
            }
            if (this._incidentType === "ignored") {
                query.IsIgnored = true;
            }

            //else if (this._incidentType === '')


            CqsClient.query<Core.Incidents.Queries.FindIncidentsResult>(query)
                .done(response => {
                    if (this.pager.pageCount === 0) {
                        this.pager.update(response.PageNumber, response.PageSize, response.TotalCount);
                    }
                    this.renderTable(pageNumber, response);
                });
        }

        renderChart(result: Web.Overview.Queries.GetOverviewResult, numberOfDays: number) {
            //no applications man!
            if (result.IncidentsPerApplication.length === 0) {
                $('#chart-container').html('<h4 class="header-title m-t-0 m-b-20">Incident trend</h4><em>No applications have been created.</em>');
                return;
            }

            var data = [];
            var yKeys = [];
            var legendLabels = [];
            var availableColors = ['#0094DA', '#2B4141', '#34E4EA', '#8AB9B5', '#C8C2AE', '#84BCDA', '#ECC30B', '#F37748', '#D56062'];
            var lineColors = availableColors.slice(0, result.IncidentsPerApplication.length);
            for (let y = 0; y < result.IncidentsPerApplication.length; y++) {
                const item = result.IncidentsPerApplication[y];
                yKeys.push(item.Label);
                legendLabels.push({
                    color: availableColors[y],
                    name: item.Label
                });
            }

            for (let i = 0; i < result.TimeAxisLabels.length; ++i) {
                const dataItem = {
                    date: result.TimeAxisLabels[i]
                };

                for (let y = 0; y < result.IncidentsPerApplication.length; y++) {
                    const item = result.IncidentsPerApplication[y];
                    dataItem[item.Label] = item.Values[i];
                }
                data.push(dataItem);
            }

            $('#myChart').html('');
            this.chartOptions = {
                element: 'myChart',
                data: data,
                xkey: 'date',
                ykeys: yKeys,
                //xLabels: numberOfDays === 1 ? "hour" as morris.Interval : "day",
                labels: yKeys,
                lineColors: lineColors
            };
            this.chart = Morris.Line(this.chartOptions);

            var directives = {
                labels: {
                    color: {
                        style(value) {
                            return 'color: ' + value;
                        },
                        html(value) {
                            return '';
                        }
                    },
                    name: {
                        html(value) {
                            return value;
                        }
                    }
                }
            };
            this._ctx.renderPartial("legend", { labels: legendLabels }, directives);
            this._ctx.renderPartial("StatSummary", result.StatSummary);
            $(window).resize(() => {
                (<any>this.chart).redraw();
            });
        }

        updateChart(result: Web.Overview.Queries.GetOverviewResult, numberOfDays: number) {
            var data = [];
            var lastHour = -1;
            var date: Date = new Date();
            date.setDate(date.getDate() - 1);

            for (let i = 0; i < result.TimeAxisLabels.length; ++i) {
                const dataItem = {
                    date: result.TimeAxisLabels[i]
                };

                if (numberOfDays === 1) {
                    var hour = parseInt(dataItem.date.substr(0, 2), 10);
                    if (hour < lastHour && lastHour !== -1) {
                        date = new Date();
                    }
                    lastHour = hour;
                    var minute = parseInt(dataItem.date.substr(3, 2), 10);
                    date.setHours(hour, minute);
                    dataItem.date = date.toISOString();
                }

                for (let y = 0; y < result.IncidentsPerApplication.length; y++) {
                    const item = result.IncidentsPerApplication[y];
                    dataItem[item.Label] = item.Values[i];
                }
                data.push(dataItem);
            }
            
            this.chartOptions.xLabelFormat = null;
            if (numberOfDays === 7) {
                this.chartOptions.xLabelFormat = (xDate: Date): string => {
                    return moment(xDate).format('dd');
                }
            } else if (numberOfDays === 1) {
                this.chartOptions.xLabels = "hour";
            } else {
                this.chartOptions.xLabels = "day";
            }
            this.chart.setData(data, true);
        }
    }
}