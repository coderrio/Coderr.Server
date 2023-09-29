// ReSharper disable InconsistentNaming

export class GetWhitelistEntries {
  public static TYPE_NAME: string = 'GetWhitelistEntries';
  public domainName: string;
  public applicationId: number | null;
}
export class GetWhitelistEntriesResult {
  public entries: GetWhitelistEntriesResultItem[];
}
export class GetWhitelistEntriesResultItem {
  public id: number;
  public applications: GetWhiteListEntriesResultApp[];
  public domainName: string;
  public ipAddresses: GetWhiteListEntriesResultIp[];
}
export class GetWhiteListEntriesResultApp {
  public applicationId: number;
  public name: string;
}
export class GetWhiteListEntriesResultIp {
  public id: number;
  public address: string;
  public type: IpType;
}
export enum IpType {
  Lookup = 0,
  Manual = 1,
  Denied = 2
}
export class DomainIpAddress {
  public id: number;
  public value: string;
  public ipType: IpType;
}
export class AddEntry {
  public static TYPE_NAME: string = 'AddEntry';
  public applicationIds: number[] | null;
  public ipAddresses: string[] | null;
  public domainName: string;
}
export class RemoveEntry {
  public static TYPE_NAME: string = 'RemoveEntry';
  public id: Number;
}
export class EditEntry {
  public static TYPE_NAME: string = 'EditEntry';
  public domainName: string;
  public id: Number;
  public applicationIds: number[] | null;

  // Contains only manually specified ip addresses.
  // denied and lookup addresses do not appear
  public ipAddresses: string[] | null;
}
