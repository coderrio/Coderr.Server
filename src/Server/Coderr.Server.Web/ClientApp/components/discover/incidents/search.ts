import { PubSubService, MessageContext } from "@/services/PubSub";
import { FindIncidents, FindIncidentsResult, IncidentOrder } from "@/dto/Core/Incidents";
import { GetTags, TagDTO } from "@/dto/Modules/Tagging";
import { GetEnvironments, GetEnvironmentsResult } from "@/dto/Core/Environments";
import { ApplicationService, AppEvents, ApplicationCreated } from "@/services/applications/ApplicationService";
import { ApiClient } from '@/services/ApiClient';
import { AppRoot } from "@/services/AppRoot";
import { IncidentService } from "@/services/incidents/IncidentService";
import { Component, Mixins } from "vue-property-decorator";
import { AppAware } from "@/AppMixins";
import * as workItems from "@/common/services/WorkItemService";

interface Incident {
    Id: number;
    ApplicationId: number,
    ApplicationName: string,
    Name: string;
    CreatedAtUtc: Date;
    LastReportReceivedAtUtc: Date;
    ReportCount: number;

}

interface Application2 {
    id: number;
    name: string;
}

interface IEnvironment2 {
    id: number;
    name: string;
}

@Component
export default class IncidentSearchComponent extends Mixins(AppAware) {
    private apiClient$: ApiClient = AppRoot.Instance.apiClient;
    private incidentService$: IncidentService = AppRoot.Instance.incidentService;
    private readyPromises$: Promise<any>[] = [];
    isLoading = false;

    //controls
    showFilters: boolean = false;
    showApplicationColumn: boolean = true;

    //data
    incidents: Incident[] = [];
    availableTags: string[] = [];
    activeTags: string[] = [];

    availableEnvironments: IEnvironment2[] = [];
    activeEnvironment: number[] = [];

    availableApplications: Application2[] = [];
    activeApplications: number[] = [];

    freeText: string = '';
    incidentState: number = 0;
    contextCollectionName: string = '';
    contextCollectionProperty: string = '';
    contextCollectionPropertyValue: string = '';

    // incidents to assign
    checkedIncidents: number[] = [];

    sortKey = 1;
    ascendingSort = false;

    //for the close dialog
    currentIncidentId = 0;


    workItemIntegration: workItems.IIntegration = { title: '', name: '' };
    haveWorkItemIntegration: boolean | null = null;
    showCreateWorkItemButton: boolean = false;

    private activeBtn$ = 'active';

    created() {
        this.onApplicationChanged(this.onAppSelected);

        //fetch in created since we do not need the DOM
        var promise = new Promise<void>(resolve => {
            var appService = new ApplicationService(PubSubService.Instance, AppRoot.Instance.apiClient);
            appService.list()
                .then(x => {
                    x.forEach(x => {
                        this.availableApplications.push({ id: x.id, name: x.name });
                    });
                    resolve(null);
                });
        });
        this.readyPromises$.push(promise);

        var promise2 = new Promise<void>(resolve => {
            let q = new GetTags();
            AppRoot.Instance.apiClient.query<TagDTO[]>(q)
                .then(x => {
                    this.availableTags.length = 0;
                    x.forEach(x => {
                        this.availableTags.push(x.Name);
                    });
                    resolve(null);
                });
        });
        this.readyPromises$.push(promise2);

        var promise3 = new Promise<void>(resolve => {
            let q = new GetEnvironments();
            AppRoot.Instance.apiClient.query<GetEnvironmentsResult>(q)
                .then(x => {
                    this.availableEnvironments.length = 0;
                    x.Items.forEach(x => {
                        this.availableEnvironments.push({ id: x.Id, name: x.Name });
                    });
                    resolve(null);
                });
        });
        this.readyPromises$.push(promise3);

        PubSubService.Instance.subscribe(AppEvents.Created, ctx => {
            var msg = <ApplicationCreated>ctx.message.body;
            this.availableApplications.push({ id: msg.id, name: msg.name });
        });

        if (this.$route.params.applicationId) {
            var appId = parseInt(this.$route.params.applicationId, 10);
            this.activeApplications = [appId];
            this.showApplicationColumn = false;
        }
    }

    destroyed() {
        this.destroyed$ = true;
    }

    mounted() {
        this.destroyed$ = false;

        var readyPromise = AppRoot.Instance.loadState('incident-search', this);
        this.readyPromises$.push(readyPromise);
        Promise.all(this.readyPromises$)
            .then(resolve => {
                if (this.destroyed$) {
                    return;
                }
                this.isLoading = false;

                // Since the state contains appId(s), we need to override it.
                if (this.$route.params.applicationId) {
                    var id = parseInt(this.$route.params.applicationId, 10);
                    if (id > 0) {
                        this.activeApplications = [id];
                        this.showApplicationColumn = false;
                    }
                } else {
                    this.highlightActiveApps();
                    this.showApplicationColumn = true;
                }

                this.highlightActiveTags();
                this.highlightActiveEnvironments();
                this.drawSearchUi();
                this.highlightIncidentState(this.incidentState);
                this.searchInternal(true);

                if (this.activeApplications.length === 1) {
                    this.checkApplicationIntegration(this.activeApplications[0]);
                }
            });

    }

    checkAll(e: Event) {
        const target = <HTMLInputElement>e.target;
        const elems = document.querySelectorAll('#searchTable tbody input[type="checkbox"]');
        for (let i = 0; i < elems.length; i++) {
            const elem = <HTMLInputElement>elems[i];
            elem.checked = target.checked;
        }
    }

    toggleFilterDisplay() {
        this.showFilters = !this.showFilters;
    }

    toggleTag(e: MouseEvent) {
        var btn = <HTMLButtonElement>e.target;
        var tag = <string>btn.getAttribute('data-tag');

        if (btn.className.indexOf(this.activeBtn$) === -1) {
            btn.className = `btn ${this.activeBtn$}`;
            this.activeTags.push(tag);
        } else {
            btn.className = `btn`;
            this.activeTags = this.activeTags.filter(itm => itm !== tag);
        }
    }

    toggleEnvironment(e: MouseEvent) {
        var btn = <HTMLButtonElement>e.target;
        var environmentId = parseInt(btn.getAttribute('data-environment'), 10);

        if (btn.className.indexOf(this.activeBtn$) === -1) {
            btn.className = `btn ${this.activeBtn$}`;
            this.activeEnvironment.push(environmentId);
        } else {
            btn.className = `btn`;
            this.activeEnvironment = this.activeEnvironment.filter(itm => itm !== environmentId);
        }
    }

    toggleApplication(e: MouseEvent) {
        var btn = <HTMLButtonElement>e.target;
        var attr = btn.getAttribute('data-app');
        var applicationId = parseInt(<string>attr, 10);

        if (btn.className.indexOf(this.activeBtn$) === -1) {
            btn.className = `btn ${this.activeBtn$}`;
            this.activeApplications.push(applicationId);
        } else {
            btn.className = `btn`;
            this.activeApplications = this.activeApplications.filter(itm => {
                return itm !== applicationId;
            });
        }
    }

    sort(e: MouseEvent) {
        var el = <HTMLElement>e.target;
        var sortKey = parseInt(el.getAttribute('data-value'), 10);

        if (this.sortKey === sortKey) {
            this.ascendingSort = !this.ascendingSort;
        } else {
            this.sortKey = sortKey;
            this.ascendingSort = true;
        }

        this.drawSearchUi();
        this.searchInternal();
    }

    public search() {
        // Required, or vue will pass the mouse event to the method.
        this.searchInternal(false);
    }
    private searchInternal(byCode?: boolean) {
        var query = new FindIncidents();
        query.FreeText = this.freeText;
        switch (this.incidentState) {
            case -1:
                break;
            case 0:
                query.IsNew = true;
                break;
            case 1:
                query.IsAssigned = true;
                break;
            case 3:
                query.IsClosed = true;
                break;
            case 2:
                query.IsIgnored = true;
                break;
        }

        query.SortAscending = this.ascendingSort;
        query.SortType = this.sortKey;
        query.Tags = this.activeTags;

        if (this.activeApplications.length > 0) {
            query.ApplicationIds = this.activeApplications;
        }

        if (this.activeEnvironment.length > 0) {
            query.EnvironmentIds = this.activeEnvironment;
        }

        if (byCode !== true) {
            this.isLoading = true;
            AppRoot.Instance.storeState({
                name: 'incident-search',
                component: this,
                excludeProperties: ["incidents", "availableApplications", "availableTags", "checkedIncidents", "availableEnvironments"]
            });
        }
        query.PageNumber = 1;
        query.ItemsPerPage = 50;
        if (this.contextCollectionName != null && this.contextCollectionName !== "") {
            query.ContextCollectionName = this.contextCollectionName;
        }
        if (this.contextCollectionProperty != null && this.contextCollectionProperty !== "") {
            query.ContextCollectionPropertyName = this.contextCollectionProperty;
        }
        if (this.contextCollectionPropertyValue != null && this.contextCollectionPropertyValue !== "") {
            query.ContextCollectionPropertyValue = this.contextCollectionPropertyValue;
        }


        AppRoot.Instance.apiClient.query<FindIncidentsResult>(query)
            .then(result => {
                if (this.destroyed$) {
                    return;
                }

                this.isLoading = false;
                this.incidents.splice(0);
                result.Items.forEach(item => {
                    var entity: Incident = {
                        ApplicationId: item.ApplicationId,
                        ApplicationName: item.ApplicationName,
                        CreatedAtUtc: item.CreatedAtUtc,
                        Id: item.Id,
                        LastReportReceivedAtUtc: item.LastReportReceivedAtUtc,
                        Name: item.Name,
                        ReportCount: item.ReportCount
                    };
                    this.incidents.push(entity);
                });

            });
    }

    assignToMe(incidentId: number) {
        this.incidentService$.assignToMe(incidentId)
            .then(x => {
                this.$router.push({ name: 'analyzeIncident', params: { 'incidentId': incidentId.toString() } });
            });
    }

    assignAllToMe() {
        var incidents = this.getSelectedIncidents();
        incidents.forEach(incidentId => {
            this.incidentService$.assignToMe(incidentId);
        });
        AppRoot.notify('All selected incidents have been assigned to you. Click on the "Analyze" menu to start working with them.');
        setTimeout(() => { this.searchInternal() }, 1000);
    }

    deleteSelectedIncidents() {
        var incidents = this.getSelectedIncidents();
        incidents.forEach(incidentId => {
            this.incidentService$.delete(incidentId, "yes");
        });
        AppRoot.notify('All selected incidents have deleted.');
        setTimeout(() => { this.searchInternal() }, 1000);
    }

    close(incidentId: number) {
        this.currentIncidentId = incidentId;
        AppRoot.Instance.incidentService.showClose(incidentId, "CloseBody")
            .then(x => {
                if (x.requiresStatusUpdate) {
                    this.$router.push({
                        name: 'notifyUsers',
                        params: { incidentId: incidentId.toString() }
                    });
                    return;
                }
            });

    }

    private getSelectedIncidents(): number[] {
        var incidents: number[] = [];
        const elems = document.querySelectorAll('#searchTable tbody input[type="checkbox"]:checked');
        for (let i = 0; i < elems.length; i++) {
            const elem = <HTMLInputElement>elems[i];
            incidents.push(parseInt(elem.value));
        }
        return incidents;
    }

    private onAppSelected(applicationId: number) {
        if (this.$route.name !== 'findIncidents') {
            return;
        }
        if (applicationId == 0) {
            this.activeApplications = [];
            this.showApplicationColumn = true;
        } else {
            this.activeApplications = [applicationId];
            this.showApplicationColumn = false;
        }

        this.searchInternal();
        this.checkApplicationIntegration(applicationId);
    }
    private drawSearchUi() {
        var els = document.querySelectorAll('.search-head th i');
        for (var i = 0; i < els.length; i++) {
            els[i].classList.remove('fa-chevron-down');
            els[i].classList.remove('fa-chevron-up');
        }

        var us = document.querySelector('.search-head th[data-value="' + this.sortKey + '"] i');
        if (us == null) {
            console.log('failed to find', '.search-head th[data-value="' + this.sortKey + '"] i');
        } else {
            if (this.ascendingSort) {
                us.classList.add('fa-chevron-up');
            } else {
                us.classList.add('fa-chevron-down');
            }
        }

    }
    private toggleState(state: number, e: MouseEvent) {
        this.highlightIncidentState(state);
    }

    private highlightActiveTags() {
        this.activeTags.forEach(tag => {
            var elem = <HTMLElement>document.querySelector(`[data-tag="${tag}"`);
            if (!elem) {

                // reset so that we do not have a lot of invalid tags
                // in our localStorage
                this.activeTags.length = 0;
                return;
            }
            elem.classList.add(this.activeBtn$);
        });
    }

    private highlightActiveEnvironments() {
        this.activeEnvironment.forEach(value => {
            var elem = <HTMLElement>document.querySelector(`[data-environment="${value}"`);
            if (!elem) {

                // reset so that we do not have a lot of invalid environments
                // in our localStorage
                this.activeEnvironment.length = 0;
                return;
            }
            elem.classList.add(this.activeBtn$);
        });
    }

    private highlightIncidentState(incidentState: number) {
        var buttons = document.querySelectorAll('#IncidentSearchView .state button');
        for (var i = 0; i < buttons.length; ++i) {
            var button = <HTMLButtonElement>buttons[i];

            if (button.value === incidentState.toString()) {
                button.classList.add(this.activeBtn$);
            } else {
                if (button.classList.contains(this.activeBtn$))
                    button.classList.remove(this.activeBtn$);
            }
        }

        this.incidentState = incidentState;
    }


    private highlightActiveApps() {
        this.activeApplications.forEach(app => {
            var elem = <HTMLElement>document.querySelector(`[data-app="${app}"`);
            if (!elem) {
                // reset so that we do not have a lot of invalid tags
                // in our localStorage
                this.activeApplications.length = 0;

                return;
            }

            elem.classList.add(this.activeBtn$);
        });
    }

    private loadTags() {

    }

    private async checkApplicationIntegration(applicationId: number) {
        var service = new workItems.WorkItemService();
        
        var workItemIntegration = await service.findIntegration(applicationId);
        if (workItemIntegration == null) {
            this.workItemIntegration.title = '';
            this.haveWorkItemIntegration = false;
            return;
        }
        this.workItemIntegration = workItemIntegration;
        this.haveWorkItemIntegration = true;
    }

    createWorkItems() {
        var service = new workItems.WorkItemService();
        var incidents = this.getSelectedIncidents();
        incidents.forEach(incidentId => {
            service.createWorkItem(this.activeApplications[0], incidentId);
        });

        AppRoot.notify('Selected incidents have been added.');
    }
}
