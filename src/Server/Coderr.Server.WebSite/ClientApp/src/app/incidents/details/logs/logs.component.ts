import { Component, OnInit, OnDestroy } from '@angular/core';
import { ApiClient } from "../../../utils/HttpClient";
import { ActivatedRoute } from "@angular/router";
import * as api from "../../../../server-api/Common/Logs";

interface ILogEntry {
  message: string;
  logLevel: number;
  exception: string;
  timeStampUtc: Date;
}

@Component({
  selector: 'app-logs',
  templateUrl: './logs.component.html',
  styleUrls: ['./logs.component.scss']
})
export class LogsComponent implements OnInit, OnDestroy {
  private sub: any;
  incidentId: number = 0;
  entries: ILogEntry[] = [];
  allEntries: ILogEntry[] = [];
  filterText = "";

  constructor(
    private readonly apiClient: ApiClient,
    private route: ActivatedRoute) {

  }

  ngOnInit(): void {
    this.sub = this.route.parent.params.subscribe(params => {
      this.incidentId = +params['incidentId'];
      var q = new api.GetLogs();
      q.incidentId = this.incidentId;
      this.apiClient.query<api.GetLogsResult>(q)
        .then(result => {
          result.entries.forEach(x => {
            if (x.message) {
              this.allEntries.push({
                message: this.escapeHtml(x.message).replace(/\r\n/, "<br>\r\n"),
                exception: this.escapeHtml(x.exception).replace(/\r\n/, "<br>\r\n"),
                timeStampUtc: x.timeStampUtc,
                logLevel: x.level
              });
              
            }
          });
          console.log(this.allEntries)
          this.entries = this.allEntries;
        });
    });
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
  filterEntries($event: KeyboardEvent) {
    console.log($event, this.filterText)
    var re = new RegExp(this.filterText, 'i');
    this.entries = this.allEntries.filter(x => re.test(x.message) || re.test(x.exception));
  }

  private escapeHtml(unsafe: string): string {
    if (!unsafe) {
      return "";
    }
    return unsafe
      .replace(/&/g, "&amp;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;")
      .replace(/"/g, "&quot;")
      .replace(/'/g, "&#039;");
  }
}
