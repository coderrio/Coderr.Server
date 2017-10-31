/// <reference path="../../Scripts/Models/AllModels.ts" />
/// <reference path="../../Scripts/Promise.ts" />
/// <reference path="../../Scripts/Griffin.WebApp.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />
/// <reference path="../ChartViewModel.ts" />
/// <reference path="../../Scripts/Griffin.Yo.d.ts" />
declare function htmlentities(text: string): string;

declare function nl2br(text: string): string;

module codeRR.Feedback {
    import CqsClient = Griffin.Cqs.CqsClient;
    import OverviewFeedback = codeRR.Web.Feedback.Queries.GetFeedbackForDashboardPage;
    import OverviewFeedbackResult = Web.Feedback.Queries.GetFeedbackForDashboardPageResult;
    import Yo = Griffin.Yo;

    export class OverviewViewModel implements Yo.Spa.ViewModels.IViewModel {
        private applicationTitle: string;
        private ctx: Yo.Spa.ViewModels.IActivationContext;
        private static renderDirectives = {
            Items: {
                Message: {
                    html(value) {
                        return nl2br(value);
                    }
                },
                Title: {
                    style() {
                        return '';
                    },
                    html(value, dto) {
                        console.log(dto);
                        return `Reported for <a href="#/application/${dto.ApplicationId}/feedback">${
                            dto.ApplicationName}</a> ${momentsAgo(dto.WrittenAtUtc)}`;
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
            Applications.Navigation.breadcrumbs([{ href: `/feedback`, title: 'Feedback' }]);
            Applications.Navigation.pageTitle = 'All feedback for all applications';
            return "All feedback";
        }

        public isEmpty() {
            return this.empty;
        }

        private empty: boolean;

        activate(context: Griffin.Yo.Spa.ViewModels.IActivationContext): void {
            const query = new OverviewFeedback();
            CqsClient.query<OverviewFeedbackResult>(query)
                .done(result => {
                    this.empty = result.TotalCount === 0;
                    context.render(result, OverviewViewModel.renderDirectives);
                    context.resolve();
                });

        }

        deactivate() {

        }
    }
}