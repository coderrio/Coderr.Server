import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

import { NewComponent } from './new/new.component';
import { EditComponent } from './edit/edit.component';
import { EnvironmentListComponent } from './list/list.component';

const routes: Routes = [
  { path: 'application/:applicationId/admin/environments/:id/edit', component: EditComponent },
  { path: 'application/:applicationId/admin/environments/new', component: NewComponent }
];


@NgModule({
  declarations: [
    NewComponent,
    EditComponent,
    EnvironmentListComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    RouterModule.forChild(routes)
  ],
  exports: [
    EnvironmentListComponent
  ]
})
export class EnvironmentsModule { }
