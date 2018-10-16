import * as MenuApi from "../../../services/menu/MenuApi";
import { AppRoot } from "../../../services/AppRoot";
import { IncidentTopcis, IncidentAssigned } from "../../../services/incidents/IncidentService";
import { PubSubService } from "../../../services/PubSub";
import Vue from 'vue';
import { Component, Watch } from 'vue-property-decorator';
import { Location } from "vue-router";

@Component
export default class ManageMenuComponent extends Vue {

    currentApplicationId = 0;

    @Watch('$route.params.applicationId')
    onApplicationIdChanged(value: string, oldValue: string) {
        this.currentApplicationId = parseInt(value, 10);
    }

    mounted() {
        if (this.$route.params.applicationId) {
            this.currentApplicationId = parseInt(this.$route.params.applicationId, 10);
        }
    }

}
