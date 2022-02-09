import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ApplicationHomeComponent } from './home.component';

describe('HomeComponent', () => {
  let component: ApplicationHomeComponent;
  let fixture: ComponentFixture<ApplicationHomeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ApplicationHomeComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ApplicationHomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
