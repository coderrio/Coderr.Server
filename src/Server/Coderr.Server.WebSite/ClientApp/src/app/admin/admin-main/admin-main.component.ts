import { Component, OnInit } from '@angular/core';
import { NavMenuService } from "../../nav-menu/nav-menu.service";

@Component({
  selector: 'app-admin-main',
  templateUrl: './admin-main.component.html',
  styleUrls: ['./admin-main.component.scss']
})
export class AdminMainComponent implements OnInit {

  constructor(
    navMenuService: NavMenuService) {
    navMenuService.updateNav([
      { title: 'System Administration', route: ['/admin'] }
    ]);
  }

  ngOnInit(): void {
  }

}
