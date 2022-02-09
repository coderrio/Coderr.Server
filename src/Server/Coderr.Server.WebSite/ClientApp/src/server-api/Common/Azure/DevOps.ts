export class SaveAzureSettings {
  public static TYPE_NAME: string = 'SaveAzureSettings';
  public PersonalAccessToken: string;
  public Url: string;
  public ApplicationId: number;
  public ProjectName: string;
  public ProjectId: string;
  public AreaPath: string;
  public AreaPathId: string;
}

export class GetAzureSettings {
  public static TYPE_NAME: string = 'GetAzureSettings';
  public ApplicationId: number;
}


export class GetAzureSettingsResult {
  public PersonalAccessToken: string;
  public Url: string;
  public ApplicationId: number;
  public ProjectName: string;
  public ProjectId: string;
  public AreaPath: string;
  public AreaPathId: string;
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

