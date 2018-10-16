export class TagDTO
{
    public static TYPE_NAME: string = 'TagDTO';
    public Name: string;
    public OrderNumber: number;
}
export class GetTagsForApplication
{
    public static TYPE_NAME: string = 'GetTagsForApplication';
    public ApplicationId: number;
}
export class GetTags
{
    public static TYPE_NAME: string = 'GetTags';
    public ApplicationId?: number;
    public IncidentId?: number;
}
export class GetTagsForIncident
{
    public static TYPE_NAME: string = 'GetTagsForIncident';
    public IncidentId: number;
}
export class TagAttachedToIncident
{
    public static TYPE_NAME: string = 'TagAttachedToIncident';
    public IncidentId: number;
    public Tags: TagDTO[];
}
