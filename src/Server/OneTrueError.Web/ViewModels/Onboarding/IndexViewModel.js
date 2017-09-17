/// <reference path="../../Scripts/Griffin.Yo.d.ts" />
/// <reference path="../../app/Application.ts" />
var OneTrueError;
(function (OneTrueError) {
    var Onboarding;
    (function (Onboarding) {
        var IndexViewModel = (function () {
            function IndexViewModel() {
            }
            IndexViewModel.prototype.getTitle = function () { return "Installation instructions"; };
            IndexViewModel.prototype.activate = function (context) {
                this.hasApplicationId = context.routeData["applicationId"] != null;
                console.log(this.hasApplicationId, context.routeData["applicationId"]);
                OneTrueError.Applications.Navigation.breadcrumbs([{ href: "#/onboarding", title: "Onboarding" }]);
                OneTrueError.Applications.Navigation.pageTitle = 'Onboarding';
                context.resolve();
            };
            IndexViewModel.prototype.deactivate = function () { };
            return IndexViewModel;
        }());
        Onboarding.IndexViewModel = IndexViewModel;
    })(Onboarding = OneTrueError.Onboarding || (OneTrueError.Onboarding = {}));
})(OneTrueError || (OneTrueError = {}));
//# sourceMappingURL=IndexViewModel.js.map