import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from "@angular/router";
import { WorkItemService, IIntegration } from "../../services/work-item.service";
import { FindIncidents, FindIncidentsResult, SortOrder } from "../../../server-api/Core/Incidents";
import { ApiClient } from "../../utils/HttpClient";
import { GetEnvironments, GetEnvironmentsResult } from "../../../server-api/Core/Environments";
import { GetTags, TagDTO } from "../../../server-api/Modules/Tagging";
import { AccountService } from "../../accounts/account.service";
import { states } from "../incident.model";
import { ModalService } from "../../_controls/modal/modal.service";
import { NavMenuService } from "../../nav-menu/nav-menu.service";
import { ApplicationService } from "../../applications/application.service";

interface Incident {
  id: number;
  applicationId: number,
  applicationName: string,
  name: string;
  createdAtUtc: Date;
  lastReportReceivedAtUtc: Date;
  reportCount: number;
}

interface Application2 {
  id: number;
  name: string;
}

interface IListItem {
  id: number;
  name: string;
}

interface IListItemWithSelection extends IListItem {
  selected: boolean;
}

class SearchSettings {
  tabName: string;

  userId: number = 0;
  environmentId: number = 0;
  selectedTags: string[] = [];
  freeText = '';
  state = 0;
  contextCollectionName = '';
  contextCollectionProperty = '';
  contextCollectionPropertyValue = '';

  sortKey = 1;
  ascendingSort = false;
}


@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.scss']
})
export class SearchComponent implements OnInit, OnDestroy {
  showFilters = false;
  showApplicationColumn = false;

  canSave = false;
  haveSavedSearches = false;
  newTabName = '';

  /**
   * Selected in the save dropdown.
   */
  selectedTabName = '';

  selectedTags = '';

  settings: SearchSettings = new SearchSettings();
  storedSearches: SearchSettings[] = [];
  applicationId: number;

  states = states.map<IListItem>(x => {
    return {
      id: x.id,
      name: x.name,
      selected: false
    }
  });

  sortColumn = 'created';
  sortAscending = true;

  //data
  incidents: Incident[] = [];
  tags: IListItemWithSelection[] = [];
  environments: IListItem[] = [];
  users: IListItem[] = [];

  // incidents to assign
  checkedIncidents: number[] = [];


  //for the close dialog
  currentIncidentId = 0;


  workItemIntegration: IIntegration = { title: '', name: '' };
  haveWorkItemIntegration: boolean | null = null;
  showCreateWorkItemButton = false;

  private sub: any;


  constructor(
    private route: ActivatedRoute,
    private apiClient: ApiClient,
    private workItemService: WorkItemService,
    private accountService: AccountService,
    private modalService: ModalService,
    private appService: ApplicationService,
    private navService: NavMenuService) {
  }

  ngOnInit(): void {
    this.sub = this.route.params.subscribe(params => {
      this.applicationId = +params['applicationId'];
      var p1 = this.loadEnvironments();
      var p2 = this.loadTags();
      var p3 = this.loadUsers();

      Promise.all([p1, p2, p3]).then(x => {
        this.loadSearches();
        this.searchInternal(true);
      });

    });


  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  search() {
    this.searchInternal(false);
  }

  sort(columnName: string) {
    if (this.sortColumn === columnName) {
      this.sortAscending = !this.sortAscending;
    } else {
      this.sortColumn = columnName;
      this.sortAscending = true;
    }

    this.settings.ascendingSort = this.sortAscending;

    switch (columnName) {
      case 'created':
        this.settings.sortKey = SortOrder.Newest;
        break;
      case 'lastReport':
        this.settings.sortKey = SortOrder.LatestReport;
        break;
      case 'reportCount':
        this.settings.sortKey = SortOrder.ReportCount;
        break;
    }
    this.searchInternal(false);
  }

  getSortClass(columnName: string): string {
    if (columnName === this.sortColumn) {
      return this.sortAscending ? 'fa fa-chevron-up' : 'fa fa-chevron-down';
    }

    return '';
  }

  reset() {
    this.settings = new SearchSettings();
    this.newTabName = '';
    this.sortColumn = 'created';
    this.sortAscending = true;
    this.searchInternal(false);
  }

  private async loadEnvironments(): Promise<void> {
    const q = new GetEnvironments();
    const response = await this.apiClient.query<GetEnvironmentsResult>(q);
    this.environments.length = 0;
    response.items.forEach(x => {
      this.environments.push({ id: x.id, name: x.name });
    });

    var app = await this.appService.get(this.applicationId);
    this.navService.updateNav([
      { title: app.name, route: ['application', app.id] },
      { title: 'Errors', route: ['application', app.id, 'errors'] }
    ]);

  }

  private async loadTags(): Promise<void> {
    let q = new GetTags();
    var response = await this.apiClient.query<TagDTO[]>(q);

    this.tags.length = 0;
    response.forEach(x => {
      this.tags.push({ id: 0, name: x.name, selected: false });
    });
  }

  private async loadUsers(): Promise<void> {
    var users = await this.accountService.getAllButMe();
    this.users = users.map<IListItem>(x => {
      return {
        id: x.id,
        name: x.userName,
      }
    });
  }

  private async searchInternal(byCode?: boolean): Promise<void> {
    var query = new FindIncidents();
    query.freeText = this.settings.freeText;

    switch (+this.settings.state) {
      case -1:
        break;
      case 0:
        query.isNew = true;
        break;
      case 1:
        query.isAssigned = true;
        break;
      case 3:
        query.isClosed = true;
        break;
      case 2:
        query.isIgnored = true;
        break;
    }

    query.sortAscending = this.settings.ascendingSort;
    query.sortType = this.settings.sortKey;
    query.tags = this.parseTags();

    if (this.applicationId > 0) {
      query.applicationIds = [this.applicationId];
    }

    if (this.settings.environmentId > 0) {
      query.environmentIds = [+this.settings.environmentId];
    }

    if (this.settings.userId > 0) {
      query.assignedToId = +this.settings.userId;
    }

    query.pageNumber = 1;
    query.itemsPerPage = 50;

    if (this.settings.contextCollectionName != null && this.settings.contextCollectionName !== "") {
      query.contextCollectionName = this.settings.contextCollectionName;
    }
    if (this.settings.contextCollectionProperty != null && this.settings.contextCollectionProperty !== "") {
      query.contextCollectionPropertyName = this.settings.contextCollectionProperty;
    }
    if (this.settings.contextCollectionPropertyValue != null && this.settings.contextCollectionPropertyValue !== "") {
      query.contextCollectionPropertyValue = this.settings.contextCollectionPropertyValue;
    }

    if (!byCode && this.selectedTabName.length > 0) {
      this.settings.selectedTags = this.parseTags();
      this.storeSearches();
    }

    var result = await this.apiClient.query<FindIncidentsResult>(query);
    this.incidents.splice(0);
    result.items.forEach(item => {
      var entity: Incident = {
        applicationId: item.applicationId,
        applicationName: item.applicationName,
        createdAtUtc: item.createdAtUtc,
        id: item.id,
        lastReportReceivedAtUtc: item.lastReportReceivedAtUtc,
        name: item.name,
        reportCount: item.reportCount
      };
      this.incidents.push(entity);
    });
  }

  createWorkItems() {
    var incidents = this.getSelectedIncidents();
    incidents.forEach(incidentId => {
      this.workItemService.createWorkItem(this.applicationId, incidentId);
    });

    //TODO: toasstr
  }

  selectSearch(search: SearchSettings) {
    if (!search.selectedTags) {
      search.selectedTags = [];
    }

    this.settings = search;
    this.tags.forEach(x => {
      x.selected = search.selectedTags.includes(x.name);
    });

    this.selectedTags = search.selectedTags.join(',');
    this.selectedTabName = this.settings.tabName;
    this.newTabName = this.settings.tabName;

    this.sortAscending = this.settings.ascendingSort;
    switch (this.settings.sortKey) {
      case SortOrder.Newest:
        this.sortColumn = "created";
        break;
      case SortOrder.LatestReport:
        this.sortColumn = "lastReport";
        break;
      case SortOrder.ReportCount:
        this.sortColumn = "reportCount";
        break;
    }

    this.search();
  }

  showSaveSearch() {
    this.modalService.open("newSearchModal");
  }

  saveSearch() {
    // storing a new search
    if (this.selectedTabName.length === 0) {
      var newSettings = Object.assign({}, this.settings);
      newSettings.tabName = this.newTabName;
      this.selectedTabName = this.newTabName;
      this.settings = newSettings;
      this.storedSearches.push(newSettings);
    }

    // renaming an existing search
    else if (this.newTabName.length > 0 && this.newTabName !== this.selectedTabName) {
      this.settings.tabName = this.newTabName;
      this.selectedTabName = this.newTabName;
    }

    this.settings.selectedTags = this.parseTags();
    this.modalService.close("newSearchModal");
    this.storeSearches();
    this.haveSavedSearches = true;
  }

  cancelSaveSearch() {
    this.modalService.close("newSearchModal");
  }

  showDeleteSearch() {
    this.modalService.open("removeSearchModal");
  }

  deleteSearch() {
    this.storedSearches = this.storedSearches.filter(x => x.tabName !== this.selectedTabName);
    if (this.selectedTabName === this.settings.tabName) {
      this.settings = new SearchSettings();
    }

    this.storeSearches();
    this.haveSavedSearches = true;
    this.modalService.close("removeSearchModal");
  }

  cancelRemoveSearch() {
    this.modalService.close("removeSearchModal");
  }


  private parseTags(): string[] {
    if (this.selectedTags.length < 2) {
      return null;
    }

    return this.selectedTags.split(",").map(item => item.trim());
  }

  private async checkApplicationIntegration(applicationId: number) {
    var workItemIntegration = await this.workItemService.findIntegration(applicationId);
    if (workItemIntegration == null) {
      this.workItemIntegration.title = '';
      this.haveWorkItemIntegration = false;
      return;
    }
    this.workItemIntegration = workItemIntegration;
    this.haveWorkItemIntegration = true;
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

  private loadSearches() {
    this.haveSavedSearches = false;
    var json = localStorage.getItem('searchSettings');
    if (!json) {
      return;
    }

    this.storedSearches = JSON.parse(json);
    this.haveSavedSearches = this.storedSearches.length > 0;
  }

  private storeSearches() {
    var json = JSON.stringify(this.storedSearches);
    localStorage.setItem('searchSettings', json);
  }



}
