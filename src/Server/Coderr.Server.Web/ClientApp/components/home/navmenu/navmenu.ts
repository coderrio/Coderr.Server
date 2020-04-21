import { PubSubService } from "../../../services/PubSub";
import * as MenuApi from "../../../services/menu/MenuApi";
import { AppRoot, IMyApplication } from "../../../services/AppRoot";
import { AppEvents, ApplicationChanged } from "@/services/applications/ApplicationService";
import { Component, Vue } from 'vue-property-decorator';
import * as Router from "vue-router";

interface IRouteNavigation {
    routeName: string;
    url: string;
    setMenu(name: String): void;
}
type NavigationCallback = (context: IRouteNavigation) => void;
declare var window: any;

@Component
export default class NavMenuComponent extends Vue {
    private callbacks: NavigationCallback[] = [];
    private loaded = false;
    private allApps: IMyApplication[] = [];

    childMenu: MenuApi.MenuItem[] = [];

    myApplicationsPromise: Promise<null>;
    currentApplicationName: string = 'All applications';
    currentApplicationId: number | null = null;
    myApplications: IMyApplication[] = [];
    missedReportsMessage: string = '';
    isDiscoverActive: boolean = true;
    isAnalyzeActive: boolean = false;
    isDeploymentActive: boolean = false;
    lastPublishedId$ = 0;
    onboarding: boolean = false;
    isAdmin = false;
    licenseText = '';

    discoverLink: string = '/discover/';

    created() {
        this.myApplicationsPromise = new Promise((accept, reject) => {
            AppRoot.Instance.loadCurrentUser().then(x => {
                this.allApps = x.applications;
                this.myApplications = x.applications;
                accept();
            });
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

        AppRoot.Instance.loadCurrentUser().then(x => {
            this.isAdmin = x.isSysAdmin;
        });

        PubSubService.Instance.subscribe(MenuApi.MessagingTopics.IgnoredReportCountUpdated, ctx => {
            if (ctx.message.body > 0) {
                this.missedReportsMessage = `Coderr have discarded ${ctx.message.body} error report(s) this month. (Community Server limit)`;
            } else {
                this.missedReportsMessage = '';
            }
        });

        // we need to do this so that the route is updated.
        PubSubService.Instance.subscribe(AppEvents.Selected, ctx => {


            var msg = <ApplicationChanged>ctx.message.body;
            if (msg.applicationId === null || msg.applicationId === 0) {

                // analyze uses it's own logic
                if (this.$route.path.indexOf('/analyze/') !== -1)
                    return;

                AppRoot.Instance.currentApplicationId = null;
                //this.updateCurrent(0);
                return;
            }

            const params = this.$route.params;
            if (!params.applicationId) {
                return;
            }

            params.applicationId = msg.applicationId.toString();
            this.$router.replace({ name: this.$route.name, params: params });

            //this.updateCurrent(msg.applicationId);
        });
        PubSubService.Instance.subscribe(AppEvents.Removed, ctx => {
            this.myApplications = this.myApplications.filter(x => x.id !== ctx.message.body);
            if (this.currentApplicationId === <number>ctx.message.body) {
                this.changeApplication(null);
            }
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

    changeApplication(applicationId: number | null) {
        if (applicationId === this.currentApplicationId) {
            return;
        }
        this.currentApplicationId = applicationId;
        this.updateCurrent(applicationId);
        if (applicationId == null) {
            applicationId = 0;
        }
        AppRoot.Instance.applicationService.changeApplication(applicationId);
        return;

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

        const appIdStr = applicationId == null ? null : applicationId.toString();
        if (currentRoute.path.indexOf('/manage/') !== -1) {
            if (applicationId) {
                const route = { name: 'manageAppSettings', params: { applicationId: appIdStr } };
                this.$router.push(route);
            } else {
                const route = { name: 'manageHome' };
                this.$router.push(route);
            }
        } else if (paramCount === 1 && currentRoute.params.hasOwnProperty('applicationId')) {
            if (applicationId == null) {
                this.$router.push({ name: currentRoute.name });
                AppRoot.Instance.applicationService.changeApplication(0);
            } else {
                this.$router.push({ name: currentRoute.name, params: { applicationId: applicationId.toString() } });
                AppRoot.Instance.applicationService.changeApplication(applicationId);
            }
            return;
        } else if (currentRoute.path.indexOf('/discover') === 0) {
            this.$router.push({ name: 'discover', params: { applicationId: appIdStr } });
        } else if (currentRoute.path.indexOf('/analyze') === 0) {
            if (applicationId == null) {
                this.$router.push({ name: 'analyzeHome' });
            } else {
                this.$router.push({ name: 'analyzeHome', params: { applicationId: appIdStr } });
            }
        } else {
            const route = { name: currentRoute.name, params: { applicationId: appIdStr } };
            this.$router.push(route);
        }

        AppRoot.Instance.applicationService.changeApplication(applicationId);
    }

    private askCallbacksForWhichMenu(route: Router.Route): string {
        var chosenMenu = "";
        const ctx: IRouteNavigation = {
            routeName: <string>route.name,
            url: route.fullPath,
            setMenu: (menuName: string) => {
                chosenMenu = menuName;
            }
        };
        for (let i = 0; i < this.callbacks.length; i++) {
            this.callbacks[i](ctx);
            if (chosenMenu !== "") {
                return chosenMenu;
            }
        }

        return "";
    }

    private logga(message: string) {
        console.log(new Date().getTime() + " " + message);
    }

    private updateCurrent(applicationId: number) {
        if (applicationId === 0 || applicationId == null) {
            this.currentApplicationName = "All applications";
            this.currentApplicationId = null;
            this.discoverLink = "/discover/";
            //const msg = new MenuApi.ApplicationChanged();
            //msg.applicationId = applicationId;
            return;
        }

        this.currentApplicationId = applicationId;
        this.myApplicationsPromise.then(x => {
            var app = this.allApps.find(y => y.id === applicationId);
            this.currentApplicationId = applicationId;

            var title = app.name;
            if (title.length > 40) {
                title = title.substr(0, 35) + "[...]";
            }
            this.currentApplicationName = title;
            this.discoverLink = `/discover/${applicationId}`;
            //var msg = new MenuApi.ApplicationChanged();
            //msg.applicationId = applicationId;
        });
    }

}
