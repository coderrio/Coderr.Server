export interface MenuItem {
    title: string;
    url: string;
    icon?: string;
    tag?: any;
    children?: MenuItem[];
}

export class Menus {
	static Discover: string = "Discover";
}

export class MessagingTopics
{
	static ChangeMenu: string = "/menu/change/";
}

export class ChangeMenu {
    menuName: string;
    routeData: any;
}