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
        var OverviewViewModel = (function () {
            function OverviewViewModel() {
            }
            OverviewViewModel.prototype.getTitle = function () {
                OneTrueError.Applications.Navigation.breadcrumbs([{ href: "/feedback", title: 'Feedback' }]);
                OneTrueError.Applications.Navigation.pageTitle = 'All feedback for all applications';
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
            return OverviewViewModel;
        }());
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
                        return "Reported for <a href=\"#/application/" + dto.ApplicationId + "/feedback\">" + dto.ApplicationName + "</a> " + moment(dto.WrittenAtUtc).fromNow();
                    }
                },
                EmailAddress: {
                    text: function (value) {
                        return value;
                    },
                    href: function (value) {
                        return "mailto:" + value;
                    }
                }
            }
        };
        Feedback.OverviewViewModel = OverviewViewModel;
    })(Feedback = OneTrueError.Feedback || (OneTrueError.Feedback = {}));
})(OneTrueError || (OneTrueError = {}));
//# sourceMappingURL=OverviewViewModel.js.map