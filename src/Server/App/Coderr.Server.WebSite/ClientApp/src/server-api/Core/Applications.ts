export class ApplicationListItem {
  public static TYPE_NAME: string = 'ApplicationListItem';
  public id: number;
  public name: string;
  public isAdmin: boolean;
  public retentionDays: number;
}
export enum TypeOfApplication {
  Mobile = 0,
  DesktopApplication = 1,
  Server = 2,
}
export class GetApplicationIdByKey {
  public static TYPE_NAME: string = 'GetApplicationIdByKey';
  public applicationKey: string;
}
export class GetApplicationIdByKeyResult {
  public static TYPE_NAME: string = 'GetApplicationIdByKeyResult';
  public id: number;
}
export class GetApplicationInfo {
  public static TYPE_NAME: string = 'GetApplicationInfo';
  public appKey: string;
  public applicationId: number;
}
export class GetApplicationInfoResult {
  public static TYPE_NAME: string = 'GetApplicationInfoResult';
  public appKey: string;
  public applicationType: TypeOfApplication;
  public id: number;
  public name: string;
  public sharedSecret: string;
  public totalIncidentCount: number;
  public lastIncidentAtUtc: Date;
  public numberOfDevelopers: number;
  public versions: string[];
  public showStatsQuestion: boolean;
  public retentionDays: number;
}
export class GetApplicationList {
  public static TYPE_NAME: string = 'GetApplicationList';
  public accountId: number;
  public filterAsAdmin: boolean;
}
export class GetApplicationOverview {
  public static TYPE_NAME: string = 'GetApplicationOverview';
  public applicationId: number;
  public numberOfDays: number;
  public version: string;
  public includeChartData: boolean = true;
  public includePartitions: boolean;
}
export class GetApplicationOverviewResult {
  public static TYPE_NAME: string = 'GetApplicationOverviewResult';
  public days: number;
  public errorReports: number[];
  public incidents: number[];
  public statSummary: OverviewStatSummary;
  public timeAxisLabels: string[];
}
export class GetApplicationTeam {
  public static TYPE_NAME: string = 'GetApplicationTeam';
  public applicationId: number;
}
export class GetApplicationTeamMember {
  public static TYPE_NAME: string = 'GetApplicationTeamMember';
  public joinedAtUtc: Date;
  public userId: number;
  public userName: string;
  public isAdmin: boolean;
}
export class GetApplicationTeamResult {
  public static TYPE_NAME: string = 'GetApplicationTeamResult';
  public invited: GetApplicationTeamResultInvitation[];
  public members: GetApplicationTeamMember[];
}
export class GetApplicationTeamResultInvitation {
  public static TYPE_NAME: string = 'GetApplicationTeamResultInvitation';
  public emailAddress: string;
  public invitedAtUtc: Date;
  public invitedByUserName: string;
}
export class OverviewStatSummary {
  public followers: number;
  public incidents: number;
  public newestIncidentReceivedAtUtc?: string;
  public reports: number;
  public newestReportReceivedAtUtc?: string;
  public userFeedback: number;
  public partitions: PartitionOverview[];
}
export class PartitionOverview {
  public name: string;
  public displayName: string;
  public value: number;
}
export class ApplicationCreated {
  public static TYPE_NAME: string = 'ApplicationCreated';
  public appKey: string;
  public applicationId: number;
  public applicationName: string;
  public createdById: number;
  public sharedSecret: string;
}
export class ApplicationDeleted {
  public static TYPE_NAME: string = 'ApplicationDeleted';
  public appKey: string;
  public applicationId: number;
  public applicationName: string;
}
export class UserAddedToApplication {
  public static TYPE_NAME: string = 'UserAddedToApplication';
  public accountId: number;
  public applicationId: number;
}
export class UserInvitedToApplication {
  public static TYPE_NAME: string = 'UserInvitedToApplication';
  public applicationId: number;
  public applicationName: string;
  public emailAddress: string;
  public invitationKey: string;
  public invitedBy: string;
}
export class CreateApplication {
  public static TYPE_NAME: string = 'CreateApplication';
  public groupId?: number;
  public applicationKey: string;
  public name: string;
  public typeOfApplication: TypeOfApplication;
  public numberOfDevelopers?: number;
  public numberOfErrors?: number;
  public retentionDays?: number;
}
export class DeleteApplication {
  public static TYPE_NAME: string = 'DeleteApplication';
  public id: number;
}
export class RemoveTeamMember {
  public static TYPE_NAME: string = 'RemoveTeamMember';
  public applicationId: number;
  public userToRemove: number;
}
export class UpdateApplication {
  public static TYPE_NAME: string = 'UpdateApplication';
  public applicationId: number;
  public name: string;
  public typeOfApplication: TypeOfApplication | null;
  public retentionDays?: number;
}
export class AddTeamMember {
  public static TYPE_NAME: string = 'AddTeamMember';
  public applicationId: number;
  public userToAdd: number;
  public roles: string[];
}
export class UpdateRoles {
  public static TYPE_NAME: string = 'UpdateRoles';
  public applicationId: number;
  public userToUpdate: number;
  public roles: string[];
}


export class GetApplicationVersions {
  public static TYPE_NAME: string = 'GetApplicationVersions';
  public applicationId: number;
}

export class GetApplicationVersionsResult {
  public items: GetApplicationVersionsResultItem[];
}

export class GetApplicationVersionsResultItem {
  public firstReportReceivedAtUtc: Date;
  public incidentCount: number;
  public lastReportReceivedAtUtc: Date;
  public reportCount: number;
  public version: string;
}

export class GetApplicationGroups {
  public static TYPE_NAME: string = 'GetApplicationGroups';
}

export class GetApplicationGroupsResult {
  public items: GetApplicationGroupsResultItem[];
}

export class GetApplicationGroupsResultItem {
  public id: number;
  public name: string;
}

export class MapApplicationsToGroup {
  public static TYPE_NAME: string = "MapApplicationsToGroup";
  public groupId: number;
  public applicationIds: number[];
}

export class RenameApplicationGroup {
  public static TYPE_NAME: string = "RenameApplicationGroup";
  public groupId: number;
  public newName: string;
}

export class GetApplicationGroupMap {
  public static TYPE_NAME: string = 'GetApplicationGroupMap';
  public applicationId?: number;
}

export class GetApplicationGroupMapResult {
  public items: GetApplicationGroupMapResultItem[];
}


export class GetApplicationGroupMapResultItem {
  public applicationId: number;
  public groupId: number;
}

export class CreateApplicationGroup {
  public static TYPE_NAME: string = 'CreateApplicationGroup';
  public name: string;
}

export class SetApplicationGroup {
  public static TYPE_NAME: string = 'SetApplicationGroup';
  public applicationId: number;
  public applicationGroupId: number;
  public appKey: string;
  public groupName: string;
}


export class DeleteApplicationGroup {
  public static TYPE_NAME: string = 'DeleteApplicationGroup';
  public groupId: number;
  public moveAppsToGroupId: number;
}
