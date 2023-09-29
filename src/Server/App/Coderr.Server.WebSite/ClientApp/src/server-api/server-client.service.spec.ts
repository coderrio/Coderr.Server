import { TestBed } from '@angular/core/testing';

import { ServerClientService } from './server-client.service';

describe('ServerClientService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: ServerClientService = TestBed.get(ServerClientService);
    expect(service).toBeTruthy();
  });
});
