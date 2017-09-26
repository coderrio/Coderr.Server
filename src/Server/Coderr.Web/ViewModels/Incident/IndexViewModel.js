/// <reference path="../../app/Application.ts" />
/// <reference path="../../Scripts/Promise.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />
var OneTrueError;
(function (OneTrueError) {
    var Incident;
    (function (Incident) {
        var ApplicationService = OneTrueError.Applications.ApplicationService;
        var IndexViewModel = (function () {
            function IndexViewModel(dto) {
                console.log(dto);
            }
            IndexViewModel.prototype.getTitle = function () {
                var appId = this.ctx.routeData['applicationId'];
                if (appId != null) {
                    var app = new ApplicationService();
                    app.get(appId)
                        .then(function (result) {
                        var bc = [
                            { href: "/application/" + appId + "/", title: result.Name },
                            { href: "/application/" + appId + "/incidents", title: 'Incidents' }
                        ];
                        OneTrueError.Applications.Navigation.breadcrumbs(bc);
                        OneTrueError.Applications.Navigation.pageTitle = 'Incidents';
                    });
                }
                else {
                    OneTrueError.Applications.Navigation.breadcrumbs([{ href: "/incidents", title: 'Incidents' }]);
                    OneTrueError.Applications.Navigation.pageTitle = 'Incidents for all applications';
                }
                return "Incidents";
            };
            IndexViewModel.prototype.activate = function (ctx) {
                this.ctx = ctx;
                this.tableModel = new IncidentTableViewModel(ctx);
                this.tableModel.load(this.ctx.routeData['applicationId']);
            };
            IndexViewModel.prototype.deactivate = function () {
            };
            return IndexViewModel;
        }());
        Incident.IndexViewModel = IndexViewModel;
    })(Incident = OneTrueError.Incident || (OneTrueError.Incident = {}));
})(OneTrueError || (OneTrueError = {}));
//# sourceMappingURL=IndexViewModel.js.map