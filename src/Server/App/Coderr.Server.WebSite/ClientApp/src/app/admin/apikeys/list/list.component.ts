import { Component, OnInit } from '@angular/core';
import { ApiKeyService } from "../api-keys.service";
import { IApiKeyListItem } from "../api-key.model";
import { NavMenuService } from "../../../nav-menu/nav-menu.service";

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ApiKeyListComponent implements OnInit {
  keys: IApiKeyListItem[] = [];

  constructor(private keyService: ApiKeyService,
    private navMenuService: NavMenuService) {
    keyService.list().then(x => this.keys = x);
    this.navMenuService.updateNav([
      { title: 'System Administration', route: ['admin'] },
      { title: 'Api Keys', route: ['admin/apikeys'] }
    ]);

  }

  ngOnInit(): void {
  }

}
