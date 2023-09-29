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
  private _refreshSeconds: number = 10;
  private _timer: any;
  chartId: string = "summaryChart";

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
  get refreshSeconds(): number {
    return this._refreshSeconds;
  }

  set refreshSeconds(value: number) {
    this._refreshSeconds = value;
  }

  private resetTimer() {
    clearInterval(this._timer);
    this._timer = setInterval(() => this.reloadStats(), this.refreshSeconds * 1000);
  }

  private async reloadStats(): Promise<object> {
    return null;
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

    this.chartService.updateLineChart(this.chartId, series);
    return null;
  }

  ngOnInit(): void {
    this.resetTimer();
  }

  ngOnDestroy() {
    clearInterval(this._timer);
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

    this.chartService.drawLineChart(this.chartId, { labels: result.timeAxisLabels, tickCount: 15 }, series);
  }
}

