import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AgoPipe } from './ago.pipe';
import { IsoDatePipe } from './iso-date.pipe';

@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [
    AgoPipe,
    IsoDatePipe
  ],
  exports: [
    AgoPipe,
    IsoDatePipe
  ]
})
export class PipeModule { }
