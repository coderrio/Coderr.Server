import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OriginsComponent } from './origins.component';

describe('OriginsComponent', () => {
  let component: OriginsComponent;
  let fixture: ComponentFixture<OriginsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ OriginsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(OriginsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
