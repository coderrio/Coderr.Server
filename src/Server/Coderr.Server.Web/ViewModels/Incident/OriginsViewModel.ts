/// <reference path="../../Scripts/Models/AllModels.ts" />
/// <reference path="../../Scripts/Promise.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />
/// <reference path="../ChartViewModel.ts" />
module codeRR.Incident {
    import CqsClient = Griffin.Cqs.CqsClient;
    import ActivationContext = Griffin.Yo.Spa.ViewModels.IActivationContext;
    declare var google: any;

    export class OriginsViewModel implements Griffin.Yo.Spa.ViewModels.IViewModel {
        private context: ActivationContext;

        getTitle(): string {
            return "Error origins";
        }

        activate(context: Griffin.Yo.Spa.ViewModels.IActivationContext): void {
            this.context = context;

            IncidentNavigation.set(context.routeData, 'Report origins', 'origins');

            context.resolve();

            //var myLatLng = { lat: -25.363, lng: 131.044 };
            //var marker = new google.maps.Marker({
            //    position: myLatLng,
            //    map: map,
            //    title: 'Hello World!'
            //});

            const js = document.createElement("script");
            js.type = "text/javascript";
            js
                .src =
                "https://maps.googleapis.com/maps/api/js?key=AIzaSyBleXqcxCLRwuhcXk-3904HaJt9Vd1-CZc&libraries=visualization";
            this.context.viewContainer.appendChild(js);

            js.onload = function(e) {
                const mapDiv = context.select.one("#map");
                var map = new google.maps.Map(mapDiv,
                {
                    zoom: 3,
                    center: { lat: 37.775, lng: -122.434 }
                });
                google.maps.event.addDomListener(window,
                    "resize",
                    function() {
                        const center = map.getCenter();
                        google.maps.event.trigger(map, "resize");
                        map.setCenter(center);
                    });


                const query = new Modules.ErrorOrigins.Queries.GetOriginsForIncident(context.routeData["incidentId"]);
                CqsClient.query<Modules.ErrorOrigins.Queries.GetOriginsForIncidentResult>(query)
                    .done(response => {

                        if (response.Items.length < 50) {
                            response.Items.forEach(item => {
                                var point = new google.maps.LatLng(item.Latitude, item.Longitude);
                                var marker = new google.maps.Marker({
                                    position: point,
                                    map: map,
                                    title: item.NumberOfErrorReports + " error report(s)"
                                });
                            });
                        } else {
                            var points: any[] = [];
                            response.Items.forEach(item => {
                                var point = new google.maps.LatLng(item.Latitude, item.Longitude);
                                points.push({ location: point, weight: item.NumberOfErrorReports });
                            });

                            var heatmap = new google.maps.visualization.HeatmapLayer({
                                data: points,
                                map: map,
                                dissipating: true,
                                radius: 15
                            });
                        }

                    });
            };
        }

        deactivate() {}
    }
}