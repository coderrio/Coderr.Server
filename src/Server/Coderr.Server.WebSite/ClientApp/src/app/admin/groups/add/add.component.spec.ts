import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GroupAddComponent } from './add.component';

describe('AddComponent', () => {
  let component: GroupAddComponent;
  let fixture: ComponentFixture<GroupAddComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [GroupAddComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GroupAddComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
