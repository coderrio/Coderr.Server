import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute } from "@angular/router";
import { FormBuilder } from '@angular/forms';
import * as api from "../../../../server-api/Web/Feedback";
import { NotifySubscribers } from "../../../../server-api/Core/Incidents";
import { ApiClient } from "../../../utils/HttpClient";
import { ToastrService } from 'ngx-toastr';

export interface IBugReport {
  message: string;
  email: string;
  writtenAtUtc: Date;
}

@Component({
  selector: 'incident-bugreports',
  templateUrl: './bugreports.component.html',
  styleUrls: ['./bugreports.component.scss']
})
export class BugReportsComponent implements OnInit {
  private _incidentId: number;
  private sub: any;
  emails: string[] = [];
  feedback: IBugReport[] = [];
  form = this.formBuilder.group({
    subject: '',
    body: '',
  });

  constructor(
    private formBuilder: FormBuilder,
    private apiClient: ApiClient,
    private activeRoute: ActivatedRoute,
    private toastrService: ToastrService
    ) {
  }

  ngOnInit(): void {
    this.sub = this.activeRoute.parent.params.subscribe(params => {
      this._incidentId = +params['incidentId'];
      if (this._incidentId > 0) {
        this.loadFeedback();
      }
    });
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }


  @Input()
  get incidentId(): number { return this._incidentId; }
  set incidentId(incidentId: number) {
    this._incidentId = incidentId;
    this.loadFeedback();
  }

  sendReport() {
    var cmd = new NotifySubscribers();
    cmd.incidentId = this.incidentId;
    cmd.body = this.form.value.body;
    cmd.title = this.form.value.subject;
    this.apiClient.command(cmd)
      .then(x => {
        this.toastrService.success('Message have been sent.');
        
      });
  }

  private async loadFeedback() {
    var query = new api.GetIncidentFeedback();
    query.incidentId = this._incidentId;
    var result = await this.apiClient.query<api.GetIncidentFeedbackResult>(query);
    this.feedback = result.items.map(this.convertItem);
    this.emails = result.emails;
  }

  private convertItem(dto: api.GetIncidentFeedbackResultItem): IBugReport {
    var item: IBugReport = {
      email: dto.emailAddress,
      message: dto.message,
      writtenAtUtc: new Date(dto.writtenAtUtc + "Z")
    };

    return item;
  }

}
