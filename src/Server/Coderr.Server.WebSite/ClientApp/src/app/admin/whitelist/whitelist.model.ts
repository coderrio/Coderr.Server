import { IApplication } from "../../applications/application.model";

export interface IWhitelistApp extends IApplicationListItem {
  selected: boolean;
}

export interface IApplicationListItem {
  id: number;
  name: string;
}


export interface IIpAddress {
  id: number;
  address: string;
  type: IpType;
}

export enum IpType {
  Lookup = 0,
  Manual = 1,
  Rejected = 2
}

export class WhitelistEntry {
  constructor(public domainName: string, public id?: number) {

  }

  applications: IApplicationListItem[] = [];
  ipAddresses: IIpAddress[] = [];

  /**
   * Create a list with all apps and make ours selected.
   * @param apps All applications that the user has permissions to.
   */
  mapAllApps(apps: IApplication[]): IWhitelistApp[] {
    if (!apps) {
      throw new Error("apps are required.");
    }

    var selectedIds = this.applications.map(x => x.id);
    return apps.map(x => {
      return {
        id: x.id,
        name: x.name,
        selected: selectedIds.indexOf(x.id) >= 0
      };
    });
  }
}
