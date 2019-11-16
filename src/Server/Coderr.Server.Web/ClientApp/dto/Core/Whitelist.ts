// ReSharper disable InconsistentNaming

export class GetWhitelistEntries {
    public static TYPE_NAME: string = 'GetWhitelistEntries';
    public DomainName: string;
    public ApplicationId: number | null;
}
export class GetWhitelistEntriesResult {
    public Entries: GetWhitelistEntriesResultItem[];
}
export class GetWhitelistEntriesResultItem {
    public Id: number;
    public Applications: GetWhiteListEntriesResultApp[];
    public DomainName: string;
    public IpAddresses: GetWhiteListEntriesResultIp[];
}
export class GetWhiteListEntriesResultApp {
    public ApplicationId: number;
    public Name: string;
}
export class GetWhiteListEntriesResultIp {
    public Id: number;
    public Address: string;
    public Type: IpType;
}
export enum IpType {
    Lookup,
    Manual,
    Denied
}
export class DomainIpAddress {
    public Id: number;
    public Value: string;
    public IpType: IpType;
}
export class AddEntry {
    public static TYPE_NAME: string = 'AddEntry';
    public ApplicationIds: number[] | null;
    public IpAddresses: string[] | null;
    public DomainName: string;
}
export class RemoveEntry {
    public static TYPE_NAME: string = 'RemoveEntry';
    public Id: Number;
}
export class EditEntry {
    public static TYPE_NAME: string = 'EditEntry';
    public DomainName: string;
    public Id: Number;
    public ApplicationIds: number[] | null;

    // Contains only manually specified ip addresses.
    // denied and lookup addresses do not appear
    public IpAddresses: string[] | null;
}
