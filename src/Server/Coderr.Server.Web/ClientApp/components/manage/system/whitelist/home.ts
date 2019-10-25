import { AppRoot } from "../../../../services/AppRoot";
import * as whitelist from "../../../../dto/Core/Whitelist";
import Vue from "vue";
import { Component, Watch } from "vue-property-decorator";

interface IApp {
    selected: boolean;
    id: number;
    name: string;
}

@Component
export default class ManageWhitelistComponent extends Vue {
    entries: whitelist.GetWhitelistEntriesResultItem[] = [];

    created() {
        var q = new whitelist.GetWhitelistEntries();
        AppRoot.Instance.apiClient.query<whitelist.GetWhitelistEntriesResult>(q)
            .then(x => {
                this.entries = x.Entries;
            });
    }


    mounted() {
    }
}
