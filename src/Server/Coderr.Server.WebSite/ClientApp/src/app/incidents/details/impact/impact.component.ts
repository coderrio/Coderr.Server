import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from "@angular/router";
import * as dto from "../../../../server-api/Common/Partitions"
import { IncidentsService } from "../../incidents.service";
import { ApiClient } from "../../../utils/HttpClient";

@Component({
  selector: 'app-impact',
  templateUrl: './impact.component.html',
  styleUrls: ['./impact.component.scss']
})
export class ImpactComponent implements OnInit, OnDestroy {
  solution = "";
  partitions: dto.GetPartitionsResultItem[] = [];
  values: dto.GetPartitionValuesResultItem[] = [];
  incidentId: number = 0;
  private sub: any;

  constructor(
    private readonly apiClient: ApiClient,
    private activeRoute: ActivatedRoute,
    private readonly incidentService: IncidentsService) {

  }

  ngOnInit(): void {
    this.sub = this.activeRoute.parent.params.subscribe(params => {
      this.incidentId = +params['incidentId'];
      this.incidentService.get(this.incidentId)
        .then(incident => {
          var query = new dto.GetPartitions();
          query.applicationId = incident.applicationId;
          this.apiClient.query<dto.GetPartitionsResult>(query)
            .then(x => {
              this.partitions = x.items;
              console.log('partitions', x);
            });
        });
    });
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }


  selectItem(item: dto.GetPartitionsResultItem) {
    var query = new dto.GetPartitionValues();
    query.incidentId = this.incidentId;
    query.partitionId = item.id;
    this.apiClient.query<dto.GetPartitionValuesResult>(query)
      .then(result => {
        this.values = result.items;
      });

  }
}
