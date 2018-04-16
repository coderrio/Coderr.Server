import { AppRoot } from "../../services/AppRoot";
import { ApiClient } from '../../services/ApiClient';
import { GetApplicationInfo, GetApplicationInfoResult, GetApplicationOverview, GetApplicationOverviewResult } from "../../dto/Core/Applications";
import Vue from "vue";
import { Component } from "vue-property-decorator";
import Chartist from "chartist";
import 'chartist/dist/chartist.css';
import * as moment from 'moment';
import * as Incidents from "../../dto/Core/Incidents";
import FindIncidentsResultItem = Incidents.FindIncidentsResultItem;


interface ISeries {
    name?: string;
    data: number[];
}

@Component
export default class ApplicationDetailsComponent extends Vue {
    private static activeBtnTheme: string = 'btn-dark';
    private apiClient: ApiClient = AppRoot.Instance.apiClient;

    //chartData: [['Jan', 44], ['Feb', 27], ['Mar', 60], ['Apr', 55], ['May', 37], ['Jun', 40], ['Jul', 69], ['Aug', 33], ['Sept', 76], ['Oct', 90], ['Nov', 34], ['Dec', 22]];

    applicationId: number = 0;
    name: string = '';
    versions: string = '';

    // summary, changes when time window changes
    reportCount: number = 0;
    incidentCount:number= 0;
    feedbackCount: number;
    followers: number;

    incidents: FindIncidentsResultItem[] = [];
    myIncidents: FindIncidentsResultItem[] = [];

    created() {
        this.applicationId = parseInt(this.$route.params.applicationId, 10);

        var query = new GetApplicationInfo();
        query.ApplicationId = this.applicationId;
        this.apiClient.query<GetApplicationInfoResult>(query)
            .then(x => {
                this.applicationId = x.Id;
                this.name = x.Name;
                this.versions = 'v' + x.Versions.join(', v');
            });

        var q2 = new GetApplicationOverview();
        q2.ApplicationId = this.applicationId;
        this.apiClient.query<GetApplicationOverviewResult>(q2)
            .then(x => {
                this.incidentCount = x.StatSummary.Incidents;
                this.reportCount = x.StatSummary.Reports;
                this.feedbackCount = x.StatSummary.UserFeedback;
                this.followers = x.StatSummary.Followers;
                this.displayChart(x);
            });
        AppRoot.Instance.incidentService.find(this.applicationId)
            .then(result => this.incidents = result.Items);

        AppRoot.Instance.incidentService.getMine(this.applicationId)
            .then(result => this.myIncidents = result);
    }

    mounted() {
        //this.name = 'Arne';
    }

    private copyObject(source: any, destination: any) {
        for (var prop in source) {
            if (source.hasOwnProperty(prop)) {
                destination[prop] = source[prop];
                //Vue.set(destination, prop, source[prop]);
            }
        }
    }

    private displayChart(stats: GetApplicationOverviewResult) {
        var labels: Date[] = [];
        var series: ISeries[] = [];
        series.push({
            name: 'Incidents',
            data: stats.Incidents
        });
        series.push({
            name: 'Reports',
            data: stats.Incidents
        });

        for (var i = 0; i < stats.TimeAxisLabels.length; i++) {
            var value = new Date(stats.TimeAxisLabels[i]);
            labels.push(value);
        }

        var options = {
            axisY: {
                onlyInteger: true,
                offset: 0
            },
            axisX: {
                labelInterpolationFnc(value: any, index: number, labels: any) {
                    if (index % 3 !== 0) {
                        return '';
                    }
                    return moment(value).format('MMM D');
                }
            }
        };
        var data = {
            labels: labels,
            series: series
        };
        new Chartist.Line('.ct-chart', data, options);
    }

}
