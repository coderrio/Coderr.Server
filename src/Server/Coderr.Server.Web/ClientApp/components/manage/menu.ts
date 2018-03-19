import * as MenuApi from "../../services/menu/MenuApi";
import { AppRoot } from "../../services/AppRoot";
import { IncidentTopcis, IncidentAssigned } from "../../services/incidents/IncidentService";
import { PubSubService } from "../../services/PubSub";
import Vue from 'vue';
import { Component, Watch } from 'vue-property-decorator';
import { Location } from "vue-router";

@Component
export default class ManageMenuComponent extends Vue {

    currentApplicationId = 1;

    @Watch('$route.params.incidentId')
    onIncidentSelected(value: string, oldValue: string) {
    }

    mounted() {
    }

}
