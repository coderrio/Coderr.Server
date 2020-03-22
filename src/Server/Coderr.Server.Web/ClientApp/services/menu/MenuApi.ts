export interface MenuItem {
    title: string;
    url: string;
    icon?: string;
    tag?: any;
    children?: MenuItem[];
}

export class MessagingTopics {
    static IgnoredReportCountUpdated = "/menu/missedreportcount/updated/";
}
