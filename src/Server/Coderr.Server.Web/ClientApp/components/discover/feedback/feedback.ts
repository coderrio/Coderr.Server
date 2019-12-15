import { PubSubService, MessageContext } from "@/services/PubSub";
import { AppRoot } from '@/services/AppRoot';
import * as MenuApi from "@/services/menu/MenuApi";
import * as feedback from "@/dto/Web/Feedback";
import { Component, Vue } from "vue-property-decorator";

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
export default class DiscoverFeedbackComponent extends Vue {

    applicationId: number = 0;
    feedbackList: IFeedback[] = [];
    destroyed$ = false;

    created() {
        PubSubService.Instance.subscribe(MenuApi.MessagingTopics.ApplicationChanged, this.onApplicationChangedInNavMenu);

        this.applicationId = parseInt(this.$route.params.applicationId, 10);
        this.fetchFeedback();
    }

    destroyed() {
        PubSubService.Instance.unsubscribe(MenuApi.MessagingTopics.ApplicationChanged, this.onApplicationChangedInNavMenu);
        this.destroyed$ = true;
    }

    private onApplicationChangedInNavMenu(ctx: MessageContext) {
        if (this.$route.name !== 'discoverFeedback') {
            return;
        }

        this.feedbackList = [];
        const body = <MenuApi.ApplicationChanged>ctx.message.body;
        this.applicationId = body.applicationId;
        this.fetchFeedback();
    }

    private fetchFeedback() {
        console.log(this.applicationId);
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
