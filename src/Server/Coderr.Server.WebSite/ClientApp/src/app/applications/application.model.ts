import { Observable } from 'rxjs';

export class Roles {
  static Admin = "Admin";
  static Member = "Member";
}

export interface IApplicationListItem {
  readonly id: number;
  readonly name: string;
}

export interface IApplication extends IApplicationListItem {
  readonly members: IApplicationMember[];
  readonly groupIds: number[];
  readonly totalIncidentCount: number;
  readonly latestIncidentDate?: Date;
  readonly versions: string[];
  readonly sharedSecret: string;
  readonly appKey: string;
}

export class EmptyApplication implements IApplication {
  id: number = 0;
  name = "";
  members = [];
  groupIds = [1];
  totalIncidentCount = 0;
  versions = [];
  appKey = '';
  sharedSecret = '';
}

export interface IApplicationMember {
  /**
   * -1 for invited that do not have an account
   */
  readonly accountId: number;
  readonly userName: string;
  readonly isAdmin: boolean;
  readonly isInvited: boolean;
}

export interface IApplicationSummary {
  readonly applicationId: number;
  readonly name: string;
}

export class Guid {
  static newGuid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
      const r = Math.random() * 16 | 0,
        v = c === 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
    });
  }
}
