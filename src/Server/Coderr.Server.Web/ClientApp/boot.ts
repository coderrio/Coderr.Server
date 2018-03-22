import "./css/site.css";
import "bootstrap";
import Vue from "vue";
import VueRouter from "vue-router";
import moment from "moment";
import { AppRoot } from "./services/AppRoot"
import VeeValidate from 'vee-validate';

Vue.use(VeeValidate);
Vue.use(VueRouter);

Vue.filter("ago",
    (value: string) => {
        if (!value) return "n/a";
        return moment(value).fromNow();
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
        path: "/application/:applicationId/",
        component: require("./components/applications/application-details.vue.html")
    },
    {
        path: "/discover/",
        name: "discover",
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
                name: "discoverForApplication",
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
        path: "/manage/application/",
        component: require("./components/manage/manage.vue.html"),
        children: [
            {
                name: "manageApp",
                path: ":applicationId/",
                component: require("./components/manage/home/home.vue.html")
            },
            {
                name: "manageAppTeam",
                path: ":applicationId/team/",
                component: require("./components/manage/team/team.vue.html")
            },
            {
                name: "managePartitions",
                path: ":applicationId/partitions/",
                component: require("./components/manage/partitions/partition.vue.html")
            },
            {
                name: "manageApiKeys",
                path: ":applicationId/apikeys/",
                component: require("./components/manage/apikeys/apikeys.vue.html")
            },
            {
                name: "manageApiKey",
                path: ":applicationId/apikeys/:apiKey",
                component: require("./components/manage/apikeys/apikey.vue.html")
            },
            {
                name: "editApiKey",
                path: ":applicationId/apikeys/:apiKey/edit",
                component: require("./components/manage/apikeys/apikey-edit.vue.html")
            },
            {
                name: "createApiKey",
                path: ":applicationId/apikeys/create",
                component: require("./components/manage/apikeys/apikey-create.vue.html")
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

