/// <reference path="../../Scripts/Promise.ts" />
/// <reference path="../../Scripts/Griffin.WebApp.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />
/// <reference path="../ChartViewModel.ts" />
var OneTrueError;
(function (OneTrueError) {
    var Application;
    (function (Application) {
        var CqsClient = Griffin.Cqs.CqsClient;
        var IncidentOrder = OneTrueError.Core.Incidents.IncidentOrder;
        var FindIncidents = OneTrueError.Core.Incidents.Queries.FindIncidents;
        var Yo = Griffin.Yo;
        var DetailsViewModel = (function () {
            function DetailsViewModel() {
                this.freeText = '';
                this._sortType = IncidentOrder.Newest;
                this._sortAscending = false;
                this._incidentType = "active";
            }
            DetailsViewModel.prototype.getTitle = function () {
                $("#appTitle").text(this.applicationName);
                return this.applicationName;
            };
            DetailsViewModel.prototype.activate = function (ctx) {
                var _this = this;
                this._ctx = ctx;
                this.applicationId = ctx.routeData["applicationId"];
                this.pager = new Griffin.WebApp.Pager(0, 0, 0);
                this.pager.subscribe(this);
                this.pager.draw(ctx.select.one("#pager"));
                var self = this;
                var firstIsRun = false;
                var chartResult = null;
                var chartRendering = function (result) {
                    //chart must be rendered after the element being attached and visible.
                    ctx.resolve();
                    self.lineChart = new OneTrueError.LineChart(ctx.select.one("#myChart"));
                    self.renderChart(result);
                };
                var appQuery = new OneTrueError.Core.Applications.Queries.GetApplicationInfo();
                appQuery.ApplicationId = ctx.routeData["applicationId"];
                CqsClient.query(appQuery)
                    .done(function (info) {
                    Yo.GlobalConfig.applicationScope["application"] = info;
                    _this.applicationName = info.Name;
                    if (chartResult != null) {
                        chartRendering(chartResult);
                    }
                    firstIsRun = true;
                    _this.renderInfo(info);
                });
                var query = new OneTrueError.Core.Applications.Queries.GetApplicationOverview(this.applicationId);
                CqsClient.query(query)
                    .done(function (response) {
                    if (!firstIsRun) {
                        chartResult = response;
                    }
                    else {
                        chartRendering(response);
                    }
                });
                this.getIncidentsFromServer(1);
                ctx.handle.click("#btnClosed", function (e) { return _this.onBtnClosed(e); });
                ctx.handle.click("#btnActive", function (e) { return _this.onBtnActive(e); });
                ctx.handle.click("#btnActive", function (e) { return _this.onBtnActive(e); });
                ctx.handle.click("#LastReportCol", function (e) { return _this.onLastReportCol(e); });
                ctx.handle.click("#CountCol", function (e) { return _this.onCountCol(e); });
                ctx.handle.change('[name="range"]', function (e) { return _this.onRange(e); });
                ctx.handle.keyUp('[data-name="freeText"]', function (e) { return _this.onFreeText(e); });
            };
            DetailsViewModel.prototype.deactivate = function () {
            };
            DetailsViewModel.prototype.onRange = function (e) {
                var _this = this;
                var elem = e.target;
                var days = parseInt(elem.value, 10);
                var query = new OneTrueError.Core.Applications.Queries.GetApplicationOverview(this.applicationId);
                query.NumberOfDays = days;
                CqsClient.query(query)
                    .done(function (response) {
                    _this.renderChart(response);
                });
            };
            DetailsViewModel.prototype.onFreeText = function (e) {
                var el = (e.target);
                if (el.value.length >= 3) {
                    this.freeText = el.value;
                    this.getIncidentsFromServer(this.pager.currentPage);
                }
                else if (el.value === '') {
                    this.freeText = el.value;
                    this.getIncidentsFromServer(this.pager.currentPage);
                }
                else {
                    this.freeText = el.value;
                }
            };
            DetailsViewModel.prototype.onBtnActive = function (e) {
                e.preventDefault();
                this._incidentType = "active";
                this.pager.reset();
                this.getIncidentsFromServer(-1);
            };
            DetailsViewModel.prototype.onBtnClosed = function (e) {
                e.preventDefault();
                this._incidentType = "closed";
                this.pager.reset();
                this.getIncidentsFromServer(-1);
            };
            DetailsViewModel.prototype.onBtnIgnored = function (e) {
                e.preventDefault();
                this._incidentType = "ignored";
                this.pager.reset();
                this.getIncidentsFromServer(-1);
            };
            DetailsViewModel.prototype.onPager = function (pager) {
                this.getIncidentsFromServer(pager.currentPage);
                var table = this._ctx.select.one("#incidentTable");
                //.scrollIntoView(true);
                var y = this.findYPosition(table);
                window.scrollTo(null, y - 50); //navbar 
            };
            DetailsViewModel.prototype.findYPosition = function (element) {
                if (element.offsetParent) {
                    var curtop = 0;
                    do {
                        curtop += element.offsetTop;
                        element = (element.offsetParent);
                    } while (element);
                    return curtop;
                }
            };
            DetailsViewModel.prototype.onCountCol = function (args) {
                if (this._sortType !== IncidentOrder.MostReports) {
                    this._sortType = IncidentOrder.MostReports;
                    this._sortAscending = true; //will be changed below
                }
                this.updateSortingUI(args.target);
                this.getIncidentsFromServer(this.pager.currentPage);
            };
            DetailsViewModel.prototype.onLastReportCol = function (args) {
                if (this._sortType !== IncidentOrder.Newest) {
                    this._sortType = IncidentOrder.Newest;
                    this._sortAscending = true; //will be changed below
                }
                this.updateSortingUI(args.target);
                this.getIncidentsFromServer(this.pager.currentPage);
            };
            DetailsViewModel.prototype.renderTable = function (pageNumber, data) {
                var directives = {
                    Items: {
                        IncidentName: {
                            text: function (data) {
                                return data;
                            },
                            href: function (data, dto) {
                                return "#/application/" + dto.ApplicationId + "/incident/" + dto.Id;
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
                            href: function (value, dto) {
                                return "#/application/" + dto.ApplicationId;
                            }
                        }
                    }
                };
                //workaround for root dto name rendering over our collection name
                data.Items.forEach(function (item) {
                    item.IncidentName = item.Name;
                });
                this._ctx.renderPartial("#incidentTable", data, directives);
            };
            DetailsViewModel.prototype.renderInfo = function (dto) {
                var directives = {
                    CreatedAtUtc: {
                        text: function (value) {
                            return new Date(value).toLocaleString();
                        }
                    },
                    UpdatedAtUtc: {
                        text: function (value) {
                            return new Date(value).toLocaleString();
                        }
                    },
                    SolvedAtUtc: {
                        text: function (value) {
                            return new Date(value).toLocaleString();
                        }
                    }
                };
                dto["StatSummary"] = {
                    AppKey: dto.AppKey,
                    SharedSecret: dto.SharedSecret
                };
                this._ctx.render(dto, directives);
            };
            DetailsViewModel.prototype.updateSortingUI = function (parentElement) {
                this._sortAscending = !this._sortAscending;
                var icon = DetailsViewModel.UP;
                if (!this._sortAscending) {
                    icon = DetailsViewModel.DOWN;
                }
                $("#ApplicationView thead th span")
                    .removeClass("glyphicon-chevron-down")
                    .addClass("glyphicon " + icon)
                    .css("visibility", "hidden");
                $("span", parentElement)
                    .attr("class", "glyphicon " + icon)
                    .css("visibility", "inherit");
            };
            DetailsViewModel.prototype.getIncidentsFromServer = function (pageNumber) {
                var _this = this;
                if (pageNumber === -1) {
                    pageNumber = this.pager.currentPage;
                }
                var query = new FindIncidents();
                query.SortType = this._sortType;
                query.SortAscending = this._sortAscending;
                query.PageNumber = pageNumber;
                query.ItemsPerPage = 10;
                query.ApplicationId = this.applicationId;
                query.FreeText = this.freeText;
                if (this._incidentType === "closed") {
                    query.Closed = true;
                    query.Open = false;
                }
                else if (this._incidentType === "ignored") {
                    query.Closed = false;
                    query.Open = false;
                    query.ReOpened = false;
                    query.Ignored = true;
                }
                CqsClient.query(query)
                    .done(function (response) {
                    if (_this.pager.pageCount === 0) {
                        _this.pager.update(response.PageNumber, response.PageSize, response.TotalCount);
                    }
                    _this.renderTable(pageNumber, response);
                });
            };
            DetailsViewModel.prototype.renderChart = function (result) {
                var legend = [
                    {
                        color: OneTrueError.LineChart.LineThemes[0].strokeColor,
                        label: "Incidents"
                    }, {
                        color: OneTrueError.LineChart.LineThemes[1].strokeColor,
                        label: "Reports"
                    }
                ];
                var incidentsDataset = new OneTrueError.Dataset();
                incidentsDataset.label = "Incidents";
                incidentsDataset.data = result.Incidents;
                var reportDataset = new OneTrueError.Dataset();
                reportDataset.label = "Reports";
                reportDataset.data = result.ErrorReports;
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
                //$('#chart-legend').render(legend, directives);
                var data = new OneTrueError.LineData();
                data.datasets = [incidentsDataset, reportDataset];
                data.labels = result.TimeAxisLabels;
                this.lineChart.render(data);
                this._ctx.renderPartial('[data-name="StatSummary"]', result.StatSummary);
            };
            DetailsViewModel.UP = "glyphicon-chevron-up";
            DetailsViewModel.DOWN = "glyphicon-chevron-down";
            return DetailsViewModel;
        }());
        Application.DetailsViewModel = DetailsViewModel;
    })(Application = OneTrueError.Application || (OneTrueError.Application = {}));
})(OneTrueError || (OneTrueError = {}));
//# sourceMappingURL=DetailsViewModel.js.map