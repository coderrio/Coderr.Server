export class GetEnvironments {
  public static TYPE_NAME: string = 'GetEnvironments';
  public applicationId: number;
}
export class GetEnvironmentsResult {
  public static TYPE_NAME: string = 'GetEnvironments';
  public items: GetEnvironmentsResultItem[];
}
export class GetEnvironmentsResultItem {
  public static TYPE_NAME: string = 'GetEnvironmentsResultItem';
  public id: number;
  public name: string;
  public deleteIncidents: boolean;
}
export class ResetEnvironment {
  public static TYPE_NAME: string = 'ResetEnvironment';
  public environmentId: number;
  public applicationId: number;
}
export class UpdateEnvironment {
  public static TYPE_NAME: string = 'UpdateEnvironment';
  public environmentId: number;
  public applicationId: number;
  public deleteIncidents: boolean;
}

export class CreateEnvironment {
  public static TYPE_NAME: string = 'CreateEnvironment';
  public applicationId: number;
  public name: string;
  public deleteIncidents: boolean;
}
