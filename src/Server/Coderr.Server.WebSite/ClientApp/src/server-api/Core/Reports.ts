export class ContextCollectionDTO {
  public static TYPE_NAME: string = 'ContextCollectionDTO';
  public name: string;
  public properties: string[];
}
export class ReportDTO {
  public static TYPE_NAME: string = 'ReportDTO';
  public applicationId: number;
  public contextCollections: ContextCollectionDTO[];
  public createdAtUtc: Date;
  public exception: ReportExeptionDTO;
  public id: number;
  public incidentId: number;
  public remoteAddress: string;
  public reportId: string;
  public reportVersion: string;
}
export class ReportExeptionDTO {
  public static TYPE_NAME: string = 'ReportExeptionDTO';
  public assemblyName: string;
  public baseClasses: string[];
  public everything: string;
  public fullName: string;
  public innerException: ReportExeptionDTO;
  public message: string;
  public name: string;
  public namespace: string;
  public properties: string[];
  public stackTrace: string;
}
export class GetReport {
  public static TYPE_NAME: string = 'GetReport';
  public reportId: number;
}
export class GetReportException {
  public static TYPE_NAME: string = 'GetReportException';
  public assemblyName: string;
  public baseClasses: string[];
  public everything: string;
  public fullName: string;
  public innerException: GetReportException;
  public message: string;
  public name: string;
  public namespace: string;
  public stackTrace: string;
}
export class GetReportList {
  public static TYPE_NAME: string = 'GetReportList';
  public incidentId: number;
  public pageNumber: number;
  public pageSize: number;
}
export class GetReportListResult {
  public static TYPE_NAME: string = 'GetReportListResult';
  public items: GetReportListResultItem[];
  public pageNumber: number;
  public pageSize: number;
  public totalCount: number;
}
export class GetReportListResultItem {
  public static TYPE_NAME: string = 'GetReportListResultItem';
  public createdAtUtc: string;
  public id: number;
  public message: string;
  public remoteAddress: string;
}
export class GetReportResult {
  public static TYPE_NAME: string = 'GetReportResult';
  public contextCollections: GetReportResultContextCollection[];
  public createdAtUtc: string;
  public emailAddress: string;
  public errorId: string;
  public exception: GetReportException;
  public id: string;
  public incidentId: string;
  public message: string;
  public stackTrace: string;
  public userFeedback: string;
}
export class GetReportResultContextCollection {
  public static TYPE_NAME: string = 'GetReportResultContextCollection';
  public name: string;
  public properties: KeyValuePair[];
}
export class KeyValuePair {
  public static TYPE_NAME: string = 'KeyValuePair';
  public key: string;
  public value: string;
}
