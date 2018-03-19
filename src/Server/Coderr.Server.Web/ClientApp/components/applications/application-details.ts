import { ApiClient } from '../../services/ApiClient';
import { GetApplicationInfo, GetApplicationInfoResult, GetApplicationOverview, GetApplicationOverviewResult } from "../../dto/Core/Applications";
import Vue from "vue";
import { Component } from "vue-property-decorator";

@Component
export default class ApplicationDetailsComponent extends Vue {
    private static activeBtnTheme: string = 'btn-dark';
    private apiClient: ApiClient = new ApiClient('http://localhost:50473/cqs/');

    //chartData: [['Jan', 44], ['Feb', 27], ['Mar', 60], ['Apr', 55], ['May', 37], ['Jun', 40], ['Jul', 69], ['Aug', 33], ['Sept', 76], ['Oct', 90], ['Nov', 34], ['Dec', 22]];

    applicationId: number = 0;
    name: string = '';
    versions: string = '';

    // summary, changes when time window changes
    reportCount: number = 0;
    incidentCount:number= 0;
    feedbackCount: number;
    followers: number;

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

                //this.chartData.labels = x.TimeAxisLabels;
                //this.chartData.datasets = [];

                //var item1 = new ChartDataSet();
                //item1.label = 'Incidents';
                //item1.data = x.Incidents;
                //this.chartData.datasets.push(item1);

                //var item2 = new ChartDataSet();
                //item2.label = 'Reports';
                //item2.data = x.ErrorReports;
                //this.chartData.datasets.push(item2);

                //var d = new LineChart('');
                //d.render(this.chartData);
            });
    }

    mounted() {
        //this.name = 'Arne';
    }

    private copyObject(source: any, destination: any) {
        for (var prop in source) {
            if (source.hasOwnProperty(prop)) {
                console.log('copying' + source[prop]);
                destination[prop] = source[prop];
                //Vue.set(destination, prop, source[prop]);
            }
        }
    }

}
