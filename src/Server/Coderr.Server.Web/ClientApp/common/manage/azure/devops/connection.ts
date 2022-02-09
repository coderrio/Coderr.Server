import { AppRoot } from '@/services/AppRoot';
import * as DevOps from "@/dto/Common/Azure/DevOps";
import { Component, Mixins } from "vue-property-decorator";
import { AppAware } from "@/AppMixins";

interface IItem {
    id: string;
    text: string;
}

@Component
export default class AzureDevOpsConnectionComponent extends Mixins(AppAware) {
    applicationId: number = 0;
    applicationName: string = null;
    personalAccessToken: string = null;
    url: string = null;
    project: IItem = null;
    areaPath: IItem = null;
    projects: IItem[] = [];
    areas: IItem[] = [];
    iterations: IItem[] = [];
    isConnected = false;
    showConnectError = false;
    hangOn = false;
    isSaved = false;

    // state fields
    workItemTypes: string[] = [];
    stateTypes: string[] = [];
    isWorkItemTypeSelected = false;
    autoAddImportant = false;
    autoAddCritical = false;
    assignedStateName: string = null;
    closedStateName: string = null;
    solvedStateName: string = null;
    workItemTypeName: string = null;

    mounted() {
        this.onApplicationChanged(this.onAppSelected);
        this.applicationId = parseInt(this.$route.params.applicationId);
        AppRoot.Instance.applicationService.get(this.applicationId)
            .then(x => this.applicationName = x.name);
        this.load();
    }

    tryConnect() {
        this.load();
    }

    save() {
        var cmd = new DevOps.SaveSettings();
        cmd.ApplicationId = this.applicationId;
        cmd.AreaPathId = this.areaPath.id;
        cmd.AreaPath = this.areaPath.text;
        cmd.ProjectId = this.project.id;
        cmd.ProjectName = this.project.text;
        cmd.PersonalAccessToken = this.personalAccessToken;
        cmd.Url = this.url;
        cmd.AutoAddCritical = this.autoAddCritical;
        cmd.AutoAddImportant = this.autoAddImportant;

        cmd.AssignedStateName = this.assignedStateName;
        cmd.ClosedStateName = this.closedStateName;
        cmd.SolvedStateName = this.solvedStateName;
        cmd.WorkItemTypeName = this.workItemTypeName;

        AppRoot.Instance.apiClient.command(cmd);
        this.isSaved = true;
        AppRoot.notify("Settings have been saved.");
    }

    private onAppSelected(applicationId: number) {
        this.clear();
        this.applicationId = applicationId;
        AppRoot.Instance.applicationService.get(this.applicationId)
            .then(x => this.applicationName = x.name);
        this.load();
    }

    onWorkItemTypeChanged() {
        if (this.workItemTypeName != null) {
            this.isWorkItemTypeSelected = true;
            this.loadStateTypes();
        } else {
            this.isWorkItemTypeSelected = false;
        }
    }

    onProjectChanged() {
        this.loadAreas();
        this.loadWorkItemTypes();
        if (this.workItemTypeName != null) {
            this.loadStateTypes();
        }
    }

    private clear() {
        this.projects = [];
        this.areas = [];
        this.areaPath = null;
        this.project = null;
        this.isSaved = false;
        this.isConnected = false;
    }

    private async load(): Promise<object> {
        if (!this.personalAccessToken) {
            var query = new DevOps.GetSettings();
            query.ApplicationId = this.applicationId;
            var result = await AppRoot.Instance.apiClient.query<DevOps.GetSettingsResult>(query);
            if (result != null) {
                this.isSaved = true;
                this.url = result.Url;
                this.personalAccessToken = result.PersonalAccessToken;
                this.autoAddCritical = result.AutoAddCritical;
                this.autoAddImportant = result.AutoAddImportant;

                if (result.ProjectName) {
                    this.project = { id: result.ProjectId, text: result.ProjectName };
                }

                if (result.AreaPath) {
                    this.areaPath = { id: result.AreaPathId, text: result.AreaPath };
                }

                this.assignedStateName = result.AssignedStateName;
                this.closedStateName = result.ClosedStateName;
                this.solvedStateName = result.SolvedStateName;
                this.workItemTypeName = result.WorkItemTypeName;
                if (this.isWorkItemTypeSelected) {
                    this.loadStateTypes();
                }
            }
        }

        if (!this.url || !this.personalAccessToken) {
            return;
        }

        if (this.projects.length === 0) {
            var couldConnect = await this.loadProjects();
            this.showConnectError = !couldConnect;
            this.isConnected = couldConnect;
            if (!this.isConnected) {
                return;
            }
        }

        // we do not wait here to load in parallel.

        if (this.areas.length === 0 && this.project) {
            this.loadAreas();
        }
        if (this.iterations.length === 0 && this.project) {
            this.loadIterations();
        }

        if (this.project) {
            this.loadWorkItemTypes();
        }
    }

    private async loadProjects(): Promise<boolean> {

        this.showConnectError = false;
        this.hangOn = true;
        var dto = new DevOps.GetProjects();
        dto.PersonalAccessToken = this.personalAccessToken;
        dto.Url = this.url;

        let result = await AppRoot.Instance.apiClient.query<DevOps.GetProjectsResult>(dto);
        this.hangOn = false;
        if (result.Items.length === 0) {
            return false;
        }
        result.Items.forEach(item => {
            this.projects.push({
                id: item.Id,
                text: item.Name
            });
        });

        return true;
    }

    private async loadAreas(): Promise<boolean> {
        var dto = new DevOps.GetAreaPaths();
        dto.PersonalAccessToken = this.personalAccessToken;
        dto.ProjectNameOrId = this.project.id;
        dto.Url = this.url;

        this.areas = [];
        let result = await AppRoot.Instance.apiClient.query<DevOps.GetAreaPathsResult>(dto);
        if (result.Items.length === 0) {
            return false;
        }
        result.Items.forEach(item => {
            this.areas.push({
                id: item.Path,
                text: item.Path
            });
        });

        return true;
    }

    private async loadWorkItemTypes(): Promise<boolean> {
        var query = new DevOps.GetWorkItemTypes();
        query.ProjectNameOrId = this.project.id;
        query.PersonalAccessToken = this.personalAccessToken;
        query.Url = this.url;

        let result = await AppRoot.Instance.apiClient.query<DevOps.GetWorkItemTypesResult>(query);
        if (result.Items.length === 0) {
            return false;
        }
        this.workItemTypes = [];
        result.Items.forEach(item => {
            this.workItemTypes.push(item.Name);
        });

        let isLoaded = false;

        if (this.workItemTypeName != null) {
            if (!this.workItemTypes.find(x => x === this.workItemTypeName)) {
                this.workItemTypeName = null;
                this.isWorkItemTypeSelected = false;
            } else {
                isLoaded = true;
            }
        }
        if (this.workItemTypeName == null) {
            if (this.workItemTypes.find(x => x === "Bug")) {
                this.workItemTypeName = "Bug";
                isLoaded = true;
            }
            else if (this.workItemTypes.find(x => x === "Issue")) {
                this.workItemTypeName = "Issue";
                isLoaded = true;
            }
        }
        if (isLoaded) {
            this.isWorkItemTypeSelected = true;
            this.loadStateTypes();
        }

        return true;
    }

    private async loadStateTypes(): Promise<boolean> {
        var query = new DevOps.GetWorkItemStates();
        query.ProjectId = this.project.id;
        query.PersonalAccessToken = this.personalAccessToken;
        query.Url = this.url;
        query.WorkItemTypeName = this.workItemTypeName;

        let result = await AppRoot.Instance.apiClient.query<DevOps.GetWorkItemStatesResult>(query);
        this.stateTypes = [];
        if (result.Items.length === 0) {
            return false;
        }
        result.Items.forEach(item => {
            this.stateTypes.push(item.Name);
        });

        if (!this.assignedStateName) {
            if (this.stateTypes.find(x => x === "Committed")) {
                this.assignedStateName = "Committed";
            }
            else if (this.stateTypes.find(x => x === "Active")) {
                this.assignedStateName = "Active";
            }
            else if (this.stateTypes.find(x => x === "Doing")) {
                this.assignedStateName = "Doing";
            }
            else if (this.stateTypes.find(x => x === "Assigned")) {
                this.assignedStateName = "Assigned";
            }
        }

        if (!this.solvedStateName) {
            if (this.stateTypes.find(x => x === "Resolved")) {
                this.solvedStateName = "Resolved";
            }
            else if (this.stateTypes.find(x => x === "Done")) {
                this.solvedStateName = "Done";
            }
        }
        if (!this.closedStateName) {
            if (this.stateTypes.find(x => x === "Closed")) {
                this.closedStateName = "Closed";
            }
            else if (this.stateTypes.find(x => x === "Removed")) {
                this.closedStateName = "Removed";
            }
            else if (this.stateTypes.find(x => x === "Done")) {
                this.closedStateName = "Done";
            }
        }
        return true;
    }

    private async loadIterations(): Promise<boolean> {
        var dto = new DevOps.GetIterations();
        dto.ProjectNameOrId = this.project.id;
        dto.PersonalAccessToken = this.personalAccessToken;
        dto.Url = this.url;

        let result = await AppRoot.Instance.apiClient.query<DevOps.GetIterationPathsResult>(dto);
        if (result.Items.length === 0) {
            return false;
        }
        result.Items.forEach(item => {
            this.iterations.push({
                id: item.Id,
                text: item.Name
            });
        });

        return true;
    }
}