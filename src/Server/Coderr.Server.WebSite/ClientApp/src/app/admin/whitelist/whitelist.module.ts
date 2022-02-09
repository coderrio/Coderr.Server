import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ControlsModule } from "../../_controls/controls.module";

import { AddComponent } from './add/add.component';
import { EditComponent } from './edit/edit.component';
import { DetailsComponent } from './details/details.component';
import { ListComponent } from './list/list.component';

export const whitelistRoutes: Routes = [
  { path: 'whitelists/new', component: AddComponent },
  { path: 'whitelists', component: ListComponent },
  { path: 'whitelists/:id', component: DetailsComponent },
  { path: 'whitelists/:id/edit', component: EditComponent }
];


@NgModule({
  declarations: [AddComponent, EditComponent, DetailsComponent, ListComponent],
  imports: [
    CommonModule,
    FormsModule,
    ControlsModule,
    RouterModule
  ]
})
export class WhitelistModule { }
