import { Group } from './group.model';

describe('Group', () => {
  it('should create an instance', () => {
    expect(new Group(1, 'lala')).toBeTruthy();
  });
});
