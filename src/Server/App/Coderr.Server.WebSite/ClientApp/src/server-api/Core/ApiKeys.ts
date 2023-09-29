// ReSharper disable InconsistentNaming

export class GetApiKey {
  public static TYPE_NAME: string = 'GetApiKey';
  public apiKey: string;
  public id: number;
}
export class GetApiKeyResult {
  public static TYPE_NAME: string = 'GetApiKeyResult';
  public allowedApplications: GetApiKeyResultApplication[];
  public applicationName: string;
  public createdAtUtc: Date;
  public createdById: number;
  public generatedKey: string;
  public id: number;
  public sharedSecret: string;
}
export class GetApiKeyResultApplication {
  public static TYPE_NAME: string = 'GetApiKeyResultApplication';
  public applicationId: number;
  public applicationName: string;
}
export class ListApiKeys {
  public static TYPE_NAME: string = 'ListApiKeys';
}
export class ListApiKeysResult {
  public static TYPE_NAME: string = 'ListApiKeysResult';
  public keys: ListApiKeysResultItem[];
}
export class ListApiKeysResultItem {
  public static TYPE_NAME: string = 'ListApiKeysResultItem';
  public apiKey: string;
  public applicationName: string;
  public id: number;
}
export class ApiKeyCreated {
  public static TYPE_NAME: string = 'ApiKeyCreated';
  public apiKey: string;
  public applicationIds: number[];
  public applicationNameForTheAppUsingTheKey: string;
  public createdById: number;
  public sharedSecret: string;
}
export class ApiKeyRemoved {
  public static TYPE_NAME: string = 'ApiKeyRemoved';
}
export class CreateApiKey {
  public static TYPE_NAME: string = 'CreateApiKey';
  public accountId: number;
  public apiKey: string;
  public applicationIds: number[];
  public applicationName: string;
  public sharedSecret: string;
}
export class DeleteApiKey {
  public static TYPE_NAME: string = 'DeleteApiKey';
  public apiKey: string;
  public id: number;
}
export class EditApiKey {
  public static TYPE_NAME: string = 'EditApiKey';
  public applicationIds: number[];
  public applicationName: string;
  public id: number;
}
