import { Injectable } from '@angular/core';
import { ApiClient } from "./HttpClient";
import * as api from "../../server-api/Core/Users";
@Injectable({
  providedIn: 'root'
})
export class SettingsService {

  constructor(
    private apiClient: ApiClient) {

  }

  async get(name: string): Promise<string> {
    var query = new api.GetAccountSetting();
    query.name = name;
    var result = await this.apiClient.query<api.GetAccountSettingResult>(query);
    return result?.value;
  }

  async set(name: string, value: string): Promise<void> {
    var cmd = new api.SaveAccountSetting();
    cmd.value = value;
    cmd.name = name;
    console.log('Saving ' + name + " with " + value);
    await this.apiClient.command(cmd);
  }
}
