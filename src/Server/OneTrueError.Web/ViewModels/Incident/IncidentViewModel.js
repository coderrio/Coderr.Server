/// <reference path="../../Scripts/Promise.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />
/// <reference path="../ChartViewModel.ts" />
var OneTrueError;
(function (OneTrueError) {
    var Incident;
    (function (Incident) {
        var CqsClient = Griffin.Cqs.CqsClient;
        var Pager = Griffin.WebApp.Pager;
        var IncidentViewModel = (function () {
            function IncidentViewModel(appScope) {
                this.isIgnored = false;
            }
            IncidentViewModel.prototype.getTitle = function () {
                return "Incident " + this.name;
            };
            IncidentViewModel.prototype.activate = function (ctx) {
                var _this = this;
                var query = new OneTrueError.Core.Incidents.Queries.GetIncident(ctx.routeData["incidentId"]);
                this.ctx = ctx;
                CqsClient.query(query)
                    .done(function (response) {
                    _this.isIgnored = response.IsIgnored;
                    _this.name = response.Description;
                    _this.id = response.Id;
                    _this.applicationId = response.ApplicationId;
                    _this.pager = new Pager(0, 0, 0);
                    _this.pager.subscribe(_this);
                    _this.pager.draw(ctx.select.one("#pager"));
                    _this.renderInfo(response);
                    var query = new OneTrueError.Core.Reports.Queries.GetReportList(_this.id);
                    query.PageNumber = 1;
                    query.PageSize = 20;
                    CqsClient.query(query)
                        .done(function (result) {
                        _this.pager.update(result.PageNumber, result.PageSize, result.TotalCount);
                        _this.renderTable(1, result);
                    });
                    var elem = ctx.select.one("#myChart");
                    ctx.resolve();
                    _this.renderInitialChart(elem, response.DayStatistics);
                });
                ctx.handle.change('[name="range"]', function (e) { return _this.onRange(e); });
            };
            IncidentViewModel.prototype.deactivate = function () {
            };
            IncidentViewModel.prototype.onPager = function (pager) {
                var _this = this;
                var query = new OneTrueError.Core.Reports.Queries.GetReportList(this.id);
                query.PageNumber = pager.currentPage;
                query.PageSize = 20;
                CqsClient.query(query)
                    .done(function (result) {
                    _this.renderTable(1, result);
                });
            };
            IncidentViewModel.prototype.onRange = function (e) {
                var elem = e.target;
                var days = parseInt(elem.value, 10);
                this.loadChartInfo(days);
            };
            IncidentViewModel.prototype.renderInitialChart = function (chartElement, stats) {
                var labels = new Array();
                var dataset = new OneTrueError.Dataset();
                dataset.label = "Error reports";
                dataset.data = new Array();
                stats.forEach(function (item) {
                    labels.push(new Date(item.Date).toLocaleDateString());
                    dataset.data.push(item.Count);
                });
                var data = new OneTrueError.LineData();
                data.datasets = [dataset];
                data.labels = labels;
                this.lineChart = new OneTrueError.LineChart(chartElement);
                this.lineChart.render(data);
                //this.loadChartInfo(30);
            };
            IncidentViewModel.prototype.renderTable = function (pageNumber, data) {
                var self = this;
                var directives = {
                    Items: {
                        CreatedAtUtc: {
                            text: function (value, dto) {
                                return new Date(value).toLocaleString();
                            }
                        },
                        Message: {
                            text: function (value, dto) {
                                if (!value) {
                                    return "(No exception message)";
                                }
                                return dto.Message;
                            },
                            href: function (value, dto) {
                                return "#/application/" + self.applicationId + "/incident/" + self.id + "/report/" + dto.Id;
                            }
                        }
                    }
                };
                this.ctx.renderPartial("#reportsTable", data, directives);
            };
            IncidentViewModel.prototype.renderInfo = function (dto) {
                var self = this;
                if (dto.IsSolved) {
                    dto.Tags.push("solved");
                }
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
                    },
                    Tags: {
                        "class": function (value) {
                            if (value === "solved")
                                return "tag bg-danger";
                            return "tag nephritis";
                        },
                        text: function (value) {
                            return value;
                        },
                        href: function (value) {
                            return "http://stackoverflow.com/search?q=%5B" + value + "%5D+" + dto.Description;
                        }
                    }
                };
                this.ctx.render(dto, directives);
                if (dto.IsSolved) {
                    this.ctx.select.one("#actionButtons").style.display = "none";
                    this.ctx.select.one('[data-name="Description"]').style.textDecoration = "line-through";
                }
            };
            IncidentViewModel.prototype.loadChartInfo = function (days) {
                var _this = this;
                var query = new OneTrueError.Core.Incidents.Queries.GetIncidentStatistics();
                query.IncidentId = this.id;
                query.NumberOfDays = days;
                CqsClient.query(query)
                    .done(function (response) {
                    var dataset = new OneTrueError.Dataset();
                    dataset.label = "Error reports";
                    dataset.data = response.Values;
                    var data = new OneTrueError.LineData();
                    data.datasets = [dataset];
                    data.labels = response.Labels;
                    _this.lineChart.render(data);
                });
            };
            IncidentViewModel.UP = "glyphicon-chevron-up";
            IncidentViewModel.DOWN = "glyphicon-chevron-down";
            return IncidentViewModel;
        }());
        Incident.IncidentViewModel = IncidentViewModel;
    })(Incident = OneTrueError.Incident || (OneTrueError.Incident = {}));
})(OneTrueError || (OneTrueError = {}));
//# sourceMappingURL=IncidentViewModel.js.map