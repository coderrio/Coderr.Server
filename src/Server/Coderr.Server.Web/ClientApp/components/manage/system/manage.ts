import { Component, Mixins } from 'vue-property-decorator';
import { AppAware } from "@/AppMixins";

@Component({
    components: {
        ManageMenu: require('./menu.vue.html').default
    }
})
export default class ManageComponent extends Mixins(AppAware) {
    created() {
        this.onApplicationChanged(applicationId => {
            if (this.$route.path.indexOf('/manage/') === -1) {
                return;
            }
            if (applicationId > 0) {
                this.$router.push({ name: 'manageAppSettings', params: { "applicationId": applicationId.toString() } });
            }
        });
    }
}
