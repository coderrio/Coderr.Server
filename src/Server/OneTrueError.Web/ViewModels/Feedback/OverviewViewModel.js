/// <reference path="../../Scripts/Models/AllModels.ts" />
/// <reference path="../../Scripts/Promise.ts" />
/// <reference path="../../Scripts/Griffin.WebApp.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />
/// <reference path="../ChartViewModel.ts" />
/// <reference path="../../Scripts/Griffin.Yo.d.ts" />
var OneTrueError;
(function (OneTrueError) {
    var Feedback;
    (function (Feedback) {
        var CqsClient = Griffin.Cqs.CqsClient;
        var OverviewFeedback = OneTrueError.Web.Feedback.Queries.GetFeedbackForDashboardPage;
        //import Yo = Griffin.Yo;
        var OverviewViewModel = (function () {
            function OverviewViewModel() {
                this.feedbackDirectives = {
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
                                return "Reported for <a style=\"color: #ee99ee\" href=\"#/application/" + dto.ApplicationId + "\">" + dto.ApplicationName + "</a> at " + new Date(dto.WrittenAtUtc).toLocaleString();
                            }
                        },
                        EmailAddress: {
                            text: function (value) {
                                return value;
                            },
                            href: function (value) {
                                return "mailto:" + value;
                            },
                            style: function (value) {
                                if (!value) {
                                    return "display:none";
                                }
                                return "color: #ee99ee";
                            }
                        }
                    }
                };
            }
            OverviewViewModel.prototype.getTitle = function () {
                return "All feedback";
            };
            OverviewViewModel.prototype.isEmpty = function () {
                return this.empty;
            };
            OverviewViewModel.prototype.activate = function (context) {
                var _this = this;
                var query = new OverviewFeedback();
                CqsClient.query(query)
                    .done(function (result) {
                    _this.empty = result.TotalCount === 0;
                    context.render(result, _this.feedbackDirectives);
                    context.resolve();
                });
            };
            OverviewViewModel.prototype.deactivate = function () {
            };
            return OverviewViewModel;
        }());
        Feedback.OverviewViewModel = OverviewViewModel;
    })(Feedback = OneTrueError.Feedback || (OneTrueError.Feedback = {}));
})(OneTrueError || (OneTrueError = {}));
//# sourceMappingURL=OverviewViewModel.js.map