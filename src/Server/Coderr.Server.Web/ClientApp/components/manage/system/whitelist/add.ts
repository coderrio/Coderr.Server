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
    Lookup = 0,
    Manual = 1,
    Rejected = 2
}


@Component
export default class ManageWhitelistComponent extends Vue {
    domainName: string = '';
    apps: IApp[] = [];
    ipAddresses: IIpAddress[] = [];
    newIpAddress: string = '';

    created() {
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
        var cmd = new whitelist.AddEntry();
        cmd.ApplicationIds = myApps;
        cmd.DomainName = this.domainName;
        cmd.IpAddresses = this.ipAddresses.map(x => x.address);
        AppRoot.Instance.apiClient.command(cmd);
        AppRoot.notify('Whitelist entry is being added..', 'fa-info', 'success');
        this.$router.push({ name: 'manageWhitelistedDomains', params: { updated: '1' } });
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
