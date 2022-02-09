export class GetDemoIncidentOptions {
  public static TYPE_NAME: string = 'GetDemoIncidentOptions';
}
export class GetDemoIncidentOptionsResult {
  public items: GetDemoIncidentOptionsResultItem[];
}
export class GetDemoIncidentOptionsResultItem {
  public category: string;
  public description: string;
  public id: string;
  public title: string;
}

export class GenerateDemoIncidents {
  public static TYPE_NAME: string = 'GenerateDemoIncidents';
  public demoOptionIds: string[];
}
