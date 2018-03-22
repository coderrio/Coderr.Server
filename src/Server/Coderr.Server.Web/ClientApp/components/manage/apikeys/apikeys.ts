import { AppRoot } from "../../../services/AppRoot";
import {
    GetApiKey, GetApiKeyResult, GetApiKeyResultApplication,
    ListApiKeys, ListApiKeysResult, ListApiKeysResultItem
} from "../../../dto/Core/ApiKeys";
import Vue from "vue";
import { Component, Watch } from "vue-property-decorator";


@Component
export default class ManageApiKeysComponent extends Vue {
    applicationId: number = 0;
    applicationName: string = "";

    keys: ListApiKeysResultItem[] = [];

    created() {
        var appIdStr = this.$route.params.applicationId;
        this.applicationId = parseInt(appIdStr, 10);

        var q = new ListApiKeys();
        AppRoot.Instance.apiClient.query<ListApiKeysResult>(q)
            .then(x => {
                this.keys = x.Keys;
            });

    }


    mounted() {
    }

    @Watch('$route.params.applicationId')
    onApplicationChanged(value: string, oldValue: string) {
        this.applicationId = parseInt(value, 10);

        var q = new ListApiKeys();
        AppRoot.Instance.apiClient.query<ListApiKeysResult>(q)
            .then(x => {
                this.keys = x.Keys;
            });
    }

}
