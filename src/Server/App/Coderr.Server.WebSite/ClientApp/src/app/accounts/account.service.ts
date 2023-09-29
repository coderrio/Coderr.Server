import { Injectable } from '@angular/core';
import { ApiClient } from "../utils/HttpClient";
import { SignalRService } from "../services/signal-r.service";
import { AuthorizeService } from "../../api-authorization/authorize.service";
import * as api from "../../server-api/Core/Accounts";

export interface User {
  id: number;
  userName: string;
  email: string;
}

export interface IRegisterDTO {
  UserName: string;
  Password: string;
  Password2: string;
  Email: string;
  FirstName?: string;
  LastName?: string;
}

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private users: User[] = [];

  constructor(private readonly apiClient: ApiClient,
    private readonly signalR: SignalRService,

    private readonly authService: AuthorizeService) { }

  async getAllButMe(): Promise<User[]> {

    if (this.users.length === 0) {
      await this.loadUsers();
    }

    return this.users.filter(x => x.id !== this.authService.user.accountId);
  }

  async getAll(): Promise<User[]> {
    if (this.users.length === 0) {
      await this.loadUsers();
    }

    return this.users;
  }

  /**
   * 
   * @param userName
   * @param password
   * @param emailAddress
   * @returns If activation is required.
   */
  async register(userName: string, password: string, emailAddress: string): Promise<boolean> {
    var dto: IRegisterDTO = {
      UserName: userName,
      Password: password,
      Password2: password,
      Email: emailAddress
    };
    var response = await fetch('/api/account/register',
      {
        method: 'POST',
        headers: {
          'Accept': 'application/json',
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(dto)
      });

    var json = await response.json();
    if (!json.success) {
      throw new Error(json.errorMessage);
    }

    return <boolean>json.verificationIsRequested;
  }

  async activate(activationCode: string) {
    var response = await fetch('/api/account/activate/' + activationCode,
      {
        method: 'POST',
      });

    var json = await response.json();
    if (!json.success) {
      throw new Error(json.errorMessage);
    }

    
  }

  private async loadUsers() {
    const query = new api.ListAccounts();
    const result = await this.apiClient.query<api.ListAccountsResult>(query);
    this.users = result.accounts.map(x => {
      return {
        id: x.accountId,
        email: x.email,
        userName: x.userName
      };
    });
  }
}
