export class SearchAd {
    public static TYPE_NAME: string = 'SearchAd';
    public FindGroups: boolean;
    public FindUsers: boolean;
    public Text: string;

    public PageNumber: number;
    public PageSize: number;
}

export class SearchAdResult {
    public static TYPE_NAME: string = 'SearchAdResult';
    public Items: SearchAdResultItem[];
    public PageNumber: number;
    public TotalCount: number;
}

export class SearchAdResultItem {
    public static TYPE_NAME: string = 'SearchAdResultItem';
    public Name: string;
    public FullName: string;
    public Sid: string;
    public Type: string;
}

export class AddAdGroupToTeam {
    public static TYPE_NAME: string = 'AddAdGroupToTeam';
    public ApplicationId: number;
    public Sid: string;
    public IsAdmin?: boolean;
}

export class AddAdUserToTeam {
    public static TYPE_NAME: string = 'AddAdUserToTeam';
    public ApplicationId: number;
    public Sid: string;
    public IsAdmin?: boolean;
}

export class ChangeAdTeamRole {
    public static TYPE_NAME: string = 'ChangeAdTeamRole';
    public ApplicationId: number;
    public Sid: string;
    public IsAdmin: boolean;
}

export class RemoveAdTeamMember {
    public static TYPE_NAME: string = 'RemoveAdTeamMember';
    public ApplicationId: number;
    public Sid: string; 
}

export class GetAdTeamMembers {
    public static TYPE_NAME: string = 'GetAdTeamMembers';
    public ApplicationId: number;
}

export class GetAdTeamMembersResult {
    public static TYPE_NAME: string = 'GetAdTeamMembersResult';
    public Items: GetAdTeamMembersResultItem[];
}

export class GetAdTeamMembersResultItem {
    public static TYPE_NAME: string = 'GetAdTeamMembersResultItem';
    public Name: string;
    public Sid: string;
    public IsGroup: boolean;
    public IsAdmin: boolean;
}
