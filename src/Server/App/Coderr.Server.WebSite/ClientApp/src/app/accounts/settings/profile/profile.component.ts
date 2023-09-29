import { Component, OnInit } from '@angular/core';
import { ApiClient } from "../../../utils/HttpClient";
import { ToastrService } from "ngx-toastr";
import { FormBuilder } from '@angular/forms';
import * as dto from "../../../../server-api/Core/Users";

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  form = this.formBuilder.group({
    workEmail: '',
    firstName: '',
    lastName: ''
  });
  errorMessage = '';
  constructor(private apiClient: ApiClient,
    private formBuilder: FormBuilder,
    private toastrService: ToastrService) { }

  ngOnInit(): void {
    var query = new dto.GetUserSettings();

    this.apiClient.query<dto.GetUserSettingsResult>(query)
      .then(x => {
        this.form.get("workEmail").setValue(x.emailAddress);
        this.form.get("firstName").setValue(x.firstName);
        this.form.get("lastName").setValue(x.lastName);
      });
  }

  onSubmit(): void {
    this.saveSettings();
  }

  async saveSettings() {
    var cmd = new dto.UpdatePersonalSettings();
    cmd.emailAddress = this.form.value.workEmail;
    cmd.firstName = this.form.value.firstName;
    cmd.lastName = this.form.value.lastName;
    await this.apiClient.command(cmd);
    this.toastrService.success('Saved OK');

  }

}
