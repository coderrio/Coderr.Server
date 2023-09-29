import { TestBed } from '@angular/core/testing';

import { ApplicationService as ApplicationsService } from './application.service';

describe('ApplicationService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: ApplicationsService = TestBed.get(ApplicationsService);
    expect(service).toBeTruthy();
  });
});
