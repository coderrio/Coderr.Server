import { TestBed } from '@angular/core/testing';

import { PartitionService } from './partition.service';

describe('PartitionService', () => {
  let service: PartitionService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PartitionService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
