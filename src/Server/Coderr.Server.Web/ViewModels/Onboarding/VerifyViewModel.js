/// <reference path="../../Scripts/Griffin.Yo.d.ts" />
/// <reference path="../../app/Application.ts" />
var codeRR;
(function (codeRR) {
    var Onboarding;
    (function (Onboarding) {
        var VerifyViewModel = /** @class */ (function () {
            function VerifyViewModel() {
            }
            VerifyViewModel.prototype.getTitle = function () { return "Onboarding"; };
            VerifyViewModel.prototype.activate = function (context) {
                var appId = context.routeData["applicationId"];
                codeRR.Applications.Navigation.breadcrumbs([
                    { href: "#/onboarding", title: "Onboarding" },
                    { href: "#/onboarding/application/" + appId + "/nuget/", title: "Nuget" },
                    { href: "#/onboarding/application/" + appId + "/verify/", title: "Verify" }
                ]);
                codeRR.Applications.Navigation.pageTitle = 'Onboarding - Verify configuration';
                var service = new codeRR.Applications.ApplicationService();
                service.get(appId)
                    .done(function (app) {
                    app.AppUrl = window["API_URL"];
                    context.render(app);
                    $("#appTitle").text(app.Name);
                    context.resolve();
                });
            };
            VerifyViewModel.prototype.deactivate = function () { };
            return VerifyViewModel;
        }());
        Onboarding.VerifyViewModel = VerifyViewModel;
    })(Onboarding = codeRR.Onboarding || (codeRR.Onboarding = {}));
})(codeRR || (codeRR = {}));
//# sourceMappingURL=VerifyViewModel.js.map