class IncidentNavigation {
    static set(routeData: any, subTitle?: string, subAction?: string) {

        var data = {
            incident: codeRR.Core.Incidents.Queries.GetIncidentResult = null,
            app: codeRR.Core.Applications.Queries.GetApplicationInfoResult = null,
            subTitle: subTitle,
            subAction: subAction
        };

        var promises: P.Promise<any>[] = [];
        var appId = routeData['applicationId'];
        var incidentId = routeData['incidentId'];

        if (window['currentIncident'] == null) {
            data.incident = <codeRR.Core.Incidents.Queries.GetIncidentResult>window['currentIncident'];
            if (data.incident == null || data.incident.Id !== incidentId) {
                var query = new codeRR.Core.Incidents.Queries.GetIncident(incidentId);
                var p = CqsClient.query<codeRR.Core.Incidents.Queries.GetIncidentResult>(query);
                promises.push(p);
                p.done(result => {
                    window['currentIncident'] = result;
                    data.incident = result;
                });
            }
        } else {
            data.incident = window['currentIncident'];
        }

        var app = new codeRR.Applications.ApplicationService();
        var p2 = app.get(appId)
            .then(result => {
                data.app = result;
            });
        promises.push(p2);

        P.when(...promises).done(x => this.setData(data));
    }

    private static setData(data:any) {
        var bc: codeRR.Applications.IBreadcrumb[] = [
            { href: `/application/${data.app.Id}/`, title: data.app.Name },
            { href: `/application/${data.app.Id}/incident/${data.incident.Id}/`, title: 'Incident' }
        ];

        if (data.subTitle != null) {
            var pos = data.subTitle.indexOf('<');
            var cleanTitle = pos === -1 ? data.subTitle : data.subTitle.substr(0,pos);
            bc.push({
                href: `/application/${data.app.Id}/incident/${data.incident.Id}/${data.subAction}`,
                title: cleanTitle
            });
            codeRR.Applications.Navigation.pageTitle = data.subTitle;
        } else {
            codeRR.Applications.Navigation.pageTitle = data.incident.Description;
        }

        codeRR.Applications.Navigation.breadcrumbs(bc);
    }

}