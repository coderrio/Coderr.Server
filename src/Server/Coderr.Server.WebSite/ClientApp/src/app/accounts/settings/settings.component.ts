import { Component, OnInit } from '@angular/core';
import { NavMenuService } from "../../nav-menu/nav-menu.service";

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss']
})
export class SettingsComponent implements OnInit {

  constructor(menuService: NavMenuService) {
    menuService.updateNav([{id: null, route: null, title: 'Account settings'}]);
  }

  ngOnInit(): void {
  }

}
