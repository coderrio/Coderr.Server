import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AppInsightsDetailsComponent } from './details.component';

describe('DetailsComponent', () => {
  let component: AppInsightsDetailsComponent;
  let fixture: ComponentFixture<AppInsightsDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AppInsightsDetailsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AppInsightsDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
