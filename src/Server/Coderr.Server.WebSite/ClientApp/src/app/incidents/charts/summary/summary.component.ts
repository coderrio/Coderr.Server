import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import * as api from "../../../../server-api/Core/Applications";
import { ApiClient } from "../../../utils/HttpClient";
import { ChartService, IChartSeries } from "../../../services/chart.service";

@Component({
  selector: 'app-summarychart',
  templateUrl: './summary.component.html',
  styleUrls: ['./summary.component.scss']
})
export class SummaryChartComponent implements OnInit, OnDestroy {
  private _applicationId: number;
  private static counter = 1;
  private timer: any;
  chartId: string = "summaryChart";

  constructor(
    private apiClient: ApiClient,
    private chartService: ChartService
  ) {
    this.chartId = this.chartService.generateChartId();
    this.timer = setInterval(this.onRefreshStats, 5000);
  }

  @Input()
  get applicationId(): number { return this._applicationId; }
  set applicationId(applicationId: number) {
    this._applicationId = applicationId;
    this.loadStats();
  }

  ngOnInit(): void {
  }

  ngOnDestroy(): void {
    clearTimeout(this.timer);
  }

  private onRefreshStats() {
    this.loadStats();
  }

  private async loadStats() {
    var query = new api.GetApplicationOverview();
    query.applicationId = this.applicationId;
    var result = await this.apiClient.query<api.GetApplicationOverviewResult>(query);

    var series: IChartSeries[] = [];
    series.push({
      name: 'Distinct new errors',
      data: result.incidents
    });
    series.push({
      name: 'Received error reports',
      data: result.errorReports
    });

    this.chartService.drawLineChart(this.chartId, { labels: result.timeAxisLabels, tickCount: 15 }, series)
  }
}

