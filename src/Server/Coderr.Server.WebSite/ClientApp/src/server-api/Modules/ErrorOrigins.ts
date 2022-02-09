export class GetOriginsForIncident {
  public static TYPE_NAME: string = 'GetOriginsForIncident';
  public incidentId: number;
}
export class GetOriginsForIncidentResult {
  public static TYPE_NAME: string = 'GetOriginsForIncidentResult';
  public items: GetOriginsForIncidentResultItem[];
}
export class GetOriginsForIncidentResultItem {
  public static TYPE_NAME: string = 'GetOriginsForIncidentResultItem';
  public latitude: number;
  public longitude: number;
  public numberOfErrorReports: number;
}
