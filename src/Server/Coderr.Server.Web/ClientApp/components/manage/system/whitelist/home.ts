import { AppRoot, IModalContext } from "../../../../services/AppRoot";
import * as whitelist from "../../../../dto/Core/Whitelist";
import { AddDomain, GetWhitelistEntries } from "../../../../dto/Core/Whitelist";
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
    newDomainName: string = '';
    newApps: IApp[] = [];

    created() {
        var q = new GetWhitelistEntries();
        AppRoot.Instance.apiClient.query<whitelist.GetWhitelistEntriesResult>(q)
            .then(x => {
                this.entries = x.Entries;
            });
        AppRoot.Instance.applicationService.list().then(x => {
            this.newApps.push({
                id: 0,
                name: '(All)',
                selected: true
            });
            x.forEach(app => {
                this.newApps.push({
                    id: app.id,
                    name: app.name,
                    selected: false
                });
            });
        });
    }


    mounted() {
    }

    showAdd() {
        var ctx: IModalContext = {
            contentId: 'add-item',
            title: 'Add domain',
            submitButtonText: 'Save',
            cancelButtonText: 'Cancel'
        }
        AppRoot.modal(ctx).then(x => {
            var cmd = new AddDomain();
            cmd.ApplicationId = AppRoot.Instance.currentApplicationId;
            cmd.DomainName = this.newDomainName;
            AppRoot.Instance.apiClient.command(cmd);
            console.log('success', this.newDomainName);
        });
    }
}
