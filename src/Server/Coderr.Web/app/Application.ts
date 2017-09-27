/// <reference path="../Scripts/Models/AllModels.ts" />
/// <reference path="../Scripts/Griffin.Yo.d.ts" />
/// <reference path="../Scripts/CqsClient.ts" />
module codeRR.Applications {
    import Yo = Griffin.Yo;
    import CqsClient = Griffin.Cqs.CqsClient;

    export interface IBreadcrumb {
        title: string;
        href: string;
    }

    export class MenuItem {
        public title: string;
        public url: string;
        public awesomeIcon: string;
    }

    export class Navigation {
        public static breadcrumbs(items: IBreadcrumb[]) {
            var str = `<li class="breadcrumb-item"><a href="#/">Dashboard</a></li>`;
            items.forEach(item => {
                str += `<li class="breadcrumb-item"><a href="#${item.href}">${item.title}</a></li>`;
            });
            $('#breadCrumbs').html(str);
        }
        public static set pageTitle(name: string) {
            $('#pageTitle').html(name);
        }

        static setPageMenu(items?: MenuItem[]) {
            var container = document.getElementById('pageMenu');
            container.innerHTML = '';
            if (name == null || items == null) {
                return;
            }

            items.forEach(function(item) {
                var i = document.createElement("i");
                i.className = "fa fa-" + item.awesomeIcon;

                var a = <HTMLAnchorElement>document.createElement('a');
                a.className = "text-dark";
                a.href = item.url;
                a.setAttribute("data-title", item.title);
                a.appendChild(i);
                container.appendChild(a);
                if (item.title) {
                    var aa = <any>$(a);
                    aa.tooltip();
                }
            });
        }
    }

    export class ApplicationService {
        //private static cache: ICache = new WindowCache();

        get(applicationId: number): P.Promise<Core.Applications.Queries.GetApplicationInfoResult> {
            var def = P.defer<Core.Applications.Queries.GetApplicationInfoResult>();

            const cacheItem = Yo.GlobalConfig
                .applicationScope["application"] as Core.Applications.Queries.GetApplicationInfoResult;
            if (cacheItem && cacheItem.Id === applicationId) {
                def.resolve(cacheItem);
                return def.promise();
            }

            const query = new Core.Applications.Queries.GetApplicationInfo();
            query.ApplicationId = applicationId;
            CqsClient.query<Core.Applications.Queries.GetApplicationInfoResult>(query)
                .done(result => {
                    Yo.GlobalConfig.applicationScope["application"] = result;
                    def.resolve(result);
                });

            return def.promise();
        }
    }

    export interface ICache {
        get(key: string): any;
        set(key: string, value: any);
        exists(key: string): boolean;
    }

    export class WindowCache implements ICache {
        private static cacheCounter: number = 0;
        private items: { [id: string]: any };
        private id: number;

        constructor() {
            this.id = WindowCache.cacheCounter++;
            //window["codeRRCache"+this.id] = 
        }

        get(key: string): any {
            return this.items[key];
        }

        set(key: string, value: any) {
            this.items[key] = value;
        }

        exists(key: string): boolean {
            return this.items.hasOwnProperty(key);
        }
    }
};