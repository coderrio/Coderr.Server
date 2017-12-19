/// <reference path="../../Scripts/Griffin.Yo.d.ts" />
/// <reference path="../../app/Application.ts" />
var codeRR;
(function (codeRR) {
    var Application;
    (function (Application) {
        var ApplicationVersions = codeRR.Modules.Versions.Queries.GetApplicationVersions;
        var CqsClient = Griffin.Cqs.CqsClient;
        var VersionsViewModel = /** @class */ (function () {
            function VersionsViewModel() {
            }
            VersionsViewModel.prototype.getTitle = function () { return "Application versions"; };
            VersionsViewModel.prototype.activate = function (context) {
                var _this = this;
                var query = new ApplicationVersions(context.routeData["applicationId"]);
                CqsClient.query(query)
                    .done(function (result) {
                    console.log(result.Items.length);
                    _this.haveItems = result.Items.length > 0;
                    console.log(_this.haveItems);
                    var directives = {
                        FirstReportReceivedAtUtc: {
                            text: function (value) {
                                return new Date(value).toLocaleString();
                            }
                        },
                        LastReportReceivedAtUtc: {
                            text: function (value) {
                                return new Date(value).toLocaleString();
                            }
                        }
                    };
                    var itemsElem = context.viewContainer.querySelector("#itemsTable");
                    context.render(result, { Items: directives });
                    //Yo.G.render(itemsElem, (<any>result).Items, directives);
                    context.resolve();
                });
            };
            VersionsViewModel.prototype.deactivate = function () { };
            return VersionsViewModel;
        }());
        Application.VersionsViewModel = VersionsViewModel;
    })(Application = codeRR.Application || (codeRR.Application = {}));
})(codeRR || (codeRR = {}));
//# sourceMappingURL=VersionsViewModel.js.map