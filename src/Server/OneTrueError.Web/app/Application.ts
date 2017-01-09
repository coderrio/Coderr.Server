/// <reference path="../Scripts/Models/AllModels.ts" />
/// <reference path="../Scripts/Griffin.Yo.d.ts" />
/// <reference path="../Scripts/CqsClient.ts" />
module OneTrueError.Applications {
    import Yo = Griffin.Yo;
    import CqsClient = Griffin.Cqs.CqsClient;

    export class ApplicationService {

        get(applicationId: number): P.Promise<Core.Applications.Queries.GetApplicationInfoResult> {
            var def = P.defer<Core.Applications.Queries.GetApplicationInfoResult>();

            const cacheItem = Yo.GlobalConfig
                .applicationScope["application"] as Core.Applications.Queries.GetApplicationInfoResult;
            if (cacheItem && cacheItem.Id === applicationId) {
                def.resolve(cacheItem);
                return def.promise();
            }

            const query = new Core.Applications.Queries.GetApplicationInfo();
            query.ApplicationId = applicationId;
            CqsClient.query<Core.Applications.Queries.GetApplicationInfoResult>(query)
                .done(result => {
                    Yo.GlobalConfig.applicationScope["application"] = result;
                    def.resolve(result);
                });

            return def.promise();
        }
    }
};