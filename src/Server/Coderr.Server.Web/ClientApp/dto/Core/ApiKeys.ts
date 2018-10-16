export class GetApiKey
{
    public static TYPE_NAME: string = 'GetApiKey';
    public ApiKey: string;
    public Id: number;
}
export class GetApiKeyResult
{
    public static TYPE_NAME: string = 'GetApiKeyResult';
    public AllowedApplications: GetApiKeyResultApplication[];
    public ApplicationName: string;
    public CreatedAtUtc: Date;
    public CreatedById: number;
    public GeneratedKey: string;
    public Id: number;
    public SharedSecret: string;
}
export class GetApiKeyResultApplication
{
    public static TYPE_NAME: string = 'GetApiKeyResultApplication';
    public ApplicationId: number;
    public ApplicationName: string;
}
export class ListApiKeys
{
    public static TYPE_NAME: string = 'ListApiKeys';
}
export class ListApiKeysResult
{
    public static TYPE_NAME: string = 'ListApiKeysResult';
    public Keys: ListApiKeysResultItem[];
}
export class ListApiKeysResultItem
{
    public static TYPE_NAME: string = 'ListApiKeysResultItem';
    public ApiKey: string;
    public ApplicationName: string;
    public Id: number;
}
export class ApiKeyCreated
{
    public static TYPE_NAME: string = 'ApiKeyCreated';
    public ApiKey: string;
    public ApplicationIds: number[];
    public ApplicationNameForTheAppUsingTheKey: string;
    public CreatedById: number;
    public SharedSecret: string;
}
export class ApiKeyRemoved
{
    public static TYPE_NAME: string = 'ApiKeyRemoved';
}
export class CreateApiKey
{
    public static TYPE_NAME: string = 'CreateApiKey';
    public AccountId: number;
    public ApiKey: string;
    public ApplicationIds: number[];
    public ApplicationName: string;
    public SharedSecret: string;
}
export class DeleteApiKey
{
    public static TYPE_NAME: string = 'DeleteApiKey';
    public ApiKey: string;
    public Id: number;
}
export class EditApiKey
{
    public static TYPE_NAME: string = 'EditApiKey';
    public ApplicationIds: number[];
    public ApplicationName: string;
    public Id: number;
}
