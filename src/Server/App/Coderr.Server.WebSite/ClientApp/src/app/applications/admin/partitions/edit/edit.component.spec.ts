import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PartitionEditComponent } from './edit.component';

describe('EditComponent', () => {
  let component: PartitionEditComponent;
  let fixture: ComponentFixture<PartitionEditComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PartitionEditComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PartitionEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
