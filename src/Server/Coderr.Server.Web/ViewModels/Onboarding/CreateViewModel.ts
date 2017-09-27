/// <reference path="../../app/Application.ts" />
module codeRR.Onboarding {
    import GetApplicationList = Core.Applications.Queries.GetApplicationList;
    import ApplicationListItem = Core.Applications.ApplicationListItem;
    import CreateApplication = Core.Applications.Commands.CreateApplication;

    export class CreateViewModel implements Griffin.Yo.Spa.ViewModels.IViewModel {
        private context: Griffin.Yo.Spa.ViewModels.IActivationContext;

        getTitle(): string { return "Onboarding"; }

        activate(context: Griffin.Yo.Spa.ViewModels.IActivationContext): void {
            this.context = context;


            Applications.Navigation.breadcrumbs([
                { href: "#/onboarding", title: "Onboarding" }
            ]);
            Applications.Navigation.pageTitle = "Onboarding - Create application";

            context.handle.click("#create-onboarding-application button", evt => this.onSubmit(evt));

            const apps = new GetApplicationList();
            CqsClient.query<ApplicationListItem[]>(apps)
                .done(reply => {
                    if (reply.length > 1) {
                        window.location.hash = `#/onboarding/application/${reply[0].Id}/nuget`;
                    } else {
                        context.resolve();
                    }

                });
        }

        deactivate() {}


        onSubmit(mouseEvent: MouseEvent) {
            mouseEvent.preventDefault();
            const frm = this.context.readForm("create-onboarding-application");
            const cmd = new CreateApplication(frm.ApplicationName, frm.TypeOfApplication);
            CqsClient.command(cmd)
                .done(response => {
                    var apps = new GetApplicationList();
                    CqsClient.query<ApplicationListItem[]>(apps)
                        .done(reply => {
                            window["addApplication"](reply[0]);
                            window.location.hash = `#/onboarding/application/${reply[0].Id}/nuget`;
                        });
                });

        }
    }
}