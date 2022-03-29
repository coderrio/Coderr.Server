import { Component, OnInit, Input } from '@angular/core';
import { IncidentsService } from "../../incidents.service";
import { IncidentState, Incident, states, IState } from "../../incident.model";
import { ModalService } from "../../../_controls/modal/modal.service";
import { ToastrService } from "ngx-toastr";
import { AccountService, User } from "../../../accounts/account.service";

@Component({
  selector: 'search-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit {
  incident = new Incident();
  states = states;
  myIncidentId: number;
  users: User[] = [];

  constructor(
    private incidentService: IncidentsService,
    private modalService: ModalService,
    private toastrService: ToastrService,
    private accountService: AccountService
  ) {
  }

  ngOnInit(): void {
  }

  @Input()
  get incidentId(): number { return this.myIncidentId; }
  set incidentId(incidentId: number) {
    this.myIncidentId = incidentId;
    if (this.myIncidentId > 0) {
      this.loadEverything();
    }
  }


  assignIncident(accountId: number) {
    this.incidentService.assign(this.incidentId, +accountId);
    this.incident.state = IncidentState.Active;
    this.toastrService.success("Error has been assigned.");
  }

  changeState(stateId: number) {
    var state = this.states[stateId];

    switch (state.id) {
      case IncidentState.Active:
        this.assignIncident(-1);
        break;

      case IncidentState.Closed:
        this.close();
        break;

      case IncidentState.Ignored:
        this.incidentService.ignore(this.incidentId);
        this.toastrService.success("Future reports of this error will be ignored.");
        break;

      default:
        this.toastrService.warning(`Changing state to ${state.name} is invalid when state is ${this.incident.state}.`);
    }

  }

  close() {
    this.modalService.open("closeIncidentModal");

  }

  closeCloseModal(evt: any) {
    this.modalService.close("closeIncidentModal");

  }

  private async loadEverything() {
    this.incident = await this.incidentService.get(this.myIncidentId);
    this.users = await this.accountService.getAll();
  }
}
