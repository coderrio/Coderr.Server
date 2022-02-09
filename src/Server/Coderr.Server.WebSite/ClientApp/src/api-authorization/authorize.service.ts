import { Injectable } from '@angular/core';
import { BehaviorSubject, Subscriber, Observable, Subject } from 'rxjs';
import { HttpClient } from "../app/utils/HttpClient"
import jwt_decode from "jwt-decode";

//https://jasonwatmore.com/post/2019/05/17/angular-7-tutorial-part-4-login-form-authentication-service-route-guard
export enum AuthenticationResultStatus {
  Success,
  Redirect,
  Fail
}

interface Dictionary<T> {
  [key: string]: T;
}

export interface IUser {
  userName: string;
  emailAddress: string;
  accountId: number;
  isSysAdmin: boolean;
  role: string;
  claims: Dictionary<string>;
}

@Injectable({
  providedIn: 'root'
})
export class AuthorizeService {
  private userSubject: BehaviorSubject<IUser | null> = new BehaviorSubject(null);
  private _token: string;
  private subscriber: Subscriber<string> = new Subscriber<string>();
  private _user: IUser | null;


  constructor(private service: HttpClient) {
    var user = this.user;
    this.userSubject.next(user);
  }

  isAuthenticated(): boolean {
    return this.getAccessToken() != null;
  }

  get user(): IUser | null {
    if (this._user == null) {
      var json = localStorage.getItem('user');
      this._user = JSON.parse(json);
    }
    return this._user;
  }

  get userEvents(): Observable<IUser> {
    return this.userSubject.asObservable();
  }

  public getAccessToken(): string {
    if (!this._token) {
      this._token = localStorage.getItem('jwt');
    }

    return this._token;
  }

  public getAccessToken2(): Observable<string> {
    if (!this._token) {
      this._token = localStorage.getItem('jwt');
    }

    return new BehaviorSubject<string>(this._token);
  }

  async logout(): Promise<object> {
    localStorage.removeItem('jwt');
    this._user = null;
    this.userSubject.next(null);
    return null;
  }

  async login(userName: string, password: string): Promise<IUser> {
    var reply = await this.service.post("/api/account/login", JSON.stringify({ UserName: userName, Password: password }));
    if (reply.statusCode !== 200) {
      throw new Error(reply.statusReason);
    }
    if (!reply.body.success) {
      throw new Error(reply.body.errorMessage);
    }

    localStorage.setItem('jwt', reply.body.jwtToken);

    /*
        application: 1
        application/admin: 1
        aud: "https://coderr.io"
        exp: 1616162760
        iat: 1615557960
        iss: "https://coderr.io"
        nameid: 1
        nbf: 1615557960
        role: "SysAdmin"
        unique_name: "admin"
     */
    let tokenInfo = this.getDecodedAccessToken(reply.body.jwtToken);

    var claims: Dictionary<string> = {};
    for (var key in tokenInfo) {
      if (!tokenInfo.hasOwnProperty(key)) {
        continue;
      }
      switch (key) {
        case "aud":
        case "exp":
        case "iat":
        case "iss":
        case "nbf":
        case "unique_name":
          break;
        default:
          claims[key] = tokenInfo[key];
      }
    }

    var user: IUser =
    {
      accountId: tokenInfo.nameid,
      claims: claims,
      emailAddress: null,
      isSysAdmin: tokenInfo.role === "SysAdmin",
      role: tokenInfo.role,
      userName: tokenInfo.unique_name
    };
    localStorage.setItem('user', JSON.stringify(user));
    this._user = user;
    this.subscriber.next(this._token);
    this.userSubject.next(user);
    return user;
  }

  getDecodedAccessToken(token: string): any {
    try {
      return jwt_decode(token);
    }
    catch (e) {
      console.log(e);
      return null;
    }
  }

}
