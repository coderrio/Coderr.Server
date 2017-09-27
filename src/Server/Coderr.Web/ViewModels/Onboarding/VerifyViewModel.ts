/// <reference path="../../Scripts/Griffin.Yo.d.ts" />
/// <reference path="../../app/Application.ts" />
module codeRR.Onboarding {
    export class VerifyViewModel implements Griffin.Yo.Spa.ViewModels.IViewModel {
        getTitle(): string { return "Onboarding"; }

        activate(context: Griffin.Yo.Spa.ViewModels.IActivationContext): void {

            var appId = context.routeData["applicationId"];
            Applications.Navigation.breadcrumbs([
                { href: "#/onboarding", title: "Onboarding" },
                { href: `#/onboarding/application/${appId}/nuget/`, title: "Nuget" },
                { href: `#/onboarding/application/${appId}/verify/`, title: "Verify" }
            ]);
            Applications.Navigation.pageTitle = 'Onboarding - Verify configuration';


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