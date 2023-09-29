import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BugReportsComponent } from './bugreports.component';

describe('BugreportsComponent', () => {
  let component: BugReportsComponent;
  let fixture: ComponentFixture<BugReportsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [BugReportsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BugReportsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
