/// <reference path="../../Scripts/Promise.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />
/// <reference path="../ChartViewModel.ts" />
/// <reference path="../../Scripts/Griffin.Yo.d.ts" />
declare function htmlentities(text: string): string;

declare function nl2br(text: string): string;

module codeRR.Feedback {
    import CqsClient = Griffin.Cqs.CqsClient;
    import Yo = Griffin.Yo;
    import ApplicationService = codeRR.Applications.ApplicationService;

    export class ApplicationViewModel implements Griffin.Yo.Spa.ViewModels.IViewModel {
        private dto: Web.Feedback.Queries.GetFeedbackForApplicationPageResult;
        private context: Griffin.Yo.Spa.ViewModels.IActivationContext;
        private applicationId: number;
        private renderDirectives = {
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
                        return `Reported for <a href="#/application/${dto.applicationId}/incident/${dto.IncidentId}">${
                            dto.IncidentName}</a> at ${momentsAgo(dto.WrittenAtUtc)}`;
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

        getTitle(): string {
            var appId = this.context.routeData['applicationId'];
            var app = new ApplicationService();
            app.get(appId)
                .then(result => {
                    var bc: Applications.IBreadcrumb[] = [
                        { href: `/application/${appId}/`, title: result.Name },
                        { href: `/application/${appId}/feedback`, title: 'Feedback' }
                    ];
                    Applications.Navigation.breadcrumbs(bc);
                    Applications.Navigation.pageTitle = 'Feedback';
                });

            return "Feedback";
        }

        activate(context: Griffin.Yo.Spa.ViewModels.IActivationContext): void {
            this.context = context;
            const query = new Web.Feedback.Queries.GetFeedbackForApplicationPage(context.routeData["applicationId"]);
            CqsClient.query<Web.Feedback.Queries.GetFeedbackForApplicationPageResult>(query)
                .done(result => {
                    this.dto = result;
                    this.dto.Items.forEach(item => {
                        item['applicationId'] = context.routeData['applicationId'];
                    });
                    context.render(result, this.renderDirectives);
                    context.resolve();
                });
        }

        deactivate() {

        }


    }
}