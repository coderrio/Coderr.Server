export class TagDTO
{
    public static TYPE_NAME: string = 'TagDTO';
    public name: string;
    public orderNumber: number;
}
export class GetTagsForApplication
{
    public static TYPE_NAME: string = 'GetTagsForApplication';
    public aplicationId: number;
}
export class GetTags
{
    public static TYPE_NAME: string = 'GetTags';
    public applicationId?: number;
    public incidentId?: number;
}
export class GetTagsForIncident
{
    public static TYPE_NAME: string = 'GetTagsForIncident';
    public incidentId: number;
}
export class TagAttachedToIncident
{
    public static TYPE_NAME: string = 'TagAttachedToIncident';
    public incidentId: number;
    public tags: TagDTO[];
}
