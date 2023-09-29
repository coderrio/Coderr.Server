import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SearchComponent } from './search/search.component';
import { DetailsComponent } from './details/details.component';
import { RouterModule, Routes } from '@angular/router';
import { ReportsComponent } from './details/reports/reports.component';
import { BugReportsComponent } from './details/bugreports/bugreports.component';
import { OriginsComponent } from './details/origins/origins.component';
import { LogsComponent } from './details/logs/logs.component';
import { ImpactComponent } from './details/impact/impact.component';
import { HomeComponent } from './details/home/home.component';
import { PipeModule } from "../../pipes/pipe.module";
import { RecommendComponent } from './recommend/recommend.component';
import { IncidentStubsComponent } from "./stubs/stubs.component";
import { FeedbackComponent } from './feedback/feedback.component';
import { InsightChartComponent } from "./charts/insights/insightchart.component";
import { CloseComponent } from './details/close/close.component';
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { ControlsModule } from "../_controls/controls.module";
import { NavbarComponent } from './details/navbar/navbar.component';

const incidentsRoutes: Routes = [
  { path: 'application/:applicationId/errors', component: SearchComponent },
  {
    path: 'application/:applicationid/error/:incidentId',
    component: DetailsComponent,
    children: [
      {
        path: '',
        component: HomeComponent
      },
      {
        path: 'home',
        component: HomeComponent
      },
      {
        path: 'impact',
        component: ImpactComponent
      },
      {
        path: 'origins',
        component: OriginsComponent
      },
      {
        path: 'close',
        component: CloseComponent
      },
      {
        path: 'logs',
        component: LogsComponent
      },
      {
        path: 'users',
        component: BugReportsComponent
      }
    ]
  }
];


@NgModule({
  declarations: [
    IncidentStubsComponent,
    SearchComponent,
    DetailsComponent,
    ReportsComponent,
    BugReportsComponent,
    OriginsComponent,
    LogsComponent,
    ImpactComponent,
    HomeComponent,
    RecommendComponent,
    FeedbackComponent,
    InsightChartComponent,
    CloseComponent,
    NavbarComponent
  ],
  imports: [
    PipeModule,
    CommonModule,
    ControlsModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forChild(incidentsRoutes)
  ],
  exports: [
    RouterModule,
    IncidentStubsComponent,
    InsightChartComponent,
    CloseComponent
  ]
})
export class IncidentsModule { }
