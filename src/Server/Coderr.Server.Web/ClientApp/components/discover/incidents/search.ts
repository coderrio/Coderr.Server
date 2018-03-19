import { PubSubService } from "../../../services/PubSub";
import { FindIncidents, FindIncidentsResult } from "../../../dto/Core/Incidents";
import { GetTags, TagDTO } from "../../../dto/Modules/Tagging";
import { ApplicationService, AppEvents, ApplicationCreated } from "../../../services/applications/ApplicationService";
import { ApiClient } from '../../../services/ApiClient';
import { AppRoot } from "../../../services/AppRoot";
import { IncidentService } from "../../../services/incidents/IncidentService";
import Vue from "vue";
import { Component } from "vue-property-decorator";

declare var $: any;
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



@Component
export default class IncidentSearchComponent extends Vue {
    private apiClient$: ApiClient = AppRoot.Instance.apiClient;
    private incidentService$: IncidentService = AppRoot.Instance.incidentService;
    private readyPromises$: Promise<any>[] = [];

    //controls
    showFilters: boolean = false;
    showApplicationColumn: boolean = true;

    //data
    incidents: Incident[] = [];
    availableTags: string[] = [];
    activeTags: string[] = [];

    availableApplications: Application2[] = [];
    activeApplications: number[] = [];

    freeText: string = '';
    incidentState: number = 0;
    contextCollectionName: string = '';
    contextCollectionProperty: string = '';
    contextCollectionPropertyValue: string = '';

    created() {
        //fetch in created since we do not need the DOM
        var promise = new Promise<any>(resolve => {
            var appService = new ApplicationService(PubSubService.Instance, AppRoot.Instance.apiClient);
            appService.list()
                .then(x => {
                    x.forEach(x => {
                        this.availableApplications.push({ id: x.id, name: x.name });
                    });
                    resolve();
                });
        });
        this.readyPromises$.push(promise);

        var promise2 = new Promise<any>(resolve => {
            let q = new GetTags();
            AppRoot.Instance.apiClient.query<TagDTO[]>(q)
                .then(x => {
                    this.availableTags.length = 0;
                    x.forEach(x => {
                        this.availableTags.push(x.Name);
                    });
                    resolve();
                });
        });
        this.readyPromises$.push(promise2);

        PubSubService.Instance.subscribe(AppEvents.Created, ctx => {
            var msg = <ApplicationCreated>ctx.message.body;
            this.availableApplications.push({ id: msg.id, name: msg.name });
        });

        if (this.$route.params.applicationId) {
            this.showApplicationColumn = false;
            var appId = parseInt(this.$route.params.applicationId);
            this.activeApplications.push(appId);
        }
    }

    mounted() {

        var readyPromise = AppRoot.Instance.loadState('incident-search', this);
        this.readyPromises$.push(readyPromise);
        Promise.all(this.readyPromises$)
            .then(resolve => {
                console.log(this, this.$route, this.$route.params);
                if (this.$route.params.applicationId) {
                    var id = parseInt(this.$route.params.applicationId, 10);
                    if (id > 0) {
                        this.activeApplications.push(id);
                    }
                }
                this.highlightActiveApps();
                this.highlightActiveTags();
                this.search(true);
            });
    }

    toggleFilterDisplay() {
        this.showFilters = !this.showFilters;
    }

    toggleTag(e: MouseEvent) {
        var btn = <HTMLButtonElement>e.target;
        var tag = <string>btn.getAttribute('data-tag');

        if (btn.className.indexOf('btn-dark') === -1) {
            btn.className = 'btn btn-dark';
            this.activeTags.push(tag);
        } else {
            btn.className = 'btn btn-light';
            this.activeTags = this.activeTags.filter(itm => itm !== tag);
        }
    }

    toggleApplication(e: MouseEvent) {
        var btn = <HTMLButtonElement>e.target;
        var attr = btn.getAttribute('data-app');
        var applicationId = parseInt(<string>attr, 10);

        if (btn.className.indexOf('btn-dark') === -1) {
            btn.className = 'btn btn-dark';
            this.activeApplications.push(applicationId);
        } else {
            btn.className = 'btn btn-light';
            this.activeApplications = this.activeApplications.filter(itm => {
                console.log(itm, applicationId);
                return itm !== applicationId;
            });
        }
    }

    search(byCode?: boolean) {
        var query = new FindIncidents();
        query.FreeText = this.freeText;

        switch (this.incidentState) {
            case 0:
                query.IsNew = true;
                break;
            case 1:
                query.IsAssigned = true;
            case 2:
                query.IsClosed = true;
                break;
            case 3:
                query.IsIgnored = true;
                break;
        }

        query.Tags = this.activeTags;

        if (this.activeApplications.length > 0) {
            query.ApplicationIds = this.activeApplications;
        }

        if (byCode !== true) {
            AppRoot.Instance.storeState({
                name: 'incident-search',
                component: this,
                excludeProperties: ["incidents", "availableApplications", "availableTags"]
            });
        }

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
                this.incidents.length = 0;
                result.Items.forEach(item => {
                    var entity: Incident = {
                        ApplicationId: parseInt(item.ApplicationId, 10),
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

    close(incidentId: number) {
        AppRoot.modal({
            title: 'Close incident',
            contentId: '#CloseBody',
        }).then(x => {
            var modal = <HTMLDivElement>document.getElementById(x.modalId);
            var area = <HTMLTextAreaElement>modal.querySelector('textarea');
            var reason = area.value;

            this.incidentService$.close(incidentId, reason)
                .then(numberOfFollowers => {
                    AppRoot.notify('Incident have been closed');

                    if (numberOfFollowers <= 0)
                        return;

                    AppRoot.modal({
                        title: 'Incident have followers',
                        htmlContent:
                        'Incident have users following it. We strongly recommend that you use Coderr to send them a status update saying that the error is corrected.',
                        submitButtonText: 'Draft an update'
                    }).then(result => {
                        this.$router.push({
                            name: 'incidentStatusUpdate',
                            params: {
                                'incidentId': incidentId.toString(),
                                'closed': 'true'
                            }
                        });
                    });

                });
        })

    }

    private toggleState(state: number, e: MouseEvent) {
        var buttons = document.querySelectorAll('#IncidentSearchView .state button');
        for (var i = 0; i < buttons.length; ++i) {
            buttons[i].classList.add('btn-light');
            buttons[i].classList.remove('btn-dark');
        }

        var elem = <HTMLElement>e.target;
        elem.classList.add('btn-dark');
        this.incidentState = state;
    }

    private highlightActiveTags() {
        this.activeTags.forEach(tag => {
            var elem = <HTMLElement>document.querySelector(`[data-tag="${tag}"`);
            if (!elem) {
                console.log('failed to find tag ' + tag, document.querySelectorAll('[data-tag]'));

                // reset so that we do not have a lot of invalid tags
                // in our localStorage
                this.activeTags.length = 0;
                return;
            }
            elem.classList.remove('btn-light');
            elem.classList.add('btn-dark');
        });
    }


    private highlightActiveApps() {
        this.activeApplications.forEach(app => {
            var elem = <HTMLElement>document.querySelector(`[data-app="${app}"`);
            if (!elem) {
                // reset so that we do not have a lot of invalid tags
                // in our localStorage
                this.activeApplications.length = 0;

                console.log('failed to find app ' + app, document.querySelectorAll('[data-app]'));
                return;
            }

            elem.classList.remove('btn-light');
            elem.classList.add('btn-dark');
        });
    }

    private loadTags() {

    }
}
