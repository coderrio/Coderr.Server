import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GroupEditComponent } from './edit.component';

describe('EditComponent', () => {
  let component: GroupEditComponent;
  let fixture: ComponentFixture<GroupEditComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [GroupEditComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GroupEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
