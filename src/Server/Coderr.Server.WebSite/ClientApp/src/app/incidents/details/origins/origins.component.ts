import { Component, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { ApiClient } from "../../../utils/HttpClient";
import { ActivatedRoute } from "@angular/router";
import { IncidentsService } from "../../incidents.service";
import { GetOriginsForIncident, GetOriginsForIncidentResult } from "../../../../server-api/Modules/ErrorOrigins";
declare var google: any;

@Component({
  selector: 'app-origins',
  templateUrl: './origins.component.html',
  styleUrls: ['./origins.component.scss']
})
export class OriginsComponent implements OnInit, OnDestroy {
  incidentId: number = 0;
  gotItems = true;
  private sub: any;
  @ViewChild('mapScript') mapScript: ElementRef;

  constructor(
    private readonly apiClient: ApiClient,
    private activeRoute: ActivatedRoute,
    private readonly incidentService: IncidentsService) {

  }

  ngOnInit(): void {
    this.sub = this.activeRoute.parent.params.subscribe(params => {
      this.incidentId = +params['incidentId'];
      
    });
  }

  ngAfterViewInit() {
    this.loadOrigins(this.incidentId);
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  private async loadOrigins(incidentId: number): Promise<any> {
    const js = document.createElement("script");
    js.type = "text/javascript";
    js.src =
      "https://maps.googleapis.com/maps/api/js?key=AIzaSyBleXqcxCLRwuhcXk-3904HaJt9Vd1-CZc&libraries=visualization";

    var el = this.mapScript.nativeElement;
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
      query.incidentId = incidentId;
      this.apiClient.query<GetOriginsForIncidentResult>(query)
        .then(response => {
          //disabled in commercial
          //this.gotItems = response.Items.length > 0;

          console.log('resp', response);
          if (response.items.length < 50) {
            response.items.forEach(item => {
              var point = new google.maps.LatLng(item.latitude, item.longitude);
              var marker = new google.maps.Marker({
                position: point,
                map: map,
                title: item.numberOfErrorReports + " error report(s)"
              });
            });
          } else {
            var points: any[] = [];
            response.items.forEach(item => {
              var point = new google.maps.LatLng(item.latitude, item.longitude);
              points.push({ location: point, weight: item.numberOfErrorReports });
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
