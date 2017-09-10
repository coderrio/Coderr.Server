/// <reference path="Promise.ts" />
// ReSharper disable InconsistentNaming
declare class Base64 {
    static encode(data: string): string;
}

module Griffin.Net {

    export interface IHttpRequest {
        httpMethod: string;
        url: string;
        queryString: any;
        body: any;
        isAsync: boolean;
        credentials: ICredentials;
        contentType: string;
        headers: any;
        Cors: CorsRequest;
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
        Cors: CorsRequest;

        constructor(httpMethod: string, url: string) {
            this.url = url;
            this.httpMethod = httpMethod;
            this.isAsync = true;
        }
    };

    export class CorsRequest {
        withCredentials: boolean;
    }

    export class HttpResponse {
        statusCode: number;
        statusReason: string;
        contentType: string;
        body: any;
        charset: string;
    }

    export interface ICredentials {
        username: string;
        password: string;
    };

    export class HttpRejection implements P.Rejection {
        message: string;
        Reponse: HttpResponse;

        public constructor(response: HttpResponse) {
            this.message = response.statusReason;
            this.Reponse = response;
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
                .reduce((ret, param) => {
                        var parts = param.replace(/\+/g, " ").split("=");
                        var key = parts[0];
                        var val = parts[1];

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

        static stringify(data: string): string {

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

        get(url: string, queryString: any = null, headers: any = null): P.Promise<HttpResponse> {
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
            headers: any = null): P.Promise<HttpResponse> {
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


        invokeRequest(request: IHttpRequest): P.Promise<HttpResponse> {
            var d = P.defer<HttpResponse>();

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
                        d.resolve(response);
                    } else {
                        if (xhr.status === 401 && HttpClient.REDIRECT_401_TO) {
                            window.location.assign(HttpClient.REDIRECT_401_TO +
                                "?ReturnTo=" +
                                encodeURIComponent(window.location.href.replace("#", "&hash=")));
                        }
                        d.reject(new HttpRejection(response));
                    }
                }
            };
            xhr.send(request.body);
            return d.promise();
        }

        private urlEncodeObject(obj: any, prefix: string = null): string {
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

        private buildResponse(xhr: XMLHttpRequest): HttpResponse {
            const response = new HttpResponse();
            response.statusCode = xhr.status;
            response.statusReason = xhr.statusText;
            if (xhr.responseXML) {
                response.body = xhr.responseXML;
            } else {
                response.body = xhr.responseText;
            }

            response.contentType = xhr.getResponseHeader("content-type");
            if (response.contentType !== null && response.body !== null) {
                const pos = response.contentType.indexOf(";");
                if (pos > -1) {
                    response.charset = response.contentType.substr(pos + 1);
                    response.contentType = response.contentType.substr(0, pos);
                }
                if (response.contentType === "application/json") {
                    try {
                        response.body = JSON.parse(response.body);
                    } catch (error) {
                        throw `Failed to parse '${response.body}. got: ${error}`;
                    }

                }
            }
            return response;
        }
    }


} // ReSharper restore InconsistentNaming