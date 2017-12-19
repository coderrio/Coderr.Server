/// <reference path="../../Scripts/Models/AllModels.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />
/// <reference path="../ChartViewModel.ts" />
var codeRR;
(function (codeRR) {
    var Incident;
    (function (Incident) {
        var CqsClient = Griffin.Cqs.CqsClient;
        var ApplicationService = codeRR.Applications.ApplicationService;
        var GetIncident = codeRR.Core.Incidents.Queries.GetIncident;
        var CloseIncident = codeRR.Core.Incidents.Commands.CloseIncident;
        var CloseViewModel = /** @class */ (function () {
            function CloseViewModel() {
            }
            CloseViewModel.prototype.getTitle = function () {
                return "Close incident";
            };
            CloseViewModel.prototype.activate = function (context) {
                var _this = this;
                this.context = context;
                IncidentNavigation.set(context.routeData, 'Close incident', 'close');
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
                    _this.solutionEditor = new SimpleMDE({ element: $("#solution")[0] });
                    _this.userTextEditor = new SimpleMDE({ element: $("#UserText")[0] });
                });
                context.handle.click("#saveSolution", function (evt) { return _this.onCloseIncident(); });
                context.handle.click('[name="sendCustomerMessage"]', function (evt) { return _this.onToggleMessagePane(); });
            };
            CloseViewModel.prototype.deactivate = function () { };
            CloseViewModel.prototype.onToggleMessagePane = function () {
                var panel = this.context.select.one("#status-panel");
                if (panel.style.display === "none") {
                    panel.style.display = "";
                }
                else {
                    panel.style.display = "none";
                }
            };
            CloseViewModel.prototype.onCloseIncident = function () {
                var _this = this;
                var solution = this.solutionEditor.value();
                var closeCmd = new CloseIncident(solution, this.incidentId);
                var sendMessage = this.context.select.one("sendCustomerMessage");
                if (sendMessage.checked) {
                    var subject = this.context.select.one('[name="UserSubject"]');
                    var notificationText = this.userTextEditor.value();
                    if (subject.value.length === 0 || notificationText.length === 0) {
                        alert("You specified that you wanted to send a notification to your users, but you have not specified subject or body of the message.");
                        return;
                    }
                    closeCmd.NotificationText = notificationText;
                    closeCmd.CanSendNotification = true;
                    closeCmd.NotificationTitle = subject.value;
                }
                CqsClient.command(closeCmd);
                //seems like everything is not updated otherwise.
                setTimeout(function () {
                    window.location.hash = "#/application/" + _this.context.routeData["applicationId"] + "/incident/" + _this.incidentId + "/";
                    humane.log("Incident has been closed.");
                }, 500);
            };
            return CloseViewModel;
        }());
        Incident.CloseViewModel = CloseViewModel;
    })(Incident = codeRR.Incident || (codeRR.Incident = {}));
})(codeRR || (codeRR = {}));
//# sourceMappingURL=CloseViewModel.js.map