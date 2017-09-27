/// <reference path="../../Scripts/Promise.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />
/// <reference path="../../Scripts/Griffin.Yo.d.ts" />
/// <reference path="../../Scripts/typings/jquery/jquery.d.ts" />
/// <reference path="../../Scripts/Models/AllModels.ts" />
module codeRR.Incident {
    import CqsClient = Griffin.Cqs.CqsClient;
    import ViewRenderer = Griffin.Yo.Views.ViewRenderer;
    import Similarities = Modules.ContextData;
    import ApplicationService = codeRR.Applications.ApplicationService;

    export class SimilaritiesViewModel implements Griffin.Yo.Spa.ViewModels.IViewModel {
        private static VIEW_NAME = "SimilaritiesView";
        private dto: Similarities.Queries.GetSimilaritiesResult;
        private currentCollection: Similarities.Queries.GetSimilaritiesCollection;
        private incidentId: number;
        private ctx: Griffin.Yo.Spa.ViewModels.IActivationContext;

        activate(context: Griffin.Yo.Spa.ViewModels.IActivationContext) {
            this.incidentId = parseInt(context.routeData["incidentId"], 10);
            this.ctx = context;

            //spans the navigation.
            IncidentNavigation.set(context.routeData, 'Context data analysis <em class="small">(Presents what context data all reports had in common)</em>', 'contextdata');


            CqsClient
                .query<Similarities.Queries.
                    GetSimilaritiesResult>(new Similarities.Queries.GetSimilarities(this.incidentId))
                .done(result => {
                    this.dto = result;
                    context.render(result);
                    context.resolve();
                });
            //context.render(result);
            context.handle.click("#ContextCollections",
                evt => {
                    var target = evt.target as HTMLElement;
                    if (target.tagName === "LI") {
                        this.selectCollection(target.firstElementChild.textContent);
                        $("li", target.parentElement).removeClass("active");
                        $(target).addClass("active");
                    } else if (target.tagName === "A") {
                        this.selectCollection(target.textContent);
                        $("li", target.parentElement.parentElement).removeClass("active");
                        $(target.parentElement).addClass("active");
                    }
                },
                true);
            context.handle.click("#ContextProperty",
                evt => {
                    var target = evt.target as HTMLElement;
                    if (target.tagName === "LI") {
                        this.selectProperty(target.firstElementChild.textContent);
                        $("li", target.parentElement).removeClass("active");
                        $(target).addClass("active");
                    } else if (target.tagName === "A") {
                        this.selectProperty(target.textContent);
                        $("li", target.parentElement.parentElement).removeClass("active");
                        $(target.parentElement).addClass("active");
                    }
                });

            //    var service = new ApplicationService();
            //    var appId = parseInt(context.routeData["applicationId"], 10);
            //    service.get(appId).done(result => {
            //        this.app = result;
            //    })

            //    context.resolve();
            //});


        }

        getTitle(): string {
            return "Similarities";
        }

        deactivate() {

        }
        


        private selectCollection(collectionName: string) {
            this.dto.Collections.forEach(item => {
                if (item.Name === collectionName) {
                    const directives = {
                        SimilarityName: {
                            html(value, dto) {
                                return dto.Name;
                            }
                        }
                    };

                    this.currentCollection = item;
                    const container = this.ctx.select.one("ContextProperty");
                    container.style.display = "";
                    const renderer = new ViewRenderer(container);
                    renderer.render(item.Similarities, directives);
                    this.ctx.select.one("Values").style.display = "none";
                    return;
                }
            });

        }

        private selectProperty(name: string) {
            const self = this;
            this.currentCollection.Similarities.forEach(item => {
                if (item.Name === name) {
                    const directives = {
                        Value: {
                            html(value, dto) {
                                return value;
                            }
                        }
                    };
                    const elem = this.ctx.select.one("Values");
                    elem.style.display = "";
                    const renderer = new ViewRenderer(elem);
                    renderer.render(item.Values, directives);
                    return;
                }
            });
        }

        app: Core.Applications.Queries.GetApplicationInfoResult;
    }

}