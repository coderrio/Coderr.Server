import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";

import { AdminMainComponent } from "./admin-main/admin-main.component";
import { CreateApplicationComponent } from "./app-create/create.component";
import { AdminComponent } from "./admin.component";
import { ApiKeyListComponent } from "./apikeys/list/list.component";
import { EditApiKeyComponent } from "./apikeys/edit/edit.component";
import { CreateApiKeyComponent } from "./apikeys/create/create.component";
import { ApiKeyDetailsComponent } from "./apikeys/details/details.component";
import { AdminNavbarComponent } from "./navbar/navbar.component";

import { ControlsModule } from "../_controls/controls.module";
import { WhitelistModule, whitelistRoutes } from "./whitelist/whitelist.module";
import { GroupModule, groupRoutes } from "./groups/group.module";

var ourRoutes: Routes = [
  {
    path: "admin",
    component: AdminComponent,
    children: [
      { path: "", component: AdminMainComponent },
      { path: "application", component: CreateApplicationComponent },
      { path: "apikeys", component: ApiKeyListComponent },
      { path: "apikeys/new", component: CreateApiKeyComponent },
      { path: "apikeys/:id/edit", component: EditApiKeyComponent },
      { path: "apikeys/:id", component: ApiKeyDetailsComponent },
      { path: "teams", component: ApiKeyListComponent },
      { path: "teams/team", component: CreateApiKeyComponent },
      { path: "teams/team/:id/edit", component: EditApiKeyComponent },
      { path: "teams/team/:id", component: ApiKeyDetailsComponent }
    ]
  }
];
ourRoutes[0].children.push.apply(ourRoutes[0].children, whitelistRoutes);
ourRoutes[0].children.push.apply(ourRoutes[0].children, groupRoutes);

@NgModule({
  declarations: [
    AdminComponent,
    AdminMainComponent,
    CreateApplicationComponent,
    ApiKeyListComponent,
    EditApiKeyComponent,
    CreateApiKeyComponent,
    ApiKeyDetailsComponent,
    AdminNavbarComponent
  ],
  imports: [
    ControlsModule,
    CommonModule,
    FormsModule,
    WhitelistModule,
    GroupModule,
    ReactiveFormsModule,
    RouterModule.forChild(ourRoutes)
  ],
  exports: [
    CreateApplicationComponent,
    RouterModule
  ]
})
export class AdminModule {

}
