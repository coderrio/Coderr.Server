import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IncidentStubsComponent } from './stubs.component';

describe('IncidentStubsComponent', () => {
  let component: IncidentStubsComponent;
  let fixture: ComponentFixture<IncidentStubsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [IncidentStubsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(IncidentStubsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
