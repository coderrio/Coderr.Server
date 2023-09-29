import { TestBed } from '@angular/core/testing';

import { ApiKeyService } from './api-keys.service';

describe('ApiKeysServiceService', () => {
  let service: ApiKeyService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ApiKeyService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
