var IncidentNavigation = /** @class */ (function () {
    function IncidentNavigation() {
    }
    IncidentNavigation.set = function (routeData, subTitle, subAction) {
        var _this = this;
        var data = {
            incident: codeRR.Core.Incidents.Queries.GetIncidentResult = null,
            app: codeRR.Core.Applications.Queries.GetApplicationInfoResult = null,
            subTitle: subTitle,
            subAction: subAction
        };
        var promises = [];
        var appId = routeData['applicationId'];
        var incidentId = routeData['incidentId'];
        if (window['currentIncident'] == null) {
            data.incident = window['currentIncident'];
            if (data.incident == null || data.incident.Id !== incidentId) {
                var query = new codeRR.Core.Incidents.Queries.GetIncident(incidentId);
                var p = CqsClient.query(query);
                promises.push(p);
                p.done(function (result) {
                    window['currentIncident'] = result;
                    data.incident = result;
                });
            }
        }
        else {
            data.incident = window['currentIncident'];
        }
        var app = new codeRR.Applications.ApplicationService();
        var p2 = app.get(appId)
            .then(function (result) {
            data.app = result;
        });
        promises.push(p2);
        P.when.apply(P, promises).done(function (x) { return _this.setData(data); });
    };
    IncidentNavigation.setData = function (data) {
        var bc = [
            { href: "/application/" + data.app.Id + "/", title: data.app.Name },
            { href: "/application/" + data.app.Id + "/incident/" + data.incident.Id + "/", title: 'Incident' }
        ];
        if (data.subTitle != null) {
            var pos = data.subTitle.indexOf('<');
            var cleanTitle = pos === -1 ? data.subTitle : data.subTitle.substr(0, pos);
            bc.push({
                href: "/application/" + data.app.Id + "/incident/" + data.incident.Id + "/" + data.subAction,
                title: cleanTitle
            });
            codeRR.Applications.Navigation.pageTitle = data.subTitle;
        }
        else {
            codeRR.Applications.Navigation.pageTitle = data.incident.Description;
        }
        codeRR.Applications.Navigation.breadcrumbs(bc);
    };
    return IncidentNavigation;
}());
//# sourceMappingURL=IncidentNavigation.js.map