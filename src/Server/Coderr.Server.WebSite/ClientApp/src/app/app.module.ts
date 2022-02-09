import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { ApiAuthorizationModule } from 'src/api-authorization/api-authorization.module';
import { AuthorizeGuard } from 'src/api-authorization/authorize.guard';
import { AuthorizeInterceptor } from 'src/api-authorization/authorize.interceptor';

import { HomeModule } from "./home/home.module";
//import { AppDashboardComponent } from "./applications/user/dashboard.component";
import { IncidentsModule } from "./incidents/incidents.module";
import { AccountModule } from "./accounts/account.module";
import { AdminModule } from "./admin/admin.module";

import { ControlsModule } from "./_controls/controls.module";
import { SignalRService } from "./services/signal-r.service";
import { ToastrModule } from 'ngx-toastr';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

var routes = [
  //{ path: '', component: HomeComponent, pathMatch: 'full', canActivate: [AuthorizeGuard] },
  //{ path: 'dashboard', component: AppDashboardComponent, canActivate: [AuthorizeGuard] }
];

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    ControlsModule,
    HttpClientModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    ApiAuthorizationModule,
    RouterModule.forRoot(routes),
    AdminModule,
    IncidentsModule,
    HomeModule,
    AccountModule,
    ToastrModule.forRoot(),
    BrowserAnimationsModule
  ],
  exports: [
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthorizeInterceptor, multi: true },
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
  constructor(service: SignalRService) {
    service.startConnection();
  }
}
