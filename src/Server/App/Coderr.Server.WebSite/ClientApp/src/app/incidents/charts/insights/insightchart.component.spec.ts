import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InsightChartComponent } from './insightchart.component';

describe('InsightChartComponent', () => {
  let component: InsightChartComponent;
  let fixture: ComponentFixture<InsightChartComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [InsightChartComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(InsightChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
