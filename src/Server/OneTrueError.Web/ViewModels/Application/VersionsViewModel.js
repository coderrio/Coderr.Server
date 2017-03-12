/// <reference path="../../Scripts/Griffin.Yo.d.ts" />
/// <reference path="../../app/Application.ts" />
var OneTrueError;
(function (OneTrueError) {
    var Application;
    (function (Application) {
        var VersionsViewModel = (function () {
            function VersionsViewModel() {
            }
            VersionsViewModel.prototype.getTitle = function () { return "Application versions"; };
            VersionsViewModel.prototype.activate = function (context) {
                var service = new OneTrueError.Applications.ApplicationService();
                service.get(context.routeData["applicationId"])
                    .done(function (app) {
                    app.AppUrl = window["API_URL"];
                    context.render(app);
                    $("#appTitle").text(app.Name);
                    context.resolve();
                });
            };
            VersionsViewModel.prototype.deactivate = function () { };
            return VersionsViewModel;
        }());
        Application.VersionsViewModel = VersionsViewModel;
    })(Application = OneTrueError.Application || (OneTrueError.Application = {}));
})(OneTrueError || (OneTrueError = {}));
//# sourceMappingURL=VersionsViewModel.js.map