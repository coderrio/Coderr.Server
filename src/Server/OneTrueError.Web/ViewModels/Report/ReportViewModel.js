/// <reference path="../../Scripts/Promise.ts" />
/// <reference path="../../Scripts/Griffin.WebApp.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />
/// <reference path="../../Scripts/Griffin.Yo.d.ts" />
var OneTrueError;
(function (OneTrueError) {
    var Report;
    (function (Report) {
        var CqsClient = Griffin.Cqs.CqsClient;
        var ReportViewModel = (function () {
            function ReportViewModel() {
            }
            ReportViewModel.prototype.renderView = function () {
                var directives = {
                    CreatedAtUtc: {
                        text: function (value) {
                            return new Date(value).toLocaleString();
                        }
                    },
                    ContextCollections: {
                        ContextCollectionName: {
                            html: function (value, dto) {
                                return dto.Name;
                            },
                            href: function (value, dto) {
                                return "#" + dto.Name;
                            }
                        }
                    }
                };
                this.context.render(this.dto, directives);
            };
            ReportViewModel.prototype.getTitle = function () {
                return "Report";
            };
            ReportViewModel.prototype.activate = function (context) {
                var _this = this;
                this.context = context;
                var reportId = context.routeData["reportId"];
                var query = new OneTrueError.Core.Reports.Queries.GetReport(reportId);
                CqsClient.query(query)
                    .done(function (dto) {
                    _this.dto = dto;
                    _this.renderView();
                    context.resolve();
                });
                context.handle.click('[data-collection="ContextCollections"]', function (evt) {
                    evt.preventDefault();
                    var target = evt.target;
                    if (target.tagName === "LI") {
                        _this.selectCollection(target.firstElementChild.textContent);
                        $("li", target.parentElement).removeClass("active");
                        $(target).addClass("active");
                    }
                    else if (target.tagName === "A") {
                        _this.selectCollection(target.textContent);
                        $("li", target.parentElement.parentElement).removeClass("active");
                        $(target.parentElement).addClass("active");
                    }
                }, true);
            };
            ReportViewModel.prototype.selectCollection = function (collectionName) {
                var _this = this;
                this.dto.ContextCollections.forEach(function (item) {
                    if (item.Name === collectionName) {
                        var directives = {
                            Properties: {
                                Key: {
                                    html: function (value) {
                                        return value;
                                    }
                                },
                                Value: {
                                    html: function (value, dto) {
                                        if (collectionName === "Screenshots") {
                                            return "<img alt=\"Embedded Image\" src=\"data:image/png;base64," + value + "\" />";
                                        }
                                        else {
                                            return value.replace(/;;/g, "<br>");
                                        }
                                    }
                                }
                            }
                        };
                        _this.context.renderPartial("propertyTable", item, directives);
                        return;
                    }
                });
            };
            ReportViewModel.prototype.deactivate = function () { };
            return ReportViewModel;
        }());
        Report.ReportViewModel = ReportViewModel;
    })(Report = OneTrueError.Report || (OneTrueError.Report = {}));
})(OneTrueError || (OneTrueError = {}));
//# sourceMappingURL=ReportViewModel.js.map