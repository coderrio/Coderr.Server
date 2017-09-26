/// <reference path="../../Scripts/Griffin.Yo.d.ts" />
/// <reference path="../../app/Application.ts" />
var OneTrueError;
(function (OneTrueError) {
    var Onboarding;
    (function (Onboarding) {
        var ClientViewModel = (function () {
            function ClientViewModel() {
            }
            ClientViewModel.prototype.getTitle = function () { return "Onboarding"; };
            ClientViewModel.prototype.activate = function (context) {
                this.context = context;
                var appId = context.routeData["applicationId"];
                OneTrueError.Applications.Navigation.breadcrumbs([
                    { href: "#/onboarding", title: "Onboarding" },
                    { href: "#/onboarding/application/" + appId + "/nuget/", title: "Nuget" }
                ]);
                OneTrueError.Applications.Navigation.pageTitle = 'Onboarding - Project configuration';
                var service = new OneTrueError.Applications.ApplicationService();
                service.get(appId)
                    .done(function (app) {
                    app.AppUrl = window["API_URL"];
                    context.render(app);
                    $("#appTitle").text(app.Name);
                    context.resolve();
                });
            };
            ClientViewModel.prototype.deactivate = function () { };
            return ClientViewModel;
        }());
        Onboarding.ClientViewModel = ClientViewModel;
    })(Onboarding = OneTrueError.Onboarding || (OneTrueError.Onboarding = {}));
})(OneTrueError || (OneTrueError = {}));
//# sourceMappingURL=ClientViewModel.js.map