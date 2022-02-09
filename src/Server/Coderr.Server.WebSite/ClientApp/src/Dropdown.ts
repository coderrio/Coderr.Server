export class Dropdown {
  private menu: HTMLElement;
  private menuTrigger: HTMLElement;
  private visible = false;

  constructor(selectorOrElement: string | HTMLElement) {
    if (typeof selectorOrElement === "string") {
      this.menu = document.querySelector(selectorOrElement);
    } else if (selectorOrElement) {
      this.menu = <HTMLElement>selectorOrElement;
    } else {
      throw new Error("Not an element nor selector.");
    }

    window.addEventListener('resize', () => {
      this.reposition();
    });
    window.addEventListener('click', e => {
      if (e.target === this.menuTrigger) {
        return;
      }
      if (!this.isDescendant(this.menu, e.target)) {
        this.hide();
      }
    });
  }

  bindClick(selectorOrElement: string | HTMLElement) {
    if (typeof selectorOrElement === "string") {
      this.menuTrigger = document.querySelector(selectorOrElement);
    } else if (selectorOrElement) {
      this.menuTrigger = <HTMLElement>selectorOrElement;
    } else {
      throw new Error("Not an element nor selector.");
    }

    this.menuTrigger.addEventListener('click', e => {
      e.preventDefault();
      if (this.visible) {
        this.hide();
      } else {
        this.show();
      }
      this.visible = !this.visible;
    });
  }

  hide() {
    this.menu.classList.remove('shown');
  }
  show() {
    this.reposition();
    this.menu.classList.add('shown');
  }

  reposition() {
    var triggerRect = this.menuTrigger.getBoundingClientRect();
    var menuRect = this.menu.getBoundingClientRect();
    if (triggerRect.left + menuRect.width + 10 > window.innerWidth) {
      this.menu.style.left = (triggerRect.right - menuRect.width) + "px";
      this.menu.style.top = triggerRect.bottom + 5 + "px";
    } else {
      this.menu.style.left = triggerRect.left + "px";
      this.menu.style.top = triggerRect.bottom + 5 + "px";
    }
  }


  private isDescendant(parent, child) {
    let node = child.parentNode;
    while (node != null) {
      if (node === parent) {
        return true;
      }
      node = node.parentNode;
    }
    return false;
  }
}
