/// <reference path="../../Scripts/Griffin.Yo.d.ts" />
/// <reference path="../../app/Application.ts" />
var OneTrueError;
(function (OneTrueError) {
    var Application;
    (function (Application) {
        var InstallationViewModel = (function () {
            function InstallationViewModel() {
            }
            InstallationViewModel.prototype.getTitle = function () { return "Installation instructions"; };
            InstallationViewModel.prototype.activate = function (context) {
                var service = new OneTrueError.Applications.ApplicationService();
                service.get(context.routeData["applicationId"])
                    .done(function (app) {
                    app.AppUrl = window["API_URL"];
                    context.render(app);
                    $("#appTitle").text(app.Name);
                    context.resolve();
                });
            };
            InstallationViewModel.prototype.deactivate = function () { };
            return InstallationViewModel;
        }());
        Application.InstallationViewModel = InstallationViewModel;
    })(Application = OneTrueError.Application || (OneTrueError.Application = {}));
})(OneTrueError || (OneTrueError = {}));
//# sourceMappingURL=InstallationViewModel.js.map