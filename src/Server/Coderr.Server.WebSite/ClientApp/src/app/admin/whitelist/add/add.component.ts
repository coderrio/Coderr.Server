import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from "@angular/router";
import { IApplication } from '../../../applications/application.model';
import { ApplicationService } from '../../../applications/application.service';
import { NavMenuService } from '../../../nav-menu/nav-menu.service';
import { ModalService } from '../../../_controls/modal/modal.service';
import { IApplicationListItem, IIpAddress, IpType, WhitelistEntry } from '../whitelist.model';
import { WhitelistService } from '../whitelist.service';

export interface IMyApp {
  selected: boolean;
  name: string;
  id: number;
}

@Component({
  selector: 'whitelist-add',
  templateUrl: './add.component.html',
  styleUrls: ['./add.component.scss']
})
export class AddComponent implements OnInit, OnDestroy {
  domainName = "";
  applications: IMyApp[] = [];
  ipAddresses: IIpAddress[] = [];
  newIpAddress = '';
  errorMessage = '';
  disabled = false;
  private appSub: any;

  constructor(appService: ApplicationService,
    private modalService: ModalService,
    private navMenuService: NavMenuService,
    private route: ActivatedRoute,
    private router: Router,
    private service: WhitelistService) {
    this.appSub = appService.applications.subscribe(x => this.updateApplications(x));
    this.navMenuService.updateNav([
      { title: 'System Administration', route: ['admin'] },
      { title: 'Whitelists', route: ['admin/whitelists'] },
      { title: 'New', route: ['admin/whitelists/new'] }
    ]);
  }

  ngOnInit(): void {
  }

  ngOnDestroy(): void {
    this.appSub.unsubscribe();
  }

  showAddIp() {
    this.modalService.open('newIpModal');
  }

  closeIpModal() {
    this.modalService.close('newIpModal');
  }

  addNewIp() {
    this.ipAddresses.push({
      address: this.newIpAddress,
      id: 0,
      type: IpType.Manual
    });
    this.newIpAddress = '';
    this.closeIpModal();
  }

  cancel() {
    this.router.navigate(['../'], { relativeTo: this.route });
  }

  async save(): Promise<void> {
    this.disabled = true;
    var entry = new WhitelistEntry(this.domainName);
    entry.applications = this.applications.filter(x => x.selected).map(x => {
      var e: IApplicationListItem = {
        id: x.id,
        name: x.name
      };
      return e;
    });
    entry.ipAddresses = this.ipAddresses;
    await this.service.add(entry);

    this.router.navigate(['../'], { relativeTo: this.route });
  }

  private updateApplications(apps: IApplication[]) {

    var checkedOnes = this.applications.filter(x => x.selected).map(x => x.id);

    var ourApps = apps.map<IMyApp>(x => {
      var map: IMyApp = {
        id: x.id,
        name: x.name,
        selected: checkedOnes.indexOf(x.id) !== -1
      };
      return map;
    });

    this.applications = ourApps;
  }
}
