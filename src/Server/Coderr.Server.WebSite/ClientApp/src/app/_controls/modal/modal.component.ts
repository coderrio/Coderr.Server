import { Component, ViewEncapsulation, ElementRef, Input, OnInit, OnDestroy } from '@angular/core';

import { ModalService } from './modal.service';

@Component({
  selector: 'modal',
  templateUrl: 'modal.component.html',
  styleUrls: ['modal.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class ModalComponent implements OnInit, OnDestroy {
  @Input() id: string;
  private element: any;
  static activeIndices: number[] = [];
  overlayIndex = 0;
  modalIndex = 0;

  constructor(private modalService: ModalService, el: ElementRef) {
    this.element = el.nativeElement;

    var max = 500;
    ModalComponent.activeIndices.forEach(x => {
      if (x > max) {
        max = x;
      }
    });
    this.overlayIndex = max + 1;
    this.modalIndex = max + 2;
    ModalComponent.activeIndices.push(this.modalIndex);
  }

  ngOnInit(): void {
    if (!this.id) {
      throw new Error('modal must have an id');
    }

    document.body.appendChild(this.element);
    this.element.addEventListener('click', el => {
      if (el.target.className === 'modal') {
        this.close();
      }
    });

    this.modalService.add(this);
  }

  ngOnDestroy(): void {
    this.modalService.remove(this.id);
    this.element.remove();
  }

  open(): void {
    this.element.style.display = 'block';
    document.body.classList.add('modal-open');
  }

  // close modal
  close(): void {
    this.element.style.display = 'none';
    document.body.classList.remove('modal-open');

    ModalComponent.activeIndices = ModalComponent.activeIndices.filter(x => x !== this.overlayIndex);
  }
}
