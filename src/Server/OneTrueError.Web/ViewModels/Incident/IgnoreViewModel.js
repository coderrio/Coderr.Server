/// <reference path="../../Scripts/Models/AllModels.ts" />
/// <reference path="../../Scripts/Promise.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />
/// <reference path="../ChartViewModel.ts" />
var OneTrueError;
(function (OneTrueError) {
    var Incident;
    (function (Incident) {
        var CqsClient = Griffin.Cqs.CqsClient;
        var ApplicationService = OneTrueError.Applications.ApplicationService;
        var GetIncident = OneTrueError.Core.Incidents.Queries.GetIncident;
        var IgnoreIncident = OneTrueError.Core.Incidents.Commands.IgnoreIncident;
        var IgnoreViewModel = (function () {
            function IgnoreViewModel() {
            }
            IgnoreViewModel.prototype.getTitle = function () {
                return "Ignore incident";
            };
            IgnoreViewModel.prototype.activate = function (context) {
                var _this = this;
                this.context = context;
                this.incidentId = parseInt(context.routeData["incidentId"]);
                var query = new GetIncident(parseInt(context.routeData["incidentId"], 10));
                var incidentPromise = CqsClient.query(query);
                incidentPromise.done(function (result) { return context.render(result); });
                var service = new ApplicationService();
                var appPromise = service.get(context.routeData["applicationId"]);
                appPromise.done(function (result) {
                    _this.app = result;
                });
                P.when(incidentPromise, appPromise)
                    .then(function (result) {
                    context.resolve();
                });
                context.handle.click("#ignoreIncident", function (evt) { return _this.onIgnoreIncident(); });
            };
            IgnoreViewModel.prototype.deactivate = function () { };
            IgnoreViewModel.prototype.onIgnoreIncident = function () {
                var ignoreCmd = new IgnoreIncident(this.incidentId);
                CqsClient.command(ignoreCmd);
                humane.log("Incident have been marked as ignored.");
                window.location.hash = "#/application/" + this.context.routeData["applicationId"];
            };
            return IgnoreViewModel;
        }());
        Incident.IgnoreViewModel = IgnoreViewModel;
    })(Incident = OneTrueError.Incident || (OneTrueError.Incident = {}));
})(OneTrueError || (OneTrueError = {}));
//# sourceMappingURL=IgnoreViewModel.js.map