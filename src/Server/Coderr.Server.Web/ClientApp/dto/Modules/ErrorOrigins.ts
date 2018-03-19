export class GetOriginsForIncident
{
    public static TYPE_NAME: string = 'GetOriginsForIncident';
    public IncidentId: number;
}
export class GetOriginsForIncidentResult
{
    public static TYPE_NAME: string = 'GetOriginsForIncidentResult';
    public Items: GetOriginsForIncidentResultItem[];
}
export class GetOriginsForIncidentResultItem
{
    public static TYPE_NAME: string = 'GetOriginsForIncidentResultItem';
    public Latitude: number;
    public Longitude: number;
    public NumberOfErrorReports: number;
}
