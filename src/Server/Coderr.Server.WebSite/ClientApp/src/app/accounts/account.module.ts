import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from "./login.component";
import { LogoutComponent } from "./logout.component";
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { SettingsComponent } from './settings/settings.component';
import { NotificationsComponent } from './settings/notifications/notifications.component';
import { ProfileComponent } from './settings/profile/profile.component';
import { NavbarComponent } from './settings/navbar/navbar.component';
import { ControlsModule } from "../_controls/controls.module";

const routes: Routes = [
  { path: 'account/login', component: LoginComponent },
  { path: 'account/logout', component: LogoutComponent },
  {
    path: 'account/settings',
    component: SettingsComponent,
    children: [
      {
        path: '',
        component: ProfileComponent
      },
      {
        path: 'home',
        component: ProfileComponent
      },
      {
        path: 'nots',
        component: NotificationsComponent
      }
    ]
  }
];

@NgModule({
  declarations: [LoginComponent, LogoutComponent, SettingsComponent, NotificationsComponent, ProfileComponent, NavbarComponent],
  imports: [
    ControlsModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forChild(routes)
  ],
  exports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule
  ]
})
export class AccountModule { }
