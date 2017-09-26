/// <reference path="Promise.ts" />
/// <reference path="Griffin.Net.ts" />
module Griffin.Cqs {
    import Promise = P.Promise;

    export class CqsClient {
        static query<TResult>(query: any): Promise<TResult> {
            var d = P.defer<TResult>();
            const client = new Net.HttpClient();
            client.post(`${window["API_URL"]}/api/cqs/?query=${query.constructor.TYPE_NAME}`,
                    query,
                    "application/json",
                    null,
                    { 'X-Cqs-Name': query.constructor.TYPE_NAME })
                .done(response => {
                    d.resolve(response.body);
                })
                .fail(rejection => {
                    humane.log("ERROR: " + rejection.message);
                    d.reject(rejection);
                });

            return d.promise();
        }

        static request<TReply>(query: any): Promise<TReply> {
            var d = P.defer<TReply>();
            const client = new Net.HttpClient();
            client.post(`${window["API_URL"]}/api/cqs/?query=${query.constructor.TYPE_NAME}`,
                    query,
                    "application/json",
                    null,
                    { 'X-Cqs-Name': query.constructor.TYPE_NAME })
                .done(response => {
                    d.resolve(response.body);
                })
                .fail(rejection => {
                    humane.log("ERROR: " + rejection.message);
                    d.reject(rejection);
                });

            return d.promise();
        }

        static event(query: any): Promise<any> {
            var d = P.defer<any>();
            const client = new Net.HttpClient();
            client.post(`${window["API_URL"]}/api/cqs/`, query)
                .done(response => {
                    d.resolve(response.body);
                })
                .fail(rejection => {
                    d.reject(rejection);
                });

            return d.promise();
        }

        static command(cmd: any): Promise<any> {
            var d = P.defer<any>();
            const client = new Net.HttpClient();
            client.post(`${window["API_URL"]}/api/cqs/`,
                    cmd,
                    "application/json",
                    null,
                    { 'X-Cqs-Name': cmd.constructor.TYPE_NAME })
                .done(response => {
                    d.resolve(response.body);
                })
                .fail(rejection => {
                    humane.log("ERROR: " + rejection.message);
                    console.log(rejection);
                    d.reject(rejection);
                });

            return d.promise();
        }
    }
}