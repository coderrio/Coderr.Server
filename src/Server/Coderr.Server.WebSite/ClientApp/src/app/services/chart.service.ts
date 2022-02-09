import { Injectable } from '@angular/core';
import ApexCharts from 'apexcharts/dist/apexcharts.common.js'

export interface IChartSeries {
  name: string;
  data: any[];
}

export interface ILabelOptions {
  labels: string[];
  tickCount?: number;
}

@Injectable({
  providedIn: 'root'
})
export class ChartService {
  private static counter = 1;
  private charts: any = {};

  constructor() {
    ApexCharts.colors = ['#9900cc', '#E91E63', '#9C27B0'];
  }

  drawLineChart(chartId: string, labels: ILabelOptions, series: IChartSeries[]) {
    var height = document.documentElement.clientHeight / 3;

    var options: any = {
      series: series,
      chart: {
        //height: 350,
        type: 'line',
      },
      stroke: {
        width: 7,
        curve: 'smooth'
      },
      xaxis: {
        categories: labels.labels,
      },
      height: height + "px",
      width: '100%',
      fill: {
        type: 'gradient',
        gradient: {
          shade: 'dark',
          //gradientToColors: ['#59c1d5'],
          shadeIntensity: 1,
          type: 'horizontal',
          opacityFrom: 1,
          opacityTo: 1,
          stops: [0, 100]
        },
      },
      markers: {
        size: 2,
        //colors: ["#f18c65", "#f18c99"],
        strokeColors: "#fff",
        strokeWidth: 2,
        hover: {
          size: 7,
        }
      },
      yaxis: {
        min: 0,
        title: {
          text: 'Count',
        },
      },
      legend: {
        show: true
      }
    };
    if (labels.tickCount > 0) {
      options.xaxis.tickAmount = labels.tickCount;
    }

    const chart = new ApexCharts(document.querySelector('#' + chartId), options);
    this.charts[chartId] = chart;
    chart.render();
  }

  updateLineChart(chartId: string, series: IChartSeries[]) {
    //var chart = <ApexCharts>this.charts[chartId];
    //chart.updateSeries(series);
  }

  generateChartId(): string {
    return `chart${ChartService.counter++}`;
  }


  generateLabelsFromStringDate(labels: string[], dateFormatOptions = null): string[] {
    if (!dateFormatOptions) {
      dateFormatOptions = { month: 'short', year: 'numeric' };
    }

    var dates: string[] = [];
    labels.forEach(x => {

      // if no timezone was specified, assume UTC.
      var prefix = '';
      if (x.indexOf('+') === -1 && x.indexOf('Z') === -1) {
        prefix = 'Z';
      }

      const d = new Date(Date.parse(x + prefix));
      
      const fmt = new Intl.DateTimeFormat('en-US', dateFormatOptions);
      dates.push(fmt.format(d));
    });
    return dates;
  }
}
