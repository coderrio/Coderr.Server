import { HttpClient, IHttpResponse, HttpError } from "./HttpClient";

export interface ICommandResponse {
    statusCode: number;
}

export interface IQueryResponse<T> {
    data: T;
}


export class ApiClient {
    private http: HttpClient = new HttpClient();
    public static ApiUrl = '';

    constructor(private apiRootUrl: string) {
        if (!this.apiRootUrl) {
            throw new Error("URL must be specified");
        }
        if (this.apiRootUrl.substr(this.apiRootUrl.length - 1, 1) !== "/")
            this.apiRootUrl += "/";
        ApiClient.ApiUrl = this.apiRootUrl;
    }
    async command(cmd: any): Promise<any> {
        var headers = {
            "X-Cqs-Name": cmd.constructor.TYPE_NAME
        };
        await this.http.post(`${this.apiRootUrl}cqs/`, cmd, "application/json", { type: cmd.constructor.TYPE_NAME }, headers);
    }

    query<T>(query: any): Promise<T> {
        var headers = {
            "Accept": "application/json",
            "X-Cqs-Name": query.constructor.TYPE_NAME
        };
        return new Promise<T>((resolve, reject) => {
            this.http.post(`${this.apiRootUrl}cqs/`, query, "application/json", { type: query.constructor.TYPE_NAME }, headers)
                .then((result: IHttpResponse) => resolve(result.body))
                .catch((rejection: HttpError) => {
                    if (rejection.reponse.statusCode === 401) {
                        //TODO: redirect;
                        return;
                    }
                    reject(rejection);
                });
        });
    }

    async auth(): Promise<any> {
        var result = await this.http.post(`${this.apiRootUrl}authenticate/`, null, "application/json");
        return result.body;
    }

}


