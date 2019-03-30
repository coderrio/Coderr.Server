import "bootstrap";
import Vue from "vue";
import VueRouter from "vue-router";
import moment from "moment";
import { AppRoot } from "./services/AppRoot"
//import VeeValidate from 'vee-validate';
import { IUser } from "./vue-shim";

//Vue.use(VeeValidate);
Vue.use(VueRouter);
Vue.config.devtools = true;


declare module 'vue/types/vue' {
    interface Vue {
        user$: IUser;
    }
}

Vue.filter("ago",
    (value: string) => {
        if (!value) return "n/a";
        return moment.utc(value).fromNow();
    });

Vue.filter("niceTime",
    (value: string) => {
        if (!value) return "n/a";
        return moment(value).format("LLLL");
    });

Vue.filter("agoOrDate",
    (value: string) => {
        var today = moment(new Date());
        var reportDate = moment(value);
        var diff = reportDate.diff(today, "days");

        if (!value) return "n/a";
        return moment(value).fromNow();
    });

Vue.filter("incidentState",
    (value: string) => {
        switch (value) {
            case "0":
                return "New";
            case "1":
                return "Assigned";
            case "2":
                return "Ignored";
            case "3":
                return "Closed";
        }
    });


const routes = [
    {
        name: "root",
        path: "/",
        component: require("./components/home/home.vue.html").default
    },
    {
        name: "support",
        path: "/support/",
        component: require("./components/home/support/support.vue.html").default
    },
    {
        name: "application",
        path: "/discover/application/:applicationId/",
        component: require("./components/applications/application-details.vue.html").default
    },
    {
        path: "/discover/",
        component: require("./components/discover/discover.vue.html").default,
        children: [
            {
                name: "findIncidents",
                path: "incidents/:applicationId?",
                component: require("./components/discover/incidents/search.vue.html").default
            },
            {
                name: "discoverIncident",
                path: "incidents/:applicationId/incident/:incidentId",
                component: require("./components/discover/incidents/incident.vue.html").default
            },
            {
                name: "configureApplication",
                path: "application/:applicationId/configuration",
                component: require("./components/discover/application/configure.vue.html").default
            }, {
                name: "discoverApplication",
                path: "application/:applicationId/",
                component: require("./components/discover/incidents/incident.vue.html").default
            },
            {
                name: "suggest",
                path: "suggest/:applicationId?",
                component: require("./components/discover/incidents/suggestions.vue.html").default
            },
            {
                name: "suggestIncident",
                path: "suggest/:applicationId/incident/:incidentId",
                component: require("./components/discover/incidents/incident.vue.html").default
            },
            {
                name: "discover",
                path: ":applicationId?",
                component: require("./components/discover/home/home.vue.html").default
            }
        ]
    },
    {
        path: "/analyze/",
        component: require("./components/analyze/analyze.vue.html").default,
        children: [
            {
                name: "analyzeHome",
                path: "",
                component: require("./components/analyze/home/home.vue.html").default
            },
            {
                name: "analyzeIncident",
                path: "incident/:incidentId/",
                component: require("./components/analyze/incidents/incident.vue.html").default
            },
            {
                name: "analyzeReport",
                path: "incident/:incidentId/report/:reportId?",
                component: require("./components/analyze/incidents/report.vue.html").default
            },
            {
                name: "analyzeOrigins",
                path: "incident/:incidentId/origins/",
                component: require("./components/analyze/incidents/origins.vue.html").default
            },
            {
                name: "closeIncident",
                path: "incident/:incidentId/close/",
                component: require("./components/analyze/incidents/close.vue.html").default
            },
            {
                name: "analyzeFeedback",
                path: "incident/:incidentId/feedback/",
                component: require("./components/analyze/incidents/feedback.vue.html").default
            }
        ]
    },
    {
        name: "notifyUsers",
        path: "users/notify/incident/:incidentId/",
        component: require("./components/analyze/incidents/status.vue.html").default
    },
    {
        path: "/onboarding/",
        component: require("./components/onboarding/onboarding.vue.html").default,
        children: [
            {
                name: "onboardApp",
                path: "",
                component: require("./components/onboarding/home/home.vue.html").default
            }
        ]
    },
    {
        path: "/manage/application/",
        component: require("./components/manage/application/app.vue.html").default,
        children: [
            {
                name: "manageAppSettings",
                path: ":applicationId/",
                component: require("./components/manage/application/settings/settings.vue.html").default
            },
            {
                name: "manageSecurity",
                path: ":applicationId/security/",
                component: require("./components/manage/application/security/security.vue.html").default
            },
            {
                name: "managePartitions",
                path: ":applicationId/partitions/",
                component: require("./components/manage/application/partitions/partition.vue.html").default
            },
            {
                name: "createPartition",
                path: ":applicationId/partition/create/",
                component: require("./components/manage/application/partitions/create.vue.html").default
            },
            {
                name: "editPartition",
                path: ":applicationId/partition/:partitionId/edit",
                component: require("./components/manage/application/partitions/edit.vue.html").default
            }
        ]
    },
    {
        path: "/manage/",
        component: require("./components/manage/system/manage.vue.html").default,
        children: [
            {
                name: "manageHome",
                path: "",
                component: require("./components/manage/system/home/home.vue.html").default,

            },
            {
                name: "createApp",
                path: "create/application/",
                component: require("./components/manage/system/create/create.vue.html").default,

            },
            {
                name: "manageApiKeys",
                path: "apikeys/",
                component: require("./components/manage/system/apikeys/apikeys.vue.html").default
            },
            {
                name: "manageApiKey",
                path: "apikey/:apiKey",
                component: require("./components/manage/system/apikeys/apikey.vue.html").default
            },
            {
                name: "editApiKey",
                path: "apikey/:apiKey/edit",
                component: require("./components/manage/system/apikeys/apikey-edit.vue.html").default
            },
            {
                name: "createApiKey",
                path: "apikeys/create",
                component: require("./components/manage/system/apikeys/apikey-create.vue.html").default
            }
        ]
    },
    {
        path: "/deployment/",
        component: require("./components/deployment/deployment.vue.html").default,

        children: [
            {
                name: "deploymentHome",
                path: ":applicationId?/",
                component: require("./components/deployment/home/home.vue.html").default
            },
            {
                name: "deploymentVersion",
                path: ":applicationId/version/:version",
                component: require("./components/deployment/version/summary.vue.html").default
            }

        ]
    }
];

var hooks = {
    mounted: function(instance:Vue) {},
    created: function(instance:Vue) {},
    afterRoute: function (to: string, from: string) {}
};

// Hack for tests
var v = <any>window;
if (v["Cypress"]) {
    v['MyVueHooks'] = hooks;
}

var ourVue: Vue;
AppRoot.Instance.loadCurrentUser()
    .then(user => {
        AppRoot.Instance.currentUser = user;
        ourVue = new Vue({
            el: "#app-root",
            router: new VueRouter({ mode: "history", routes: routes }),
            render: h => h(require("./components/home/app.vue.html").default),
            created: () => {
                hooks.created(ourVue);
            },
            mounted: () => {
                hooks.mounted(ourVue);

            }
        });
        //ourVue.$router.beforeEach((to, from, next) => {
        //    next();
        //});
        ourVue.$router.afterEach((to, from) => {
            hooks.afterRoute(to.path, from.path);
        })
        ourVue.user$ = user;

        if (v["Cypress"]) {
            v["MyVue"] = ourVue;
        }
    });

