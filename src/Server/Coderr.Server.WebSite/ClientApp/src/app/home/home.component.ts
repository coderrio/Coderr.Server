import { Component, OnInit, OnDestroy } from '@angular/core';
import { ApplicationService } from "../applications/application.service";
import { ApiClient } from "../utils/HttpClient";
import { GetApplicationOverview, GetApplicationOverviewResult } from "../../server-api/Core/Applications";
import { ModalService } from '../_controls/modal/modal.service';
import { NavMenuService } from "../nav-menu/nav-menu.service";
import { AuthorizeService } from "../../api-authorization/authorize.service";
import { GuideService } from "../_controls/guide/guide.service";


@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit, OnDestroy {
  applications: ApplicationOverview[] = [];
  private allApps: ApplicationOverview[] = [];
  private appSub: any;
  private filterText = '';
  selectedGroup?: IGroup;
  noApps = false;
  showGroupsStyle = "hidden";
  activePane = '';
  firstApplicationId = 0;

  constructor(private appService: ApplicationService,
    private modalService: ModalService,
    private apiClient: ApiClient,
    private guideService: GuideService,
    authService: AuthorizeService,
    navMenuService: NavMenuService) {
    navMenuService.updateNav([]);

    this.activePane = localStorage.getItem('homeActivePane') || 'applications';
  }

  filter(event) {
    this.filterText = event.target.value.toLowerCase();
    this.executeFilter();
  }

  setPane(name: string) {
    this.activePane = name;
    localStorage.setItem('homeActivePane', name);
  }


  ngOnInit(): void {
    this.appSub = this.appService.applications.subscribe(apps => {
      this.noApps = apps.length === 0;
      this.allApps.length = 0;

      if (apps.length > 0) {
        this.firstApplicationId = apps[0].id;
      }

      apps.forEach(appDto => {
        const app: ApplicationOverview = {
          id: appDto.id,
          groupIds: appDto.groupIds,
          name: appDto.name,
          errors: 0,
          latestError: null,
          latestReport: null,
          partitions: [],
          reports: 0,
          followers: 0,
          bugReports: 0
        };

        this.allApps.push(app);

        var query = new GetApplicationOverview();
        query.applicationId = app.id;
        query.includeChartData = false;
        query.includePartitions = true;
        query.numberOfDays = 30;
        this.apiClient.query<GetApplicationOverviewResult>(query)
          .then(result => {
            app.reports = result.statSummary.reports;
            app.errors = result.statSummary.incidents;
            app.latestReport = result.statSummary.newestReportReceivedAtUtc;
            app.latestError = result.statSummary.newestIncidentReceivedAtUtc;
            app.followers = result.statSummary.followers;
            app.bugReports = result.statSummary.userFeedback;
          });
      });

      this.applications = this.allApps;

    });


  }

  showGroups() {
    this.modalService.open('groupFilter');
  }

  selectGroup(evt: IGroup) {
    this.selectedGroup = evt;
    this.modalService.close('groupFilter');
    this.executeFilter();
  }
  createApplication() {
    this.modalService.open('newAppModal');
  }

  closeCreateAppModal() {
    this.modalService.close('newAppModal');
  }

  createGroup() {
    this.modalService.open('createGroupModal');
  }

  onGroupCreated() {
    this.modalService.close('createGroupModal');
  }

  ngOnDestroy(): void {
    this.appSub.unsubscribe();
  }

  private executeFilter() {
    var apps = this.allApps.filter(x => x.name.toLowerCase().indexOf(this.filterText) !== -1);
    if (this.selectedGroup != null) {
      apps = apps.filter(x => x.groupIds.includes(this.selectedGroup.id));
    }
    this.applications = apps;
  }
}

interface PartitionOverview {
  displayName: string;
  count: number;
}
interface ApplicationOverview {
  id: number;
  groupIds: number[];
  name: string;
  errors: number;
  latestReport: string | null;
  reports: number;
  latestError: string | null;
  followers: number;
  bugReports: number;
  partitions: PartitionOverview[];
}

interface IGroup {
  id: number;
  name: string;
}
