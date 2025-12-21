import { TestBed } from '@angular/core/testing';

import { TipsApi } from './tips-api';

describe('TipsApi', () => {
  let service: TipsApi;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TipsApi);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
