/// <reference path="../../../Scripts/CqsClient.ts" />

import CqsClient = Griffin.Cqs.CqsClient;
import Yo = Griffin.Yo;
import Pager = Griffin.WebApp.Pager;
import IncidentOrder = codeRR.Core.Incidents.IncidentOrder;

class IncidentTableViewModel implements Griffin.WebApp.IPagerSubscriber {
    private static UP = "fa fa-chevron-up";
    private static DOWN = "fa fa-chevron-down";
    private sortType: IncidentOrder = IncidentOrder.Newest;
    private incidentType: string = "new";
    private sortAscending = false;
    private pager: Griffin.WebApp.Pager;
    private ctx: Griffin.Yo.Spa.ViewModels.IActivationContext;
    private applicationId?: number;
    private freeText: string;
    private tags: string[] = [];

    constructor(ctx: Griffin.Yo.Spa.ViewModels.IActivationContext) {
        this.ctx = ctx;
    }

    load(applicationId?: number, applicationVersion?: string, callback?: any) {

        const query = new codeRR.Core.Incidents.Queries.FindIncidents();
        query.PageNumber = 1;
        query.ItemsPerPage = 20;
        query.IsNew = true;
        if (applicationId != null) {
            query.ApplicationIds = [applicationId];
            this.applicationId = applicationId;
        }
        if (applicationVersion != null) {
            query.Version = applicationVersion;
        }
        CqsClient.query<codeRR.Core.Incidents.Queries.FindIncidentsResult>(query)
            .done(response => {

                var itemsElem = this.ctx.viewContainer.querySelector("#incidentTable") as HTMLElement;
                this.renderTable(itemsElem, response);
                this.pager = new Griffin.WebApp.Pager(response.PageNumber, 20, response.TotalCount);
                this.pager.subscribe(this);
                this.pager.draw(this.ctx.select.one("#pager"));
                if (callback) {
                    callback();
                }
            });

        if (applicationId > 0) {
            var tagsQuery = new codeRR.Modules.Tagging.Queries.GetTagsForApplication(applicationId);
            CqsClient.query<codeRR.Modules.Tagging.TagDTO[]>(tagsQuery)
                .done(response => {
                    var directives = {
                        Name: {
                            href(data) {
                                return data;
                            },
                            html(data) {
                                return data;
                            }
                        },
                        OrderNumber: {
                            html(data) {
                            }
                        }
                    };
                    this.ctx.render({ tags: response }, directives);
                });

            var self = this;
            $('body').on('click', '.search-tag', function (e) {
                e.preventDefault();
                if ($(this).hasClass('label-primary')) {
                    $(this).addClass('label-dark').removeClass('label-primary');
                } else {
                    $(this).addClass('label-primary').removeClass('label-dark');
                }
                
                self.tags = [];
                $('.search-tag.label-primary').each(function(e) {
                    self.tags.push($(this).text());
                });

                self.pager.reset();
                self.loadItems();
            });
        }


        this.ctx.handle.click("#btnClosed", e => this.onBtnClosed(e));
        this.ctx.handle.click("#btnNew", e => this.onBtnNew(e));
        this.ctx.handle.click("#btnActive", e => this.onBtnActive(e));
        this.ctx.handle.click("#btnIgnored", e => this.onBtnIgnored(e));
        this.ctx.handle.click("#LastReportCol", e => this.onLastReportCol(e));
        this.ctx.handle.click("#CountCol", e => this.onCountCol(e));
        this.ctx.handle.keyUp('[data-name="freeText"]', e => this.onFreeText(e));
    }


    private onFreeText(e: Event): void {
        var el = <HTMLInputElement>(e.target);
        if (el.value.length >= 3) {
            this.freeText = el.value;
            this.loadItems(this.pager.currentPage);
        } else if (el.value === '') {
            this.freeText = el.value;
            this.loadItems(this.pager.currentPage);
        } else {
            this.freeText = el.value;
        }
    }

    onPager(pager: Pager): void {
        this.loadItems(pager.currentPage);
    }

    private onBtnClosed(e: Event) {
        e.preventDefault();
        this.incidentType = "closed";
        this.pager.reset();
        $(e.target).parent().find('label').removeClass('active');
        $(e.target).addClass('active');
        this.loadItems();
    }

    private onBtnActive(e: Event) {
        e.preventDefault();
        this.incidentType = "active";
        this.pager.reset();
        $(e.target).parent().find('label').removeClass('active');
        $(e.target).addClass('active');
        this.loadItems();
    }

    private onBtnNew(e: Event) {
        e.preventDefault();
        this.incidentType = "new";
        this.pager.reset();
        $(e.target).parent().find('label').removeClass('active');
        $(e.target).addClass('active');
        this.loadItems();
    }

    private onBtnIgnored(e: Event) {
        e.preventDefault();
        this.incidentType = "ignored";
        this.pager.reset();
        $(e.target).parent().find('label').removeClass('active');
        $(e.target).addClass('active');
        this.loadItems();
    }

    onCountCol(args: Event): void {
        if (this.sortType !== IncidentOrder.MostReports) {
            this.sortType = IncidentOrder.MostReports;
            this.sortAscending = true; //will be changed below
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
        let icon = IncidentTableViewModel.UP;
        if (!this.sortAscending) {
            icon = IncidentTableViewModel.DOWN;
        }
        $("#IndexView thead th span")
            .removeClass("fa-chevron-down")
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
                    return `#/application/${dto.ApplicationId}/incident/${dto.Id}/`;
                },
                text(value) {
                    return value;
                }
            },
            ApplicationName: {
                href(params, dto) {
                    return `#/application/${dto.ApplicationId}/`;
                },
                text(value) {
                    return value;
                }
            },
            LastReportReceivedAtUtc: {
                text(value) {
                    return momentsAgo(value);
                }
            }

        };
        if (this.applicationId == null) {
            delete directives.ApplicationName;
        }
        Yo.G.render(target, data.Items, directives);
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


    private loadItems(pageNumber: number = 0): void {
        const query = new codeRR.Core.Incidents.Queries.FindIncidents();

        if (this.applicationId != null) {
            query.ApplicationIds = [this.applicationId];
        }

        query.SortType = this.sortType;
        query.SortAscending = this.sortAscending;
        if (this.incidentType === "closed") {
            query.IsClosed = true;
        } else if (this.incidentType === 'ignored') {
            query.IsIgnored = true;
        } else if (this.incidentType === 'new') {
            query.IsNew = true;
        } else {
            query.IsAssigned = true;
        }
        if (pageNumber === 0) {
            query.PageNumber = this.pager.currentPage;
        } else {
            query.PageNumber = pageNumber;
        }
        if (this.tags.length > 0) {
            query.Tags = this.tags;
        }
        var searchBox = <HTMLInputElement>this.ctx.select.one('freeText');
        if (searchBox.value.length >= 3)
            query.FreeText = searchBox.value;

        query.ItemsPerPage = 20;
        CqsClient.query<codeRR.Core.Incidents.Queries.FindIncidentsResultItem>(query)
            .done(response => {
                var table = document.getElementById("incidentTable");
                this.renderTable(table, response);
                var yPos = this.findYPosition(table);
                window.scrollTo(0, yPos);
            });
    }

}