/// <reference path="../../Scripts/Griffin.WebApp.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />
var codeRR;
(function (codeRR) {
    var Application;
    (function (Application) {
        var CqsClient = Griffin.Cqs.CqsClient;
        var Yo = Griffin.Yo;
        var DetailsViewModel = (function () {
            function DetailsViewModel() {
                this.chartDays = 30;
                this.firstRender = true;
            }
            DetailsViewModel.prototype.getTitle = function () {
                var bc = [
                    { href: "/application/" + this.applicationId + "/", title: this.applicationName }
                ];
                codeRR.Applications.Navigation.breadcrumbs(bc);
                codeRR.Applications.Navigation.pageTitle = this.applicationName;
                return this.applicationName;
            };
            DetailsViewModel.prototype.activate = function (ctx) {
                var _this = this;
                this.incidentTable = new IncidentTableViewModel(ctx);
                this.ctx = ctx;
                this.applicationId = ctx.routeData["applicationId"];
                var self = this;
                var firstIsRun = false;
                var chartResult = null;
                var chartRendering = function (result) {
                    //chart must be rendered after the element being attached and visible.
                    ctx.resolve();
                    self.renderChart(result);
                };
                var items = ctx.select.all('.wizard-config');
                items.forEach(function (item) {
                    var href = window['WEB_ROOT'] +
                        'configure/choose/package?applicationId=' +
                        ctx.routeData['applicationId'];
                    item.setAttribute('href', href);
                });
                var appQuery = new codeRR.Core.Applications.Queries.GetApplicationInfo();
                appQuery.ApplicationId = ctx.routeData["applicationId"];
                CqsClient.query(appQuery)
                    .done(function (info) {
                    Yo.GlobalConfig.applicationScope["application"] = info;
                    _this.applicationName = info.Name;
                    firstIsRun = true;
                    if (chartResult != null) {
                        chartRendering(chartResult);
                    }
                    _this.renderInfo(info);
                    ctx.handle.click(".version", function (e) {
                        var version = e.target.getAttribute("data-value");
                        _this.filterVersion = version;
                        ctx.render({ ActiveVersion: 'Filter: v' + version });
                        _this.updateAppInfo();
                        _this.incidentTable.load(_this.applicationId, _this.filterVersion);
                    });
                });
                var query = new codeRR.Core.Applications.Queries.GetApplicationOverview(this.applicationId);
                query.Version = this.filterVersion;
                CqsClient.query(query)
                    .done(function (response) {
                    if (!firstIsRun) {
                        chartResult = response;
                    }
                    else {
                        chartRendering(response);
                    }
                });
                this.incidentTable.load(this.applicationId);
                ctx.handle.change('[name="range"]', function (e) { return _this.onRange(e); });
            };
            DetailsViewModel.prototype.deactivate = function () {
            };
            DetailsViewModel.prototype.onRange = function (e) {
                var elem = e.target;
                var days = parseInt(elem.value, 10);
                this.chartDays = days;
                this.updateAppInfo();
            };
            DetailsViewModel.prototype.updateAppInfo = function () {
                var _this = this;
                var query = new codeRR.Core.Applications.Queries.GetApplicationOverview(this.applicationId);
                query.NumberOfDays = this.chartDays;
                query.Version = this.filterVersion;
                CqsClient.query(query)
                    .done(function (response) {
                    _this.updateChart(response);
                    _this.ctx.render(response);
                });
            };
            DetailsViewModel.prototype.renderInfo = function (dto) {
                var directives = {
                    CreatedAtUtc: {
                        text: function (value) {
                            return momentsAgo(value);
                        }
                    },
                    UpdatedAtUtc: {
                        text: function (value) {
                            return momentsAgo(value);
                        }
                    },
                    SolvedAtUtc: {
                        text: function (value) {
                            return momentsAgo(value);
                        }
                    },
                    Versions: {
                        text: function (value, dto) {
                            return "v" + dto;
                        },
                        "data-value": function (value, dto) {
                            return dto;
                        }
                    }
                };
                dto.gotIncidents = dto.TotalIncidentCount > 0;
                this.ctx.render(dto, directives);
            };
            DetailsViewModel.prototype.renderChart = function (result) {
                var _this = this;
                var data = [];
                var labels = [{ name: "Reports", color: "#0094DA" }, { name: "Incidents", color: "#2B4141" }];
                for (var i = 0; i < result.ErrorReports.length; ++i) {
                    var dataItem = {
                        date: result.TimeAxisLabels[i],
                        Reports: result.ErrorReports[i],
                        Incidents: result.Incidents[i]
                    };
                    data.push(dataItem);
                }
                $("#myChart").html("");
                this.chartOptions = {
                    element: $("#myChart")[0],
                    data: data,
                    xkey: "date",
                    ykeys: ["Reports", "Incidents"],
                    xLabels: result.Days === 1 ? "hour" : "day",
                    labels: ["Reports", "Incidents"],
                    lineColors: ["#0094DA", "#2B4141"]
                };
                this.chart = Morris.Line(this.chartOptions);
                if (this.firstRender) {
                    this.firstRender = false;
                    $(window).resize(function () {
                        _this.chart.redraw();
                    });
                }
                var directives = {
                    labels: {
                        color: {
                            style: function (value) {
                                return "color: " + value;
                            },
                            html: function (value) {
                                return "";
                            }
                        },
                        name: {
                            html: function (value) {
                                return value;
                            }
                        }
                    }
                };
                this.ctx.renderPartial("legend", { labels: labels }, directives);
                this.ctx.renderPartial('[data-name="StatSummary"]', result.StatSummary);
            };
            DetailsViewModel.prototype.updateChart = function (result) {
                var data = [];
                var lastHour = -1;
                var date = new Date();
                date.setDate(date.getDate() - 1);
                for (var i = 0; i < result.ErrorReports.length; ++i) {
                    var dataItem = {
                        date: result.TimeAxisLabels[i],
                        Reports: result.ErrorReports[i],
                        Incidents: result.Incidents[i]
                    };
                    if (this.chartDays === 1) {
                        var hour = parseInt(dataItem.date.substr(0, 2), 10);
                        if (hour < lastHour && lastHour !== -1) {
                            date = new Date();
                        }
                        lastHour = hour;
                        var minute = parseInt(dataItem.date.substr(3, 2), 10);
                        date.setHours(hour, minute);
                        dataItem.date = date.toISOString();
                    }
                    data.push(dataItem);
                }
                this.chartOptions.xLabelFormat = null;
                if (this.chartDays === 7) {
                    this.chartOptions.xLabelFormat = function (xDate) {
                        return moment(xDate).format("dd");
                    };
                }
                else if (this.chartDays === 1) {
                    this.chartOptions.xLabels = "hour";
                }
                else {
                    this.chartOptions.xLabels = "day";
                }
                this.chart.setData(data, true);
            };
            return DetailsViewModel;
        }());
        Application.DetailsViewModel = DetailsViewModel;
    })(Application = codeRR.Application || (codeRR.Application = {}));
})(codeRR || (codeRR = {}));
//# sourceMappingURL=DetailsViewModel.js.map