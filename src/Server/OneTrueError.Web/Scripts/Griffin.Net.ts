///<reference path="Promise.ts"/>
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
        public httpMethod: string;
        public url: string;
        public queryString: any;
        public body: any;
        public isAsync: boolean;
        public credentials: ICredentials;
        public contentType: string;
        public headers: any;
        public Cors: CorsRequest;

        constructor(httpMethod: string, url: string) {
            this.url = url;
            this.httpMethod = httpMethod;
            this.isAsync = true;
        }
    };

    export class CorsRequest {
        public withCredentials: boolean;
    }

    export class HttpResponse {
        public statusCode: number;
        public statusReason: string;
        public contentType: string;
        public body: any;
        public charset: string;
    }

    export interface ICredentials {
        username: string;
        password: string;
    };

    export class HttpRejection implements P.Rejection {
        public message: string;
        public Reponse: HttpResponse;

        public constructor(response: HttpResponse) {
            this.message = response.statusReason;
            this.Reponse = response;
        }
    }

    export class QueryString {
        public static parse(str: string): any {
            str = str.trim().replace(/^(\?|#)/, '');
            if (!str) {
                return null;
            }

            var data = str.trim().split('&').reduce((ret, param) => {
                var parts = param.replace(/\+/g, ' ').split('=');
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
            }, {});

            return data;
        }

        public static stringify(data: string): string {

            return data ? Object.keys(data).map(key => {
                var val = data[key];

                if (Array.isArray(val)) {
                    return val.map(val2 => (
                            encodeURIComponent(key) + '=' + encodeURIComponent(val2))
                    ).join('&');
                }

                return encodeURIComponent(key) + '=' + encodeURIComponent(val);
            }).join('&') : '';
        }
    }

    export class HttpClient {
        public static REDIRECT_401_TO: string = "/account/login";

        public get(url: string, queryString: any = null, headers: any = null): P.Promise<HttpResponse> {
            var request = new HttpRequest('GET', url);
            if (queryString != null)
                request.queryString = queryString;
            if (headers != null) {
                request.headers = headers;
            }
            return this.invokeRequest(request);
        }

        public post(url: string, body: any = null, contentType: string= 'application/x-www-form-urlencoded', queryString: any = null, headers: any = null): P.Promise<HttpResponse> {
            var request = new HttpRequest('POST', url);
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


        public invokeRequest(request: IHttpRequest): P.Promise<HttpResponse> {
            var d = P.defer<HttpResponse>();

            var uri: string;
            if (request.queryString)
                if (request.url.indexOf('?') > -1) {
                    uri = request.url + "&" + QueryString.stringify(request.queryString);
                } else {
                    uri = request.url + "?" + QueryString.stringify(request.queryString);
                }

            else
                uri = request.url;

            var xhr = new XMLHttpRequest();
            xhr.open(request.httpMethod, uri, request.isAsync);

            if (request.credentials != null) {
                var credentials = Base64.encode(request.credentials.username + ":" + request.credentials.password);
                xhr.setRequestHeader("Authorization", "Basic " + credentials);
            }

        
            if (request.headers) {
                for (var header in request.headers) {
                    if (header === "Content-Type")
                        throw "You may not specify 'Content-Type' as a header, use the specific property.";

                    //if (request.hasOwnProperty(header)) {
                        xhr.setRequestHeader(header, request.headers[header]);
                    //}
                }
            }

            if (!request.contentType || request.contentType === '')
                request.contentType = "application/x-www-form-urlencoded";

            if (request.body) {
                if (request.contentType === 'application/x-www-form-urlencoded') {
                    if (typeof request.body !== "string") {
                        request.body = this.urlEncodeObject(request.body);
                    }
                } else if (request.contentType === 'application/json') {
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
                    var response = this.buildResponse(xhr);
                    if (xhr.status >= 200 && xhr.status < 300) {
                        d.resolve(response);
                    } else {
                        if (xhr.status === 401 && HttpClient.REDIRECT_401_TO) {
                            window.location.assign(HttpClient.REDIRECT_401_TO + "?ReturnTo=" + encodeURIComponent(window.location.href.replace('#', '&hash=')));
                        }
                        d.reject(new HttpRejection(response));
                    }
                }
            }

            xhr.send(request.body);
            return d.promise();
        }

        private urlEncodeObject(obj: any, prefix: string = null): string {
            var str = [];
            for (var p in obj) {
                if (obj.hasOwnProperty(p)) {
                    var k = prefix ? prefix + "." + p : p, v = obj[p];
                    if (v instanceof Array) {

                    }
                    str.push(typeof v == "object" ?
                        this.urlEncodeObject(v, k) :
                        encodeURIComponent(k) + "=" + encodeURIComponent(v));
                }
            }
            return str.join("&");
        }

        private buildResponse(xhr: XMLHttpRequest): HttpResponse {
            var response = new HttpResponse();
            response.statusCode = xhr.status;
            response.statusReason = xhr.statusText;
            if (xhr.responseBody) {
                response.body = xhr.responseBody;
            }
            else if (xhr.responseXML) {
                response.body = xhr.responseXML;
            } else {
                response.body = xhr.responseText;
            }

            response.contentType = xhr.getResponseHeader('content-type');
            if (response.contentType !== null && response.body !== null) {
                var pos = response.contentType.indexOf(';');
                if (pos > -1) {
                    response.charset = response.contentType.substr(pos + 1);
                    response.contentType = response.contentType.substr(0, pos);
                }
                if (response.contentType === 'application/json') {
                    try {
                        response.body = JSON.parse(response.body);
                    } catch (error) {
                        throw "Failed to parse '" + response.body + '. got: ' + error;
                    }

                }
            }
            return response;
        }
    }


} // ReSharper restore InconsistentNaming