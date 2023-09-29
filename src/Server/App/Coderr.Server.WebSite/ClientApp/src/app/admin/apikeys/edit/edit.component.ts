import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from "@angular/router";
import { IApplication } from "../../../applications/application.model";
import { AuthorizeService } from "../../../../api-authorization/authorize.service";
import { ApiKeyService } from "../api-keys.service";
import { ApiKey, IApplicationSummary } from "../api-key.model";
import { ToastrService } from "ngx-toastr";
import { ApplicationService } from "../../../applications/application.service";
import { NavMenuService } from "../../../nav-menu/nav-menu.service";

export interface IMyApp {
  selected: boolean;
  name: string;
  id: number;
}

@Component({
  selector: 'app-edit',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.scss']
})
export class EditApiKeyComponent implements OnInit, OnDestroy {
  apiKeyId: number = 0;
  key: string = '';
  sharedSecret: string = '';
  applications: IMyApp[] = [];
  title: string = '';
  accountId: number;

  private appSub: any;
  private sub: any;
  private id: number;

  constructor(
    appService: ApplicationService,
    authService: AuthorizeService,
    private route: ActivatedRoute,
    private apiKeyService: ApiKeyService,
    private toastrService: ToastrService,
    private router: Router,
    private navMenuService: NavMenuService) {
    this.appSub = appService.applications.subscribe(x => this.updateApplications(x));
    this.key = Guid.newGuid();
    this.sharedSecret = Guid.newGuid();
    this.accountId = authService.user.accountId;
  }

  ngOnInit(): void {
    this.sub = this.route.params.subscribe(params => {
      this.id = +params['id'];
      this.loadKey();
    });
  }

  ngOnDestroy(): void {
    this.appSub.unsubscribe();
    this.sub.unsubscribe();
  }

  back() {
    this.router.navigate(['../'], { relativeTo: this.route });
  }

  async saveKey(): Promise<void> {
    var apiKey = new ApiKey();
    apiKey.id = this.id;
    apiKey.title = this.title;
    apiKey.applications = this.applications.filter(x=>x.selected);
    apiKey.accountId = this.accountId;
    apiKey.apiKey = this.key;
    apiKey.sharedSecret = this.sharedSecret;

    await this.apiKeyService.update(apiKey);
    this.toastrService.success('Key is being updated..');

    this.router.navigate(['../'], { relativeTo: this.route });
  }

  private async loadKey(): Promise<void> {
    var key = await this.apiKeyService.get(this.id);
    this.title = key.title;
    this.accountId = key.accountId;
    this.key = key.apiKey;
    this.sharedSecret = key.sharedSecret;

    this.navMenuService.updateNav([
      { title: 'System adminstration', route: ['admin'] },
      { title: 'Api keys', route: ['admin/apikeys'] },
      { title: this.title, route: ['admin/apikeys', this.id] }
    ]);

    if (this.applications.length === 0) {
      key.applications.forEach(x => {
        this.applications.push({
          id: x.id,
          name: x.name,
          selected: true
        });
      });
    } else {
      this.applications.forEach(x => {
        x.selected = key.applications.find(y => y.id === x.id) != null;
      });
    }
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
