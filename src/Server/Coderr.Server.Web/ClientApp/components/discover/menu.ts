import { MenuItem } from "../../services/menu/MenuApi";
import { Component, Mixins } from 'vue-property-decorator';
import { AppAware } from "@/AppMixins";

interface IRouteNavigation {
    routeName: string;
    url: string;
    setMenu(name: String): void;
}
type NavigationCallback = (context: IRouteNavigation) => void;


@Component
export default class DiscoverMenuComponent extends Mixins(AppAware) {
    childMenu: MenuItem[] = [];
    currentApplicationId: number | null = null;

    created() {

    }

    mounted() {
        this.currentApplicationId = this.applicationId;
        this.onApplicationChanged(this.onAppChanged);
    }

    private onAppChanged(applicationId: number): void {
        if (this.$route.fullPath.indexOf('/discover/') === -1) {
            return;
        }

        this.currentApplicationId = applicationId;
    }

    testMe(e: any) {
    }
}
