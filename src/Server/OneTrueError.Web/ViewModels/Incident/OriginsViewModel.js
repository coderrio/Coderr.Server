/// <reference path="../../Scripts/Models/AllModels.ts" />
/// <reference path="../../Scripts/Promise.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />
/// <reference path="../ChartViewModel.ts" />
var OneTrueError;
(function (OneTrueError) {
    var Incident;
    (function (Incident) {
        var CqsClient = Griffin.Cqs.CqsClient;
        var OriginsViewModel = (function () {
            function OriginsViewModel() {
            }
            OriginsViewModel.prototype.getTitle = function () {
                return "Error origins";
            };
            OriginsViewModel.prototype.activate = function (context) {
                this.context = context;
                context.resolve();
                var js = document.createElement("script");
                js.type = "text/javascript";
                js
                    .src =
                    "https://maps.googleapis.com/maps/api/js?key=AIzaSyBleXqcxCLRwuhcXk-3904HaJt9Vd1-CZc&libraries=visualization";
                this.context.viewContainer.appendChild(js);
                //var myLatLng = { lat: -25.363, lng: 131.044 };
                //var marker = new google.maps.Marker({
                //    position: myLatLng,
                //    map: map,
                //    title: 'Hello World!'
                //});
                js.onload = function (e) {
                    var mapDiv = context.select.one("#map");
                    var map = new google.maps.Map(mapDiv, {
                        zoom: 3,
                        center: { lat: 37.775, lng: -122.434 }
                    });
                    google.maps.event.addDomListener(window, "resize", function () {
                        var center = map.getCenter();
                        google.maps.event.trigger(map, "resize");
                        map.setCenter(center);
                    });
                    var query = new OneTrueError.Modules.ErrorOrigins.Queries.GetOriginsForIncident(context.routeData["incidentId"]);
                    CqsClient.query(query)
                        .done(function (response) {
                        var points = [];
                        response.Items.forEach(function (item) {
                            var point = new google.maps.LatLng(item.Latitude, item.Longitude);
                            for (var a = 0; a < item.NumberOfErrorReports; a++) {
                                points.push(point);
                            }
                        });
                        var heatmap = new google.maps.visualization.HeatmapLayer({
                            data: points,
                            map: map,
                            dissipating: false,
                            radius: 5
                        });
                    });
                };
            };
            OriginsViewModel.prototype.deactivate = function () { };
            return OriginsViewModel;
        }());
        Incident.OriginsViewModel = OriginsViewModel;
    })(Incident = OneTrueError.Incident || (OneTrueError.Incident = {}));
})(OneTrueError || (OneTrueError = {}));
//# sourceMappingURL=OriginsViewModel.js.map