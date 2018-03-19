import { AppRoot } from "../../../services/AppRoot";
import {
    GetApiKey, GetApiKeyResult, GetApiKeyResultApplication,
    ListApiKeys, ListApiKeysResult, ListApiKeysResultItem
} from "../../../dto/Core/ApiKeys";
import { GetApplicationTeam, GetApplicationTeamResult, GetApplicationTeamResultInvitation, GetApplicationTeamMember } from "../../../dto/Core/Applications";
import Vue from "vue";
import { Component } from "vue-property-decorator";


@Component
export default class ManageHomeComponent extends Vue {
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



}
