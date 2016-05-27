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
            var query = new GetIncident(parseInt(context.routeData["incidentId"], 10));
            var incidentPromise = CqsClient.query<IncidentResult>(query);
            incidentPromise.done(result => context.render(result));


            var service = new ApplicationService();
            var appPromise = service.get(context.routeData["applicationId"]);
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
            var panel = this.context.select.one("#status-panel");
            if (panel.style.display === "none") {
                panel.style.display = "";
            } else {
                panel.style.display = "none";
            }

        }

        onCloseIncident() {
            var solution = <HTMLInputElement>this.context.select.one('[name="solution"]');

            var closeCmd = new CloseIncident(solution.value, this.incidentId);

            var sendMessage = <HTMLInputElement>this.context.select.one('[name="sendCustomerMessage"]');
            if (sendMessage.checked) {
                var subject = (<HTMLInputElement>this.context.select.one('[name="UserSubject"]'));
                var text = (<HTMLInputElement>this.context.select.one('[name="UserText"]'));
                if (subject.value.length === 0 || text.value.length === 0) {
                    alert("You specified that you wanted to send a notification to your users, but you have not specified subject or body of the message.");
                    return;
                }
            }

            CqsClient.command(closeCmd);
            window.location.hash = `#/application/${this.context.routeData["applicationId"]}`;
            humane.log("Incident have been closed.");
        }

        incidentId: number;
    }

}