import { Component, OnInit } from '@angular/core';
import { IncidentsService } from "../../incidents/incidents.service";

@Component({
  selector: 'app-my-errors',
  templateUrl: './my-errors.component.html',
  styleUrls: ['./my-errors.component.scss']
})
export class MyErrorsComponent implements OnInit {


  constructor(private readonly incidentService: IncidentsService) {
    incidentService.myIncidents
  }

  ngOnInit(): void {

  }

}
