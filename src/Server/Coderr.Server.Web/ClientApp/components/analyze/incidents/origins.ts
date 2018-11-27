import { PubSubService } from "../../../services/PubSub";
import { ApiClient } from '../../../services/ApiClient';
import { AppRoot } from '../../../services/AppRoot';
import { GetOriginsForIncident, GetOriginsForIncidentResult } from "../../../dto/Modules/ErrorOrigins";
import Vue from "vue";
import { Component, Watch } from "vue-property-decorator";

declare var google: any;

@Component
export default class OriginsComponent extends Vue {
    private apiClient: ApiClient = AppRoot.Instance.apiClient;

    mapScript: string = '';
    gotItems = false;

    mounted() {
        var self = this;
        var incidentId = parseInt(this.$route.params.incidentId, 10);

        const js = document.createElement("script");
        js.type = "text/javascript";
        js.src =
            "https://maps.googleapis.com/maps/api/js?key=AIzaSyBleXqcxCLRwuhcXk-3904HaJt9Vd1-CZc&libraries=visualization";

        var el = <HTMLElement>document.getElementById('originsScript');
        el.appendChild(js);

        js.onload = () => {
            const mapDiv = document.getElementById("map");
            var map = new google.maps.Map(mapDiv,
                {
                    zoom: 3,
                    center: { lat: 37.775, lng: -122.434 }
                });
            google.maps.event.addDomListener(window,
                "resize",
                () => {
                    const center = map.getCenter();
                    google.maps.event.trigger(map, "resize");
                    map.setCenter(center);
                });


            const query = new GetOriginsForIncident();
            query.IncidentId = incidentId;
            self.apiClient.query<GetOriginsForIncidentResult>(query)
                .then(response => {
                    this.gotItems = response.Items.length > 0;
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

}
