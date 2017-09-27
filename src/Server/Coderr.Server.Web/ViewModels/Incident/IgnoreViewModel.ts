/// <reference path="../../Scripts/Models/AllModels.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />
/// <reference path="../ChartViewModel.ts" />
module codeRR.Incident {
    import CqsClient = Griffin.Cqs.CqsClient;
    import ApplicationService = Applications.ApplicationService;
    import ApplicationInfoResult = Core.Applications.Queries.GetApplicationInfoResult;
    import GetIncident = Core.Incidents.Queries.GetIncident;
    import IncidentResult = Core.Incidents.Queries.GetIncidentResult;
    import ActivationContext = Griffin.Yo.Spa.ViewModels.IActivationContext;
    import IgnoreIncident = Core.Incidents.Commands.IgnoreIncident;

    export class IgnoreViewModel implements Griffin.Yo.Spa.ViewModels.IViewModel {
        private app: ApplicationInfoResult;
        private context: ActivationContext;

        getTitle(): string {
            return "Ignore incident";
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

            context.handle.click("#ignoreIncident", evt => this.onIgnoreIncident());
        }

        deactivate() {}


        onIgnoreIncident() {
            const ignoreCmd = new IgnoreIncident(this.incidentId);
            CqsClient.command(ignoreCmd);
            humane.log("Incident have been marked as ignored.");
            window.location.hash = `#/application/${this.context.routeData["applicationId"]}/`;
        }

        incidentId: number;
    }

}