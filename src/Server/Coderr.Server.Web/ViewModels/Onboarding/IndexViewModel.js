/// <reference path="../../Scripts/Griffin.Yo.d.ts" />
/// <reference path="../../app/Application.ts" />
var codeRR;
(function (codeRR) {
    var Onboarding;
    (function (Onboarding) {
        var IndexViewModel = /** @class */ (function () {
            function IndexViewModel() {
            }
            IndexViewModel.prototype.getTitle = function () { return "Installation instructions"; };
            IndexViewModel.prototype.activate = function (context) {
                this.hasApplicationId = context.routeData["applicationId"] != null;
                console.log(this.hasApplicationId, context.routeData["applicationId"]);
                codeRR.Applications.Navigation.breadcrumbs([{ href: "#/onboarding", title: "Onboarding" }]);
                codeRR.Applications.Navigation.pageTitle = 'Onboarding';
                context.resolve();
            };
            IndexViewModel.prototype.deactivate = function () { };
            return IndexViewModel;
        }());
        Onboarding.IndexViewModel = IndexViewModel;
    })(Onboarding = codeRR.Onboarding || (codeRR.Onboarding = {}));
})(codeRR || (codeRR = {}));
//# sourceMappingURL=IndexViewModel.js.map