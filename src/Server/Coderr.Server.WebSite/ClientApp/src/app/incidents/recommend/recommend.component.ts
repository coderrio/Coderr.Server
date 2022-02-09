import { Component, OnInit } from '@angular/core';
import { IncidentsService } from "../incidents.service";

@Component({
  selector: 'app-recommend',
  templateUrl: './recommend.component.html',
  styleUrls: ['./recommend.component.scss']
})
export class RecommendComponent implements OnInit {

  constructor(private incidentService: IncidentsService) {
  }

  ngOnInit(): void {
  }

}
