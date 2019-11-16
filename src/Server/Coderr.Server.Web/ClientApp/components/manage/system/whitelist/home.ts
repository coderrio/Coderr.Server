import { AppRoot } from "../../../../services/AppRoot";
import * as whitelist from "../../../../dto/Core/Whitelist";
import Vue from "vue";
import { Component, Watch } from "vue-property-decorator";

interface IApp {
    id: number;
    name: string;
}

interface IIpAddress {
    id: number;
    value: string;
    icon: string;
    tooltip: string;
}

interface IEntry {
    id: number;
    domainName: string;
    apps: IApp[];
    ips: IIpAddress[];
}


@Component
export default class ManageWhitelistComponent extends Vue {
    entries: IEntry[] = [];

    created() {
        this.loadData();
        if (this.$route.params.updated === "1") {
            setTimeout(this.loadData, 1500);
        }

    }


    mounted() {
    }

    loadData() {
        var q = new whitelist.GetWhitelistEntries();
        AppRoot.Instance.apiClient.query<whitelist.GetWhitelistEntriesResult>(q)
            .then(result => {
                result.Entries.forEach(result => {
                    this.entries = [];
                    if (result.Applications == null)
                        result.Applications = [];
                    if (result.IpAddresses == null)
                        result.IpAddresses = [];

                    let entry = <IEntry>{
                        id: result.Id,
                        domainName: result.DomainName,
                        ips: result.IpAddresses.map(y => <IIpAddress>{
                            id: y.Id,
                            value: y.Address,
                            icon: this.getIcon(y.Type),
                            tooltip: this.getTooltip(y.Type)
                        }),
                        apps: result.Applications.map(y => <IApp>{
                            id: y.ApplicationId,
                            name: y.Name
                        })
                    };
                    if (entry.apps.length === 0) {
                        entry.apps.push({ id: 0, name: '(All applications)' });
                    }
                    this.entries.push(entry);
                });
            });
    }

    removeEntry(entry: IEntry) {
        AppRoot.modal({
            submitButtonText: 'Delete',
            title: 'Delete ' + entry.domainName,
            htmlContent: 'Do you really want to delete that entry?'
        }).then(x => {
            console.log(this.entries);
            var cmd = new whitelist.RemoveEntry();
            cmd.Id = entry.id;
            AppRoot.Instance.apiClient.command(cmd);
            this.entries = this.entries.filter(x => x.id !== entry.id);
            AppRoot.notify('Entry deleted', 'fa-check', 'success');
        });

    }


    getIcon(ipType: whitelist.IpType): string {
        switch (ipType) {
            case whitelist.IpType.Denied:
                return 'fa-times-circle';
            case whitelist.IpType.Lookup:
                return 'fa-globe';
            default:
                return 'fa-keyboard';
        }
    }

    getTooltip(ipType: whitelist.IpType): string {
        switch (ipType) {
            case whitelist.IpType.Denied:
                return 'Got reports from this IP, but it wasn\'t registered in the DNS server for the given domain';
            case whitelist.IpType.Lookup:
                return 'Found as match during DNS lookup.';
            default:
                return 'Manually specified by an user.';
        }
    }
}
