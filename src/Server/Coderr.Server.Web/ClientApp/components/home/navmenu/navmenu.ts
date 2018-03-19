import { PubSubService, MessageContext } from "../../../services/PubSub";
import * as MenuApi from "../../../services/menu/MenuApi";
import { AppRoot } from "../../../services/AppRoot";
import { ApplicationSummary } from "../../../services/applications/ApplicationService";
import Vue from 'vue';
import { Component, Watch } from 'vue-property-decorator';
import * as Router from "vue-router";


class CoderrMenu {
    discover: MenuApi.MenuItem = {
        title: 'Discover',
        url: '/discover/',

        children: [
            {
                title: 'Discover',
                url: '/discover/',
                children: []
            }
        ]
    };
}

var DiscoverMenu: MenuApi.MenuItem[] = [
    {
        title: 'Overview',
        url: '/discover/:applicationId',
    },
    {
        title: 'Suggestions',
        url: '/discover/suggestions/:applicationId',
    },
    {
        title: 'Find incidents',
        url: '/discover/incidents/:applicationId',
    }
];

var AnalyzeMenu: MenuApi.MenuItem[] = [
    {
        title: 'Current incident',
        url: '/analyze/incident/:incidentId',
    },
    {
        title: 'Context data',
        url: '/analyze/incident/:incidentId/context',
    },
    {
        title: 'Report origins',
        url: '/analyze/incident/:incidentId/origins',
    }
];

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
            console.log(to, from);
            if (to.fullPath.indexOf('/onboarding/') === -1 && this.onboarding) {
                this.onboarding = false;
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

    private askCallbacksForWhichMenu(route: Router.Route):string {
        var chosenMenu = "";
        var ctx:IRouteNavigation = {
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
   
    private onMenuChange(ctx: MessageContext) {
        var msg = <MenuApi.ChangeMenu>ctx.message.body;
        this.applyMenu(msg.menuName);
    }

    private applyMenu(name: string) {
        var mnu = name === 'Discover' ? DiscoverMenu : AnalyzeMenu;

        mnu.forEach(mnuItem => {
            mnuItem.url = mnuItem.url.replace(/\:([a-zA-Z0-9]+)/gi,
                (match, key) => {
                    var value = this.$route.params[key];
                    return value ? value : '';
                });
            this.childMenu = mnu;
        });

        if (mnu === DiscoverMenu) {
            this.isDiscoverActive = true;
            this.isDeploymentActive = false;
            this.isAnalyzeActive = false;
        } else {
            this.isDiscoverActive = false;
            this.isDeploymentActive = false;
            this.isAnalyzeActive = true;
        }
    }

    private updateCurrent(applicationId: number) {
        var app = this.getApplication(applicationId);
        this.currentApplicationId = applicationId;
        this.currentApplicationName = app.title;
    }


    private createAppMenuItem(applicationId: number, name: string): MenuApi.MenuItem {
        const routeLocation: Router.Location =
            { name: 'discoverForApplication', params: { 'applicationId': applicationId.toString() } };
        const url = <string>this.$router.resolve(routeLocation).href;
        const app: MenuApi.MenuItem =
        {
            title: name,
            url: url,
            tag: 'app:' + applicationId
        };
        return app;

    }
    private getApplication(applicationId: number): MenuApi.MenuItem {
        for (var i = 0; i < this.myApplications.length; i++) {
            if (this.myApplications[i].tag === `app:${applicationId}`) {
                return this.myApplications[i];
            }
        }

        throw new Error('Failed to find application ' + applicationId);
    }

}
