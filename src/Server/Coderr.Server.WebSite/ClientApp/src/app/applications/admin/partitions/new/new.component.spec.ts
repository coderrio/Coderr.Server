import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PartitionNewComponent } from './new.component';

describe('NewComponent', () => {
  let component: PartitionNewComponent;
  let fixture: ComponentFixture<PartitionNewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PartitionNewComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PartitionNewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
