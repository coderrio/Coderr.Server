import { AppRoot } from '../../../services/AppRoot';
import * as api from "@/dto/Common/Logs";
import { Component, Vue } from "vue-property-decorator";

interface ILogEntry {
    message: string;
    logLevel: number;
    exception: string;
    timeStampUtc: Date;
}

@Component
export default class AnalyzeIncidentLogsComponent extends Vue {

    incidentId: number = 0;
    entries: ILogEntry[] = [];
    allEntries: ILogEntry[] = [];
    filterText = "";

    created() {
        this.incidentId = parseInt(this.$route.params.incidentId, 10);

        var q = new api.GetLogs();
        q.IncidentId = this.incidentId;
        AppRoot.Instance.apiClient.query<api.GetLogsResult>(q)
            .then(result => {
                console.log(result);
                result.Entries.forEach(x => {
                    if (x.Message) {
                        this.allEntries.push({
                            message: this.escapeHtml(x.Message).replace(/\r\n/, "<br>\r\n"),
                            exception: this.escapeHtml(x.Exception).replace(/\r\n/, "<br>\r\n"),
                            timeStampUtc: x.TimeStampUtc,
                            logLevel: x.Level
                        });
                    }
                });
                this.entries = this.allEntries;
            });
    }

    filterEntries($event: KeyboardEvent) {
        var re = new RegExp(this.filterText, 'i');
        this.entries = this.allEntries.filter(x => re.test(x.message) || re.test(x.exception));
    }
    
    private escapeHtml(unsafe: string): string {
        if (!unsafe) {
            return "";
        }
        return unsafe
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;");
    }
}
