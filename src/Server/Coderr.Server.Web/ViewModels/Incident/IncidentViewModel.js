/// <reference path="../../Scripts/Promise.ts" />
/// <reference path="../../Scripts/typings/moment/moment.d.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />
/// <reference path="../ChartViewModel.ts" />
var codeRR;
(function (codeRR) {
    var Incident;
    (function (Incident) {
        var CqsClient = Griffin.Cqs.CqsClient;
        var Pager = Griffin.WebApp.Pager;
        var ReOpenIncident = codeRR.Core.Incidents.Commands.ReOpenIncident;
        var AssignIncident = codeRR.Core.Incidents.Commands.AssignIncident;
        var IncidentViewModel = /** @class */ (function () {
            function IncidentViewModel(appScope) {
                this.isIgnored = false;
                this.isAssigned = false;
                this.isNew = true;
            }
            IncidentViewModel.prototype.getTitle = function () {
                codeRR.Applications.Navigation.pageTitle = "Incident '" + this.name + "'";
                return "Incident " + this.name;
            };
            IncidentViewModel.prototype.activate = function (ctx) {
                var _this = this;
                this.ctx = ctx;
                var query = new codeRR.Core.Incidents.Queries.GetIncident(ctx.routeData["incidentId"]);
                CqsClient.query(query)
                    .done(function (response) {
                    window['currentIncident'] = response;
                    IncidentNavigation.set(ctx.routeData);
                    _this.isIgnored = response.IsIgnored;
                    _this.isAssigned = response.AssignedToId != null;
                    _this.isNew = response.AssignedToId == null && !response.IsSolved && !response.IsIgnored;
                    _this.assignedToName = response.AssignedTo;
                    if (response.IsSolved) {
                        response.Solution = marked(response.Solution);
                    }
                    _this.name = response.Description;
                    _this.id = response.Id;
                    _this.applicationId = response.ApplicationId;
                    _this.pager = new Pager(0, 0, 0);
                    _this.pager.subscribe(_this);
                    _this.pager.draw(ctx.select.one("#pager"));
                    _this.renderInfo(response);
                    var query = new codeRR.Core.Reports.Queries.GetReportList(_this.id);
                    query.PageNumber = 1;
                    query.PageSize = 20;
                    CqsClient.query(query)
                        .done(function (result) {
                        _this.pager.update(result.PageNumber, result.PageSize, result.TotalCount);
                        _this.renderTable(1, result);
                    });
                    var elem = ctx.select.one("#myChart");
                    ctx.handle.click("#reopenBtn", function (e) {
                        e.preventDefault();
                        CqsClient.command(new ReOpenIncident(_this.id))
                            .done(function (e) {
                            window.location.reload();
                        });
                    });
                    ctx.resolve();
                    var el = document.getElementById("pageMenuSource");
                    if (el) {
                        $('#pageMenu').html(el.innerHTML);
                        el.parentElement.removeChild(el);
                    }
                    _this.renderInitialChart(elem, response.DayStatistics);
                });
                ctx.handle.change('[name="range"]', function (e) { return _this.onRange(e); });
                ctx.handle.click('#assignToMe', function (e) { return _this.assignToMe(e); });
            };
            IncidentViewModel.prototype.deactivate = function () {
            };
            IncidentViewModel.prototype.assignToMe = function (e) {
                var cmd = new AssignIncident(this.id, window['ACCOUNT_ID'], window['ACCOUNT_ID']);
                CqsClient.command(cmd);
                humane.log('Assignment request have been sent.');
            };
            IncidentViewModel.prototype.onPager = function (pager) {
                var _this = this;
                var query = new codeRR.Core.Reports.Queries.GetReportList(this.id);
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
                var _this = this;
                var data = [];
                for (var i = 0; i < stats.length; ++i) {
                    //TODO: Why don't we get the same format here as in the other stat DTO
                    var dataItem = {
                        date: stats[i].Date.substr(0, 10),
                        Reports: stats[i].Count
                    };
                    data.push(dataItem);
                }
                $('#myChart').html('');
                this.chartOptions = {
                    element: 'myChart',
                    data: data,
                    xkey: 'date',
                    xLabels: "day",
                    ykeys: ['Reports'],
                    labels: ['Reports'],
                    lineColors: ['#0094DA']
                };
                this.chart = Morris.Line(this.chartOptions);
                $(window).resize(function () {
                    _this.chart.redraw();
                });
            };
            IncidentViewModel.prototype.renderTable = function (pageNumber, data) {
                var self = this;
                var directives = {
                    Items: {
                        CreatedAtUtc: {
                            text: function (value, dto) {
                                return momentsAgo(value);
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
                if (dto.IsSolved) {
                    dto.Tags.push("solved");
                }
                if (dto.IsIgnored) {
                    dto.Tags.push("ignored");
                }
                dto.Facts.push({
                    Title: "Last report",
                    Value: momentsAgo(dto.LastReportReceivedAtUtc),
                    Description: "When we received the last report",
                    Url: ''
                });
                if (this.isAssigned) {
                    dto.Facts.push({
                        Title: "Assigned to",
                        Value: dto.AssignedTo,
                        Description: "User that is currently working with this incident",
                        Url: ''
                    });
                }
                var directives = {
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
                    SolvedAtUtc: {
                        text: function (value) {
                            return momentsAgo(value);
                        }
                    },
                    Tags: {
                        "class": function (value) {
                            if (value === "solved" || value === "ignored")
                                return "label label-danger m-r-5";
                            return "label label-default m-r-5";
                        },
                        text: function (value) {
                            return value;
                        },
                        href: function (value) {
                            if (value.substr(0, 2) !== 'v-' && value !== "ignored" && value !== "solved")
                                return "http://stackoverflow.com/search?q=%5B" + value + "%5D+" + dto.Description;
                            return null;
                        }
                    },
                    Facts: {
                        Description: {
                            text: function (value) {
                                return '';
                            },
                            title: function (value) {
                                return value;
                            }
                        }
                    }
                };
                this.ctx.render(dto, directives);
            };
            IncidentViewModel.prototype.loadChartInfo = function (days) {
                var _this = this;
                var query = new codeRR.Core.Incidents.Queries.GetIncidentStatistics();
                query.IncidentId = this.id;
                query.NumberOfDays = days;
                CqsClient.query(query)
                    .done(function (response) {
                    var data = [];
                    var lastHour = -1;
                    var date = new Date();
                    date.setDate(date.getDate() - 1);
                    for (var i = 0; i < response.Values.length; ++i) {
                        var dataItem = {
                            date: response.Labels[i],
                            Reports: response.Values[i]
                        };
                        if (days === 1) {
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
                    _this.chartOptions.xLabelFormat = null;
                    if (days === 7) {
                        _this.chartOptions.xLabelFormat = function (xDate) {
                            return moment(xDate).format('dd');
                        };
                    }
                    else if (days === 1) {
                        _this.chartOptions.xLabels = "hour";
                    }
                    else {
                        _this.chartOptions.xLabels = "day";
                    }
                    _this.chart.setData(data, true);
                });
            };
            IncidentViewModel.UP = "fa-chevron-up";
            IncidentViewModel.DOWN = "fa-chevron-down";
            return IncidentViewModel;
        }());
        Incident.IncidentViewModel = IncidentViewModel;
    })(Incident = codeRR.Incident || (codeRR.Incident = {}));
})(codeRR || (codeRR = {}));
//# sourceMappingURL=IncidentViewModel.js.map