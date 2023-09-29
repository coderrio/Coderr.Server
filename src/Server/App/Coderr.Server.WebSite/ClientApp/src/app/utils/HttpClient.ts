import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { AuthorizeGuard } from "../../api-authorization/authorize.guard";
declare var window: any;

export interface IRequestOptions {
  method?: "GET" | "POST" | "PUT" | "DELETE",
  mode?: "cors" | "no-cors", "*cors", "same-origin",
  cache: "default" | "no-store" | "reload" | "no-cache" | "force-cache" | "only-if-cached",
  credentials: "omit" | "same-origin" | "include",
  headers: Map<string, string>,
  redirect: 'follow' | "manual" | "*follow" | "error",
  referrerPolicy: 'no-referrer' | "*no-referrer-when-downgrade" | "origin" | "origin-when-cross-origin" | "same-origin" | "strict-origin" | "strict-origin-when-cross-origin" | "unsafe-url",
  body: any
}

export interface IHttpResponse {
  statusCode: number;
  statusReason: string;
  contentType: string | null;
  body: any;
  charset: string | null;
}

export class HttpError extends Error {
  message: string;
  reponse: IHttpResponse;

  constructor(response: IHttpResponse) {
    super(response.statusReason);
    this.message = response.statusReason;
    this.reponse = response;
  }
}

@Injectable({
  providedIn: 'root'
})
export class ApiClient {
  private http: HttpClient = new HttpClient();
  private cqsUrl;
  private rootUrl;
  private static redirected = false;

  constructor(private router: Router) {
    var apiRootUrl = '/';
    if (apiRootUrl.substr(apiRootUrl.length - 1, 1) !== "/")
      apiRootUrl += '/';

    this.rootUrl = apiRootUrl;
    this.cqsUrl = apiRootUrl + "cqs/";
  }

  async command(cmd: any): Promise<any> {
    const headers = {
      "X-Cqs-Name": cmd.constructor.TYPE_NAME,
      "Content-Type": "application/json",
    };
    const json = JSON.stringify(cmd);
    await this.http.post(`${this.cqsUrl}?type=${cmd.constructor.TYPE_NAME}`, json, {
      headers: headers,
    });
  }

  async query<T>(query: any): Promise<T> {
    var headers = {
      "Content-Type": "application/json",
      "Accept": "application/json",
      "X-Cqs-Name": query.constructor.TYPE_NAME,
    };
    var json = JSON.stringify(query);
    var response = await this.http.post(`${this.cqsUrl}?type=${query.constructor.TYPE_NAME}`, json, { headers: headers });
    if (response.statusCode === 401) {

      if (!ApiClient.redirected) {
        ApiClient.redirected = true;
      }


      if (AuthorizeGuard.isOpenAccountPage(window.location.pathname)) {
        return null;
      }

      const loginUrl = localStorage.getItem('loginUrl');
      if (loginUrl) {
        if (window.location.pathname.indexOf('account') === -1) {
          window.location.href =
            loginUrl + "?returnUrl=" + encodeURIComponent(window.location.pathname + window.location.search);
        }

        return null;
      }

      //this.router.navigate(['account/login'], { queryParams: { returnUrl: window.location.pathname + window.location.search } });
      throw new HttpError(response);
    }
    ApiClient.redirected = false;

    if (response.statusCode >= 200 && response.statusCode < 300) {
      return response.body;
    }
    if (response.statusCode === 501) {
      return null;
    }

    throw new Error(response.statusCode + ": " + response.body);
  }

  async auth(): Promise<any> {
    var result = await this.http.post(`${this.rootUrl}authenticate/`, null);
    return result.body;
  }
}

@Injectable({
  providedIn: 'root'
})
export class HttpClient {
  async request(url: string, options?: RequestInit): Promise<IHttpResponse> {
    var token = localStorage.getItem('jwt');
    if (token && options) {
      options.headers["Authorization"] = "Bearer " + token;
    }

    const response = await fetch(url, options);

    if (!response.ok) {
      return {
        statusCode: response.status,
        statusReason: response.statusText,
        contentType: response.headers.get('content-type'),
        body: await response.text(),
        charset: response.headers.get('charset')
      }
    }

    var body = null;
    if (response.status !== 204) {
      if (response.headers.get("Content-Type").indexOf('json') > 0) {
        body = await response.json();
      } else {
        body = await response.text();
      }
    }

    return {
      statusCode: response.status,
      statusReason: response.statusText,
      contentType: response.headers.get('content-type'),
      body: body,
      charset: response.headers.get('charset')
    };
  }

  async get(url: string, options?: RequestInit): Promise<IHttpResponse> {
    if (!options) {
      options = {
        method: 'GET',
        headers: { 'accept': 'application/json' }
      }
    } else {
      options.method = 'GET';
    }

    return this.request(url, options);
  }

  async post(url: string, data: BodyInit, options?: RequestInit): Promise<IHttpResponse> {
    if (!options) {
      options = {
        method: 'POST',
        body: data,
        headers: { 'content-type': 'application/json' }
      }
    } else {
      options.method = 'POST';
      options.body = data;
    }

    return this.request(url, options);
  }
}
