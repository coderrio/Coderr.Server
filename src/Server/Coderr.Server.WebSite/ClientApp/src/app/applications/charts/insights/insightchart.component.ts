import { Component, OnInit, OnDestroy, Input } from "@angular/core";
import * as api from "../../../../server-api/Common/Partitions";
import { ApiClient } from "../../../utils/HttpClient";
import { ChartService, IChartSeries } from "../../../services/chart.service";

@Component({
  selector: "app-insightchart",
  templateUrl: "./insightchart.component.html",
  styleUrls: ["./insightchart.component.scss"]
})
export class InsightChartComponent implements OnInit, OnDestroy {
  private _applicationId: number;
  private _resfreshSeconds = 10;
  private _timer: any;
  chartId = "";
  noData = false;

  constructor(
    private apiClient: ApiClient,
    private chartService: ChartService
  ) {
    this.chartId = this.chartService.generateChartId();
  }

  @Input()
  get applicationId(): number {
    return this._applicationId;
  }

  set applicationId(applicationId: number) {
    this._applicationId = applicationId;
    this.loadStats();
    this.resetTimer();
  }

  @Input()
  get refreshSeconds(): number {
    return this._resfreshSeconds;
  }

  set refreshSeconds(value: number) {
    this._resfreshSeconds = value;
  }

  private resetTimer() {
    clearInterval(this._timer);
    this._timer = setInterval(() => this.reloadStats(), this.refreshSeconds * 1000);
  }

  private async reloadStats(): Promise<object> {

    const query = new api.GetPartitionInsights();
    query.applicationIds = [this.applicationId];
    const result = await this.apiClient.query<api.GetPartitionInsightsResult>(query);

    // null = not supported.
    if (!result || result.applications.length === 0) {
      this.noData = true;
      return null;
    }

    var series: IChartSeries[] = [];
    result.applications[0].indicators.forEach(indicator => {
      series.push({
        name: indicator.displayName,
        data: indicator.values
      });
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
    const query = new api.GetPartitionInsights();
    query.applicationIds = [this.applicationId];
    const result = await this.apiClient.query<api.GetPartitionInsightsResult>(query);

    // null = not supported.
    if (!result || result.applications.length === 0) {
      if (!result) {
        clearInterval(this._timer);
      }

      this.noData = true;
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
