/// <reference path="../../Scripts/Promise.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />
/// <reference path="../../Scripts/Griffin.Yo.d.ts" />
/// <reference path="../ChartViewModel.ts" />
declare function htmlentities(text: string): string;

declare function nl2br(text: string): string;

module codeRR.Feedback {
    import CqsClient = Griffin.Cqs.CqsClient;
    import GetIncidentFeedback = Web.Feedback.Queries.GetIncidentFeedback;
    import GetIncidentFeedbackResult = Web.Feedback.Queries.GetIncidentFeedbackResult;
    import ApplicationService = codeRR.Applications.ApplicationService;

    export class IncidentViewModel implements Griffin.Yo.Spa.ViewModels.IViewModel {
        private ctx: Griffin.Yo.Spa.ViewModels.IActivationContext;
        private incidentId: number;
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
                        return `Written ${momentsAgo(dto.WrittenAtUtc)}`;
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
            this.updateNavigation();
            this.incidentId = ctx.routeData["incidentId"];
            const query = new GetIncidentFeedback(ctx.routeData["incidentId"]);
            CqsClient.query<GetIncidentFeedbackResult>(query)
                .done(result => {
                    this.ctx.render(result, IncidentViewModel.directives);
                    ctx.resolve();
                });


        }

        private updateNavigation() {
            var appId = this.ctx.routeData['applicationId'];
            if (appId != null) {
                var app = new ApplicationService();
                app.get(appId)
                    .then(result => {
                        var bc: Applications.IBreadcrumb[] = [
                            { href: `/application/${appId}/`, title: result.Name },
                            { href: `/application/${appId}/incident/${this.incidentId}/`, title: 'Incident' },
                            { href: `/application/${appId}/incident/${this.incidentId}/feedback`, title: 'Feedback' }
                        ];
                        Applications.Navigation.breadcrumbs(bc);
                        Applications.Navigation.pageTitle = 'Feedback';
                    });
            }
        }

        deactivate() {
        }

        getTitle(): string {
            return "Incident";
        }
    }
}