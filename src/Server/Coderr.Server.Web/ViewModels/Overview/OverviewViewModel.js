/// <reference path="../../Scripts/Promise.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />
/// <reference path="../ChartViewModel.ts" />
var codeRR;
(function (codeRR) {
    var Overview;
    (function (Overview) {
        var CqsClient = Griffin.Cqs.CqsClient;
        var IncidentOrder = codeRR.Core.Incidents.IncidentOrder;
        var Pager = Griffin.WebApp.Pager;
        var FindIncidents = codeRR.Core.Incidents.Queries.FindIncidents;
        var GetApplicationList = codeRR.Core.Applications.Queries.GetApplicationList;
        var OverviewViewModel = (function () {
            function OverviewViewModel() {
                this._sortType = IncidentOrder.Newest;
                this._sortAscending = false;
                this._incidentType = "new";
                this.pager = new Pager(0, 0, 0);
                this.pager.subscribe(this);
            }
            OverviewViewModel.prototype.getTitle = function () {
                codeRR.Applications.Navigation.breadcrumbs([]);
                codeRR.Applications.Navigation.pageTitle = 'Dashboard <em class="small">(summary for all applications)</em>';
                return "Overview";
            };
            OverviewViewModel.prototype.activate = function (ctx) {
                var _this = this;
                this._ctx = ctx;
                var query = new codeRR.Web.Overview.Queries.GetOverview();
                CqsClient.query(query)
                    .done(function (response) {
                    ctx.render(response);
                    ctx.resolve();
                    _this.renderChart(response, 30);
                });
                var apps = new GetApplicationList();
                CqsClient.query(apps)
                    .done(function (reply) {
                    if (reply.length === 0) {
                        ctx.select.one('config-help-guru').style.display = '';
                    }
                });
                var pagerElement = ctx.select.one("pager");
                this.pager.draw(pagerElement);
                this.getIncidentsFromServer(1);
                ctx.handle.change('[name="range"]', function (e) { return _this.OnRange(e); });
                ctx.handle.click("#btnNew", function (e) { return _this.onBtnNew(e); });
                ctx.handle.click("#btnAssigned", function (e) { return _this.onBtnAssigned(e); });
                ctx.handle.click("#btnIgnored", function (e) { return _this.onBtnIgnored(e); });
                ctx.handle.click("#btnClosed", function (e) { return _this.onBtnClosed(e); });
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
                var query = new codeRR.Web.Overview.Queries.GetOverview();
                var elem = e.target;
                query.NumberOfDays = parseInt(elem.value, 10);
                CqsClient.query(query)
                    .done(function (response) {
                    _this.updateChart(response, query.NumberOfDays);
                });
            };
            OverviewViewModel.prototype.onBtnClosed = function (e) {
                e.preventDefault();
                this._incidentType = "closed";
                this.pager.reset();
                $(e.target).parent().find('label').removeClass('active');
                $(e.target).addClass('active');
                this.getIncidentsFromServer(0);
            };
            OverviewViewModel.prototype.onBtnNew = function (e) {
                e.preventDefault();
                this._incidentType = "new";
                this.pager.reset();
                $(e.target).parent().find('label').removeClass('active');
                $(e.target).addClass('active');
                this.getIncidentsFromServer(0);
            };
            OverviewViewModel.prototype.onBtnAssigned = function (e) {
                e.preventDefault();
                this._incidentType = "assigned";
                this.pager.reset();
                $(e.target).parent().find('label').removeClass('active');
                $(e.target).addClass('active');
                this.getIncidentsFromServer(0);
            };
            OverviewViewModel.prototype.onBtnIgnored = function (e) {
                e.preventDefault();
                this._incidentType = "ignored";
                this.pager.reset();
                $(e.target).parent().find('label').removeClass('active');
                $(e.target).addClass('active');
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
                                return momentsAgo(value);
                            }
                        },
                        LastReportReceivedAtUtc: {
                            text: function (value) {
                                return momentsAgo(value);
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
                    //this._ctx.select.one("getting-started").style.display = "";
                }
                this._ctx.renderPartial("#incidentTable", data, directives);
            };
            OverviewViewModel.prototype.updateSortingUI = function (parentElement) {
                this._sortAscending = !this._sortAscending;
                var icon = OverviewViewModel.UP;
                if (!this._sortAscending) {
                    icon = OverviewViewModel.DOWN;
                }
                $("#OverviewView thead th span")
                    .removeClass("fa-chevron-down")
                    .addClass("fa " + icon)
                    .css("visibility", "hidden");
                $("span", parentElement)
                    .attr("class", "fa " + icon)
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
                CqsClient.query(query)
                    .done(function (response) {
                    if (_this.pager.pageCount === 0) {
                        _this.pager.update(response.PageNumber, response.PageSize, response.TotalCount);
                    }
                    _this.renderTable(pageNumber, response);
                });
            };
            OverviewViewModel.prototype.renderChart = function (result, numberOfDays) {
                var _this = this;
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
                for (var y = 0; y < result.IncidentsPerApplication.length; y++) {
                    var item = result.IncidentsPerApplication[y];
                    yKeys.push(item.Label);
                    legendLabels.push({
                        color: availableColors[y],
                        name: item.Label
                    });
                }
                for (var i = 0; i < result.TimeAxisLabels.length; ++i) {
                    var dataItem = {
                        date: result.TimeAxisLabels[i]
                    };
                    for (var y = 0; y < result.IncidentsPerApplication.length; y++) {
                        var item = result.IncidentsPerApplication[y];
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
                            style: function (value) {
                                return 'color: ' + value;
                            },
                            html: function (value) {
                                return '';
                            }
                        },
                        name: {
                            html: function (value) {
                                return value;
                            }
                        }
                    }
                };
                this._ctx.renderPartial("legend", { labels: legendLabels }, directives);
                this._ctx.renderPartial("StatSummary", result.StatSummary);
                $(window).resize(function () {
                    _this.chart.redraw();
                });
            };
            OverviewViewModel.prototype.updateChart = function (result, numberOfDays) {
                var data = [];
                var lastHour = -1;
                var date = new Date();
                date.setDate(date.getDate() - 1);
                for (var i = 0; i < result.TimeAxisLabels.length; ++i) {
                    var dataItem = {
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
                    for (var y = 0; y < result.IncidentsPerApplication.length; y++) {
                        var item = result.IncidentsPerApplication[y];
                        dataItem[item.Label] = item.Values[i];
                    }
                    data.push(dataItem);
                }
                this.chartOptions.xLabelFormat = null;
                if (numberOfDays === 7) {
                    this.chartOptions.xLabelFormat = function (xDate) {
                        return moment(xDate).format('dd');
                    };
                }
                else if (numberOfDays === 1) {
                    this.chartOptions.xLabels = "hour";
                }
                else {
                    this.chartOptions.xLabels = "day";
                }
                this.chart.setData(data, true);
            };
            return OverviewViewModel;
        }());
        OverviewViewModel.UP = "fa-chevron-up";
        OverviewViewModel.DOWN = "fa-chevron-down";
        Overview.OverviewViewModel = OverviewViewModel;
    })(Overview = codeRR.Overview || (codeRR.Overview = {}));
})(codeRR || (codeRR = {}));
//# sourceMappingURL=OverviewViewModel.js.map