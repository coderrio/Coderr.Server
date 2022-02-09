import { PubSubService } from "../../../services/PubSub";
import * as MenuApi from "../../../services/menu/MenuApi";
import { AppRoot, IMyApplication } from "../../../services/AppRoot";
import { AppEvents, ApplicationChanged, ApplicationGroup } from "@/services/applications/ApplicationService";
import * as appContracts from "@/dto/Core/Applications"
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
    private toggleMenu: boolean = false;

    appGroups: ApplicationGroup[] = [];
    childMenu: MenuApi.MenuItem[] = [];

    myApplicationsPromise: Promise<void>;
    currentApplicationName: string = 'All applications';
    currentApplicationId: number | null = null;
    myApplications: IMyApplication[] = [];
    missedReportsMessage: string = '';
    isDiscoverActive: boolean = true;
    isAnalyzeActive: boolean = false;
    isDeploymentActive: boolean = false;
    lastPublishedId$ = 0;
    showOnboarding = false;
    showMenu: boolean = true;
    isAdmin = false;
    licenseText = '';

    discoverLink: string = '/discover/';

    created() {
        if (this.$route.params.applicationId && this.$route.path.indexOf('/onboarding/') === -1) {
            AppRoot.Instance.currentApplicationId = parseInt(this.$route.params.applicationId);
            this.checkOnboarding(AppRoot.Instance.currentApplicationId);
        } else {
            AppRoot.Instance.applicationService.list().then(x => {
                if (x.length > 0) {
                    this.checkOnboarding(x[0].id);
                }
            });

        }

        this.myApplicationsPromise = new Promise((accept, reject) => {
            AppRoot.Instance.loadCurrentUser().then(x => {
                this.allApps = x.applications;
                console.log('allApps', this.allApps);
                accept(null);
            });
        });

        AppRoot.Instance.applicationService.getGroups().then(x => {
            if (x.length > 1) {
                this.appGroups = x;
                this.showApps(x[0].id);
            } else {
                this.showApps(-1);
            }
        });

        this.$router.beforeEach((to, from, next) => {
            if (to.fullPath.indexOf('/onboarding/') === -1 && !this.showMenu) {
                this.showMenu = true;
            }
            if (to.fullPath.indexOf('/onboarding/') === 0 && this.showMenu) {
                this.showMenu = false;
            }

            next();
        });

        AppRoot.Instance.loadCurrentUser().then(x => {
            this.isAdmin = x.isSysAdmin;
            this.licenseText = x.licenseText;
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

                return;
            }

            const params = this.$route.params;
            if (!params.applicationId || params.applicationId.toString() === msg.applicationId.toString()) {
                return;
            }

            params.applicationId = msg.applicationId.toString();
            this.$router.replace({ name: this.$route.name, params: params });
            this.checkOnboarding(msg.applicationId);
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
            this.showMenu = false;
        }
    }

    showApps(groupId: number) {
        if (groupId === -1) {
            this.myApplications = this.allApps;
        } else {
            this.myApplications = this.allApps.filter(x => x.groupId === groupId);
        }
    }

    /**
     *  Toggled from the menu 
     * @param applicationId
     */
    changeApplication(applicationId: number | null) {
        this.toggleMenu = false;
        if (applicationId === this.currentApplicationId) {
            return;
        }

        this.currentApplicationId = applicationId;
        this.updateCurrent(applicationId);
        if (applicationId == null) {
            applicationId = 0;
        }

        if (this.$route.params.hasOwnProperty('applicationId') && this.$route.path.indexOf('/analyze') !== 0) {
            var newParams = Object.assign(this.$route.params, { 'applicationId': applicationId.toString() });
            this.$router.push({ "name": this.$route.name, "params": newParams });
        }
        this.checkOnboarding(applicationId);
        AppRoot.Instance.applicationService.changeApplication(applicationId);
    }

    private checkOnboarding(applicationId: number) {
        AppRoot.Instance.incidentService.getMine(applicationId)
            .then(result => {
                this.showOnboarding = result.length === 0;
            });
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
