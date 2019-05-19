import { AppRoot } from '../../../../services/AppRoot';
import { GetEnvironments, GetEnvironmentsResult } from "../../../../dto/Core/Environments";
import Vue from "vue";
import { Component, Watch } from "vue-property-decorator";
import Applications = require("../../../../dto/Core/Applications");
import GetApplicationTeamResult = Applications.GetApplicationTeamResult;
import Environments = require("../../../../dto/Core/Environments");

interface IEnvironment {
    name: string;
    id: number;
}

@Component
export default class ManageEnvironmentsComponent extends Vue {
    applicationId: number = 0;
    environments: IEnvironment[] = [];


    mounted() {
        this.load();
    }

    @Watch('$route.params.applicationId')
    onApplicationChanged(value: string, oldValue: string) {
        this.applicationId = parseInt(value);
        this.load();
    }

    resetEnvironment(environmentId: number) {

    }


    private load() {

        var q = new GetEnvironments();
        q.ApplicationId = this.applicationId;
        AppRoot.Instance.apiClient.query<GetEnvironmentsResult>(q)
            .then(result => {
                result.Items.forEach(item =>
                    this.environments.push({ id: item.Id, name: item.Name });
            });
    }
}
