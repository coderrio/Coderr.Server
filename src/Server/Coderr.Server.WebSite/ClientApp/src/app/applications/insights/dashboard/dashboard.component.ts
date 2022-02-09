import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ApiClient } from "../../../utils/HttpClient";
import { GetApplicationVersions, GetApplicationVersionsResult } from "../../../../server-api/Core/Applications";
import { ChartService, IChartSeries, ILabelOptions } from "../../../services/chart.service";

interface IVersion {
  version: string;
  versionForRoutes: string;
  reportCount: number;
  reportSign?: "fa-arrow-up text-danger" | "fa-arrow-down text-success";
  reportPercentage?: number;
  incidentCount: number;
  incidentSign?: "fa-arrow-up text-danger" | "fa-arrow-down text-success";
  incidentPercentage?: number;
}

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class AppInsightsDashboardComponent implements OnInit, OnDestroy {
  private sub: any;
  applicationId: number = null;
  versions: IVersion[] = [];
  showNoData = true;
  chartId = "";

  constructor(
    private readonly apiClient: ApiClient,
    private chartService: ChartService,
    private activatedRoute: ActivatedRoute) {
    this.chartId = this.chartService.generateChartId();
  }

  ngOnInit(): void {
    this.sub = this.activatedRoute.params.subscribe(params => {
      this.applicationId = +params['applicationId'];
      this.loadVersions();
    });
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  private async loadVersions() {
    var q2 = new GetApplicationVersions();
    q2.applicationId = this.applicationId;
    var result = await this.apiClient.query<GetApplicationVersionsResult>(q2);
    if (result.items.length === 0) {
      return;
    }

    this.showNoData = false;
    this.versions = [];
    for (var i = result.items.length - 1; i >= 0; i--) {
      let dto = result.items[i];

      var v: IVersion = {
        version: dto.version,
        versionForRoutes: dto.version.replace(/\./g, "_"),
        reportCount: dto.reportCount,
        incidentCount: dto.incidentCount
      };

      if (i < result.items.length - 1) {
        var previousVersion = result.items[i + 1];

        v.incidentPercentage = this.calculatePercentage(previousVersion.incidentCount, dto.incidentCount);
        if (v.incidentPercentage < 0) {
          v.incidentSign = "fa-arrow-down text-success";
        } else if (v.incidentPercentage > 0) {
          if (v.incidentPercentage === Infinity) {
            v.incidentPercentage = 0;
          }
          v.incidentSign = "fa-arrow-up text-danger";
        }
        v.reportPercentage = this.calculatePercentage(previousVersion.reportCount, dto.reportCount);
        if (v.reportPercentage < 0) {
          v.reportSign = "fa-arrow-down text-success";
        } else if (v.reportPercentage > 0) {
          if (v.reportPercentage === Infinity) {
            v.reportPercentage = 0;
          }
          v.reportSign = "fa-arrow-up text-danger";
        }
      }

      this.versions.push(v);
    }

    var versions: string[] = [];
    var volumes: number[] = [];
    var series: IChartSeries[] = [];
    this.versions.forEach(version => {
      versions.push(version.version);
      volumes.push(version.incidentCount);
    });

    series.push({
      name: 'Errors',
      data: volumes
    });

    this.chartService.drawLineChart(this.chartId, { labels: versions }, series);
  }

  private calculatePercentage(before: number, after: number): number {
    //number of reports increased
    if (before < after) {
      let diff = after - before;
      return Math.round(diff / before * 100);
    } else {
      let diff = before - after;
      return Math.round(-(diff / before * 100));
    }
  }

}
