import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthorizeService } from "../../api-authorization/authorize.service";
declare var window: any;

@Component({
  selector: 'logout',
  templateUrl: './logout.component.html'
})
export class LogoutComponent {
  constructor(
    private authService: AuthorizeService,
    private router: Router
  ) {
    this.authService.logout();
    var loginUrl = localStorage.getItem('loginUrl');
    if (loginUrl) {
      window.location.href = loginUrl;
    } else {
      this.router.navigate(['account', 'login']);
    }
  }
}
