import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { Incident } from "../../incident.model";
import { IncidentsService } from "../../incidents.service";
import { ActivatedRoute } from "@angular/router";
import { ReportService, ReportSummary, Report, ReportCollection } from "../../report.service";
import { ChartService } from "../../../services/chart.service";
import { ModalService } from "../../../_controls/modal/modal.service";

@Component({
  selector: 'incident-details-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit, OnDestroy {
  private sub: any;
  incidentId: number;
  applicationId: number;
  incident = new Incident();
  reports: ReportSummary[] = [];
  currentReport: Report;
  currentCollection: ReportCollection;
  currentCollectionName: string;
  canShowNext = true;
  canShowPrevious = true;
  reportIndex = 0;

  constructor(
    private incidentService: IncidentsService,
    private route: ActivatedRoute,
    private chartService: ChartService,
    private reportService: ReportService,
    private modalService: ModalService
  ) {

  }

  ngOnInit(): void {
    this.sub = this.route.parent.params.subscribe(params => {
      this.incidentId = +params['incidentId'];
      this.loadEverything();
    });
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  private async loadEverything() {
    this.incident = await this.incidentService.get(this.incidentId);
    this.applicationId = this.incident.applicationId;
    const labels = this.chartService.generateLabelsFromStringDate(this.incident.monthReports.days, { month: 'short', day: 'numeric' });
    this.chartService.drawLineChart('dayReports',
      {
        labels: labels
      },
      [
        {
          name: 'Received reports',
          data: this.incident.monthReports.values
        }
      ]);

    this.reports = await this.reportService.getReportList(this.incidentId);
    if (this.reports.length > 0) {
      this.selectReport(this.reports[0].id);
    }

  }


  selectReport(reportId: number) {
    this.reportService.getReport(reportId)
      .then(report => {
        this.currentReport = report;
        if (report.contextCollections.length > 0) {
          let collection = report.contextCollections.filter(x => x.name === this.currentCollectionName);
          if (collection.length > 0) {
            this.selectCollection(collection[0].name);
            return;
          }
          collection = report.contextCollections.filter(x => x.name === 'CustomData');
          if (collection.length > 0) {
            this.selectCollection(collection[0].name);
            return;
          }
          collection = report.contextCollections.filter(x => x.name === 'ExceptionProperties');
          if (collection.length > 0) {
            this.selectCollection(collection[0].name);
            return;
          }

          this.selectCollection(report.contextCollections[0].name);
        }
      }
      );

    this.canShowNext = true;
    this.canShowPrevious = true;
    if (this.reportIndex === 0) {
      this.canShowPrevious = false;
    }
    if (this.reportIndex === this.reports.length - 1) {
      this.canShowNext = false;
    }
  }


  showNextReport() {
    if (this.reportIndex < this.reports.length - 1) {
      this.reportIndex++;
      this.selectReport(this.reports[this.reportIndex].id);
    }

  }

  showPreviousReport() {
    if (this.reportIndex > 0) {
      this.reportIndex--;
      this.selectReport(this.reports[this.reportIndex].id);
    }

  }


  showReportSelector() {
    this.modalService.open("chooseReportModal");
  }
  selectSpecificReport(report: ReportSummary) {
    this.selectReport(report.id);
    this.modalService.close("chooseReportModal");
  }

  selectCollection(name: string) {
    const collection = this.currentReport.contextCollections.filter(x => x.name === name);
    if (collection.length > 0) {
      this.currentCollectionName = name;
      this.currentCollection = collection[0];
    } else {
      this.currentCollectionName = null;
      this.currentCollection = null;
    }
  }
}
