/// <reference path="../../Scripts/Promise.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />
/// <reference path="../ChartViewModel.ts" />
/// <reference path="../../Scripts/Griffin.Yo.d.ts" />
var OneTrueError;
(function (OneTrueError) {
    var Feedback;
    (function (Feedback) {
        var CqsClient = Griffin.Cqs.CqsClient;
        var Yo = Griffin.Yo;
        var ApplicationViewModel = (function () {
            function ApplicationViewModel() {
                this.RenderDirectives = {
                    Items: {
                        Message: {
                            html: function (value) {
                                return nl2br(value);
                            }
                        },
                        Title: {
                            style: function () {
                                return "color:#ccc";
                            },
                            html: function (value, dto) {
                                return "Reported for <a style=\"color: #ee99ee\" href=\"/application/" + dto.ApplicationId + "\">" + dto.ApplicationName + "</a> at " + new Date(dto.WrittenAtUtc).toLocaleString();
                            }
                        },
                        EmailAddressVisible: {
                            style: function (value, dto) {
                                if (!dto.EmailAddress || dto.EmailAddress === "") {
                                    return "display:none";
                                }
                                return "";
                            }
                        },
                        EmailAddress: {
                            text: function (value) {
                                return value;
                            },
                            href: function (value) {
                                return "mailto:" + value;
                            },
                            style: function () {
                                return "color: #ee99ee";
                            }
                        }
                    }
                };
            }
            ApplicationViewModel.prototype.getTitle = function () {
                var app = Yo.GlobalConfig
                    .applicationScope["application"];
                if (app) {
                    return app.Name;
                }
                return "Feedback";
            };
            ApplicationViewModel.prototype.activate = function (context) {
                var _this = this;
                this.context = context;
                var query = new OneTrueError.Web.Feedback.Queries.GetFeedbackForApplicationPage(context.routeData["applicationId"]);
                CqsClient.query(query)
                    .done(function (result) {
                    _this.dto = result;
                    context.render(result, _this.RenderDirectives);
                    context.resolve();
                });
            };
            ApplicationViewModel.prototype.deactivate = function () {
            };
            return ApplicationViewModel;
        }());
        Feedback.ApplicationViewModel = ApplicationViewModel;
    })(Feedback = OneTrueError.Feedback || (OneTrueError.Feedback = {}));
})(OneTrueError || (OneTrueError = {}));
//# sourceMappingURL=ApplicationViewModel.js.map