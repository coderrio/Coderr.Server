import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ModalService {
  private modals: any[] = [];

  /**
   * Invoked by the component itself and should not be called directly.
   * @param modalComponent Component to add to the list
   */
  add(modalComponent: any) {
    this.modals.push(modalComponent);
  }

  /**
   * Invoked by the component itself and should not be called directly.
   * @param modalComponent Component to remove from the list
   */
  remove(id: string) {
    this.modals = this.modals.filter(x => x.id !== id);
  }

  /**
   * Open a modal component
   * @param id id of the modal
   */
  open(id: string) {
    const modal = this.modals.find(x => x.id === id);
    modal.open();
  }

  /**
   * Close a modal
   * @param id id of the modal.
   */
  close(id: string) {
    const modal = this.modals.find(x => x.id === id);
    modal.close();
  }
}
