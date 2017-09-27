/// <reference path="../../Scripts/Griffin.Yo.d.ts" />
/// <reference path="../../app/Application.ts" />
module codeRR.Onboarding {
    import CreateApplication = codeRR.Core.Applications.Commands.CreateApplication;

    export class ClientViewModel implements Griffin.Yo.Spa.ViewModels.IViewModel {
        private context: Griffin.Yo.Spa.ViewModels.IActivationContext;

        getTitle(): string { return "Onboarding"; }

        activate(context: Griffin.Yo.Spa.ViewModels.IActivationContext): void {
            this.context = context;


            var appId = context.routeData["applicationId"];
            Applications.Navigation.breadcrumbs([
                { href: "#/onboarding", title: "Onboarding" },
                { href: `#/onboarding/application/${appId }/nuget/`, title: "Nuget" }
            ]);
            Applications.Navigation.pageTitle = 'Onboarding - Project configuration';

            const service = new Applications.ApplicationService();
            service.get(appId)
                .done(app => {

                    (app as any).AppUrl = window["API_URL"];
                    context.render(app);
                    $("#appTitle").text(app.Name);
                    context.resolve();
                });
        }

        deactivate() {}

    }
}