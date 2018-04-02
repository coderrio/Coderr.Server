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
            this.currentApplicationId = null;
            this.currentApplicationName = 'All applications';
            return;
        }

        var applicationId = parseInt(value);
        var app = this.getApplication(applicationId);
        this.currentApplicationId = applicationId;

        var title = app.title;
        if (title.length > 20) {
            title = title.substr(0, 15) + '[...]';
        }
        this.currentApplicationName = title;
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
        console.log(applicationId);
        if (this.$route.path.indexOf('/discover') === 0) {
            console.log('disc');
            this.$router.push({ name: 'discover', params: { applicationId: applicationId.toString() } });
        } else if (this.$route.path.indexOf('/analyze') === 0) {
            this.$router.push({ name: 'analyzeHome', params: { applicationId: applicationId.toString() } });
        } else if (this.$route.path.indexOf("/deployment") === 0) {
            this.$router.push({ name: 'deploymentHome', params: { applicationId: applicationId.toString() } });
        } else if (this.$route.path.indexOf("/manage") === 0) {
            this.$router.push({ name: 'manageAppSettings', params: { applicationId: applicationId.toString() } });
        }

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
        var app = this.getApplication(applicationId);
        this.currentApplicationId = applicationId;
        this.currentApplicationName = app.title;
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
