import { Injectable } from '@angular/core';
import { ApiClient } from "../../utils/HttpClient";
import * as api from "../../../server-api/Core/ApiKeys";
import { ApiKey, IApiKeyListItem } from "./api-key.model";

@Injectable({
  providedIn: 'root'
})
export class ApiKeyService {

  constructor(private apiClient: ApiClient) {
  }

  async create(apiKey: ApiKey): Promise<void> {
    if (!apiKey) {
      throw new Error("Key must be specified.");
    }
    if (apiKey.id) {
      throw new Error("Key.id must not be specified.");
    }

    var cmd = new api.CreateApiKey();
    cmd.applicationName = apiKey.title;
    cmd.applicationIds = apiKey.applications.map(x => x.id);
    cmd.accountId = apiKey.accountId;
    cmd.apiKey = apiKey.apiKey;
    cmd.sharedSecret = apiKey.sharedSecret;
    await this.apiClient.command(cmd);
  }

  async update(apiKey: ApiKey): Promise<void> {
    if (!apiKey) {
      throw new Error("Key must be specified.");
    }
    if (!apiKey.id) {
      throw new Error("Key.id must be specified.");
    }

    var cmd = new api.EditApiKey();
    cmd.applicationName = apiKey.title;
    cmd.applicationIds = apiKey.applications.map(x => x.id);
    cmd.id = apiKey.id;
    await this.apiClient.command(cmd);
  }

  async get(id: number): Promise<ApiKey> {
    if (!id) {
      throw new Error("id must be specified.");
    }

    var query = new api.GetApiKey();
    query.id = id;
    var result = await this.apiClient.query<api.GetApiKeyResult>(query);

    var key = new ApiKey();
    key.id = id;
    key.title = result.applicationName;
    key.applications = result.allowedApplications.map(x => {
      return { id: x.applicationId, name: x.applicationName };
    });

    key.accountId = result.createdById;
    key.apiKey = result.generatedKey;
    key.sharedSecret = result.sharedSecret;
    return key;
  }

  async list(): Promise<IApiKeyListItem[]> {

    var query = new api.ListApiKeys();
    var result = await this.apiClient.query<api.ListApiKeysResult>(query);
    return result.keys.map(x => {
      var key: IApiKeyListItem = {
        apiKey: x.apiKey,
        id: x.id,
        title: x.applicationName
      };
      return key;
    });
  }
}
