/// <reference path="../../Scripts/Models/AllModels.ts" />
/// <reference path="../../Scripts/Promise.ts" />
/// <reference path="../../Scripts/Griffin.WebApp.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />
/// <reference path="../ChartViewModel.ts" />
/// <reference path="../../Scripts/Griffin.Yo.d.ts" />
var codeRR;
(function (codeRR) {
    var Feedback;
    (function (Feedback) {
        var CqsClient = Griffin.Cqs.CqsClient;
        var OverviewFeedback = codeRR.Web.Feedback.Queries.GetFeedbackForDashboardPage;
        var OverviewViewModel = /** @class */ (function () {
            function OverviewViewModel() {
            }
            OverviewViewModel.prototype.getTitle = function () {
                codeRR.Applications.Navigation.breadcrumbs([{ href: "/feedback", title: 'Feedback' }]);
                codeRR.Applications.Navigation.pageTitle = 'All feedback for all applications';
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
                    context.render(result, OverviewViewModel.renderDirectives);
                    context.resolve();
                });
            };
            OverviewViewModel.prototype.deactivate = function () {
            };
            OverviewViewModel.renderDirectives = {
                Items: {
                    Message: {
                        html: function (value) {
                            return nl2br(value);
                        }
                    },
                    Title: {
                        style: function () {
                            return '';
                        },
                        html: function (value, dto) {
                            console.log(dto);
                            return "Reported for <a href=\"#/application/" + dto.ApplicationId + "/feedback\">" + dto.ApplicationName + "</a> " + momentsAgo(dto.WrittenAtUtc);
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
            return OverviewViewModel;
        }());
        Feedback.OverviewViewModel = OverviewViewModel;
    })(Feedback = codeRR.Feedback || (codeRR.Feedback = {}));
})(codeRR || (codeRR = {}));
//# sourceMappingURL=OverviewViewModel.js.map