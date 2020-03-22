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
    sharedSecret: string;
    appKey: string;

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

        console.log('changing app', applicationId);
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

        var query = new dto.GetApplicationList();
        this.fetchPromise = this.apiClient.query<dto.ApplicationListItem[]>(query);
        var result = await this.fetchPromise;

        result.forEach(app => {
            var dto = new ApplicationSummary(app.Id, app.Name, 'n/a', 'n/a');
            this.applications.push(dto);
        });
        return this.applications;
    }

    async get(appId: number): Promise<ApplicationSummary> {
        for (var i = 0; i < this.applications.length; i++) {
            if (this.applications[i].id !== appId)
                continue;

            if (this.applications[i].appKey === 'n/a') {
                const appQuery = new dto.GetApplicationInfo();
                appQuery.ApplicationId = appId;
                const appInfo = await this.apiClient.query<dto.GetApplicationInfoResult>(appQuery);
                this.applications[i].appKey = appInfo.AppKey;
                this.applications[i].sharedSecret = appInfo.SharedSecret;
            }

            return this.applications[i];
        }

        const q = new dto.GetApplicationInfo();
        q.ApplicationId = appId;
        const result = await this.apiClient.query<dto.GetApplicationInfoResult>(q);

        var summary: ApplicationSummary = {
            id: result.Id,
            name: result.Name,
            appKey: result.AppKey,
            sharedSecret: result.SharedSecret,
            Team: []
        };
        return summary;
    }

    async create(applicationName: string, fullTimeDevelopers?: number, numberOfErrors?: number): Promise<string> {
        if (!applicationName) {
            throw new Error("Application name must be specified.");
        }

        var cmd = new dto.CreateApplication();
        cmd.Name = applicationName;
        cmd.ApplicationKey = Guid.newGuid().replace(/\-/g, '');
        cmd.TypeOfApplication = dto.TypeOfApplication.DesktopApplication;
        cmd.NumberOfDevelopers = fullTimeDevelopers;
        cmd.NumberOfErrors = numberOfErrors;

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

    update(applicationId: number, applicationName: string) {
        const cmd = new dto.UpdateApplication();
        cmd.Name = applicationName;
        cmd.ApplicationId = applicationId;
        AppRoot.Instance.apiClient.command(cmd);
    }
}

