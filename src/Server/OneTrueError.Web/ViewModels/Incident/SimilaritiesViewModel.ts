/// <reference path="../../Scripts/Promise.ts"/>
/// <reference path="../../Scripts/CqsClient.ts"/>
/// <reference path="../../Scripts/Griffin.Yo.d.ts"/>
/// <reference path="../../Scripts/typings/jquery/jquery.d.ts"/>
/// <reference path="../../Scripts/Models/AllModels.ts"/>

module OneTrueError.Incident {
    import CqsClient = Griffin.Cqs.CqsClient;
    import ViewRenderer = Griffin.Yo.Views.ViewRenderer;
    import Similarities = OneTrueError.Modules.ContextData;

    export class SimilaritiesViewModel implements Griffin.Yo.Spa.ViewModels.IViewModel {
        private static VIEW_NAME: string = "SimilaritiesView";
        private dto: Similarities.Queries.GetSimilaritiesResult;
        private currentCollection: Similarities.Queries.GetSimilaritiesCollection;
        private incidentId: number;
        private ctx: Griffin.Yo.Spa.ViewModels.IActivationContext;

        public activate(context: Griffin.Yo.Spa.ViewModels.IActivationContext) {
            this.incidentId = parseInt(context.routeData["incidentId"], 10);
            this.ctx = context;
            CqsClient.query<Similarities.Queries.GetSimilaritiesResult>(new Similarities.Queries.GetSimilarities(this.incidentId))
                .done(result => {
                this.dto = result;
                    context.render(result);
                    context.resolve();
                });
                //context.render(result);
            context.handle.click('#ContextCollections', evt => {
                var target = <HTMLElement>evt.target;
                if (target.tagName === 'LI') {
                    this.selectCollection(target.firstElementChild.textContent);
                } else if (target.tagName === 'A') {
                    this.selectCollection(target.textContent);
                }
            }, true);
            context.handle.click('#ContextProperty', evt => {
                var target = <HTMLElement>evt.target;
                if (target.tagName === 'LI') {
                    this.selectProperty(target.firstElementChild.textContent);
                } else if (target.tagName === 'A') {
                    this.selectProperty(target.textContent);
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

        public getTitle(): string {
            return "context data";
        }

        public deactivate() {

        }

        private selectCollection(collectionName: string) {
            this.dto.Collections.forEach(item => {
                if (item.Name === collectionName) {
                    var directives = {
                        SimilarityName: {
                            html(value, dto) {
                                return dto.Name;
                            }
                        }
                    };

                    this.currentCollection = item;
                    var container = this.ctx.select.one('ContextProperty');
                    container.style.display = '';
                    var renderer = new ViewRenderer(container);
                    renderer.render(item.Similarities, directives);
                    this.ctx.select.one("Values").style.display = 'none';
                    return;
                }
            });

        }

        private selectProperty(name: string) {
            var self = this;
            this.currentCollection.Similarities.forEach(item => {
                if (item.Name === name) {
                    var directives = {
                        Value: {
                            html(value, dto) {
                                return value;
                            }
                        }
                    };
                    var elem = this.ctx.select.one("Values");
                    elem.style.display = '';
                    var renderer = new ViewRenderer(elem);
                    renderer.render(item.Values, directives);
                    return;
                }
            });
        }

        app: OneTrueError.Core.Applications.Queries.GetApplicationInfoResult;
    }

}