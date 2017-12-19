/// <reference path="../../app/Application.ts" />
/// <reference path="../../Scripts/Promise.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />
var codeRR;
(function (codeRR) {
    var Incident;
    (function (Incident) {
        var ApplicationService = codeRR.Applications.ApplicationService;
        var IndexViewModel = /** @class */ (function () {
            function IndexViewModel(dto) {
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
                        codeRR.Applications.Navigation.breadcrumbs(bc);
                        codeRR.Applications.Navigation.pageTitle = 'Incidents';
                    });
                }
                else {
                    codeRR.Applications.Navigation.breadcrumbs([{ href: "/incidents", title: 'Incidents' }]);
                    codeRR.Applications.Navigation.pageTitle = 'Incidents for all applications';
                }
                return "Incidents";
            };
            IndexViewModel.prototype.activate = function (ctx) {
                this.ctx = ctx;
                this.tableModel = new IncidentTableViewModel(ctx);
                this.tableModel.load(this.ctx.routeData['applicationId'], null, function () {
                    ctx.resolve();
                });
            };
            IndexViewModel.prototype.deactivate = function () {
            };
            return IndexViewModel;
        }());
        Incident.IndexViewModel = IndexViewModel;
    })(Incident = codeRR.Incident || (codeRR.Incident = {}));
})(codeRR || (codeRR = {}));
//# sourceMappingURL=IndexViewModel.js.map