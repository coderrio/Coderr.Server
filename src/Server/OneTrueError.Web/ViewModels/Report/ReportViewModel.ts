/// <reference path="../../Scripts/Promise.ts"/>
/// <reference path="../../Scripts/Griffin.WebApp.ts" />
/// <reference path="../../Scripts/CqsClient.ts"/>
/// <reference path="../../Scripts/Griffin.Yo.d.ts"/>

module OneTrueError.Report {
    import CqsClient = Griffin.Cqs.CqsClient;
    import ClickEventArgs = Griffin.WebApp.ClickEventArgs;
    import Yo = Griffin.Yo;
    import ReportResult = OneTrueError.Core.Reports.Queries.GetReportResult;
    import ReportResultContextCollection = OneTrueError.Core.Reports.Queries.GetReportResultContextCollection;

    export class ReportViewModel implements Griffin.Yo.Spa.ViewModels.IViewModel {
        private context: Griffin.Yo.Spa.ViewModels.IActivationContext;
        private dto: ReportResult;

        private renderView(): void {
            var directives = {
                CreatedAtUtc: {
                    text(value) {
                        return new Date(value).toLocaleString();
                    }
                },
                ContextCollections: {
                    ContextCollectionName: {
                        html(value, dto) {
                            return dto.Name;
                        },
                        href(value,dto) {
                            return '#' + dto.Name;
                        }
                    }
                }
            };
            this.context.render(this.dto, directives);
        }

        getTitle(): string {
            return 'Report';

        }

        activate(context: Griffin.Yo.Spa.ViewModels.IActivationContext): void {
            this.context = context;
            var reportId = context.routeData["reportId"];
            var query = new Core.Reports.Queries.GetReport(reportId);
            CqsClient.query<ReportResult>(query)
                .done(dto => {
                    this.dto = dto;
                    this.renderView();
                    context.resolve();
                });

            context.handle.click('[data-collection="ContextCollections"]', evt => {
                evt.preventDefault();
                var target = <HTMLElement>evt.target;
                if (target.tagName === 'LI') {
                    this.selectCollection(target.firstElementChild.textContent);
                    $('li', target.parentElement).removeClass('active');
                    $(target).addClass('active');
                } else if (target.tagName === 'A') {
                    this.selectCollection(target.textContent);
                    $('li', target.parentElement.parentElement).removeClass('active');
                    $(target.parentElement).addClass('active');
                }
            }, true);
            
        }

        private selectCollection(collectionName: string) {
            this.dto.ContextCollections.forEach(item => {
                if (item.Name === collectionName) {
                    var directives = {
                    Properties: {
                            Key: {
                                html(value) {
                                    return value;
                                }
                            },
                            Value: {
                                html(value, dto) {
                                    console.log('here')
                                    if (collectionName === 'Screenshots') {
                                        return '<img alt="Embedded Image" src="data:image/png;base64,' + value + '" />';
                                    } else {
                                        return value.replace(/;;/g, "<br>");
                                    }
                                }
                            }
                        }
                    };
                    console.log('item', item);
                    this.context.renderPartial('propertyTable', item, directives);
                    return;
                }
            });

        }


        deactivate() {}
    }
}