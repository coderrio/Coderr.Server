import Vue from "vue";
import { Component, Watch } from "vue-property-decorator";

interface IItem {
    id: string;
    text: string;
}

@Component
export default class AzureDevOpsConnectionComponent extends Vue {
    applicationId: number = 0;
    applicationName: string = null;
    personalAccessToken: string = null;
    url: string = null;
    projectName: string = null;
    projectId: string | null = null;
    areaPath: string = null;
    areaPathId: string = null;
    projects: IItem[] = [];
    areas: IItem[] = [];
    iterations: IItem[] = [];
    isConnected = false;
    showConnectError = false;

    mounted() {

    }

    @Watch('$route.params.applicationId')
    onApplicationChanged(value: string, oldValue: string) {
        this.applicationId = parseInt(value);
    }

}