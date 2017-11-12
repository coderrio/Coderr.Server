/// <reference path="../../app/Application.ts" />
/// <reference path="../../Scripts/Promise.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />
module codeRR.Incident {
    import CqsClient = Griffin.Cqs.CqsClient;
    import IncidentOrder = Core.Incidents.IncidentOrder;
    import Yo = Griffin.Yo;
    import Pager = Griffin.WebApp.Pager;
    import ApplicationService = codeRR.Applications.ApplicationService;

    export class IndexViewModel implements Griffin.Yo.Spa.ViewModels.IViewModel {
        private ctx: Griffin.Yo.Spa.ViewModels.IActivationContext;
        private tableModel: IncidentTableViewModel;

        constructor(dto: any) {
        }


        getTitle(): string {
            var appId = this.ctx.routeData['applicationId'];
            if (appId != null) {
                var app = new ApplicationService();
                app.get(appId)
                    .then(result => {
                        var bc: Applications.IBreadcrumb[] = [
                            { href: `/application/${appId}/`, title: result.Name },
                            { href: `/application/${appId}/incidents`, title: 'Incidents' }
                        ];
                        Applications.Navigation.breadcrumbs(bc);
                        Applications.Navigation.pageTitle = 'Incidents';
                    });
            } else {
                Applications.Navigation.breadcrumbs([{ href: `/incidents`, title: 'Incidents' }]);
                Applications.Navigation.pageTitle = 'Incidents for all applications';
            }
            

            return "Incidents";
        }

        activate(ctx: Griffin.Yo.Spa.ViewModels.IActivationContext) {
            this.ctx = ctx;
            this.tableModel = new IncidentTableViewModel(ctx);
            this.tableModel.load(this.ctx.routeData['applicationId'], null, function() {
                ctx.resolve();    
            });
            
        }

        deactivate() {

        }

        

    }
}