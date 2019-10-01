// ReSharper disable InconsistentNaming

export class GetWhitelistEntries
{
    public static TYPE_NAME: string = 'GetWhitelistEntries';
    public DomainName: string;
    public ApplicationId: number|null;
}
export class GetWhitelistEntriesResult
{
    public Entries: GetWhitelistEntriesResultItem[];
}
export class GetWhitelistEntriesResultItem
{
    public Id: number;
    public ApplicationId: number|null;
    public DomainName: string;
}
export class AddDomain
{
    public static TYPE_NAME: string = 'AddDomain';
    public ApplicationId: number | null;
    public DomainName: string;
}
export class RemoveEntry
{
    public static TYPE_NAME: string = 'RemoveEntry';
    public Id: Number;
}
