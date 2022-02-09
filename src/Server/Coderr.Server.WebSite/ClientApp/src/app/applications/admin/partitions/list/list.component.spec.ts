import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PartitionListComponent } from './list.component';

describe('ListComponent', () => {
  let component: PartitionListComponent;
  let fixture: ComponentFixture<PartitionListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PartitionListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PartitionListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
