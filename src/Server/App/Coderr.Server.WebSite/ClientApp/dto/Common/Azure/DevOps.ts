export class SaveSettings {
    public static TYPE_NAME: string = 'SaveSettings';
    public PersonalAccessToken: string;
    public Url: string;
    public ApplicationId: number;
    public ProjectName: string;
    public ProjectId: string;
    public AreaPath: string;
    public AreaPathId: string;
    public AutoAddImportant: boolean;
    public AutoAddCritical: boolean;
    public AssignedStateName: string;
    public ClosedStateName: string;
    public SolvedStateName: string;
    public WorkItemTypeName: string;
}

export class GetSettings {
    public static TYPE_NAME: string = 'GetSettings';
    public ApplicationId: number;
}


export class GetSettingsResult {
    public PersonalAccessToken: string;
    public Url: string;
    public ApplicationId: number;
    public ProjectName: string;
    public ProjectId: string;
    public AreaPath: string;
    public AreaPathId: string;
    public AutoAddImportant: boolean;
    public AutoAddCritical: boolean;
    public AssignedStateName: string;
    public ClosedStateName: string;
    public SolvedStateName: string;
    public StateFieldName: string;
    public WorkItemTypeName: string;
}

export class GetAreaPaths {
    public static TYPE_NAME: string = 'GetAreaPaths';
    public PersonalAccessToken: string;
    public Url: string;
    public ProjectNameOrId: string;
}
export class GetAreaPathsResult {
    public Items: GetAreaPathsResultItem[];
}

export class GetAreaPathsResultItem {
    public Path: string;
    public Name: string;
    public Id: string;
}


export class GetIterations {
    public static TYPE_NAME: string = 'GetIterations';
    public PersonalAccessToken: string;
    public Url: string;
    public ProjectNameOrId: string;
}
export class GetIterationPathsResult {
    public Items: GetProjectsResultItem[];
}

export class GetIterationPathsResultItem {
    public Name: string;
    public Id: string;
}


export class GetProjects {
    public static TYPE_NAME: string = 'GetProjects';
    public PersonalAccessToken: string;
    public Url: string;
}

export class GetProjectsResult {
    public Items: GetProjectsResultItem[];
}

export class GetProjectsResultItem {
    public Name: string;
    public Id: string;
}

export class GetWorkItemStates {
    public static TYPE_NAME: string = 'GetWorkItemStates';
    public PersonalAccessToken: string;
    public Url: string;
    public ProjectId: string;
    public WorkItemTypeName: string;
}

export class GetWorkItemStatesResult {
    public Items: GetWorkItemStatesResultItem[];
}

export class GetWorkItemStatesResultItem {
    public Name: string;
}


export class GetWorkItemTypes {
    public static TYPE_NAME: string = 'GetWorkItemTypes';
    public PersonalAccessToken: string;
    public Url: string;
    public ProjectNameOrId: string;
}

export class GetWorkItemTypesResult {
    public Items: GetWorkItemTypesResultItem[];
}

export class GetWorkItemTypesResultItem {
    public Name: string;
    public ReferenceName: string;
}

