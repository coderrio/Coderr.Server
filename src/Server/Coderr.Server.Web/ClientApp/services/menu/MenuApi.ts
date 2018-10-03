export interface MenuItem {
    title: string;
    url: string;
    icon?: string;
    tag?: any;
    children?: MenuItem[];
}

export class MessagingTopics
{
    static SetApplication: string = "/menu/application/set/";
    static ApplicationChanged: string = "/menu/application/changed/";
    static IgnoredReportCountUpdated = "/menu/missedreportcount/updated/";
}

export class SetApplication {
    applicationId: number;
}

export class ApplicationChanged {
    applicationId: number;
}