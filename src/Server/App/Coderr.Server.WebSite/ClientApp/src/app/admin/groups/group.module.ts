import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ControlsModule } from "../../_controls/controls.module";

import { GroupAddComponent } from './add/add.component';
import { GroupEditComponent } from './edit/edit.component';
import { GroupDetailsComponent } from './details/details.component';
import { GroupListComponent } from './list/list.component';

export const groupRoutes: Routes = [
  { path: 'groups/new', component: GroupAddComponent },
  { path: 'groups', component: GroupListComponent },
  { path: 'groups/:id', component: GroupDetailsComponent },
  { path: 'groups/:id/edit', component: GroupEditComponent }
];


@NgModule({
  declarations: [GroupAddComponent, GroupListComponent, GroupDetailsComponent, GroupEditComponent],
  imports: [
    ControlsModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule
  ],
  exports: [
    GroupAddComponent,
    GroupListComponent
  ]
})
export class GroupModule { }
