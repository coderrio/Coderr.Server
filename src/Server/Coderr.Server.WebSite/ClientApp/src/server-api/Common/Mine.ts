export class ListMyIncidents {
  public static TYPE_NAME: string = 'ListMyIncidents';
  public applicationId: number | null;
}
export class ListMyIncidentsResult {
  public static TYPE_NAME: string = 'ListMyIncidentsResult';
  public comment: string;
  public items: ListMyIncidentsResultItem[];
  public suggestions: ListMySuggestedItem[];
}
export class ListMyIncidentsResultItem {
  public static TYPE_NAME: string = 'ListMyIncidentsResultItem';
  public applicationId: number;
  public applicationName: string;
  public assignedAtUtc: Date;
  public createdAtUtc: Date;
  public id: number;
  public lastReportAtUtc: Date;
  public name: string;
  public reportCount: number;
}
export class ListMySuggestedItem {
  public static TYPE_NAME: string = 'ListMySuggestedItem';
  public applicationId: number;
  public applicationName: string;
  public createdAtUtc: Date;
  public exceptionTypeName: string;
  public id: number;
  public lastReportAtUtc: Date;
  public name: string;
  public weight: number;
  public reportCount: number;
  public stackTrace: string;
  public motivation: string;
}
