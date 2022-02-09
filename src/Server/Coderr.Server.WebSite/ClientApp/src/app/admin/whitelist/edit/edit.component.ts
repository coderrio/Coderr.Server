import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from "@angular/router";
import { NavMenuService } from "../../../nav-menu/nav-menu.service";
import { WhitelistService } from "../whitelist.service";
import { WhitelistEntry, IIpAddress, IApplicationListItem, IpType } from "../whitelist.model";
import { ApplicationService } from "../../../applications/application.service";
import { ModalService } from "../../../_controls/modal/modal.service";

export interface IApplicationSelection {
  selected: boolean;
  name: string;
  id: number;
}

@Component({
  selector: 'whitelist-edit',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.scss']
})
export class EditComponent implements OnInit, OnDestroy {
  id: number;
  domainName = "";
  applications: IApplicationSelection[] = [];
  ipAddresses: IIpAddress[] = [];
  newIpAddress = '';
  errorMessage = '';
  disabled = false;
  private routeSub: any;
  private appSub: any;

  constructor(
    appService: ApplicationService,
    private modalService: ModalService,
    private service: WhitelistService,
    private route: ActivatedRoute,
    private router: Router,
    private navMenuService: NavMenuService) {
    this.appSub = appService.applications.subscribe(x => {
      var apps = x.map(x => {
        var app: IApplicationSelection = {
          id: x.id,
          name: x.name,
          selected: false
        };
        return app;
      });
      this.updateApplications(apps);
    });


  }

  ngOnInit(): void {
    this.routeSub = this.route.params.subscribe(params => {
      this.id = +params['id'];
      this.load(this.id);
    });
  }

  ngOnDestroy(): void {
    this.routeSub.unsubscribe();
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

  }

  removeIp(entry: IIpAddress) {
    this.ipAddresses = this.ipAddresses.filter(x => x.address !== entry.address);
  }

  private async load(id: number): Promise<void> {
    var whitelist = await this.service.get(id);
    this.domainName = whitelist.domainName;

    var apps = this.applications;
    this.applications = whitelist.applications.map(x => {
      var entry: IApplicationSelection = {
        id: x.id,
        name: x.name,
        selected: true
      };
      return entry;
    });
    if (apps.length > 0) {
      this.updateApplications(apps);
    }

    this.ipAddresses = whitelist.ipAddresses;

    this.navMenuService.updateNav([
      { title: 'System Administration', route: ['admin'] },
      { title: 'Whitelists', route: ['admin/whitelists'] },
      { title: this.domainName, route: ['admin/whitelists/', id] }
    ]);

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
    await this.service.update(entry);

    this.router.navigate(['../'], { relativeTo: this.route });
  }

  private updateApplications(apps: IApplicationSelection[]) {
    var checkedOnes = this.applications.filter(x => x.selected).map(x => x.id);
    var ourApps = apps.map<IApplicationSelection>(x => {
      var map: IApplicationSelection = {
        id: x.id,
        name: x.name,
        selected: checkedOnes.indexOf(x.id) !== -1
      };
      return map;
    });

    this.applications = ourApps;
  }

}
