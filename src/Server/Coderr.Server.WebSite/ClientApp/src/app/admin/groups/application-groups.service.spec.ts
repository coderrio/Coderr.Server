import { TestBed } from '@angular/core/testing';

import { ApplicationGroupService as ApplicationGroupsService } from './application-groups.service';

describe('ApplicationGroupService', () => {
  let service: ApplicationGroupsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ApplicationGroupsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
