/// <reference path="../../Scripts/Promise.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />
/// <reference path="../ChartViewModel.ts" />
/// <reference path="../../Scripts/Griffin.Yo.d.ts" />
declare function htmlentities(text: string): string;

declare function nl2br(text: string): string;

module OneTrueError.Feedback {
    import CqsClient = Griffin.Cqs.CqsClient;
    import Yo = Griffin.Yo;

    export class ApplicationViewModel implements Griffin.Yo.Spa.ViewModels.IViewModel {
        private dto: Web.Feedback.Queries.GetFeedbackForApplicationPageResult;
        private context: Griffin.Yo.Spa.ViewModels.IActivationContext;
        private RenderDirectives = {
            Items: {
                Message: {
                    html(value) {
                        return nl2br(value);
                    }
                },
                Title: {
                    style() {
                        return "color:#ccc";
                    },
                    html(value, dto) {
                        return `Reported for <a style="color: #ee99ee" href="/application/${dto.ApplicationId}">${dto.ApplicationName}</a> at ${new Date(dto.WrittenAtUtc).toLocaleString()}`;
                    }
                },
                EmailAddressVisible: {
                    style(value, dto) {
                        if (!dto.EmailAddress || dto.EmailAddress === "") {
                            return "display:none";
                        }

                        return "";
                    }
                },
                EmailAddress: {
                    text(value) {
                        return value;
                    },
                    href(value) {
                        return `mailto:${value}`;
                    },
                    style() {
                        return "color: #ee99ee";
                    }
                }
            }
        };

        getTitle(): string {
            var app = <Core.Applications.Queries.GetApplicationInfoResult>Yo.GlobalConfig.applicationScope["application"];
            if (app) {
                return app.Name;
            }
            return "Feedback";
        }

        activate(context: Griffin.Yo.Spa.ViewModels.IActivationContext): void {
            this.context = context;
            var query = new Web.Feedback.Queries.GetFeedbackForApplicationPage(context.routeData["applicationId"]);
            CqsClient.query<Web.Feedback.Queries.GetFeedbackForApplicationPageResult>(query)
                .done(result => {
                    this.dto = result;
                    context.render(result, this.RenderDirectives);
                    context.resolve();
                });
        }

        deactivate() {

        }


    }
}