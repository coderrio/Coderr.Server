export class ApplicationListItem {
    public static TYPE_NAME: string = 'ApplicationListItem';
    public Id: number;
    public Name: string;
    public IsAdmin: boolean;
}
export enum TypeOfApplication {
    Mobile = 0,
    DesktopApplication = 1,
    Server = 2,
}
export class GetApplicationIdByKey {
    public static TYPE_NAME: string = 'GetApplicationIdByKey';
    public ApplicationKey: string;
}
export class GetApplicationIdByKeyResult {
    public static TYPE_NAME: string = 'GetApplicationIdByKeyResult';
    public Id: number;
}
export class GetApplicationInfo {
    public static TYPE_NAME: string = 'GetApplicationInfo';
    public AppKey: string;
    public ApplicationId: number;
}
export class GetApplicationInfoResult {
    public static TYPE_NAME: string = 'GetApplicationInfoResult';
    public AppKey: string;
    public ApplicationType: TypeOfApplication;
    public Id: number;
    public Name: string;
    public SharedSecret: string;
    public TotalIncidentCount: number;
    public Versions: string[];
}
export class GetApplicationList {
    public static TYPE_NAME: string = 'GetApplicationList';
    public AccountId: number;
    public FilterAsAdmin: boolean;
}
export class GetApplicationOverview {
    public static TYPE_NAME: string = 'GetApplicationOverview';
    public ApplicationId: number;
    public NumberOfDays: number;
    public Version: string;
}
export class GetApplicationOverviewResult {
    public static TYPE_NAME: string = 'GetApplicationOverviewResult';
    public Days: number;
    public ErrorReports: number[];
    public Incidents: number[];
    public StatSummary: OverviewStatSummary;
    public TimeAxisLabels: string[];
}
export class GetApplicationTeam {
    public static TYPE_NAME: string = 'GetApplicationTeam';
    public ApplicationId: number;
}
export class GetApplicationTeamMember {
    public static TYPE_NAME: string = 'GetApplicationTeamMember';
    public JoinedAtUtc: Date;
    public UserId: number;
    public UserName: string;
    public IsAdmin: boolean;
}
export class GetApplicationTeamResult {
    public static TYPE_NAME: string = 'GetApplicationTeamResult';
    public Invited: GetApplicationTeamResultInvitation[];
    public Members: GetApplicationTeamMember[];
}
export class GetApplicationTeamResultInvitation {
    public static TYPE_NAME: string = 'GetApplicationTeamResultInvitation';
    public EmailAddress: string;
    public InvitedAtUtc: Date;
    public InvitedByUserName: string;
}
export class OverviewStatSummary {
    public static TYPE_NAME: string = 'OverviewStatSummary';
    public Followers: number;
    public Incidents: number;
    public Reports: number;
    public UserFeedback: number;
}
export class ApplicationCreated {
    public static TYPE_NAME: string = 'ApplicationCreated';
    public AppKey: string;
    public ApplicationId: number;
    public ApplicationName: string;
    public CreatedById: number;
    public SharedSecret: string;
}
export class ApplicationDeleted {
    public static TYPE_NAME: string = 'ApplicationDeleted';
    public AppKey: string;
    public ApplicationId: number;
    public ApplicationName: string;
}
export class UserAddedToApplication {
    public static TYPE_NAME: string = 'UserAddedToApplication';
    public AccountId: number;
    public ApplicationId: number;
}
export class UserInvitedToApplication {
    public static TYPE_NAME: string = 'UserInvitedToApplication';
    public ApplicationId: number;
    public ApplicationName: string;
    public EmailAddress: string;
    public InvitationKey: string;
    public InvitedBy: string;
}
export class CreateApplication {
    public static TYPE_NAME: string = 'CreateApplication';
    public ApplicationKey: string;
    public Name: string;
    public TypeOfApplication: TypeOfApplication;
}
export class DeleteApplication {
    public static TYPE_NAME: string = 'DeleteApplication';
    public Id: number;
}
export class RemoveTeamMember {
    public static TYPE_NAME: string = 'RemoveTeamMember';
    public ApplicationId: number;
    public UserToRemove: number;
}
export class UpdateApplication {
    public static TYPE_NAME: string = 'UpdateApplication';
    public ApplicationId: number;
    public Name: string;
    public TypeOfApplication: TypeOfApplication | null;
}
export class AddTeamMember {
    public static TYPE_NAME: string = 'AddTeamMember';
    public ApplicationId: number;
    public UserToAdd: number;
    public Roles: string[];
}
export class UpdateRoles {
    public static TYPE_NAME: string = 'UpdateRoles';
    public ApplicationId: number;
    public UserToUpdate: number;
    public Roles: string[];
}