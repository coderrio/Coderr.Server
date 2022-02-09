import { WhitelistEntry } from './whitelist.model';

describe('Whitelist', () => {
  it('should create an instance', () => {
    expect(new WhitelistEntry('arnwe', 1)).toBeTruthy();
  });
});
