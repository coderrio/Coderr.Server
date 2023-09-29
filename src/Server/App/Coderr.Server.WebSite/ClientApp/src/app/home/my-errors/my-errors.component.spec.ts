import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MyErrorsComponent } from './my-errors.component';

describe('MyErrorsComponent', () => {
  let component: MyErrorsComponent;
  let fixture: ComponentFixture<MyErrorsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MyErrorsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MyErrorsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
