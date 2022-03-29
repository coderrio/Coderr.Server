import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, Validators } from '@angular/forms';
import { AccountService } from "./account.service";
import { AuthorizeService } from "../../api-authorization/authorize.service";

@Component({
  selector: 'register',
  templateUrl: './register.component.html'
  //styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  regForm = this.formBuilder.group({
    userName: ['', Validators.required],
    password: ['', Validators.required],
    password2: ['', Validators.required],
    emailAddress: ['', [Validators.required, Validators.email]],
  });
  errorMessage = '';
  returnUrl = '';
  loginUrl = '';
  strengthMessage = '';
  activationRequired = false;

  constructor(
    private accountService: AccountService,
    private authService: AuthorizeService,
    private formBuilder: FormBuilder,
    private router: Router
  ) {
    this.loginUrl = localStorage.getItem('loginUrl');
  }

  ngOnInit(): void {

  }

  saveForm(): void {
    var errors = [];
    this.validatePassword(errors);

    if (errors.length > 0) {
      this.errorMessage = errors.join('<br>');
      return;
    }

    this.register();
  }

  checkPassword(text: string) {
    this.strengthMessage = this.checkPassStrength(text);
  }

  private async register(): Promise<void> {

    try {
      var activationRequired = await this.accountService.register(this.regForm.value.userName,
        this.regForm.value.password,
        this.regForm.value.emailAddress);

      if (!activationRequired) {
        await this.authService.login(this.regForm.value.userName, this.regForm.value.password);
        this.router.navigate(['/']);
      } else {
        this.activationRequired = true;
      }

    } catch (e) {
      this.errorMessage = e.message.replace(/\n/g, "<br>\n");
    }
  }

  private validatePassword(errors: string[]) {
    if (this.regForm.value.password !== this.regForm.value.password2) {
      errors.push('Entered passwords do not match.');
    }
  }

  private scorePassword(pass: string): number {
    let score = 0;
    if (!pass)
      return score;

    // award every unique letter until 5 repetitions
    const letters = new Object();
    for (let i = 0; i < pass.length; i++) {
      letters[pass[i]] = (letters[pass[i]] || 0) + 1;
      score += 5.0 / letters[pass[i]];
    }

    // bonus points for mixing it up
    var variations = {
      digits: /\d/.test(pass),
      lower: /[a-z]/.test(pass),
      upper: /[A-Z]/.test(pass),
      nonWords: /\W/.test(pass),
    };

    let variationCount = 0;
    for (let check in variations) {
      variationCount += (variations[check] === true) ? 1 : 0;
    }
    score += (variationCount - 1) * 10;

    return score;
  }

  private checkPassStrength(pass: string) {
    const score = this.scorePassword(pass);
    if (score > 80)
      return "You got a new high score!";
    if (score > 50)
      return "Good enough, but you can do better..";
    if (score >= 20)
      return "Weak password";
    if (score < 20)
      return "'1234' is not a real password...";
    return "";
  }

}
