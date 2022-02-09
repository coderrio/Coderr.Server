import { TestBed } from '@angular/core/testing';

import { WorkItemService } from './work-item.service';

describe('WorkItemServiceService', () => {
  let service: WorkItemService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(WorkItemService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
