import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AppInsightsDashboardComponent } from './dashboard.component';

describe('DashboardComponent', () => {
  let component: AppInsightsDashboardComponent;
  let fixture: ComponentFixture<AppInsightsDashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AppInsightsDashboardComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AppInsightsDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
