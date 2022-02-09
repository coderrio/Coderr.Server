import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SummaryChartComponent as SummarychartComponent } from './summary.component';

describe('SummaryChartComponent', () => {
  let component: SummarychartComponent;
  let fixture: ComponentFixture<SummarychartComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SummarychartComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SummarychartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
