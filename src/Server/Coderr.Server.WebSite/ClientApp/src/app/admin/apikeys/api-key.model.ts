export interface IApplicationSummary {
  id: number;
  name: string;
}
export class ApiKey {
  id: number;
  title: string;
  applications: IApplicationSummary[] = [];
  accountId: number;
  apiKey: string;
  sharedSecret: string;
}

export interface IApiKeyListItem {
  id: number;
  title: string;
  apiKey: string;
}
