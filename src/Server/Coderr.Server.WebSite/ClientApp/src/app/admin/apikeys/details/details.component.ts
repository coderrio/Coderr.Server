import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ApiKeyService } from "../api-keys.service";
import { ApiKey } from "../api-key.model";
import { NavMenuService } from "../../../nav-menu/nav-menu.service";

@Component({
  selector: 'app-details',
  templateUrl: './details.component.html',
  styleUrls: ['./details.component.scss']
})
export class ApiKeyDetailsComponent implements OnInit, OnDestroy {
  apiKeyId: number = 0;
  key: ApiKey = new ApiKey();

  private appPromise: Promise<void>;
  private sub: any;
  private id: number;

  constructor(
    private route: ActivatedRoute,
    private apiKeyService: ApiKeyService,
    private navMenuService: NavMenuService) {
    this.navMenuService.updateNav([
      { title: 'System Administration', route: ['admin'] },
      { title: 'Api Keys', route: ['admin/apikeys'] }
    ]);
  }

  ngOnInit(): void {
    this.sub = this.route.params.subscribe(params => {
      this.id = +params['id'];
      this.loadKey();
    });
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  private async loadKey(): Promise<void> {
    await this.appPromise;
    this.key = await this.apiKeyService.get(this.id);
    this.navMenuService.updateNav([
      { title: 'System adminstration', route: ['admin'] },
      { title: 'Api keys', route: ['admin/apikeys'] },
      { title: this.key.title, route: ['admin/apikeys', this.key.id] }
    ]);
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
