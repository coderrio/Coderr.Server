/// <reference path="../../Scripts/Griffin.Yo.d.ts" />
/// <reference path="../../app/Application.ts" />
module codeRR.Application {
    import ApplicationVersions = codeRR.Modules.Versions.Queries.GetApplicationVersions;
    import CqsClient = Griffin.Cqs.CqsClient;
    import Yo = Griffin.Yo;

    export class VersionsViewModel implements Griffin.Yo.Spa.ViewModels.IViewModel {
        getTitle(): string { return "Application versions"; }

        activate(context: Griffin.Yo.Spa.ViewModels.IActivationContext): void {
            var query = new ApplicationVersions(context.routeData["applicationId"]);
            CqsClient.query(query)
                .done(result => {
                    console.log((<any>result).Items.length);
                    this.haveItems = (<any>result).Items.length > 0;
                    console.log(this.haveItems);
                    var directives = {
                            FirstReportReceivedAtUtc: {
                                text(value) {
                                    return new Date(value).toLocaleString();
                                }
                            },
                            LastReportReceivedAtUtc: {
                                text(value) {
                                    return new Date(value).toLocaleString();
                                }
                            }
                        
                    };
                    var itemsElem = context.viewContainer.querySelector("#itemsTable") as HTMLElement;
                    context.render(result, { Items: directives });
                    //Yo.G.render(itemsElem, (<any>result).Items, directives);
                    context.resolve();
                });
        }

        public haveItems: boolean;
            
        
        deactivate() {}
    }
}