import { Component, ViewChild, ViewContainerRef, TemplateRef } from "@angular/core";


@Component({
  selector: 'showmodal',
  template: `
        <ng-container #vc></ng-container>
        <ng-template #modal_1>
            <div class="modal" tabindex="-1" role="dialog" aria-hidden="true">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header"> <button type="button" class="close" data-dismiss="modal" aria-hidden="true"> &times; </button>
                            <h4 class="modal-title">
                            </h4>
                        </div>
                        <div class="modal-body"></div>
                        <div class="modal-footer"> 
                            <button type="button" class="btn btn-default" data-dismiss="modal" (click)="closeDialog()">Close </button> 
                        </div>
                    </div>
                </div>
            </div>   
        </ng-template>
    `
})
export class ShowModalComponent {
  @ViewChild('modal_1') modal: TemplateRef<any>;
  @ViewChild('vc', { read: ViewContainerRef }) vc: ViewContainerRef;
  backdrop: any
  showDialog() {
    let view = this.modal.createEmbeddedView(null);
    this.vc.insert(view);
    this.modal.elementRef.nativeElement.previousElementSibling.classList.remove('fade');
    this.modal.elementRef.nativeElement.previousElementSibling.classList.add('modal-open');
    this.modal.elementRef.nativeElement.previousElementSibling.style.display = 'block';
    this.backdrop = document.createElement('DIV');
    this.backdrop.className = 'modal-backdrop';
    document.body.appendChild(this.backdrop);
  }

  closeDialog() {
    this.vc.clear();
    document.body.removeChild(this.backdrop);
  }
}
