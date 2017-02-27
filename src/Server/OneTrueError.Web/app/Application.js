/// <reference path="../Scripts/Models/AllModels.ts" />
/// <reference path="../Scripts/Griffin.Yo.d.ts" />
/// <reference path="../Scripts/CqsClient.ts" />
var OneTrueError;
(function (OneTrueError) {
    var Applications;
    (function (Applications) {
        var Yo = Griffin.Yo;
        var CqsClient = Griffin.Cqs.CqsClient;
        var ApplicationService = (function () {
            function ApplicationService() {
            }
            ApplicationService.prototype.get = function (applicationId) {
                var def = P.defer();
                var cacheItem = Yo.GlobalConfig
                    .applicationScope["application"];
                if (cacheItem && cacheItem.Id === applicationId) {
                    def.resolve(cacheItem);
                    return def.promise();
                }
                var query = new OneTrueError.Core.Applications.Queries.GetApplicationInfo();
                query.ApplicationId = applicationId;
                CqsClient.query(query)
                    .done(function (result) {
                    Yo.GlobalConfig.applicationScope["application"] = result;
                    def.resolve(result);
                });
                return def.promise();
            };
            return ApplicationService;
        }());
        Applications.ApplicationService = ApplicationService;
    })(Applications = OneTrueError.Applications || (OneTrueError.Applications = {}));
})(OneTrueError || (OneTrueError = {}));
;
//# sourceMappingURL=Application.js.map