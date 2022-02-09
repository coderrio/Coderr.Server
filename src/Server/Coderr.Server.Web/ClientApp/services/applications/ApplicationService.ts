import { PubSubService, Message } from '../PubSub';
import { ApiClient } from '../ApiClient';
import { AppRoot } from '../AppRoot';
import * as dto from "@/dto/Core/Applications"

export class AppEvents {
    static readonly Created: string = "application/created";
    static readonly Removed: string = "application/removed";
    static readonly Updated: string = "application/updated";
    static readonly Selected: string = "application/selected";
};

export class ApplicationChanged {
    applicationId: number;
}

export class ApplicationCreated {
    readonly id: number;
    readonly name: string;
    readonly sharedSecret: string;
    readonly appKey: string;

    constructor(id: number, name: string, sharedSecret: string, appKey: string) {
        this.id = id;
        this.name = name;
        this.sharedSecret = sharedSecret;
        this.appKey = appKey;
    }
}

export class ApplicationSummary {
    readonly id: number;
    readonly name: string;
    groupId: number;
    sharedSecret: string;
    appKey: string;

    /**
     * Number of days to keep old incidents.
     */
    retentionDays: number;

    constructor(id: number, name: string, sharedSecret: string, appKey: string) {
        this.id = id;
        this.name = name;
        this.sharedSecret = sharedSecret;
        this.appKey = appKey;
    }
    Team: ApplicationMember[];
}
export interface ApplicationMember {
    id: number;
    name: string;
}

export class AppLink {
    readonly id: number;
    readonly name: string;
}

export interface ApplicationToCreate {
    name: string;
    appKey: string;
}

export interface ApplicationGroup {
    id: number;
    name: string;
}
//const IocService = (): ClassDecorator => {
//    return (target:Function) => {
//        console.log(Reflect.getMetadata('design:paramtypes', target));
//    };
//};

class Guid {
    static newGuid() {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    }
}


//@IocService()
export class ApplicationService {
    private applications: ApplicationSummary[] = [];
    private fetchPromise: Promise<dto.ApplicationListItem[]>;
    private applicationGroups: ApplicationGroup[] = [];

    constructor(private pubSub: PubSubService, private apiClient: ApiClient) {
        if (!pubSub) {
            throw new Error("PubSub must be specified");
        }
        if (!apiClient) {
            throw new Error("ApiClient must be specified");
        }
    }

    changeApplication(applicationId: number) {
        if (AppRoot.Instance.currentApplicationId === applicationId) {
            return;
        }

        var body = new ApplicationChanged();
        body.applicationId = applicationId;
        AppRoot.Instance.currentApplicationId = applicationId;
        this.pubSub.publish(AppEvents.Selected, body);
    }

    async list(): Promise<ApplicationSummary[]> {
        if (this.applications.length > 0)
            return this.applications;

        // when someone else already request apps
        // so we have a pending promise.
        if (this.fetchPromise) {
            await this.fetchPromise;
            return this.applications;
        }

        var q1 = new dto.GetApplicationGroupMap();
        var groupResult = await this.apiClient.query<dto.GetApplicationGroupMapResult>(q1);


        var query = new dto.GetApplicationList();
        this.fetchPromise = this.apiClient.query<dto.ApplicationListItem[]>(query);
        var result = await this.fetchPromise;

        result.forEach(app => {
            var dto = new ApplicationSummary(app.Id, app.Name, 'n/a', 'n/a');
            dto.retentionDays = app.RetentionDays;
            var map = groupResult.Items.find(x => x.ApplicationId === app.Id);
            if (map) {
                dto.groupId = map.GroupId;
            } else {
                console.log('failed to find map for ' + app.Id + " in ", groupResult, "AllApps: ", result);
            }

            this.applications.push(dto);
        });

        return this.applications;
    }

    async get(appId: number): Promise<ApplicationSummary> {
        if (appId === -1 && this.applications.length > 0) {
            console.log(this.applications);
            appId = this.applications[0].id;
        }

        for (let i = 0; i < this.applications.length; i++) {
            if (this.applications[i].id !== appId)
                continue;

            if (this.applications[i].appKey === 'n/a') {
                const appQuery = new dto.GetApplicationInfo();
                appQuery.ApplicationId = appId;
                const appInfo = await this.apiClient.query<dto.GetApplicationInfoResult>(appQuery);
                this.applications[i].appKey = appInfo.AppKey;
                this.applications[i].sharedSecret = appInfo.SharedSecret;
                this.applications[i].retentionDays = appInfo.RetentionDays;
            }

            return this.applications[i];
        }

        const q = new dto.GetApplicationInfo();
        q.ApplicationId = appId;
        const result = await this.apiClient.query<dto.GetApplicationInfoResult>(q);

        var q1 = new dto.GetApplicationGroupMap();
        q1.ApplicationId = q.ApplicationId;
        var groupResult = await this.apiClient.query<dto.GetApplicationGroupMapResult>(q1);

        var summary: ApplicationSummary = {
            id: result.Id,
            name: result.Name,
            groupId: groupResult.Items[0].GroupId,
            retentionDays: result.RetentionDays,
            appKey: result.AppKey,
            sharedSecret: result.SharedSecret,
            Team: []
        };
        return summary;
    }

    async create(groupId: number, applicationName: string, fullTimeDevelopers?: number, numberOfErrors?: number, retentionDays?: number): Promise<string> {
        if (!groupId) {
            throw new Error("Application name must be specified.");
        }
        if (!applicationName) {
            throw new Error("Application name must be specified.");
        }

        var cmd = new dto.CreateApplication();
        cmd.GroupId = groupId;
        cmd.Name = applicationName;
        cmd.ApplicationKey = Guid.newGuid().replace(/\-/g, '');
        cmd.TypeOfApplication = dto.TypeOfApplication.DesktopApplication;
        cmd.NumberOfDevelopers = fullTimeDevelopers;
        cmd.NumberOfErrors = numberOfErrors;
        cmd.RetentionDays = retentionDays;
        await this.apiClient.command(cmd);
        return cmd.ApplicationKey;
    }

    async delete(applicationId: number) {
        var cmd = new dto.DeleteApplication();
        cmd.Id = applicationId;
        await this.apiClient.command(cmd);

        this.applications = this.applications.filter(x => x.id !== applicationId);
        PubSubService.Instance.publish(AppEvents.Removed, applicationId);
    }

    async getTeam(id: number): Promise<ApplicationMember[]> {
        if (id === 0) {
            throw new Error("Expected an incidentId");
        }

        var q = new dto.GetApplicationTeam();
        q.ApplicationId = id;
        var result = await this.apiClient.query<dto.GetApplicationTeamResult>(q);
        var members: ApplicationMember[] = [];
        result.Members.forEach(x => {
            members.push({
                id: x.UserId,
                name: x.UserName
            });
        });
        return members;
    }

    update(applicationId: number, applicationName: string, retentionDays?: number) {
        const cmd = new dto.UpdateApplication();
        cmd.Name = applicationName;
        cmd.ApplicationId = applicationId;
        cmd.RetentionDays = retentionDays;
        AppRoot.Instance.apiClient.command(cmd);
    }

    async createGroup(name: string): Promise<ApplicationGroup> {
        const cmd = new dto.CreateApplicationGroup();
        cmd.Name = name;
        AppRoot.Instance.apiClient.command(cmd);

        let triesLeft = 3;
        while (triesLeft-- > 0) {
            const query = new dto.GetApplicationGroups();
            const result = await AppRoot.Instance.apiClient.query<dto.GetApplicationGroupsResult>(query);
            let foundGroup: ApplicationGroup = null;
            result.Items.find(x => {
                if (x.Name === name) {
                    const group = {
                        id: x.Id,
                        name: name
                    };
                    this.applicationGroups.push(group);
                    foundGroup = group;
                    return true;
                }

                return false;
            });
            if (foundGroup) {
                return foundGroup;
            }

            const promise = new Promise<object>((resolve, reject) => {
                setTimeout(() => {
                        resolve(null);
                    },
                    500);
            });
            await promise;
        }

        throw new Error(`Failed to fetch created group '${name}'.`);
    }

    /**
     * 
     * @param applicationId
     * @param groupIdOrName Either id or name
     */
    setGroup(applicationId: number, groupId: number) {
        var item = this.applications.find(x => x.id === applicationId);
        item.groupId = groupId;

        var cmd = new dto.SetApplicationGroup();
        cmd.ApplicationId = applicationId;
        cmd.ApplicationGroupId = groupId;
        this.apiClient.command(cmd);
    }

    async getGroups(): Promise<ApplicationGroup[]> {
        if (this.applicationGroups.length > 0) {
            return this.applicationGroups;
        }

        var groupsQuery = new dto.GetApplicationGroups();
        var result2 = await this.apiClient.query<dto.GetApplicationGroupsResult>(groupsQuery);
        if (this.applicationGroups.length > 0) {
            return this.applicationGroups;
        }

        result2.Items.forEach(item => {
            this.applicationGroups.push({
                id: item.Id,
                name: item.Name
            });
        });

        return this.applicationGroups;
    }
}

