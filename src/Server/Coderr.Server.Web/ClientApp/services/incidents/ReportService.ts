import { ApiClient } from '../ApiClient';
import { AppRoot } from '../AppRoot';
import {
    GetReport, GetReportResult, GetReportResultContextCollection,
    GetReportList, GetReportListResult, GetReportListResultItem
} from "../../dto/Core/Reports";

export class IncidentService {
    // Cache of the 10 last fetched incidents.
    // Rolling access.
    private cachedReports: GetReportResult[] = [];

    reports: GetReportListResultItem[] = [];

    constructor(private apiClient: ApiClient) {

    }

    async get(id: number): Promise<GetReportResult> {
        if (id === 0) {
            throw new Error("Expected a reportId");
        }

        var cached = this.getFromCache(id);
        if (cached) {
            return cached;
        }

        var q = new GetReport();
        q.ReportId = id;
        var result = await this.apiClient.query<GetReportResult>(q);
        this.pushToCache(result);
        return result;
    }

    private getList(incidentId: number) {
        var q = new GetReportList();
        q.IncidentId = incidentId;
        this.apiClient.query<GetReportListResult>(q)
            .then(x => {
                this.reports = x.Items;
            });
    }

    private getFromCache(id: number): GetReportResult | null {
        for (var i = 0; i < this.cachedReports.length; i++) {
            if (this.cachedReports[i].Id === id.toString())
                return this.cachedReports[i];
        }

        return null;
    }

    private pushToCache(incident: GetReportResult) {
        if (this.cachedReports.length === 10) {
            this.cachedReports.shift();
        }

        this.cachedReports.push(incident);
    }
}
