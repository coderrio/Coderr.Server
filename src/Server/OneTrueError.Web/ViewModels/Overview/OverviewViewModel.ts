/// <reference path="../../Scripts/Promise.ts"/>
/// <reference path="../../Scripts/CqsClient.ts"/>
/// <reference path="../ChartViewModel.ts"/>

module OneTrueError.Overview {
    import CqsClient = Griffin.Cqs.CqsClient;
    import IncidentOrder = OneTrueError.Core.Incidents.IncidentOrder;
    import PagerSubscriber = Griffin.WebApp.IPagerSubscriber;
    import Pager = Griffin.WebApp.Pager;
    import FindIncidents = OneTrueError.Core.Incidents.Queries.FindIncidents;
    import FindIncidentResultItem = OneTrueError.Core.Incidents.Queries.FindIncidentResultItem;
    import Yo = Griffin.Yo;

    export class OverviewViewModel implements PagerSubscriber, Griffin.Yo.Spa.ViewModels.IViewModel {
        private pager: Pager;
        private lineChart: LineChart;
        private static UP: string = 'glyphicon-chevron-up';
        private static DOWN: string = 'glyphicon-chevron-down';
        private _sortType: IncidentOrder = IncidentOrder.Newest;
        private _sortAscending: boolean = false;
        private _incidentType: string = 'active';
        private _ctx: Griffin.Yo.Spa.ViewModels.IActivationContext;

        constructor() {
            this.pager = new Pager(0, 0, 0);
            this.pager.subscribe(this);
        }

        public getTitle(): string {
            return 'Overview';
        }
        public activate(ctx: Griffin.Yo.Spa.ViewModels.IActivationContext) {
            this._ctx = ctx;

            var query = new Web.Overview.Queries.GetOverview();
            CqsClient.query<Web.Overview.Queries.GetOverviewResult>(query)
                .done(response => {
                    this.renderInfo(response);
                    ctx.resolve();
                    this.lineChart = new LineChart(ctx.select.one("#myChart"));
                    this.renderChart(response);

                });

            var pagerElement = ctx.select.one("pager");
            this.pager.draw(pagerElement);
            this.getIncidentsFromServer(1);
            ctx.handle.change('[name="range"]', e => this.OnRange(e));
            ctx.handle.click('#btnClosed', e => this.onBtnClosed(e));
            ctx.handle.click('#btnActive', e => this.onBtnActive(e));
            ctx.handle.click('#btnIgnored', e => this.onBtnIgnored(e));
            ctx.handle.click('#LastReportCol', e => this.onLastReportCol(e));
            ctx.handle.click('#CountCol', e => this.onCountCol(e));
        }

        public deactivate() {

        }


        public onPager(pager: Pager): void {
            this.getIncidentsFromServer(pager.currentPage);
            var table = this._ctx.select.one("#incidentTable");
            //.scrollIntoView(true);
            var y = this.findYPosition(table);
            window.scrollTo(null, y-50); //navbar 
        }

        private findYPosition(element: HTMLElement) : number {
            var curtop = 0;
            if (element.offsetParent) {
                do {
                    curtop += element.offsetTop;
                    element = <HTMLElement>(element.offsetParent);
                } while (element);
                return curtop;
            }
        }

        public OnRange(e: Event) {
            var query = new Web.Overview.Queries.GetOverview();
            var elem = <HTMLInputElement>e.target;
            query.NumberOfDays = parseInt(elem.value, 10);
            CqsClient.query<Web.Overview.Queries.GetOverviewResult>(query)
                .done(response => {
                    this.renderChart(response);
                });
        }

        private onBtnClosed(e: Event) {
            e.preventDefault();
            this._incidentType = 'closed';
            this.pager.reset();
            this.getIncidentsFromServer(0);
        }
        private onBtnActive(e: Event) {
            e.preventDefault();
            this._incidentType = 'active';
            this.pager.reset();
            this.getIncidentsFromServer(0);
        }
        private onBtnIgnored(e: Event) {
            e.preventDefault();
            this._incidentType = 'ignored';
            this.pager.reset();
            this.getIncidentsFromServer(0);
        }

        public onCountCol(e: Event): void {
            if (this._sortType !== IncidentOrder.MostReports) {
                this._sortType = IncidentOrder.MostReports;
                this._sortAscending = true;//will be changed below
            }
            this.updateSortingUI(e.target);
            this.getIncidentsFromServer(this.pager.currentPage);
        }

        public onLastReportCol(e: Event): void {
            if (this._sortType !== IncidentOrder.Newest) {
                this._sortType = IncidentOrder.Newest;
                this._sortAscending = true;//will be changed below
            }
            this.updateSortingUI(e.target);
            this.getIncidentsFromServer(this.pager.currentPage);
        }

        private renderTable(pageNumber: number, data: Core.Incidents.Queries.FindIncidentResult) {
            var self = this;
            var directives = {
                Items: {
                    Name: {
                        text(value) {
                            return value;
                        },
                        href(data, parentDto) {
                            return '#/application/' + parentDto.ApplicationId + '/incident/' + parentDto.Id;
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
                            return '#/application/' + parentDto.ApplicationId;
                        }
                    }
                }
            };

            if (data.TotalCount === 0) {
                this._ctx.select.one('getting-started').style.display = '';
            }
            this._ctx.renderPartial('#incidentTable', data, directives);
        }

        private renderInfo(dto: any) {
            var self = this;
            var directives = {
                CreatedAtUtc: {
                    text: function (params) {
                        return new Date(this.CreatedAtUtc).toLocaleString();
                    }
                },
                UpdatedAtUtc: {
                    text: function (params) {
                        return new Date(this.UpdatedAtUtc).toLocaleString();
                    }
                },
                SolvedAtUtc: {
                    text: function (params) {
                        return new Date(this.SolvedAtUtc).toLocaleString();
                    }
                },

            };

            this._ctx.render(dto, directives);
        }




        private updateSortingUI(parentElement: any) {
            this._sortAscending = !this._sortAscending;
            var icon = OverviewViewModel.UP;
            if (!this._sortAscending) {
                icon = OverviewViewModel.DOWN;
            }
            $('#OverviewView thead th span')
                .removeClass('glyphicon-chevron-down')
                .addClass('glyphicon ' + icon)
                .css('visibility', 'hidden');
            $('span', parentElement)
                .attr('class', 'glyphicon ' + icon)
                .css('visibility', 'inherit');
        }

        private getIncidentsFromServer(pageNumber: number): void {
            var query = new FindIncidents();
            query.SortType = this._sortType;
            query.SortAscending = this._sortAscending;
            query.PageNumber = pageNumber;
            query.ItemsPerPage = 10;
            if (this._incidentType === 'closed') {
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

        renderChart(result: OneTrueError.Web.Overview.Queries.GetOverviewResult) {
            var data = new LineData();
            data.datasets = [];

            var legend = [];
            var found = false;
            var index = 0;
            result.IncidentsPerApplication.forEach(function (app) {
                var ds = new Dataset();
                ds.label = app.Label;
                ds.data = app.Values;
                data.datasets.push(ds);
                var l = {
                    label: app.Label,
                    color: LineChart.LineThemes[index++].strokeColor
                };
                legend.push(l);
                found = true;
            });
            var directives = {
                color: {
                    text() {
                        return '';
                    },
                    style(value, dto) {
                        return 'display: inline; font-weight: bold; color: ' + dto.color;
                    }
                },
                label: {
                    style(value, dto) {
                        return 'font-weight: bold; color: ' + dto.color;
                    }
                }
            }

            this._ctx.renderPartial('#chart-legend', legend, directives);
            data.labels = result.TimeAxisLabels;
            //if (data.)
            this.lineChart.render(data);
            this._ctx.renderPartial('StatSummary', result.StatSummary);
        }
    }
}
 