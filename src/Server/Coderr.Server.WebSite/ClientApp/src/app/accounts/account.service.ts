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
