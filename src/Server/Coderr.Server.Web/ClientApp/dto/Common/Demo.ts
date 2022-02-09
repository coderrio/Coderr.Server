export class GetDemoIncidentOptions {
    public static TYPE_NAME: string = 'GetDemoIncidentOptions';
}
export class GetDemoIncidentOptionsResult {
    public Items: GetDemoIncidentOptionsResultItem[];
}
export class GetDemoIncidentOptionsResultItem {
    public Category: string;
    public Description: string;
    public Id: string;
    public Title: string;
}

export class GenerateDemoIncidents {
    public static TYPE_NAME: string = 'GenerateDemoIncidents';
    public DemoOptionIds: string[];
}
