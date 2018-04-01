import "./css/site.css";
import "bootstrap";
import Vue from "vue";
import VueRouter from "vue-router";
import moment from "moment";
import { AppRoot } from "./services/AppRoot"
import VeeValidate from 'vee-validate';

Vue.use(VeeValidate);
Vue.use(VueRouter);
Vue.config.devtools = true;

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
        component: require("./components/home/home.vue.html")
    },
    {
        name: "support",
        path: "/support/",
        component: require("./components/home/support/support.vue.html")
    },
    {
        name: "application",
        path: "/discover/application/:applicationId/",
        component: require("./components/applications/application-details.vue.html")
    },
    {
        path: "/discover/",
        component: require("./components/discover/discover.vue.html"),
        children: [
            {
                name: "findIncidents",
                path: "incidents/:applicationId?",
                component: require("./components/discover/incidents/search.vue.html")
            },
            {
                name: "discoverIncident",
                path: "incidents/:applicationId/incident/:incidentId",
                component: require("./components/discover/incidents/incident.vue.html")
            },
            {
                name: "configureApplication",
                path: "application/:applicationId/configuration",
                component: require("./components/discover/application/configure.vue.html")
            }, {
                name: "discoverApplication",
                path: "application/:applicationId/",
                component: require("./components/discover/incidents/incident.vue.html")
            },
            {
                name: "suggest",
                path: "suggest/:applicationId?",
                component: require("./components/discover/incidents/suggestions.vue.html")
            },
            {
                name: "suggestIncident",
                path: "suggest/:applicationId/incident/:incidentId",
                component: require("./components/discover/incidents/incident.vue.html")
            },
            {
                name: "discover",
                path: ":applicationId?",
                component: require("./components/discover/home/home.vue.html")
            },
        ]
    },
    {
        path: "/analyze/",
        component: require("./components/analyze/analyze.vue.html"),
        children: [
            {
                name: "analyzeHome",
                path: "",
                component: require("./components/analyze/home/home.vue.html")
            },
            {
                name: "analyzeIncident",
                path: "incident/:incidentId/",
                component: require("./components/analyze/incidents/incident.vue.html")
            },
            {
                name: "analyzeReport",
                path: "incident/:incidentId/report/:reportId?",
                component: require("./components/analyze/incidents/report.vue.html")
            },
            {
                name: "analyzeOrigins",
                path: "incident/:incidentId/origins/",
                component: require("./components/analyze/incidents/origins.vue.html")
            },
            {
                name: "closeIncident",
                path: "incident/:incidentId/close/",
                component: require("./components/analyze/incidents/close.vue.html")
            },
            {
                name: "analyzeFeedback",
                path: "incident/:incidentId/feedback/",
                component: require("./components/analyze/incidents/feedback.vue.html")
            }
        ]
    },
    {
        path: "/onboarding/",
        component: require("./components/onboarding/onboarding.vue.html"),
        children: [
            {
                name: "onboardApp",
                path: "",
                component: require("./components/onboarding/home/home.vue.html")
            }
        ]
    },
    {
        name: "manageApp",
        path: "/manage/application/",
        component: require("./components/manage/application/app.vue.html"),
        children: [
            {
                name: "manageAppSettings",
                path: ":applicationId/",
                component: require("./components/manage/application/settings/settings.vue.html")
            },
            {
                name: "manageSecurity",
                path: ":applicationId/security/",
                component: require("./components/manage/application/security/security.vue.html")
            },
            {
                name: "managePartitions",
                path: ":applicationId/partitions/",
                component: require("./components/manage/application/partitions/partition.vue.html")
            },
            {
                name: "createPartition",
                path: ":applicationId/partition/create/",
                component: require("./components/manage/application/partitions/create.vue.html")
            },
            {
                name: "editPartition",
                path: ":applicationId/partition/:partitionId/edit",
                component: require("./components/manage/application/partitions/edit.vue.html")
            }
        ]
    },
    {
        path: "/manage/",
        component: require("./components/manage/system/manage.vue.html"),
        children: [
            {
                name: "manageHome",
                path: "",
                component: require("./components/manage/system/home/home.vue.html"),
            },
            {
                name: "createApp",
                path: "application/create",
                component: require("./components/manage/system/create/create.vue.html"),
            },
            {
                name: "manageApiKeys",
                path: "apikeys/",
                component: require("./components/manage/system/apikeys/apikeys.vue.html")
            },
            {
                name: "manageApiKey",
                path: "apikey/:apiKey",
                component: require("./components/manage/system/apikeys/apikey.vue.html")
            },
            {
                name: "editApiKey",
                path: "apikey/:apiKey/edit",
                component: require("./components/manage/system/apikeys/apikey-edit.vue.html")
            },
            {
                name: "createApiKey",
                path: "apikeys/create",
                component: require("./components/manage/system/apikeys/apikey-create.vue.html")
            }
        ]
    },
    {
        path: "/deployment/",
        component: require("./components/deployment/deployment.vue.html"),
        children: [
            {
                name: "deploymentHome",
                path: ":applicationId?/",
                component: require("./components/deployment/home/home.vue.html")
            },
            {
                name: "deploymentVersion",
                path: ":applicationId/version/:version",
                component: require("./components/deployment/version/summary.vue.html")
            }

        ]
    }
];

var ourVue: Vue;
AppRoot.Instance.loadCurrentUser()
    .then(user => {
        AppRoot.Instance.currentUser = user;
        ourVue = new Vue({
            el: "#app-root",
            router: new VueRouter({ mode: "history", routes: routes }),
            render: h => h(require("./components/home/app.vue.html")),
            created: () => {
            },
            mounted: () => {
            }
        });
        ourVue.user$ = user;
    });

