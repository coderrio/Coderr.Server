import { Component } from '@angular/core';
import { Router, Route } from '@angular/router';
import { FormsModule, ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { AuthorizeService } from "../../api-authorization/authorize.service";

@Component({
  selector: 'admin-main',
  templateUrl: './admin.component.html'
  //styleUrls: ['./admin.component.css']
})
export class AdminComponent {
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
    //this.printpath('', this.router.config);

  }
  
  printpath(parent: String, config: Route[]) {
    for (let i = 0; i < config.length; i++) {
      const route = config[i];
      if (route.children) {
        const currentPath = route.path ? parent + '/' + route.path : parent;
        this.printpath(currentPath, route.children);
      }
    }
  }

  onSubmit(values): void {

    this.authService.login(this.loginForm.value.userName, this.loginForm.value.password)
      .then(x => {
        this.loginForm.reset();
        this.router.navigate([this.returnUrl]);
      }).catch(e => {
        this.errorMessage = e.message.replace(/\n/g, "<br>\n");
      });

  }
}
