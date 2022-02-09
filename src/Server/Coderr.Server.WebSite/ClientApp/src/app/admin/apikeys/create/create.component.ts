import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from "@angular/router";
import { ApplicationService } from "../../../applications/application.service";
import { IApplication } from "../../../applications/application.model";
import { AuthorizeService } from "../../../../api-authorization/authorize.service";
import { ToastrService } from "ngx-toastr";
import { ApiKeyService } from "../api-keys.service";
import { ApiKey, IApplicationSummary } from "../api-key.model";
import { NavMenuService } from "../../../nav-menu/nav-menu.service";

export interface IMyApp {
  selected: boolean;
  name: string;
  id: number;
}

@Component({
  selector: 'apikey-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.scss']
})
export class CreateApiKeyComponent implements OnInit, OnDestroy {
  apiKeyId: number = 0;
  key: string = '';
  sharedSecret: string = '';
  applications: IMyApp[] = [];
  title: string = '';
  accountId: number;
  private appSub: any;

  constructor(
    appService: ApplicationService,
    authService: AuthorizeService,
    private apiKeyService: ApiKeyService,
    private toastrService: ToastrService,
    private router: Router,
    private route: ActivatedRoute,
    private navMenuService: NavMenuService) {

    this.appSub = appService.applications.subscribe(x => this.updateApplications(x));
    this.key = Guid.newGuid();
    this.sharedSecret = Guid.newGuid();
    this.accountId = authService.user.accountId;
    this.navMenuService.updateNav([
      { title: 'System Administration', route: ['admin'] },
      { title: 'Api keys', route: ['admin/apikeys'] },
      { title: 'New', route: ['admin/apikeys/new'] }
    ]);
  }

  ngOnInit(): void {
  }

  ngOnDestroy(): void {
    this.appSub.unsubscribe();
  }

  async saveKey(): Promise<void> {
    var apiKey = new ApiKey();
    apiKey.title = this.title;
    apiKey.applications = this.applications.filter(x => x.selected);
    apiKey.accountId = this.accountId;
    apiKey.apiKey = this.key;
    apiKey.sharedSecret = this.sharedSecret;

    await this.apiKeyService.create(apiKey);
    this.toastrService.success('Key is being created..');

    this.router.navigate(['../../'], { relativeTo: this.route });
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

class Guid {
  static newGuid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => {
      var r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
    });
  }
}
