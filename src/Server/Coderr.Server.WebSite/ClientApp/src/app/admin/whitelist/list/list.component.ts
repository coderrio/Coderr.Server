import { Component, OnInit } from '@angular/core';
import { IApplication } from '../../../applications/application.model';
import { ApplicationService } from '../../../applications/application.service';
import { IApplicationListItem, IIpAddress, WhitelistEntry } from '../whitelist.model';
import { WhitelistService } from "../whitelist.service";
import { NavMenuService } from "../../../nav-menu/nav-menu.service";

@Component({
  selector: 'whitelist',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit {
  entries: WhitelistEntry[] = [];
  private apps: IApplication[] = [];

  constructor(
    private appService: ApplicationService,
    navMenuService: NavMenuService,
    private service: WhitelistService) {

    navMenuService.updateNav([
      { title: 'System Administration', route: ['admin'] },
      { title: 'Whitelists', route: ['admin/whitelists'] }
    ]);

    this.load();
  }

  ngOnInit(): void {
  }

  friendlyIps(items: IIpAddress[]) {
    return items.length === 0 ? 'All' : items.map(x => x.address).join('<br>');
  }

  friendlyApps(items: IApplicationListItem[]) {
    return items.length === 0 ? 'All' : items.map(x=>x.name).join('<br>');
  }

  private async load(): Promise<void> {
    this.entries = await this.service.list();
    this.apps = await this.appService.list();
    this.entries.forEach(x => {
      x.mapAllApps(this.apps);
    });
  }
}
