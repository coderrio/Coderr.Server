import { TestBed } from '@angular/core/testing';

import { WhitelistService } from './whitelist.service';

describe('WhitelistService', () => {
  let service: WhitelistService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(WhitelistService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
