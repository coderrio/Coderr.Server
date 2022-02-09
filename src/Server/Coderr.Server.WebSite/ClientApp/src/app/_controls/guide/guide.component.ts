import { Component, ViewEncapsulation, ElementRef, Input, OnInit, OnDestroy, ViewChild, AfterViewInit } from '@angular/core';

import { GuideService } from './guide.service';

@Component({
  selector: 'guide',
  templateUrl: 'guide.component.html',
  styleUrls: ['guide.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class GuideComponent implements OnInit, OnDestroy, AfterViewInit {
  @Input() name: string;
  private element: HTMLElement;

  constructor(private service: GuideService, private elemRef: ElementRef) {
  }

  @ViewChild('child') textElement: ElementRef;

  ngOnInit(): void {
  }

  ngAfterViewInit() {
    if (!this.name) {
      throw new Error('Guide must have a name.');
    }

    // to access the guideContent element.
    this.element = this.elemRef.nativeElement.children[0];

    if (this.element.children.length === 0) {
      console.log(this.name + " does not have any children");
    }

    this.service.register(this);
  }

  ngOnDestroy(): void {
    this.service.unregister(this);
  }

  get title(): string {
    if (this.element.children.length > 1) {
      return this.element.children[0].textContent;
    }
    return 'Guide';
  }

  get body(): HTMLElement {
    if (this.element.children.length === 0) {
      throw new Error("Expected " + this.name + " do have child content.");
    }

    return <HTMLElement>this.element.children[1];
  }
}
