import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { environment } from "../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class ServerClientService {

  constructor(private httpClient: HttpClient) {
    if (!environment.apiUrl) {
      throw new Error("environment.apiUrl must be specified");
    }
    if (environment.apiUrl.substr(environment.apiUrl.length - 1, 1) !== "/")
      environment.apiUrl += "/";
  }
  async command(cmd: any): Promise<any> {

    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        "X-Cqs-Name": cmd.constructor.TYPE_NAME
      })
    };
    await this.httpClient.post(`${environment.apiUrl}cqs/`, cmd, httpOptions);
  }

  async query<TResult>(query: any): Promise<TResult> {
    const httpOptions = {
      headers: new HttpHeaders({
        "Accept": "application/json",
        'Content-Type': 'application/json',
        "X-Cqs-Name": query.constructor.TYPE_NAME
      })
    };

    var result = await this.httpClient.post<TResult>(`${environment.apiUrl}cqs/`, query, httpOptions).toPromise();
    return result;
  }

  async auth(): Promise<any> {
    const httpOptions = {
      headers: new HttpHeaders({
        "Accept": "application/json",
      })
    };

    await this.httpClient.post<any>(`${environment.apiUrl}authenticate/`, null, httpOptions);
    var result = <any>await this.httpClient.post(`${environment.apiUrl}authenticate/`, null, httpOptions).toPromise();
    return result.body;
  }
}
