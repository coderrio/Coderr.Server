import { PubSubService, MessageContext } from "@/services/PubSub";
import { Component, Watch, Vue } from "vue-property-decorator";
import { AppRoot } from "@/services/AppRoot";
import { AppEvents, ApplicationChanged } from "@/services/applications/ApplicationService";

@Component
export class AppAware extends Vue {
    applicationId: number = 0;
    protected destroyed$ = false;
    private callback: (applicationId: number) => void;

    mounted() {
        PubSubService.Instance.subscribe(AppEvents.Selected, this.onApplicationChangedInNavMenu);

        var value = this.$route.params.applicationId;
        if (value) {
            this.applicationId = parseInt(value);
        }
    }

    beforeDestroy() {
        PubSubService.Instance.unsubscribe(AppEvents.Selected, this.onApplicationChangedInNavMenu);
        this.destroyed$ = true;
    }

    @Watch('$route.params.applicationId')
    onApplicationSelected(value: string, oldValue: string) {
        if (!value) {
            AppRoot.Instance.applicationService.changeApplication(0);
            return;
        }

        this.applicationId = parseInt(value);
        AppRoot.Instance.applicationService.changeApplication(this.applicationId);
    }


    onApplicationChanged(callback: (applicationId: number) => void) {
        if (this.destroyed$) {
            return;
        }

        this.callback = callback;
    }

    private onApplicationChangedInNavMenu(ctx: MessageContext) {
        var body = <ApplicationChanged>ctx.message.body;
        this.callback(body.applicationId);
    }

    changeApplication(applicationId: number) {
        AppRoot.Instance.applicationService.changeApplication(applicationId);
    }
}
