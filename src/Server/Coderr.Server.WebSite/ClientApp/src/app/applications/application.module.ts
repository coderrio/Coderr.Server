import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PipeModule } from "../../pipes/pipe.module";

import { EditComponent } from './admin/edit/edit.component';
import { MembersComponent } from './admin/members/members.component';
import { GroupSelectorComponent } from "./groups/selector.component";
import { EnvironmentsModule } from "./admin/environments/environments.module";
import { SummaryChartComponent } from "./charts/summary/summary.component";
import { InsightChartComponent } from "./charts/insights/insightchart.component";
import { AppInsightsDashboardComponent } from './insights/dashboard/dashboard.component';
import { AppInsightsDetailsComponent } from './insights/details/details.component';

import { ConfigureComponent } from './admin/configure/configure.component';

//import { IncidentsModule } from "../incidents/incidents.module";
import { ApplicationDetailsComponent } from "./details/details.component";
import { ApplicationHomeComponent } from "./details/home/home.component";
import { NavbarComponent } from './details/navbar/navbar.component';

import { ControlsModule } from "../_controls/controls.module";
import { IncidentsModule } from "../incidents/incidents.module";
import { MetricComponent } from './charts/metric/metric.component';

import { AdminHomeComponent } from './admin/home/home.component';
import { PartitionListComponent } from "./admin/partitions/list/list.component";
import { PartitionEditComponent } from "./admin/partitions/edit/edit.component";
import { PartitionNewComponent } from "./admin/partitions/new/new.component";

import { NewComponent } from './admin/members/new/new.component';
import { TeamListComponent } from "./admin/members/list/list.component";

const ourRoutes: Routes = [
  {
    path: 'application/:applicationId',
    component: ApplicationDetailsComponent,
    children: [
      {
        path: '',
        outlet: 'application-details-outlet',
        component: ApplicationHomeComponent
      }
    ]
  },
  { path: 'application/:applicationId/configure', component: ConfigureComponent },
  { path: 'application/:applicationId/members', component: MembersComponent },
  { path: 'application/:applicationId/edit', component: EditComponent },
  { path: 'application/:applicationId/insights', component: AppInsightsDashboardComponent },
  { path: 'application/:applicationId/admin', component: AdminHomeComponent },
  { path: 'application/:applicationId/admin/team', component: TeamListComponent },
  { path: 'application/:applicationId/admin/partitions/:id/edit', component: PartitionEditComponent },
  { path: 'application/:applicationId/admin/partitions/new', component: PartitionNewComponent }
];

@NgModule({
  declarations: [
    ConfigureComponent,
    GroupSelectorComponent,
    EditComponent,
    MembersComponent,
    SummaryChartComponent,
    InsightChartComponent,
    ApplicationDetailsComponent,
    ApplicationHomeComponent,
    NavbarComponent,
    AppInsightsDashboardComponent,
    AppInsightsDetailsComponent,
    MetricComponent,
    AdminHomeComponent,
    PartitionListComponent,
    PartitionEditComponent,
    PartitionNewComponent,
    TeamListComponent,
    NewComponent
  ],
  imports: [
    PipeModule,
    CommonModule,
    ControlsModule,
    EnvironmentsModule,
    FormsModule,
    IncidentsModule,
    ReactiveFormsModule,
    RouterModule.forChild(ourRoutes)
  ],
  exports: [
    GroupSelectorComponent,
    SummaryChartComponent,
    InsightChartComponent,
    MembersComponent,
    PartitionListComponent,
    ConfigureComponent
  ]
})
export class ApplicationModule { }
