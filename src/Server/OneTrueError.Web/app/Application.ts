/// <reference path="../Scripts/Models/AllModels.ts"/>
/// <reference path="../Scripts/Griffin.Yo.d.ts"/>
/// <reference path="../Scripts/CqsClient.ts"/>
/// <reference path="../Scripts/Promise.ts"/>

module OneTrueError.Applications {
    import Yo = Griffin.Yo;
    import CqsClient = Griffin.Cqs.CqsClient;
    
    export class ApplicationService {

        public get(applicationId: number): P.Promise<OneTrueError.Core.Applications.Queries.GetApplicationInfoResult> {
            var def = P.defer<OneTrueError.Core.Applications.Queries.GetApplicationInfoResult>();

            var cacheItem = <OneTrueError.Core.Applications.Queries.GetApplicationInfoResult>Yo.GlobalConfig.applicationScope["application"];
            if (cacheItem && cacheItem.Id === applicationId) {
                def.resolve(cacheItem);
                return def.promise();
            }

            var query = new OneTrueError.Core.Applications.Queries.GetApplicationInfo();
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