/// <reference path="../../Scripts/Promise.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />
var OneTrueError;
(function (OneTrueError) {
    var Incident;
    (function (Incident) {
        var CqsClient = Griffin.Cqs.CqsClient;
        var IncidentOrder = OneTrueError.Core.Incidents.IncidentOrder;
        var Yo = Griffin.Yo;
        var IndexViewModel = (function () {
            function IndexViewModel(dto) {
                this.sortType = IncidentOrder.Newest;
                this.sortAscending = false;
                this.closed = false;
                this.open = true;
                this.reOpened = false;
            }
            IndexViewModel.prototype.getTitle = function () {
                return "Incidents";
            };
            IndexViewModel.prototype.activate = function (ctx) {
                var _this = this;
                var query = new OneTrueError.Core.Incidents.Queries.FindIncidents();
                query.PageNumber = 1;
                query.ItemsPerPage = 20;
                CqsClient.query(query)
                    .done(function (response) {
                    var itemsElem = ctx.viewContainer.querySelector("#incidentTable");
                    _this.renderTable(itemsElem, response);
                    ctx.resolve();
                    _this.pager = new Griffin.WebApp.Pager(response.PageNumber, 20, response.TotalCount);
                    _this.pager.subscribe(_this);
                    _this.pager.draw(ctx.select.one("#pager"));
                });
                ctx.handle.click("#btnClosed", function (e) { return _this.onBtnClosed(e); });
                ctx.handle.click("#btnActive", function (e) { return _this.onBtnActive(e); });
                ctx.handle.click("#btnIgnored", function (e) { return _this.onBtnIgnored(e); });
                ctx.handle.click("#LastReportCol", function (e) { return _this.onLastReportCol(e); });
                ctx.handle.click("#CountCol", function (e) { return _this.onCountCol(e); });
            };
            IndexViewModel.prototype.deactivate = function () {
            };
            IndexViewModel.prototype.onPager = function (pager) {
                this.loadItems(pager.currentPage);
            };
            IndexViewModel.prototype.onBtnActive = function (e) {
                e.preventDefault();
                this.reOpened = true;
                this.open = true;
                this.closed = false;
                this.pager.reset();
                this.loadItems(0);
            };
            IndexViewModel.prototype.onBtnClosed = function (e) {
                e.preventDefault();
                this.reOpened = false;
                this.open = false;
                this.closed = true;
                this.pager.reset();
                this.loadItems(0);
            };
            IndexViewModel.prototype.onBtnIgnored = function (e) {
                e.preventDefault();
                this.reOpened = false;
                this.open = false;
                this.closed = false;
                this.pager.reset();
                this.loadItems(0);
            };
            IndexViewModel.prototype.onCountCol = function (args) {
                if (this.sortType !== IncidentOrder.MostReports) {
                    this.sortType = IncidentOrder.MostReports;
                    this.sortAscending = true; //will be changed below
                }
                if (this.sortAscending) {
                }
                else {
                }
                this.updateSorting(args.target);
                this.loadItems();
            };
            IndexViewModel.prototype.onLastReportCol = function (args) {
                if (this.sortType !== IncidentOrder.Newest) {
                    this.sortType = IncidentOrder.Newest;
                    this.sortAscending = false; //will be changed below
                }
                if (this.sortAscending) {
                }
                else {
                }
                this.updateSorting(args.target);
                this.loadItems();
            };
            IndexViewModel.prototype.updateSorting = function (parentElement) {
                this.sortAscending = !this.sortAscending;
                var icon = IndexViewModel.UP;
                if (!this.sortAscending) {
                    icon = IndexViewModel.DOWN;
                }
                $("#IndexView thead th span")
                    .removeClass("glyphicon-chevron-down")
                    .addClass("glyphicon " + icon)
                    .css("visibility", "hidden");
                $("span", parentElement)
                    .attr("class", "glyphicon " + icon)
                    .css("visibility", "inherit");
            };
            IndexViewModel.prototype.renderTable = function (target, data) {
                var directives = {
                    Name: {
                        href: function (params, dto) {
                            return "#/application/" + dto.ApplicationId + "/incident/" + dto.Id;
                        },
                        text: function (value) {
                            return value;
                        }
                    },
                    ApplicationName: {
                        href: function (params, dto) {
                            return "#application/" + dto.ApplicationId + "/incidents/";
                        },
                        text: function (value) {
                            return value;
                        }
                    },
                    LastUpdateAtUtc: {
                        text: function (value) {
                            return new Date(value).toLocaleString();
                        }
                    }
                };
                Yo.G.render(target, data.Items, directives);
            };
            IndexViewModel.prototype.loadItems = function (pageNumber) {
                var _this = this;
                if (pageNumber === void 0) { pageNumber = 0; }
                var query = new OneTrueError.Core.Incidents.Queries.FindIncidents();
                query.SortType = this.sortType;
                query.SortAscending = this.sortAscending;
                query.Closed = this.closed;
                query.Open = this.open;
                query.ReOpened = this.reOpened;
                query.Ignored = !this.reOpened && !this.closed && !this.open;
                if (pageNumber === 0) {
                    query.PageNumber = this.pager.currentPage;
                }
                else {
                    query.PageNumber = pageNumber;
                }
                query.ItemsPerPage = 20;
                CqsClient.query(query)
                    .done(function (response) {
                    //this.pager.update(response.PageNumber, 20, response.TotalCount);
                    _this.renderTable(document.getElementById("incidentTable"), response);
                    window.scrollTo(0, 0);
                });
            };
            IndexViewModel.UP = "glyphicon-chevron-up";
            IndexViewModel.DOWN = "glyphicon-chevron-down";
            return IndexViewModel;
        }());
        Incident.IndexViewModel = IndexViewModel;
    })(Incident = OneTrueError.Incident || (OneTrueError.Incident = {}));
})(OneTrueError || (OneTrueError = {}));
//# sourceMappingURL=IndexViewModel.js.map