import { AppRoot } from '@/services/AppRoot';
import * as feedback from "@/dto/Web/Feedback";
import { Component, Mixins } from "vue-property-decorator";
import { AppAware } from "@/AppMixins";

interface IFeedback {
    description: string;
    email: string;
    writtenAtUtc: Date;
    applicationName: string,
    applicationId: number;
    incidentId?: number;
    incidentName?: string;
}

@Component
export default class DiscoverFeedbackComponent extends Mixins(AppAware) {

    applicationId: number = 0;
    feedbackList: IFeedback[] = [];

    created() {
        this.onApplicationChanged(applicationId => {
            if (this.$route.name !== 'discoverFeedback') {
                return;
            }

            this.feedbackList = [];
            this.applicationId = applicationId;
            this.fetchFeedback();
        });

        this.applicationId = AppRoot.Instance.currentApplicationId;
        this.fetchFeedback();
    }

    private fetchFeedback() {
        if (this.applicationId > 0) {
            const q = new feedback.GetFeedbackForApplicationPage();
            q.ApplicationId = this.applicationId;
            AppRoot.Instance.apiClient.query<feedback.GetFeedbackForDashboardPageResult>(q)
                .then(result => this.processFeedback(result));
        } else {
            const q = new feedback.GetFeedbackForDashboardPage();
            AppRoot.Instance.apiClient.query<feedback.GetFeedbackForDashboardPageResult>(q)
                .then(result => this.processFeedback(result));
        }
    }
    private processFeedback(result: feedback.GetFeedbackForDashboardPageResult) {
        result.Items.forEach(x => {
            if (!x.Message) {
                return;
            }

            var any = <any>x;
            this.feedbackList.push({
                applicationName: x.ApplicationName,
                applicationId: x.ApplicationId,
                incidentId: any.IncidentId,
                incidentName: any.IncidentName,
                description: x.Message,
                email: x.EmailAddress,
                writtenAtUtc: x.WrittenAtUtc
            });
        });
    }


}
