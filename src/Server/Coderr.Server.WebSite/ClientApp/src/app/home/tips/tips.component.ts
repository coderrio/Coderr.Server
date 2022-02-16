import { Component, OnInit } from '@angular/core';
import { ApplicationService } from "../../applications/application.service";

@Component({
  selector: 'app-tips',
  templateUrl: './tips.component.html',
  styleUrls: ['./tips.component.scss']
})
export class TipsComponent implements OnInit {
  firstApplicationId = 0;

  constructor(private appService: ApplicationService) { }

  ngOnInit(): void {
    this.appService.list().then(apps => {
      if (apps.length > 0) {
        this.firstApplicationId = apps[0].id;
      }
    });
  }

}
