import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LatestErrorsComponent } from './latest-errors.component';

describe('LatestErrorsComponent', () => {
  let component: LatestErrorsComponent;
  let fixture: ComponentFixture<LatestErrorsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LatestErrorsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LatestErrorsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
