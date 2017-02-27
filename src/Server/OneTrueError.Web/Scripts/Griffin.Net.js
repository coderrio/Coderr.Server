/// <reference path="Promise.ts" />
var Griffin;
(function (Griffin) {
    var Net;
    (function (Net) {
        ;
        var HttpRequest = (function () {
            function HttpRequest(httpMethod, url) {
                this.url = url;
                this.httpMethod = httpMethod;
                this.isAsync = true;
            }
            return HttpRequest;
        }());
        Net.HttpRequest = HttpRequest;
        ;
        var CorsRequest = (function () {
            function CorsRequest() {
            }
            return CorsRequest;
        }());
        Net.CorsRequest = CorsRequest;
        var HttpResponse = (function () {
            function HttpResponse() {
            }
            return HttpResponse;
        }());
        Net.HttpResponse = HttpResponse;
        ;
        var HttpRejection = (function () {
            function HttpRejection(response) {
                this.message = response.statusReason;
                this.Reponse = response;
            }
            return HttpRejection;
        }());
        Net.HttpRejection = HttpRejection;
        var QueryString = (function () {
            function QueryString() {
            }
            QueryString.parse = function (str) {
                str = str.trim().replace(/^(\?|#)/, "");
                if (!str) {
                    return null;
                }
                var data = str.trim()
                    .split("&")
                    .reduce(function (ret, param) {
                    var parts = param.replace(/\+/g, " ").split("=");
                    var key = parts[0];
                    var val = parts[1];
                    key = decodeURIComponent(key);
                    val = val === undefined ? null : decodeURIComponent(val);
                    if (!ret.hasOwnProperty(key)) {
                        ret[key] = val;
                    }
                    else if (Array.isArray(ret[key])) {
                        ret[key].push(val);
                    }
                    else {
                        ret[key] = [ret[key], val];
                    }
                    return ret;
                }, {});
                return data;
            };
            QueryString.stringify = function (data) {
                return data
                    ? Object.keys(data)
                        .map(function (key) {
                        var val = data[key];
                        if (Array.isArray(val)) {
                            return val.map(function (val2) { return (encodeURIComponent(key) + "=" + encodeURIComponent(val2)); })
                                .join("&");
                        }
                        return encodeURIComponent(key) + "=" + encodeURIComponent(val);
                    })
                        .join("&")
                    : "";
            };
            return QueryString;
        }());
        Net.QueryString = QueryString;
        var HttpClient = (function () {
            function HttpClient() {
            }
            HttpClient.prototype.get = function (url, queryString, headers) {
                if (queryString === void 0) { queryString = null; }
                if (headers === void 0) { headers = null; }
                var request = new HttpRequest("GET", url);
                if (queryString != null)
                    request.queryString = queryString;
                if (headers != null) {
                    request.headers = headers;
                }
                return this.invokeRequest(request);
            };
            HttpClient.prototype.post = function (url, body, contentType, queryString, headers) {
                if (body === void 0) { body = null; }
                if (contentType === void 0) { contentType = "application/x-www-form-urlencoded"; }
                if (queryString === void 0) { queryString = null; }
                if (headers === void 0) { headers = null; }
                var request = new HttpRequest("POST", url);
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
            };
            HttpClient.prototype.invokeRequest = function (request) {
                var _this = this;
                var d = P.defer();
                var uri;
                if (request.queryString)
                    if (request.url.indexOf("?") > -1) {
                        uri = request.url + "&" + QueryString.stringify(request.queryString);
                    }
                    else {
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
                    }
                }
                if (!request.contentType || request.contentType === "")
                    request.contentType = "application/x-www-form-urlencoded";
                if (request.body) {
                    if (request.contentType === "application/x-www-form-urlencoded") {
                        if (typeof request.body !== "string") {
                            request.body = this.urlEncodeObject(request.body);
                        }
                    }
                    else if (request.contentType === "application/json") {
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
                xhr.onreadystatechange = function () {
                    if (xhr.readyState === XMLHttpRequest.DONE) {
                        var response = _this.buildResponse(xhr);
                        if (xhr.status >= 200 && xhr.status < 300) {
                            d.resolve(response);
                        }
                        else {
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
            };
            HttpClient.prototype.urlEncodeObject = function (obj, prefix) {
                if (prefix === void 0) { prefix = null; }
                var str = [];
                for (var p in obj) {
                    if (obj.hasOwnProperty(p)) {
                        var k = prefix ? prefix + "." + p : p;
                        var v = obj[p];
                        if (v instanceof Array) {
                        }
                        str.push(typeof v == "object"
                            ? this.urlEncodeObject(v, k)
                            : encodeURIComponent(k) + "=" + encodeURIComponent(v));
                    }
                }
                return str.join("&");
            };
            HttpClient.prototype.buildResponse = function (xhr) {
                var response = new HttpResponse();
                response.statusCode = xhr.status;
                response.statusReason = xhr.statusText;
                if (xhr.responseBody) {
                    response.body = xhr.responseBody;
                }
                else if (xhr.responseXML) {
                    response.body = xhr.responseXML;
                }
                else {
                    response.body = xhr.responseText;
                }
                response.contentType = xhr.getResponseHeader("content-type");
                if (response.contentType !== null && response.body !== null) {
                    var pos = response.contentType.indexOf(";");
                    if (pos > -1) {
                        response.charset = response.contentType.substr(pos + 1);
                        response.contentType = response.contentType.substr(0, pos);
                    }
                    if (response.contentType === "application/json") {
                        try {
                            response.body = JSON.parse(response.body);
                        }
                        catch (error) {
                            throw "Failed to parse '" + response.body + ". got: " + error;
                        }
                    }
                }
                return response;
            };
            HttpClient.REDIRECT_401_TO = "/account/login";
            return HttpClient;
        }());
        Net.HttpClient = HttpClient;
    })(Net = Griffin.Net || (Griffin.Net = {}));
})(Griffin || (Griffin = {})); // ReSharper restore InconsistentNaming
//# sourceMappingURL=Griffin.Net.js.map