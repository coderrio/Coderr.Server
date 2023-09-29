export class FindIntegration {
    public static TYPE_NAME: string = 'FindIntegration';
    public ApplicationId: number;
}

export class FindIntegrationResult {
    public HaveIntegration: boolean;
    public Name: string;
    public Title: string;
}

export class FindWorkItem {
    public static TYPE_NAME: string = 'FindWorkItem';
    public IncidentId: number;
}

export class FindWorkItemResult {
    public WorkItemId: string;
    public Name: string;
    public Url: string;
    public ApplicationId: number;
}

export class CreateWorkItem {
    public static TYPE_NAME: string = 'CreateWorkItem';
    public IncidentId: number;
    public ApplicationId: number;
}
