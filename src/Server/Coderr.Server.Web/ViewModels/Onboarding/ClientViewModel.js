/// <reference path="../../Scripts/Griffin.Yo.d.ts" />
/// <reference path="../../app/Application.ts" />
var codeRR;
(function (codeRR) {
    var Onboarding;
    (function (Onboarding) {
        var ClientViewModel = /** @class */ (function () {
            function ClientViewModel() {
            }
            ClientViewModel.prototype.getTitle = function () { return "Onboarding"; };
            ClientViewModel.prototype.activate = function (context) {
                this.context = context;
                var appId = context.routeData["applicationId"];
                codeRR.Applications.Navigation.breadcrumbs([
                    { href: "#/onboarding", title: "Onboarding" },
                    { href: "#/onboarding/application/" + appId + "/nuget/", title: "Nuget" }
                ]);
                codeRR.Applications.Navigation.pageTitle = 'Onboarding - Project configuration';
                var service = new codeRR.Applications.ApplicationService();
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
    })(Onboarding = codeRR.Onboarding || (codeRR.Onboarding = {}));
})(codeRR || (codeRR = {}));
//# sourceMappingURL=ClientViewModel.js.map