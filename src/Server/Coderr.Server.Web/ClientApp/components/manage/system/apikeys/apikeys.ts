import { AppRoot } from "../../../../services/AppRoot";
import {
    GetApiKey, GetApiKeyResult, GetApiKeyResultApplication,
    ListApiKeys, ListApiKeysResult, ListApiKeysResultItem
} from "../../../../dto/Core/ApiKeys";
import Vue from "vue";
import { Component, Watch } from "vue-property-decorator";


@Component
export default class ManageApiKeysComponent extends Vue {
    keys: ListApiKeysResultItem[] = [];

    created() {
        var q = new ListApiKeys();
        AppRoot.Instance.apiClient.query<ListApiKeysResult>(q)
            .then(x => {
                this.keys = x.Keys.filter(x => x.ApplicationName !== '#OTE#');
            });

    }


    mounted() {
    }
}
