import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PartitionListComponent } from './list/list.component';
import { PartitionEditComponent } from './edit/edit.component';
import { PartitionNewComponent } from './new/new.component';



@NgModule({
  declarations: [PartitionListComponent, PartitionEditComponent, PartitionNewComponent],
  imports: [
    CommonModule
  ]
})
export class PartitionsModule { }
