import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ApplicationService } from "../../application.service";
import { NavMenuService } from "../../../nav-menu/nav-menu.service";

@Component({
  selector: 'app-admin-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class AdminHomeComponent implements OnInit, OnDestroy {
  private sub: any;
  applicationId: number;
  sharedSecret = '';
  appKey = '';

  framework = '';
  lib = '';

  constructor(
    private service: ApplicationService,
    private route: ActivatedRoute,
    private menuService: NavMenuService) {
    this.applicationId = +route.snapshot.params.applicationId;
    this.service.get(this.applicationId).then(x => {
      this.sharedSecret = x.sharedSecret;
      this.appKey = x.appKey;
    });
  }

  ngOnInit(): void {
    this.sub = this.route.params.subscribe(params => {
      this.applicationId = +params['applicationId'];
      this.service.get(this.applicationId).then(x => {
        this.sharedSecret = x.sharedSecret;
        this.appKey = x.appKey;
        console.log('loaded');
        this.menuService.updateNav([
          { title: x.name, route: ['application', x.id] },
          { title: "Administration", route: ['application', x.id, 'admin'] }
        ]
        );

      });
    });
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

}
