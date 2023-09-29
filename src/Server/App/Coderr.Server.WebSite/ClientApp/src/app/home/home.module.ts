import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { LatestErrorsComponent } from './latest-errors/latest-errors.component';
import { HomeComponent } from './home.component';
import { PipeModule } from "../../pipes/pipe.module";
import { ControlsModule } from "../_controls/controls.module";
import { ApplicationModule } from "../applications/application.module";
import { ReactiveFormsModule } from '@angular/forms';
import { IncidentsModule } from "../incidents/incidents.module";
import { AuthorizeGuard } from "../../api-authorization/authorize.guard";
import { AdminModule } from "../admin/admin.module";
import { GroupModule } from "../admin/groups/group.module";
import { DemoErrorsComponent } from './demo-errors/demo-errors.component';
import { TipsComponent } from './tips/tips.component';

const incidentsRoutes: Routes = [
  { path: '', pathMatch: 'full', component: HomeComponent, canActivate: [AuthorizeGuard] },
  { path: 'demo-errors', component: DemoErrorsComponent },
  { path: 'tips', component: TipsComponent }
];


@NgModule({
  declarations: [LatestErrorsComponent, HomeComponent, DemoErrorsComponent, TipsComponent],
  imports: [
    ControlsModule,
    AdminModule,
    ApplicationModule,
    CommonModule,
    PipeModule,
    GroupModule,
    RouterModule.forChild(incidentsRoutes),
    ReactiveFormsModule,
    IncidentsModule
  ],
  exports: [

  ]
})
export class HomeModule { }
