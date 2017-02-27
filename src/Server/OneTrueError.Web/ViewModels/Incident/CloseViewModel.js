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
        var CloseIncident = OneTrueError.Core.Incidents.Commands.CloseIncident;
        var CloseViewModel = (function () {
            function CloseViewModel() {
            }
            CloseViewModel.prototype.getTitle = function () {
                return "Close incident";
            };
            CloseViewModel.prototype.activate = function (context) {
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
                var solution = this.context.select.one('[name="solution"]');
                var closeCmd = new CloseIncident(solution.value, this.incidentId);
                var sendMessage = this.context.select.one("sendCustomerMessage");
                console.log(sendMessage);
                if (sendMessage.checked) {
                    var subject = this.context.select.one('[name="UserSubject"]');
                    var text = this.context.select.one('[name="UserText"]');
                    if (subject.value.length === 0 || text.value.length === 0) {
                        alert("You specified that you wanted to send a notification to your users, but you have not specified subject or body of the message.");
                        return;
                    }
                    closeCmd.NotificationText = text.value;
                    closeCmd.CanSendNotification = true;
                    closeCmd.NotificationTitle = subject.value;
                }
                CqsClient.command(closeCmd);
                window.location.hash = "#/application/" + this.context.routeData["applicationId"];
                humane.log("Incident has been closed.");
            };
            return CloseViewModel;
        }());
        Incident.CloseViewModel = CloseViewModel;
    })(Incident = OneTrueError.Incident || (OneTrueError.Incident = {}));
})(OneTrueError || (OneTrueError = {}));
//# sourceMappingURL=CloseViewModel.js.map