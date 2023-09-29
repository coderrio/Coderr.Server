import { Component, OnInit, Input } from '@angular/core';
import * as api from "../../../../server-api/Common/Partitions";
import { ApiClient } from "../../../utils/HttpClient";
import { ChartService, IChartSeries } from "../../../services/chart.service";

@Component({
  selector: 'incident-insight-chart',
  templateUrl: './insightchart.component.html',
  styleUrls: ['./insightchart.component.scss']
})
export class InsightChartComponent implements OnInit {
  private _applicationId: number;
  private _incidentId: number;
  chartId = '';
  showGotNone = false;

  constructor(
    private apiClient: ApiClient,
    private chartService: ChartService
  ) {
    this.chartId = this.chartService.generateChartId();
  }

  @Input()
  get applicationId(): number { return this._applicationId; }
  set applicationId(applicationId: number) {
    this._applicationId = applicationId;
    this.loadStats();
  }

  @Input()
  get incidentId(): number { return this._incidentId; }
  set incidentId(incidentId: number) {
    this._incidentId = incidentId;
    this.loadStats();
  }

  ngOnInit(): void {
  }

  private async loadStats() {
    const gotAll = this.applicationId > 0 && this._incidentId > 0;
    if (!gotAll) {
      return;
    }

    const query = new api.GetPartitionInsights();
    query.applicationIds = [this.applicationId];
    query.incidentId = this._incidentId;
    const result = await this.apiClient.query<api.GetPartitionInsightsResult>(query);

    // null = not supported.
    if (!result || result.applications.length === 0) {
      this.showGotNone = true;
      return;
    }

    if (result.applications[0].indicators.length === 0) {
      this.showGotNone = true;
      return;
    }

    var series: IChartSeries[] = [];
    result.applications[0].indicators.forEach(indicator => {
      series.push({
        name: indicator.displayName,
        data: indicator.values
      });
    });

    const legend = this.chartService.generateLabelsFromStringDate(result.applications[0].indicators[0].dates);
    this.chartService.drawLineChart(this.chartId, { labels: legend }, series);
  };
}
