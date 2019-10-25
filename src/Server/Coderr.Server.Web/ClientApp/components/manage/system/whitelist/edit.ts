import { AppRoot } from "../../../../services/AppRoot";
import * as whitelist from "../../../../dto/Core/Whitelist";
import Vue from "vue";
import { Component } from "vue-property-decorator";

interface IApp {
    selected: boolean;
    id: number;
    name: string;
}

interface IIpAddress {
    id: number;
    address: string;
    type: IpType;
}

enum IpType {
    Lookup,
    Manual,
    Rejected
}


@Component
export default class EditWhitelistComponent extends Vue {
    id: number = 0;
    domainName: string = '';
    apps: IApp[] = [];
    ipAddresses: IIpAddress[] = [];
    newIpAddress: string = '';

    created() {
        this.id = parseInt(this.$route.params.id);

        AppRoot.Instance.applicationService.list().then(x => {
            this.apps.push({
                id: 0,
                name: '(All)',
                selected: true
            });
            x.forEach(app => {
                this.apps.push({
                    id: app.id,
                    name: app.name,
                    selected: false
                });
            });
        });
        var q = new whitelist.GetWhitelistEntries();
        AppRoot.Instance.apiClient.query<whitelist.GetWhitelistEntriesResult>(q)
            .then(result => {
                var entry = result.Entries.find(x => x.Id === this.id);
                this.domainName = entry.DomainName;
                this.ipAddresses = entry.IpAddresses
                    .filter(x => x.IpType === whitelist.IpType.Specified)
                    .map<IIpAddress>(x => <IIpAddress>{
                        id: x.Id,
                        address: x.Address,
                        type: <number>x.IpType
                    });
            });

    }


    mounted() {
    }

    saveEntry() {
        var myApps: number[] = [];
        this.apps.forEach(x => {
            if (x.id === 0) {
                return;
            }

            if (x.selected) {
                myApps.push(x.id);
            }
        });
        var cmd = new whitelist.EditEntry();
        cmd.ApplicationIds = myApps;
        cmd.DomainName = this.domainName;
        cmd.IpAddresses = this.ipAddresses.map(x => x.address);
        AppRoot.Instance.apiClient.command(cmd);
        AppRoot.notify('Whitelist entry is being added..');
        this.$router.push({ name: 'manageWhitelistedDomains' });
    }

    addNewIp(e: MouseEvent) {
        this.ipAddresses.push({
            address: this.newIpAddress,
            id: 0,
            type: IpType.Manual
        });
        this.newIpAddress = '';
    }
    toggleAllOnChecked(e: any) {
        if (e.target.value === "0") {
            if (e.target.checked) {
                this.apps.forEach(x => {
                    x.selected = x.id === 0;
                });
            }
        } else {
            var allApp = this.apps.find(x => x.id === 0);
            if (e.target.checked) {
                allApp.selected = false;
            }
            var anySelected = this.apps.find(x => x.selected);
            if (!anySelected) {
                allApp.selected = true;
            }
        }
    }
}
