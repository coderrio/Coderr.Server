/// <reference path="../../Scripts/Griffin.Yo.d.ts" />
/// <reference path="../../app/Application.ts" />
module OneTrueError.Application {
    export class VersionsViewModel implements Griffin.Yo.Spa.ViewModels.IViewModel {
        getTitle(): string { return "Application versions"; }

        activate(context: Griffin.Yo.Spa.ViewModels.IActivationContext): void {
            const service = new Applications.ApplicationService();
            service.get(context.routeData["applicationId"])
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