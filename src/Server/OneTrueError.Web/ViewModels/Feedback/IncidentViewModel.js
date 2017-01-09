/// <reference path="../../Scripts/Promise.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />
/// <reference path="../../Scripts/Griffin.Yo.d.ts" />
/// <reference path="../ChartViewModel.ts" />
var OneTrueError;
(function (OneTrueError) {
    var Feedback;
    (function (Feedback) {
        var CqsClient = Griffin.Cqs.CqsClient;
        var GetIncidentFeedback = OneTrueError.Web.Feedback.Queries.GetIncidentFeedback;
        var IncidentViewModel = (function () {
            function IncidentViewModel() {
            }
            IncidentViewModel.prototype.activate = function (ctx) {
                var _this = this;
                this.ctx = ctx;
                var query = new GetIncidentFeedback(ctx.routeData["incidentId"]);
                CqsClient.query(query)
                    .done(function (result) {
                    _this.ctx.render(result, IncidentViewModel.directives);
                    ctx.resolve();
                });
            };
            IncidentViewModel.prototype.deactivate = function () {
            };
            IncidentViewModel.prototype.getTitle = function () {
                return "Incident";
            };
            IncidentViewModel.directives = {
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
                            return "Written at " + new Date(dto.WrittenAtUtc).toLocaleString();
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
            return IncidentViewModel;
        }());
        Feedback.IncidentViewModel = IncidentViewModel;
    })(Feedback = OneTrueError.Feedback || (OneTrueError.Feedback = {}));
})(OneTrueError || (OneTrueError = {}));
//# sourceMappingURL=IncidentViewModel.js.map