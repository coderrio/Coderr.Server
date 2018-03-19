declare class Base64 {
    static encode(data: string): string;
}

export interface IHttpRequest {
    httpMethod: string;
    url: string;
    queryString: any;
    body: any;
    isAsync: boolean;
    credentials: ICredentials;
    contentType: string;
    headers: any;
    cors: CorsRequest;
};

export class HttpRequest implements IHttpRequest {
    httpMethod: string;
    url: string;
    queryString: any;
    body: any;
    isAsync: boolean;
    credentials: ICredentials;
    contentType: string;
    headers: any;
    cors: CorsRequest;

    constructor(httpMethod: string, url: string) {
        this.url = url;
        this.httpMethod = httpMethod;
        this.isAsync = true;
    }
};

export class CorsRequest {
    withCredentials: boolean;
}

export interface IHttpResponse {
    statusCode: number;
    statusReason: string;
    contentType: string | null;
    body: any;
    charset: string | null;
}

export interface IHeader {
    name: string;
    value: string;
}

export interface IQueryStringParam {
    key: string;
    value: string;
}

export interface ICredentials {
    username: string;
    password: string;
};

export class HttpError extends Error {
    message: string;
    reponse: IHttpResponse;

    constructor(response: IHttpResponse) {
        super(response.statusReason);
        this.message = response.statusReason;
        this.reponse = response;
    }
}

export class QueryString {
    static parse(str: string): any {
        str = str.trim().replace(/^(\?|#)/, "");
        if (!str) {
            return null;
        }

        const data = str.trim()
            .split("&")
            .reduce((ret: any, param) => {
                var parts = param.replace(/\+/g, " ").split("=");
                var key = parts[0];
                var val: string | null = parts[1];

                key = decodeURIComponent(key);
                val = val === undefined ? null : decodeURIComponent(val);
                if (!ret.hasOwnProperty(key)) {
                    ret[key] = val;
                } else if (Array.isArray(ret[key])) {
                    ret[key].push(val);
                } else {
                    ret[key] = [ret[key], val];
                }

                return ret;
            },
            {});

        return data;
    }

    static stringify(data: any): string {
        return data
            ? Object.keys(data)
                .map(key => {
                    var val = data[key];

                    if (Array.isArray(val)) {
                        return val.map(val2 => (
                            encodeURIComponent(key) + "=" + encodeURIComponent(val2))
                        )
                            .join("&");
                    }

                    return encodeURIComponent(key) + "=" + encodeURIComponent(val);
                })
                .join("&")
            : "";
    }
}

export class HttpClient {
    static REDIRECT_401_TO = "/account/login";

    get(url: string, queryString: any = null, headers: any = null): Promise<IHttpResponse> {
        const request = new HttpRequest("GET", url);
        if (queryString != null)
            request.queryString = queryString;
        if (headers != null) {
            request.headers = headers;
        }
        return this.invokeRequest(request);
    }

    post(url: string,
        body: any = null,
        contentType: string = "application/x-www-form-urlencoded",
        queryString: any = null,
        headers: any = null): Promise<IHttpResponse> {
        const request = new HttpRequest("POST", url);
        if (queryString != null)
            request.queryString = queryString;
        if (body != null) {
            request.body = body;
            request.contentType = contentType;
        }
        if (headers != null) {
            request.headers = headers;
        }
        return this.invokeRequest(request);
    }


    invokeRequest(request: IHttpRequest): Promise<IHttpResponse> {
        return new Promise<IHttpResponse>((resolve, reject) => {

            let uri: string;
            if (request.queryString)
                if (request.url.indexOf("?") > -1) {
                    uri = request.url + "&" + QueryString.stringify(request.queryString);
                } else {
                    uri = request.url + "?" + QueryString.stringify(request.queryString);
                }

            else
                uri = request.url;

            var xhr = new XMLHttpRequest();
            xhr.open(request.httpMethod, uri, request.isAsync);

            if (request.credentials != null) {
                const credentials = Base64.encode(request.credentials.username + ":" + request.credentials.password);
                xhr.setRequestHeader("Authorization", `Basic ${credentials}`);
            }


            if (request.headers) {
                for (let header in request.headers) {
                    if (header === "Content-Type")
                        throw "You may not specify 'Content-Type' as a header, use the specific property.";

                    //if (request.hasOwnProperty(header)) {
                    xhr.setRequestHeader(header, request.headers[header]);
                    //}
                }
            }

            if (!request.contentType || request.contentType === "")
                request.contentType = "application/x-www-form-urlencoded";

            if (request.body) {
                if (request.contentType === "application/x-www-form-urlencoded") {
                    if (typeof request.body !== "string") {
                        request.body = this.urlEncodeObject(request.body);
                    }
                } else if (request.contentType === "application/json") {
                    if (typeof request.body !== "string") {
                        request.body = JSON.stringify(request.body);
                    }
                }

                xhr.setRequestHeader("Content-Type", request.contentType);
            }


            //xhr.onload = () => {
            //    var response = this.buildResponse(xhr);
            //    if (xhr.status >= 200 && xhr.status < 300) {
            //        d.resolve(response);
            //    } else {
            //        d.reject(new HttpRejection(response));
            //    }
            //};
            xhr.onreadystatechange = () => {
                if (xhr.readyState === XMLHttpRequest.DONE) {
                    const response = this.buildResponse(xhr);
                    if (xhr.status >= 200 && xhr.status < 300) {
                        resolve(response);
                    } else {
                        if (xhr.status === 401 && HttpClient.REDIRECT_401_TO) {
                            window.location.assign(HttpClient.REDIRECT_401_TO +
                                "?ReturnTo=" +
                                encodeURIComponent(window.location.href.replace("#", "&hash=")));
                        }
                        reject(new HttpError(response));
                    }
                }
            };
            xhr.send(request.body);
        });
    }

    private urlEncodeObject(obj: any, prefix: string | null = null): string {
        const str: string[] = [];
        for (let p in obj) {
            if (obj.hasOwnProperty(p)) {
                const k = prefix ? prefix + "." + p : p;
                const v = obj[p];
                if (v instanceof Array) {

                }
                var value = typeof v == "object"
                    ? this.urlEncodeObject(v, k)
                    : encodeURIComponent(k) + "=" + encodeURIComponent(v);

                str.push(value);
            }
        }
        return str.join("&");
    }

    private buildResponse(xhr: XMLHttpRequest): IHttpResponse {
        var body: string | null;
        if (xhr.responseXML) {
            body = xhr.responseXML.body.innerHTML;
        } else {
            body = xhr.responseText;
        }

        var charset: string | null = null;
        var contentType = xhr.getResponseHeader("content-type");
        if (contentType != null && body != null) {
            const pos = contentType.indexOf(";");
            if (pos > -1) {
                charset = contentType.substr(pos + 1);
                contentType = contentType.substr(0, pos);
            }
            if (contentType === "application/json" && body.length > 0) {
                try {
                    body = JSON.parse(body);
                } catch (error) {
                    throw `Failed to parse '${body}'. Error: ${error}`;
                }

            }
        }
        return {
            statusCode: xhr.status,
            statusReason: xhr.statusText,
            contentType: contentType,
            body: body,
            charset: charset
        };

    }
}
