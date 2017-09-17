/// <reference path="../../app/Application.ts" />
var OneTrueError;
(function (OneTrueError) {
    var Onboarding;
    (function (Onboarding) {
        var GetApplicationList = OneTrueError.Core.Applications.Queries.GetApplicationList;
        var CreateApplication = OneTrueError.Core.Applications.Commands.CreateApplication;
        var CreateViewModel = (function () {
            function CreateViewModel() {
            }
            CreateViewModel.prototype.getTitle = function () { return "Onboarding"; };
            CreateViewModel.prototype.activate = function (context) {
                var _this = this;
                this.context = context;
                OneTrueError.Applications.Navigation.breadcrumbs([
                    { href: "#/onboarding", title: "Onboarding" }
                ]);
                OneTrueError.Applications.Navigation.pageTitle = "Onboarding - Create application";
                context.handle.click("#create-onboarding-application button", function (evt) { return _this.onSubmit(evt); });
                var apps = new GetApplicationList();
                CqsClient.query(apps)
                    .done(function (reply) {
                    if (reply.length > 1) {
                        window.location.hash = "#/onboarding/application/" + reply[0].Id + "/nuget";
                    }
                    else {
                        context.resolve();
                    }
                });
            };
            CreateViewModel.prototype.deactivate = function () { };
            CreateViewModel.prototype.onSubmit = function (mouseEvent) {
                mouseEvent.preventDefault();
                var frm = this.context.readForm("create-onboarding-application");
                var cmd = new CreateApplication(frm.ApplicationName, frm.TypeOfApplication);
                CqsClient.command(cmd)
                    .done(function (response) {
                    var apps = new GetApplicationList();
                    CqsClient.query(apps)
                        .done(function (reply) {
                        window["addApplication"](reply[0]);
                        window.location.hash = "#/onboarding/application/" + reply[0].Id + "/nuget";
                    });
                });
            };
            return CreateViewModel;
        }());
        Onboarding.CreateViewModel = CreateViewModel;
    })(Onboarding = OneTrueError.Onboarding || (OneTrueError.Onboarding = {}));
})(OneTrueError || (OneTrueError = {}));
//# sourceMappingURL=CreateViewModel.js.map