import { PubSubService } from "../../../services/PubSub";
import { AppRoot } from '../../../services/AppRoot';
import * as feedback from "../../../dto/Web/Feedback";
import {NotifySubscribers} from "../../../dto/Core/Incidents";
import Vue from "vue";
import { Component, Watch } from "vue-property-decorator";

interface IFeedback {
    description: string;
    email: string;
    writtenAtUtc: Date;
}

@Component
export default class AnalyzeStatusUpdateComponent extends Vue {

    incidentId = 0;
    emailList = "";
    title = "";
    body="";

    created() {
        this.incidentId = parseInt(this.$route.params.incidentId, 10);

        var q = new feedback.GetIncidentFeedback();
        q.IncidentId = this.incidentId;
        AppRoot.Instance.apiClient.query<feedback.GetIncidentFeedbackResult>(q)
            .then(result => {
                var items: string[] = [];
                result.Items.forEach(x => {
                    if (x.EmailAddress) {
                        items.push(x.EmailAddress);
                    }
                });
                this.emailList = items.join(', ');
            });
    }

    send() {
        var cmd = new NotifySubscribers();
        cmd.IncidentId = this.incidentId;
        cmd.Body = this.body;
        cmd.Title = this.title;
        AppRoot.Instance.apiClient.command(cmd)
            .then(x => {
                AppRoot.notify('Message have been sent.');
                this.$router.push({ name: 'analyzeHome' });
            });
    }

}
