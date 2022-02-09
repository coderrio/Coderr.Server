import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule, ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { AuthorizeService } from "../../api-authorization/authorize.service";

@Component({
  selector: 'login',
  templateUrl: './login.component.html'
  //styleUrls: ['./cart.component.css']
})
export class LoginComponent {
  loginForm = this.formBuilder.group({
    userName: '',
    password: ''
  });
  errorMessage = '';
  returnUrl = '';

  constructor(
    private authService: AuthorizeService,
    private formBuilder: FormBuilder,
    private router: Router
  ) {
  }

  onSubmit(): void {

    this.authService.login(this.loginForm.value.userName, this.loginForm.value.password)
      .then(x => {
        this.loginForm.reset();
        this.router.navigate([this.returnUrl]);
      }).catch(e => {
        console.log('got error ', e, e.description)
        this.errorMessage = e.message.replace(/\n/g, "<br>\n");
      });

  }
}
