import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DemoErrorsComponent } from './demo-errors.component';

describe('DemoErrorsComponent', () => {
  let component: DemoErrorsComponent;
  let fixture: ComponentFixture<DemoErrorsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DemoErrorsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DemoErrorsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
