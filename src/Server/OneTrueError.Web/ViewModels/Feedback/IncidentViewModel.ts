/// <reference path="../../Scripts/Promise.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />
/// <reference path="../../Scripts/Griffin.Yo.d.ts" />
/// <reference path="../ChartViewModel.ts" />
declare function htmlentities(text: string): string;
declare function nl2br(text: string): string;

module OneTrueError.Feedback {
    import CqsClient = Griffin.Cqs.CqsClient;
    import GetIncidentFeedback = Web.Feedback.Queries.GetIncidentFeedback;
    import GetIncidentFeedbackResult = Web.Feedback.Queries.GetIncidentFeedbackResult;

    export class IncidentViewModel implements Griffin.Yo.Spa.ViewModels.IViewModel {
        private ctx: Griffin.Yo.Spa.ViewModels.IActivationContext;
        private static directives = {
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
                        return `Written at ${new Date(dto.WrittenAtUtc).toLocaleString()}`;
                    }
                },
                EmailAddress: {
                    text(value) {
                        return value;
                    },
                    href(value) {
                        return `mailto:${value}`;
                    },
                    style(value) {
                        if (!value) {
                            return "display:none";
                        }
                        return "color: #ee99ee";
                    }
                }
            }
        };

        activate(ctx: Griffin.Yo.Spa.ViewModels.IActivationContext) {
            this.ctx = ctx;
            var query = new GetIncidentFeedback(ctx.routeData["incidentId"]);
            CqsClient.query<GetIncidentFeedbackResult>(query).done(result => {
                this.ctx.render(result, IncidentViewModel.directives);
                ctx.resolve();
            });


        }

        deactivate() {
        }

        getTitle(): string {
            return "Incident";
        }
    }
}