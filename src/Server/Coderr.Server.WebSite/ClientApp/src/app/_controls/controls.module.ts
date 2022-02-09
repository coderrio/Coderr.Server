import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ModalComponent } from './modal/modal.component';
import { GuideComponent } from "./guide/guide.component";

@NgModule({
    imports: [CommonModule],
    declarations: [ModalComponent, GuideComponent],
    exports: [ModalComponent, GuideComponent]
})
export class ControlsModule { }
