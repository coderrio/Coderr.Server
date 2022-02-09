export class GetEnvironments {
    public static TYPE_NAME: string = 'GetEnvironments';
    public ApplicationId: number;
}
export class GetEnvironmentsResult {
    public static TYPE_NAME: string = 'GetEnvironments';
    public Items: GetEnvironmentsResultItem[];
}
export class GetEnvironmentsResultItem {
    public static TYPE_NAME: string = 'GetEnvironmentsResultItem';
    public Id: number;
    public Name: string;
    public DeleteIncidents: boolean;
}
export class ResetEnvironment {
    public static TYPE_NAME: string = 'ResetEnvironment';
    public EnvironmentId: number;
    public ApplicationId: number;
}
export class UpdateEnvironment {
    public static TYPE_NAME: string = 'UpdateEnvironment';
    public EnvironmentId: number;
    public ApplicationId: number;
    public DeleteIncidents: boolean;
}

export class CreateEnvironment {
    public static TYPE_NAME: string = 'CreateEnvironment';
    public ApplicationId: number;
    public Name: string;
    public DeleteIncidents: boolean;
}
