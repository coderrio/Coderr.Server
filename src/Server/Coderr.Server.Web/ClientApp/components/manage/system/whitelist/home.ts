import { AppRoot } from "../../../../services/AppRoot";
import * as whitelist from "../../../../dto/Core/Whitelist";
import { GetWhitelistEntries } from "../../../../dto/Core/Whitelist";
import Vue from "vue";
import { Component, Watch } from "vue-property-decorator";


@Component
export default class ManageWhitelistComponent extends Vue {
    entries: whitelist.GetWhitelistEntriesResultItem[] = [];

    created() {
        var q = new GetWhitelistEntries();
        AppRoot.Instance.apiClient.query<whitelist.GetWhitelistEntriesResult>(q)
            .then(x => {
                this.entries = x.Entries;
            });

    }


    mounted() {
    }
}
