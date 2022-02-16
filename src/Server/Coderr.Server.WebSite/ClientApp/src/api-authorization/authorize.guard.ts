import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthorizeService } from './authorize.service';
import { tap } from 'rxjs/operators';
declare var window: any;

@Injectable({
  providedIn: 'root'
})
export class AuthorizeGuard implements CanActivate {
  constructor(private authorize: AuthorizeService, private router: Router) {
  }
  canActivate(
    _next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {

    if (this.authorize.isAuthenticated()) {
      return true;
    }

    var loginUrl = localStorage.getItem('loginUrl');
    if (loginUrl) {
      window.location.href = loginUrl;
      return false;
    }

    //window.location = "/account/login";
    this.router.navigate(['account', 'login'], {
      queryParams: {
        'returnUrl': state.url
      }
    });
    return false;
  }

  private handleAuthorization(isAuthenticated: boolean, state: RouterStateSnapshot) {
    if (!isAuthenticated) {
      this.router.navigate(['account', 'login'], {
        queryParams: {
          'returnUrl': state.url
        }
      });
    }
  }
}
