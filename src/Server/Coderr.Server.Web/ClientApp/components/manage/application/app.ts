import { PubSubService, MessageContext } from "../../../services/PubSub";
import * as MenuApi from "../../../services/menu/MenuApi";
import Vue from 'vue';
import { Component, Watch } from 'vue-property-decorator';

@Component({
    components: {
        ManageAppMenu: require('./menu.vue.html').default
    }
})
export default class ManageComponent extends Vue {
    created() {
        PubSubService.Instance.subscribe(MenuApi.MessagingTopics.ApplicationChanged, this.onApplicationChangedInNavMenu);
    }

    destroyed() {
        PubSubService.Instance.unsubscribe(MenuApi.MessagingTopics.ApplicationChanged, this.onApplicationChangedInNavMenu);
    }

    private onApplicationChangedInNavMenu(ctx: MessageContext) {
        if (this.$route.path.indexOf('/manage/') === -1) {
            return;
        }
        var body = <MenuApi.ApplicationChanged>ctx.message.body;
        if (body.applicationId == null) {
            this.$router.push({ name: 'manageHome' });
        }
    }
}
