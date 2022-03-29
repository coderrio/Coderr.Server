import { Component } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthorizeService } from "../../api-authorization/authorize.service";
declare var window: any;

@Component({
  selector: 'activate',
  templateUrl: './activate.component.html'
})
export class ActivateComponent {
  errorMessage = "";
  failed = false;
  activationCode = "";
  constructor(
    private authService: AuthorizeService,
    private router: Router,
    activatedRoute: ActivatedRoute
  ) {

    this.activationCode = activatedRoute.snapshot.params['activationCode'];
    this.activate();
  }

  private async activate(): Promise<void> {
    if (!this.activationCode || this.activationCode === "") {
      this.errorMessage = 'No activation code was specified';
      this.failed = true;
    }

    try {
      await this.authService.activate(this.activationCode);
      this.router.navigate(['/']);
    } catch (error) {
      this.errorMessage = error.message;
      this.failed = true;
    }
  }
}
