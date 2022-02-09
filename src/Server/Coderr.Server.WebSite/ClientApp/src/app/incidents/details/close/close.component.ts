import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { IncidentsService } from "../../incidents.service";
import { ModalService } from "../../../_controls/modal/modal.service";

@Component({
  selector: 'incident-close',
  templateUrl: './close.component.html',
  styleUrls: ['./close.component.scss']
})
export class CloseComponent implements OnInit {
  form = this.formBuilder.group({
    reason: '',
    version: '',
  });
  _incidentId = 0;
  numberOfUsers = 0;
  errorMessage = '';
  returnUrl = '';
  disabled = false;

  @Output() closed = new EventEmitter();

  constructor(
    private formBuilder: FormBuilder,
    private incidentService: IncidentsService,
    private modalService: ModalService) {


  }

  @Input()
  get incidentId(): number { return this._incidentId; }
  set incidentId(incidentId: number) {
    this._incidentId = incidentId;
  }

  ngOnInit() {
  }

  ngOnDestroy() {
  }

  cancel() {
    this.closed.emit({ success: false, message: "Canceled" });
  }

  onSubmit(): void {
    console.log('cloosing...');
    this.incidentService.close(this._incidentId, this.form.value.version, this.form.value.reason)
      .then(numberOfSubscribers => {
        this.numberOfUsers = numberOfSubscribers;
        console.log('should ask feedback', numberOfSubscribers);
        if (numberOfSubscribers > 0) {
          this.modalService.open("askSubscriberModal");
        }
      });

    this.disabled = true;
    this.closed.emit({ success: true, version: this.form.value.version });
  }

  notifyUsers() {
    console.log("Should notify users");
    this.closeNotifyDialog();

  }

  closeNotifyDialog() {
    this.modalService.close("askSubscriberModal");
  }

}
