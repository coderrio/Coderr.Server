/// <reference path="../../Scripts/Models/AllModels.ts" />
/// <reference path="../../Scripts/Promise.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />
/// <reference path="../ChartViewModel.ts" />
module OneTrueError.Incident {
    import CqsClient = Griffin.Cqs.CqsClient;
    import ApplicationService = Applications.ApplicationService;
    import ApplicationInfoResult = Core.Applications.Queries.GetApplicationInfoResult;
    import GetIncident = Core.Incidents.Queries.GetIncident;
    import IncidentResult = Core.Incidents.Queries.GetIncidentResult;
    import ActivationContext = Griffin.Yo.Spa.ViewModels.IActivationContext;
    import CloseIncident = Core.Incidents.Commands.CloseIncident;

    export class CloseViewModel implements Griffin.Yo.Spa.ViewModels.IViewModel {
        private app: ApplicationInfoResult;
        private context: ActivationContext;

        getTitle(): string {
            return "Close incident";
        }

        activate(context: Griffin.Yo.Spa.ViewModels.IActivationContext): void {
            this.context = context;

            this.incidentId = parseInt(context.routeData["incidentId"]);
            const query = new GetIncident(parseInt(context.routeData["incidentId"], 10));
            const incidentPromise = CqsClient.query<IncidentResult>(query);
            incidentPromise.done(result => context.render(result));


            const service = new ApplicationService();
            const appPromise = service.get(context.routeData["applicationId"]);
            appPromise.done(result => {
                this.app = result;
            });

            P.when(incidentPromise, appPromise)
                .then(result => {
                    context.resolve();
                });

            context.handle.click("#saveSolution", evt => this.onCloseIncident());
            context.handle.click('[name="sendCustomerMessage"]', evt => this.onToggleMessagePane());
        }

        deactivate() {}

        onToggleMessagePane() {
            const panel = this.context.select.one("#status-panel");
            if (panel.style.display === "none") {
                panel.style.display = "";
            } else {
                panel.style.display = "none";
            }

        }

        onCloseIncident() {
            const solution = this.context.select.one('[name="solution"]') as HTMLInputElement;

            const closeCmd = new CloseIncident(solution.value, this.incidentId);

            const sendMessage = this.context.select.one("sendCustomerMessage") as HTMLInputElement;
            console.log(sendMessage);
            if (sendMessage.checked) {
                const subject = (this.context.select.one('[name="UserSubject"]') as HTMLInputElement);
                const text = (this.context.select.one('[name="UserText"]') as HTMLInputElement);
                if (subject.value.length === 0 || text.value.length === 0) {
                    alert("You specified that you wanted to send a notification to your users, but you have not specified subject or body of the message.");
                    return;
                }
                closeCmd.NotificationText = text.value;
                closeCmd.CanSendNotification = true;
                closeCmd.NotificationTitle = subject.value;
            }

            CqsClient.command(closeCmd);
            window.location.hash = `#/application/${this.context.routeData["applicationId"]}`;
            humane.log("Incident has been closed.");
        }

        incidentId: number;
    }

}
