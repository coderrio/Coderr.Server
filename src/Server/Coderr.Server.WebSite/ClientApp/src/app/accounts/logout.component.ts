import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthorizeService } from "../../api-authorization/authorize.service";

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
    this.router.navigate(['account', 'login']);
  }
}
