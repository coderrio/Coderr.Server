/// <reference path="../../Scripts/Griffin.Yo.d.ts"/> 
/// <reference path="../../app/Application.ts"/>

module OneTrueError.Application {
    import Yo = Griffin.Yo;

    export class InstallationViewModel implements Griffin.Yo.Spa.ViewModels.IViewModel {
        getTitle(): string { return "Installation instructions"; }

        activate(context: Griffin.Yo.Spa.ViewModels.IActivationContext): void {
            var service = new Applications.ApplicationService();
            service.get(context.routeData['applicationId'])
                .done(app => {
                    (<any>app).AppUrl = window["API_URL"];
                context.render(app);
                    $('#appTitle').text(app.Name);
                    context.resolve();
                });
        }

        deactivate() {}
    }
}