import { ApiClient } from "./ApiClient";
import { IncidentService } from "./incidents/IncidentService";
import { ApplicationService } from "./applications/ApplicationService";
import { PubSubService } from './PubSub';
import * as toastr from "toastr";
import $ from 'jquery';
import * as localForage from "localforage"

export interface IModalResult {
    pressedButtonName: "submit" | "cancel";
    modalId: string;
    pressedButton: HTMLButtonElement;
}
export interface IModalContext {
    title?: string;
    contentId?: string;
    htmlContent?: string;
    submitButtonText?: string;
    cancelButtonText?: string;
    footerHint?: string;
    showFooter?: boolean
    onShowingModal?: (modalId: string) => void;
    onModalShown?: (modalId: string) => void;
    onClosingModal?: (modalId: string, pressedButtonName: "submit" | "cancel") => void;
}

export interface IMyApplication {
    id: number;
    name: string;
    isAdmin: boolean;
}

export interface IUser {
    id: number;
    name: string;
    isSysAdmin: boolean;
    applications: IMyApplication[];
}

export interface IStoreStateSettings {
    name: string;
    component: any;
    includeProperties?: string[];
    excludeProperties?: string[];
}

export class AppRoot {
    static Instance: AppRoot = new AppRoot();
    public currentUser: IUser;
    public apiClient: ApiClient;
    public incidentService: IncidentService;
    public applicationService: ApplicationService;

    constructor() {
        var base = <HTMLBaseElement>document.head.querySelector('base');
        var apiUrl = base.href + "api/";
        this.apiClient= new ApiClient(apiUrl);
        this.incidentService= new IncidentService(this.apiClient);
        this.applicationService = new ApplicationService(PubSubService.Instance, this.apiClient);
    }

    // current user is always validated server side
    // this is more to adjust the UI
    async loadCurrentUser(): Promise<IUser> {
        if (this.currentUser) {
            return this.currentUser;
        }

        var usr = await this.apiClient.auth();
        var apps: IMyApplication[] = [];
        usr.Applications.forEach((app: any) => {
            apps.push({
                id: app.Id,
                name: app.Name,
                isAdmin: app.IsAdmin
            });
        });

        var currentUser: IUser = {
            id: usr.AccountId,
            name: usr.UserName,
            isSysAdmin: usr.IsSysAdmin,
            applications: apps
        };

        this.currentUser = currentUser;
        return this.currentUser;
    }

    static notify(msg: string, icon: string = 'glyphicon glyphicon-info-sign', type: string = 'info') {
        toastr[type](msg);
        return;
    }

    storeState(options: IStoreStateSettings) {
        if (options.component.$data)
            options.component = options.component.$data;

        var ctx: any = {};
        for (var key in options.component) {
            if (options.component.hasOwnProperty(key)) {
                var value = options.component[key];
                if (options.excludeProperties && options.excludeProperties.indexOf(key) !== -1) {
                    continue;
                }
                if (options.includeProperties && options.includeProperties.indexOf(key) === -1) {
                    continue;
                }
                if (key.substr(0, 1) === '$' || key.substr(key.length - 1, 1) === '$') {
                    continue;
                }

                if (value != null && value !== '' && value !== 0 && value !== '0') {

                    // to get rid of Vue observers.
                    var val2 = JSON.parse(JSON.stringify(options.component[key]));
                    ctx[key] = val2;
                }
            }
        }

        localForage.setItem(options.name, ctx)
            .then(ey => {
                localForage.getItem(options.name);
            })
            .catch(reason => {
                console.log('ERROR', reason);
            });
    }

    async loadState(name: string, component: any): Promise<boolean> {
        var result = <any>await localForage.getItem(name);
        if (!result) 
            return false;

        var data = result;
        for (var key in data) {
            if (data.hasOwnProperty(key)) {
                let value = data[key];
                if (value instanceof Array) {
                    component[key].length = 0;
                    for (var i = 0; i < value.length; i++) {
                        component[key].push(value[i]);
                    }
                } else {
                    component[key] = value;
                }

            }
        }

        return true;
    }

    private static modalIdCounter = 1;
    static modal(modalContext: IModalContext): Promise<IModalResult> {
        var container = document.createElement('div');
        var templateNode = <HTMLDivElement>document.getElementById('modalTemplate');
        if (!templateNode) {
            throw new Error("Failed to find templateNode '#modalTemplate'.");
        }

        var ourNode = templateNode.cloneNode(true);
        container.appendChild(ourNode);
        var html = container.innerHTML;

        var id = 'modal' + this.modalIdCounter++;
        html = html.replace(/(modalTemplate)/g, id);
        container.innerHTML = html;

        var myModal = <HTMLDivElement>container.firstElementChild;
        document.body.appendChild(myModal);

        if (modalContext.onShowingModal) {
            modalContext.onShowingModal(id);
        }

        if (modalContext.cancelButtonText) {
            const el = <HTMLButtonElement>myModal.querySelector('.btn-secondary');
            el.innerHTML = modalContext.cancelButtonText;
        }
        if (modalContext.submitButtonText) {
            const el = <HTMLButtonElement>myModal.querySelector('.btn-primary');
            el.innerHTML = modalContext.submitButtonText;
            $('.btn-primary', myModal).text(modalContext.submitButtonText);
        }
        if (modalContext.title) {
            const node = <HTMLDivElement>myModal.querySelector('.modal-title');
            node.innerHTML = modalContext.title;
        } else {
            const node = <HTMLDivElement>myModal.querySelector('.modal-header');
            const parent = <HTMLElement>node.parentElement;
            parent.removeChild(node);
        }

        if (modalContext.showFooter === false) {
            const node = <HTMLElement>myModal.querySelector('.modal-footer');
            const parent = <HTMLElement>node.parentElement;
            parent.removeChild(node);
        }

        var ourBody = '';
        if (modalContext.contentId) {
            let id = modalContext.contentId;
            if (id.substr(0, 1) === '#') {
                id = id.substr(1);
            }
            var el = document.getElementById(id);
            if (!el)
                throw Error("Failed to find modal body " + modalContext.contentId);

            var hint = <HTMLElement>el.querySelector('[data-target="footerHint"]');
            if (hint != null) {
                modalContext.footerHint = hint.innerHTML;
                hint.parentElement.removeChild(hint);
            }
            ourBody = el.innerHTML;
        } else {
            ourBody = `<div>${modalContext.htmlContent}</div>`;
        }

        const body = <HTMLDivElement>myModal.querySelector('.modal-body');
        if (!body) {
            throw new Error("Your div must have a CSS class with name 'modal-body'.");
        }
        

        body.innerHTML = ourBody;
        body.style.display = '';
        (<HTMLElement>body.firstElementChild).style.display = '';

        // need to be after body handling since we can attach a body.
        if (modalContext.footerHint) {
            const node = <HTMLSpanElement>myModal.querySelector('[data-name="hint"]');
            node.innerHTML = modalContext.footerHint;
        }

        var bsModal = $(myModal).modal({ show: false });

        if (modalContext.onModalShown) {
            var callbackCopy = modalContext.onModalShown;
            bsModal.on('shown.bs.modal',
                () => {
                    callbackCopy(id);
                });
        }
        bsModal.on('hidden.bs.modal',
            () => {
                bsModal.remove();
                myModal.remove();
            });

        bsModal.modal('show');

        return new Promise<IModalResult>((resolve, reject) => {
            $('form', bsModal).submit(e => {
                e.preventDefault();
                if (modalContext.onClosingModal) {
                    modalContext.onClosingModal(id, 'submit');
                }
                bsModal.modal('hide');
                resolve({ pressedButtonName: 'submit', modalId: id, pressedButton: <HTMLButtonElement>e.target });
            });
            $('.btn-secondary', bsModal).click(e => {
                if (modalContext.onClosingModal) {
                    modalContext.onClosingModal(id, 'cancel');
                }
                bsModal.modal('hide');
                reject({ pressedButtonName: 'cancel', modalId: id, pressedButton: <HTMLButtonElement>e.target });
            });

        });
    }
}

