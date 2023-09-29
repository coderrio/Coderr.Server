import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ApiClient, HttpClient } from "../../../utils/HttpClient";
import { FindIncidents, FindIncidentsResult } from "../../../../server-api/Core/Incidents";
import { ApplicationService } from "../../application.service";
declare var window: any;

interface ILibrarySummary {
  clientFolderName: string;
  description: string;
  id: string;
  selected: boolean;


  /** ".net" or "js" */
  frameworkType: string;

  categories: string[];
}

@Component({
  selector: 'app-configure',
  templateUrl: './configure.component.html',
  styleUrls: ['./configure.component.scss']
})
export class ConfigureComponent implements OnInit {
  private lastLib: ILibrarySummary;
  libraries: ILibrarySummary[] = [];
  allLibraries: ILibrarySummary[] = [];
  instruction: string | null = null;
  frameworkNames: string[] = [];
  selectedFramework = "";
  getStartedVisible = true;

  applicationId = 0;
  appKey = "";
  sharedSecret = "";
  reportUrl = "";

  noConnection = false;
  weAreInTrouble = false;

  constructor(
    private apiClient: ApiClient,
    private httpClient: HttpClient,
    private applicationService: ApplicationService,
    activatedRoute: ActivatedRoute) {
    this.applicationId = activatedRoute.snapshot.params["applicationId"];
  }

  ngOnInit(): void {
    if (!this.applicationId) {
      this.applicationService.list().then(x => {
        if (x.length > 0) {
          this.applicationId = x[0].id;
        }
      });
    } else {
      this.load();
    }
    
  }

  @Input()
  set showGetStarted(value: boolean) {
    this.getStartedVisible = value;
  }

  async load(): Promise<void> {
    var app = await this.applicationService.get(this.applicationId);
    this.appKey = app.appKey;
    this.sharedSecret = app.sharedSecret;

    var response = await this.httpClient.get('/api/onboarding/libraries/');
    try {
      var data = <ILibrarySummary[]>response.body;
      data.forEach(lib => {
        lib.selected = false;
        this.allLibraries.push(lib);
        if (!this.frameworkNames.includes(lib.frameworkType)) {
          this.frameworkNames.push(lib.frameworkType);
        }
      });
    }
    catch (error) {
      this.noConnection = true;
    }

    var pos = window.location.href.toString().indexOf('/discover/');
    this.reportUrl = window.location.href.toString().substr(0, pos + 1);
  }

  async selectFramework(name: string): Promise<void> {
    this.selectedFramework = name;
    this.libraries = this.allLibraries
      .filter(x => x.frameworkType === name)
      .sort((x, y) => {
        return x.id.localeCompare(y.id);
      });
  }

  async selectLibrary(lib: ILibrarySummary): Promise<void> {
    this.lastLib = lib;
    var url = '/api/onboarding/library/' + lib.id + "/?appKey=" + this.appKey + "&type=" + lib.frameworkType;
    var response = await this.httpClient.get(url);

    this.instruction = response.body
      .replace('yourAppKey', this.appKey)
      .replace('yourSharedSecret', this.sharedSecret);
  }

  tagStr(lib: ILibrarySummary) {
    return lib.categories.join(', ');
  }

  completedConfiguration() {
    var q = new FindIncidents();
    q.pageNumber = 1;
    q.itemsPerPage = 1;
    q.applicationIds = [this.applicationId];
    this.apiClient.query<FindIncidentsResult>(q)
      .then(result => {
        if (result.totalCount === 0) {
          this.weAreInTrouble = true;
          return;
        }

        //this.$router.push({
        //  name: "discover",
        //  params: { applicationId: this.applicationId.toString() }
        //});
      });

  }

}
