import { PubSubService, MessageContext } from "../../../services/PubSub";
import * as MenuApi from "../../../services/menu/MenuApi";
import { AppRoot } from "../../../services/AppRoot";
import { ApplicationSummary } from "../../../services/applications/ApplicationService";
import Vue from 'vue';
import { Component, Watch } from 'vue-property-decorator';
import * as Router from "vue-router";


interface IRouteNavigation {
    routeName: string;
    url: string;
    setMenu(name: String): void;
}
type NavigationCallback = (context: IRouteNavigation) => void;

@Component
export default class NavMenuComponent extends Vue {
    private callbacks: NavigationCallback[] = [];

    childMenu: MenuApi.MenuItem[] = [];

    myApplications: MenuApi.MenuItem[] = [];
    currentApplicationName: string = 'All applications';
    currentApplicationId: number | null = null;

    isDiscoverActive: boolean = true;
    isAnalyzeActive: boolean = false;
    isDeploymentActive: boolean = false;

    onboarding: boolean = false;

    @Watch('$route.params.applicationId')
    onApplicationChanged(value: string, oldValue: string) {
        if (!value) {
            // analyze uses it's own logic
            if (this.$route.path.indexOf('/analyze/') !== -1)
                return;

            this.updateCurrent(0);
            return;
        }

        var applicationId = parseInt(value);
        this.updateCurrent(applicationId);
    }

    created() {
        AppRoot.Instance.currentUser.applications.forEach(app => {
            var mnuItem = this.createAppMenuItem(app.id, app.name);
            this.myApplications.push(mnuItem);
        });

        this.$router.beforeEach((to, from, next) => {
            if (to.fullPath.indexOf('/onboarding/') === -1 && this.onboarding) {
                this.onboarding = false;
            }
            if (to.fullPath.indexOf('/onboarding/') === 0 && !this.onboarding) {
                this.onboarding = true;
            }

            next();
        });

        PubSubService.Instance.subscribe(MenuApi.MessagingTopics.SetApplication, ctx => {
            var msg = <MenuApi.SetApplication>ctx.message.body;
            this.changeApplication(msg.applicationId);
        });
    }

    mounted() {
        if (this.$route.params.applicationId && this.$route.path.indexOf('/onboarding/') === -1) {
            var appId = parseInt(this.$route.params.applicationId);
            this.updateCurrent(appId);
        }

        if (this.$route.fullPath.indexOf('/onboarding/') !== -1) {
            this.onboarding = true;
        }
    }

    changeApplication(applicationId: number) {
        if (applicationId == null) {
            this.updateCurrent(0);
        } else {
            this.updateCurrent(applicationId);
        }

        const currentRoute = this.$route;
        let paramCount = 0;
        for (let key in currentRoute.params) {
            if (currentRoute.params.hasOwnProperty(key)) {
                paramCount++;
            }
        }

        if (paramCount === 1 && currentRoute.params.hasOwnProperty('applicationId')) {
            if (applicationId == null) {
                this.$router.push({ name: currentRoute.name  });
                PubSubService.Instance.publish(MenuApi.MessagingTopics.ApplicationChanged, { applicationId: null });
            } else {
                this.$router.push({ name: currentRoute.name, params: { applicationId: applicationId.toString() } });
                PubSubService.Instance.publish(MenuApi.MessagingTopics.ApplicationChanged, { applicationId: applicationId });
            }
            return;
        }

        if (currentRoute.path.indexOf('/discover') === 0) {
            this.$router.push({ name: 'discover', params: { applicationId: applicationId.toString() } });
        } else if (currentRoute.path.indexOf('/analyze') === 0) {
            this.$router.push({ name: 'analyzeHome', params: { applicationId: applicationId.toString() } });
        } else if (currentRoute.path.indexOf('/manage/') !== -1 && currentRoute.path.indexOf('/manage/application') === -1) {
            const route = { name: 'manageAppSettings', params: { applicationId: applicationId.toString() } };
            this.$router.push(route);
        } else {
            const route = { name: currentRoute.name, params: { applicationId: applicationId.toString() } };
            this.$router.push(route);
        }

        PubSubService.Instance.publish(MenuApi.MessagingTopics.ApplicationChanged, { applicationId: applicationId });
    }

    private askCallbacksForWhichMenu(route: Router.Route): string {
        var chosenMenu = "";
        var ctx: IRouteNavigation = {
            routeName: <string>route.name,
            url: route.fullPath,
            setMenu: (menuName: string) => {
                chosenMenu = menuName;
            }
        }
        for (var i = 0; i < this.callbacks.length; i++) {
            this.callbacks[i](ctx);
            if (chosenMenu !== "") {
                return chosenMenu;
            }
        }

        return '';
    }


    private updateCurrent(applicationId: number) {
        if (applicationId === 0) {
            this.currentApplicationName = "All applications";
            this.currentApplicationId = null;
            return;
        }

        var app = this.getApplication(applicationId);
        this.currentApplicationId = applicationId;

        var title = app.title;
        if (title.length > 20) {
            title = title.substr(0, 15) + '[...]';
        }
        this.currentApplicationName = title;
    }


    private createAppMenuItem(applicationId: number, name: string): MenuApi.MenuItem {
        if (!applicationId) {
            throw new Error("Expected an applicationId.");
        }

        const app: MenuApi.MenuItem =
            {
                title: name,
                url: '',
                tag: applicationId
            };
        return app;

    }
    private getApplication(applicationId: number): MenuApi.MenuItem {
        for (var i = 0; i < this.myApplications.length; i++) {
            if (this.myApplications[i].tag === applicationId) {
                return this.myApplications[i];
            }
        }

        throw new Error('Failed to find application ' + applicationId);
    }

}
