/// <reference path="../../Scripts/Promise.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />
/// <reference path="../ChartViewModel.ts" />
var OneTrueError;
(function (OneTrueError) {
    var Overview;
    (function (Overview) {
        var CqsClient = Griffin.Cqs.CqsClient;
        var IncidentOrder = OneTrueError.Core.Incidents.IncidentOrder;
        var Pager = Griffin.WebApp.Pager;
        var FindIncidents = OneTrueError.Core.Incidents.Queries.FindIncidents;
        var OverviewViewModel = (function () {
            function OverviewViewModel() {
                this._sortType = IncidentOrder.Newest;
                this._sortAscending = false;
                this._incidentType = "active";
                this.pager = new Pager(0, 0, 0);
                this.pager.subscribe(this);
            }
            OverviewViewModel.prototype.getTitle = function () {
                return "Overview";
            };
            OverviewViewModel.prototype.activate = function (ctx) {
                var _this = this;
                this._ctx = ctx;
                var query = new OneTrueError.Web.Overview.Queries.GetOverview();
                CqsClient.query(query)
                    .done(function (response) {
                    _this.renderInfo(response);
                    ctx.resolve();
                    _this.lineChart = new OneTrueError.LineChart(ctx.select.one("#myChart"));
                    _this.renderChart(response);
                });
                var pagerElement = ctx.select.one("pager");
                this.pager.draw(pagerElement);
                this.getIncidentsFromServer(1);
                ctx.handle.change('[name="range"]', function (e) { return _this.OnRange(e); });
                ctx.handle.click("#btnClosed", function (e) { return _this.onBtnClosed(e); });
                ctx.handle.click("#btnActive", function (e) { return _this.onBtnActive(e); });
                ctx.handle.click("#btnIgnored", function (e) { return _this.onBtnIgnored(e); });
                ctx.handle.click("#LastReportCol", function (e) { return _this.onLastReportCol(e); });
                ctx.handle.click("#CountCol", function (e) { return _this.onCountCol(e); });
            };
            OverviewViewModel.prototype.deactivate = function () {
            };
            OverviewViewModel.prototype.onPager = function (pager) {
                this.getIncidentsFromServer(pager.currentPage);
                var table = this._ctx.select.one("#incidentTable");
                //.scrollIntoView(true);
                var y = this.findYPosition(table);
                window.scrollTo(null, y - 50); //navbar 
            };
            OverviewViewModel.prototype.findYPosition = function (element) {
                if (element.offsetParent) {
                    var curtop = 0;
                    do {
                        curtop += element.offsetTop;
                        element = (element.offsetParent);
                    } while (element);
                    return curtop;
                }
            };
            OverviewViewModel.prototype.OnRange = function (e) {
                var _this = this;
                var query = new OneTrueError.Web.Overview.Queries.GetOverview();
                var elem = e.target;
                query.NumberOfDays = parseInt(elem.value, 10);
                CqsClient.query(query)
                    .done(function (response) {
                    _this.renderChart(response);
                });
            };
            OverviewViewModel.prototype.onBtnClosed = function (e) {
                e.preventDefault();
                this._incidentType = "closed";
                this.pager.reset();
                this.getIncidentsFromServer(0);
            };
            OverviewViewModel.prototype.onBtnActive = function (e) {
                e.preventDefault();
                this._incidentType = "active";
                this.pager.reset();
                this.getIncidentsFromServer(0);
            };
            OverviewViewModel.prototype.onBtnIgnored = function (e) {
                e.preventDefault();
                this._incidentType = "ignored";
                this.pager.reset();
                this.getIncidentsFromServer(0);
            };
            OverviewViewModel.prototype.onCountCol = function (e) {
                if (this._sortType !== IncidentOrder.MostReports) {
                    this._sortType = IncidentOrder.MostReports;
                    this._sortAscending = true; //will be changed below
                }
                this.updateSortingUI(e.target);
                this.getIncidentsFromServer(this.pager.currentPage);
            };
            OverviewViewModel.prototype.onLastReportCol = function (e) {
                if (this._sortType !== IncidentOrder.Newest) {
                    this._sortType = IncidentOrder.Newest;
                    this._sortAscending = true; //will be changed below
                }
                this.updateSortingUI(e.target);
                this.getIncidentsFromServer(this.pager.currentPage);
            };
            OverviewViewModel.prototype.renderTable = function (pageNumber, data) {
                var self = this;
                var directives = {
                    Items: {
                        Name: {
                            text: function (value) {
                                return value;
                            },
                            href: function (data, parentDto) {
                                return "#/application/" + parentDto.ApplicationId + "/incident/" + parentDto.Id;
                            }
                        },
                        CreatedAtUtc: {
                            text: function (value) {
                                return new Date(value).toLocaleString();
                            }
                        },
                        LastUpdateAtUtc: {
                            text: function (value) {
                                return new Date(value).toLocaleString();
                            }
                        },
                        ApplicationName: {
                            text: function (value) {
                                return value;
                            },
                            href: function (params, parentDto) {
                                return "#/application/" + parentDto.ApplicationId;
                            }
                        }
                    }
                };
                if (data.TotalCount === 0) {
                    this._ctx.select.one("getting-started").style.display = "";
                }
                this._ctx.renderPartial("#incidentTable", data, directives);
            };
            OverviewViewModel.prototype.renderInfo = function (dto) {
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
            };
            OverviewViewModel.prototype.updateSortingUI = function (parentElement) {
                this._sortAscending = !this._sortAscending;
                var icon = OverviewViewModel.UP;
                if (!this._sortAscending) {
                    icon = OverviewViewModel.DOWN;
                }
                $("#OverviewView thead th span")
                    .removeClass("glyphicon-chevron-down")
                    .addClass("glyphicon " + icon)
                    .css("visibility", "hidden");
                $("span", parentElement)
                    .attr("class", "glyphicon " + icon)
                    .css("visibility", "inherit");
            };
            OverviewViewModel.prototype.getIncidentsFromServer = function (pageNumber) {
                var _this = this;
                var query = new FindIncidents();
                query.SortType = this._sortType;
                query.SortAscending = this._sortAscending;
                query.PageNumber = pageNumber;
                query.ItemsPerPage = 10;
                if (this._incidentType === "closed") {
                    query.Closed = true;
                    query.Open = false;
                }
                //else if (this._incidentType === '')
                CqsClient.query(query)
                    .done(function (response) {
                    if (_this.pager.pageCount === 0) {
                        _this.pager.update(response.PageNumber, response.PageSize, response.TotalCount);
                    }
                    _this.renderTable(pageNumber, response);
                });
            };
            OverviewViewModel.prototype.renderChart = function (result) {
                var data = new OneTrueError.LineData();
                data.datasets = [];
                var legend = [];
                var found = false;
                var index = 0;
                result.IncidentsPerApplication.forEach(function (app) {
                    var sum = app.Values.reduce(function (a, b) { return a + b; }, 0);
                    if (sum === 0) {
                        return;
                    }
                    var ds = new OneTrueError.Dataset();
                    ds.label = app.Label;
                    ds.data = app.Values;
                    data.datasets.push(ds);
                    //not enough color themes to display all.
                    if (index > OneTrueError.LineChart.LineThemes.length) {
                        return;
                    }
                    var l = {
                        label: app.Label,
                        color: OneTrueError.LineChart.LineThemes[index++].strokeColor
                    };
                    legend.push(l);
                    found = true;
                });
                var directives = {
                    color: {
                        text: function () {
                            return "";
                        },
                        style: function (value, dto) {
                            return "display: inline; font-weight: bold; color: " + dto.color;
                        }
                    },
                    label: {
                        style: function (value, dto) {
                            return "font-weight: bold; color: " + dto.color;
                        }
                    }
                };
                this._ctx.renderPartial("#chart-legend", legend, directives);
                data.labels = result.TimeAxisLabels;
                if (data.datasets.length === 0) {
                    data.datasets.push({ label: 'No data', data: [] });
                }
                this.lineChart.render(data);
                this._ctx.renderPartial("StatSummary", result.StatSummary);
            };
            OverviewViewModel.UP = "glyphicon-chevron-up";
            OverviewViewModel.DOWN = "glyphicon-chevron-down";
            return OverviewViewModel;
        }());
        Overview.OverviewViewModel = OverviewViewModel;
    })(Overview = OneTrueError.Overview || (OneTrueError.Overview = {}));
})(OneTrueError || (OneTrueError = {}));
//# sourceMappingURL=OverviewViewModel.js.map