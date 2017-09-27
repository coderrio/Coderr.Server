/// <reference path="../../Scripts/Griffin.Yo.d.ts" />
/// <reference path="../../app/Application.ts" />
module codeRR.Onboarding {
    export class IndexViewModel implements Griffin.Yo.Spa.ViewModels.IViewModel {
        public hasApplicationId: boolean;

        getTitle(): string { return "Installation instructions"; }

        activate(context: Griffin.Yo.Spa.ViewModels.IActivationContext): void {
            this.hasApplicationId = context.routeData["applicationId"] != null;
            console.log(this.hasApplicationId, context.routeData["applicationId"]);
            Applications.Navigation.breadcrumbs([{ href: "#/onboarding", title: "Onboarding" }]);
            Applications.Navigation.pageTitle = 'Onboarding';
            context.resolve();

        }
        

        deactivate() { }
    }
}