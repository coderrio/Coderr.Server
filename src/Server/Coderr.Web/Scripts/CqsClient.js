/// <reference path="Promise.ts" />
/// <reference path="Griffin.Net.ts" />
var Griffin;
(function (Griffin) {
    var Cqs;
    (function (Cqs) {
        var CqsClient = (function () {
            function CqsClient() {
            }
            CqsClient.query = function (query) {
                var d = P.defer();
                var client = new Griffin.Net.HttpClient();
                client.post(window["API_URL"] + "/api/cqs/?query=" + query.constructor.TYPE_NAME, query, "application/json", null, { 'X-Cqs-Name': query.constructor.TYPE_NAME })
                    .done(function (response) {
                    d.resolve(response.body);
                })
                    .fail(function (rejection) {
                    humane.log("ERROR: " + rejection.message);
                    d.reject(rejection);
                });
                return d.promise();
            };
            CqsClient.request = function (query) {
                var d = P.defer();
                var client = new Griffin.Net.HttpClient();
                client.post(window["API_URL"] + "/api/cqs/?query=" + query.constructor.TYPE_NAME, query, "application/json", null, { 'X-Cqs-Name': query.constructor.TYPE_NAME })
                    .done(function (response) {
                    d.resolve(response.body);
                })
                    .fail(function (rejection) {
                    humane.log("ERROR: " + rejection.message);
                    d.reject(rejection);
                });
                return d.promise();
            };
            CqsClient.event = function (query) {
                var d = P.defer();
                var client = new Griffin.Net.HttpClient();
                client.post(window["API_URL"] + "/api/cqs/", query)
                    .done(function (response) {
                    d.resolve(response.body);
                })
                    .fail(function (rejection) {
                    d.reject(rejection);
                });
                return d.promise();
            };
            CqsClient.command = function (cmd) {
                var d = P.defer();
                var client = new Griffin.Net.HttpClient();
                client.post(window["API_URL"] + "/api/cqs/", cmd, "application/json", null, { 'X-Cqs-Name': cmd.constructor.TYPE_NAME })
                    .done(function (response) {
                    d.resolve(response.body);
                })
                    .fail(function (rejection) {
                    humane.log("ERROR: " + rejection.message);
                    console.log(rejection);
                    d.reject(rejection);
                });
                return d.promise();
            };
            return CqsClient;
        }());
        Cqs.CqsClient = CqsClient;
    })(Cqs = Griffin.Cqs || (Griffin.Cqs = {}));
})(Griffin || (Griffin = {}));
//# sourceMappingURL=CqsClient.js.map