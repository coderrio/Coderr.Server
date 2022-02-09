import { Injectable } from '@angular/core';
import { ApiClient } from "../utils/HttpClient";
import * as api from "../../server-api/Core/Reports";

export class ReportSummary {
  public createdAtUtc: string;
  public id: number;
  public message: string;
  public remoteAddress: string;
}

export class Report {
  public contextCollections: ReportCollection[];
  public createdAtUtc: string;
  public emailAddress: string;
  public errorId: string;
  public exception: ReportException;
  public id: string;
  public incidentId: string;
  public message: string;
  public stackTrace: string;
  public userFeedback: string;
}

export class ReportCollection {
  public name: string;
  public properties: KeyValuePair[];
}
export class KeyValuePair {
  public key: string;
  public value: string;
}

export class ReportException {
  public assemblyName: string;
  public baseClasses: string[];
  public fullName: string;
  public innerException: ReportException;
  public message: string;
  public name: string;
  public namespace: string;
  public stackTrace: string;
}

@Injectable({
  providedIn: 'root'
})
export class ReportService {

  constructor(private readonly apiClient: ApiClient) { }

  async getReport(id: number): Promise<Report> {
    const query = new api.GetReport();
    query.reportId = id;

    const dto = await this.apiClient.query<api.GetReportResult>(query);
    const entity = new Report();
    entity.id = dto.id;
    entity.createdAtUtc = dto.createdAtUtc;
    entity.contextCollections = this.convertCollections(dto.contextCollections);
    entity.emailAddress = dto.emailAddress;
    entity.errorId = dto.errorId;
    entity.exception = this.convertException(dto.exception);
    entity.incidentId = dto.incidentId;
    entity.message = dto.message;
    entity.stackTrace = dto.stackTrace;
    entity.userFeedback = dto.userFeedback;
    return entity;
  }

  async getReportList(incidentId: number, pageNumber = 1, pageSize = 20): Promise<ReportSummary[]> {
    const query = new api.GetReportList();
    query.incidentId = incidentId;
    query.pageNumber = pageNumber;
    query.pageSize = pageSize;

    const dto = await this.apiClient.query<api.GetReportListResult>(query);

    return dto.items.map(dtoItem => {
      var item = new ReportSummary();
      item.id = dtoItem.id;
      item.createdAtUtc = dtoItem.createdAtUtc;
      item.message = dtoItem.message;
      item.remoteAddress = dtoItem.remoteAddress;
      return item;
    });
  }

  private convertCollections(dto: api.GetReportResultContextCollection[]): ReportCollection[] {
    return dto.map(value => {
      var col = new ReportCollection();
      col.name = value.name;
      col.properties = value.properties.map(prop => {
        var p = new KeyValuePair();
        p.key = prop.key;
        p.value = prop.value;
        return p;
      });
      return col;
    });
  }
  private convertException(dto: api.GetReportException): ReportException {
    var col = new ReportException();
    col.name = dto.name;
    col.message = dto.message;
    col.assemblyName = dto.assemblyName;
    col.fullName = dto.fullName;
    if (dto.innerException != null) {
      col.innerException = this.convertException(dto.innerException);
    }

    col.namespace = dto.namespace;

    return col;
  }
}
