/// <reference path="../../Scripts/Models/AllModels.ts" />
/// <reference path="../../Scripts/Promise.ts" />
/// <reference path="../../Scripts/Griffin.WebApp.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />
/// <reference path="../ChartViewModel.ts" />
/// <reference path="../../Scripts/Griffin.Yo.d.ts" />
declare function htmlentities(text: string): string;

declare function nl2br(text: string): string;

module OneTrueError.Feedback {
    import CqsClient = Griffin.Cqs.CqsClient;
    import OverviewFeedback = Web.Feedback.Queries.GetFeedbackForDashboardPage;
    import OverviewFeedbackResult = Web.Feedback.Queries.GetFeedbackForDashboardPageResult;
    import Yo = Griffin.Yo; //import Yo = Griffin.Yo;

    export class OverviewViewModel implements Yo.Spa.ViewModels.IViewModel {
        private applicationTitle: string;
        private ctx: Yo.Spa.ViewModels.IActivationContext;
        private feedbackDirectives = {
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
                        return `Reported for <a style="color: #ee99ee" href="#/application/${dto.ApplicationId}">${dto.ApplicationName}</a> at ${new Date(dto.WrittenAtUtc).toLocaleString()}`;
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

        constructor() {


        }


        getTitle(): string {
            return "All feedback";
        }

        activate(context: Griffin.Yo.Spa.ViewModels.IActivationContext): void {
            var query = new OverviewFeedback();
            CqsClient.query<OverviewFeedbackResult>(query).done(result => {
                context.render(result, this.feedbackDirectives);
                context.resolve();
            });

        }

        deactivate() {

        }
    }
}