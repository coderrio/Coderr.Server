/// <reference path="../../Scripts/Promise.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />
module OneTrueError.Incident {
    import CqsClient = Griffin.Cqs.CqsClient;
    import IncidentOrder = Core.Incidents.IncidentOrder;
    import Yo = Griffin.Yo;
    import Pager = Griffin.WebApp.Pager;

    export class IndexViewModel implements Griffin.Yo.Spa.ViewModels.IViewModel, Griffin.WebApp.IPagerSubscriber {
        private static UP = "glyphicon-chevron-up";
        private static DOWN = "glyphicon-chevron-down";
        private sortType = IncidentOrder.Newest;
        private sortAscending = false;
        private closed = false;
        private open = true;
        private reOpened = false;
        private pager: Griffin.WebApp.Pager;

        constructor(dto: any) {
        }


        getTitle(): string {
            return "Incidents";
        }

        activate(ctx: Griffin.Yo.Spa.ViewModels.IActivationContext) {
            const query = new Core.Incidents.Queries.FindIncidents();
            query.PageNumber = 1;
            query.ItemsPerPage = 20;
            CqsClient.query<Core.Incidents.Queries.FindIncidentResult>(query)
                .done(response => {

                    var itemsElem = ctx.viewContainer.querySelector("#incidentTable") as HTMLElement;
                    this.renderTable(itemsElem, response);
                    ctx.resolve();

                    this.pager = new Griffin.WebApp.Pager(response.PageNumber, 20, response.TotalCount);
                    this.pager.subscribe(this);
                    this.pager.draw(ctx.select.one("#pager"));

                });

            ctx.handle.click("#btnClosed", e => this.onBtnClosed(e));
            ctx.handle.click("#btnActive", e => this.onBtnActive(e));
            ctx.handle.click("#btnIgnored", e => this.onBtnIgnored(e));
            ctx.handle.click("#LastReportCol", e => this.onLastReportCol(e));
            ctx.handle.click("#CountCol", e => this.onCountCol(e));
        }

        deactivate() {

        }


        onPager(pager: Pager): void {
            this.loadItems(pager.currentPage);
        }

        private onBtnActive(e: Event): void {
            e.preventDefault();
            this.reOpened = true;
            this.open = true;
            this.closed = false;
            this.pager.reset();
            this.loadItems(0);
        }

        private onBtnClosed(e: Event): void {
            e.preventDefault();
            this.reOpened = false;
            this.open = false;
            this.closed = true;
            this.pager.reset();
            this.loadItems(0);
        }

        private onBtnIgnored(e: Event): void {
            e.preventDefault();
            this.reOpened = false;
            this.open = false;
            this.closed = false;
            this.pager.reset();
            this.loadItems(0);
        }

        onCountCol(args: Event): void {
            if (this.sortType !== IncidentOrder.MostReports) {
                this.sortType = IncidentOrder.MostReports;
                this.sortAscending = true; //will be changed below
            }
            if (this.sortAscending) {

                //TODO: SORT
            } else {

            }

            this.updateSorting(args.target);
            this.loadItems();
        }

        onLastReportCol(args: Event): void {
            if (this.sortType !== IncidentOrder.Newest) {
                this.sortType = IncidentOrder.Newest;
                this.sortAscending = false; //will be changed below
            }
            if (this.sortAscending) {

                //TODO: SORT
            } else {

            }
            this.updateSorting(args.target);
            this.loadItems();
        }

        private updateSorting(parentElement: any) {
            this.sortAscending = !this.sortAscending;
            let icon = IndexViewModel.UP;
            if (!this.sortAscending) {
                icon = IndexViewModel.DOWN;
            }
            $("#IndexView thead th span")
                .removeClass("glyphicon-chevron-down")
                .addClass(`glyphicon ${icon}`)
                .css("visibility", "hidden");
            $("span", parentElement)
                .attr("class", `glyphicon ${icon}`)
                .css("visibility", "inherit");
        }

        private renderTable(target: HTMLElement, data: any) {
            const directives = {
                Name: {
                    href(params, dto) {
                        return `#/application/${dto.ApplicationId}/incident/${dto.Id}`;
                    },
                    text(value) {
                        return value;
                    }
                },
                ApplicationName: {
                    href(params, dto) {
                        return `#application/${dto.ApplicationId}/incidents/`;
                    },
                    text(value) {
                        return value;
                    }
                },
                LastUpdateAtUtc: {
                    text(value) {
                        return new Date(value).toLocaleString();
                    }
                }

            };

            Yo.G.render(target, data.Items, directives);
        }

        private loadItems(pageNumber: number = 0): void {
            const query = new Core.Incidents.Queries.FindIncidents();
            query.SortType = this.sortType;
            query.SortAscending = this.sortAscending;
            query.Closed = this.closed;
            query.Open = this.open;
            query.ReOpened = this.reOpened;
            query.Ignored = !this.reOpened && !this.closed && !this.open;
            if (pageNumber === 0) {
                query.PageNumber = this.pager.currentPage;
            } else {
                query.PageNumber = pageNumber;
            }
            query.ItemsPerPage = 20;
            CqsClient.query<Core.Incidents.Queries.FindIncidentResultItem>(query)
                .done(response => {
                    //this.pager.update(response.PageNumber, 20, response.TotalCount);
                    this.renderTable(document.getElementById("incidentTable"), response);
                    window.scrollTo(0, 0);
                });
        }

    }
}