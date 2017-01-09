/// <reference path="../../Scripts/Promise.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />
/// <reference path="../../Scripts/Griffin.Yo.d.ts" />
/// <reference path="../../Scripts/typings/jquery/jquery.d.ts" />
/// <reference path="../../Scripts/Models/AllModels.ts" />
var OneTrueError;
(function (OneTrueError) {
    var Incident;
    (function (Incident) {
        var CqsClient = Griffin.Cqs.CqsClient;
        var ViewRenderer = Griffin.Yo.Views.ViewRenderer;
        var Similarities = OneTrueError.Modules.ContextData;
        var SimilaritiesViewModel = (function () {
            function SimilaritiesViewModel() {
            }
            SimilaritiesViewModel.prototype.activate = function (context) {
                var _this = this;
                this.incidentId = parseInt(context.routeData["incidentId"], 10);
                this.ctx = context;
                CqsClient
                    .query(new Similarities.Queries.GetSimilarities(this.incidentId))
                    .done(function (result) {
                    _this.dto = result;
                    context.render(result);
                    context.resolve();
                });
                //context.render(result);
                context.handle.click("#ContextCollections", function (evt) {
                    var target = evt.target;
                    if (target.tagName === "LI") {
                        _this.selectCollection(target.firstElementChild.textContent);
                        $("li", target.parentElement).removeClass("active");
                        $(target).addClass("active");
                    }
                    else if (target.tagName === "A") {
                        _this.selectCollection(target.textContent);
                        $("li", target.parentElement.parentElement).removeClass("active");
                        $(target.parentElement).addClass("active");
                    }
                }, true);
                context.handle.click("#ContextProperty", function (evt) {
                    var target = evt.target;
                    if (target.tagName === "LI") {
                        _this.selectProperty(target.firstElementChild.textContent);
                        $("li", target.parentElement).removeClass("active");
                        $(target).addClass("active");
                    }
                    else if (target.tagName === "A") {
                        _this.selectProperty(target.textContent);
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
            };
            SimilaritiesViewModel.prototype.getTitle = function () {
                return "context data";
            };
            SimilaritiesViewModel.prototype.deactivate = function () {
            };
            SimilaritiesViewModel.prototype.selectCollection = function (collectionName) {
                var _this = this;
                this.dto.Collections.forEach(function (item) {
                    if (item.Name === collectionName) {
                        var directives = {
                            SimilarityName: {
                                html: function (value, dto) {
                                    return dto.Name;
                                }
                            }
                        };
                        _this.currentCollection = item;
                        var container = _this.ctx.select.one("ContextProperty");
                        container.style.display = "";
                        var renderer = new ViewRenderer(container);
                        renderer.render(item.Similarities, directives);
                        _this.ctx.select.one("Values").style.display = "none";
                        return;
                    }
                });
            };
            SimilaritiesViewModel.prototype.selectProperty = function (name) {
                var _this = this;
                var self = this;
                this.currentCollection.Similarities.forEach(function (item) {
                    if (item.Name === name) {
                        var directives = {
                            Value: {
                                html: function (value, dto) {
                                    return value;
                                }
                            }
                        };
                        var elem = _this.ctx.select.one("Values");
                        elem.style.display = "";
                        var renderer = new ViewRenderer(elem);
                        renderer.render(item.Values, directives);
                        return;
                    }
                });
            };
            SimilaritiesViewModel.VIEW_NAME = "SimilaritiesView";
            return SimilaritiesViewModel;
        }());
        Incident.SimilaritiesViewModel = SimilaritiesViewModel;
    })(Incident = OneTrueError.Incident || (OneTrueError.Incident = {}));
})(OneTrueError || (OneTrueError = {}));
//# sourceMappingURL=SimilaritiesViewModel.js.map