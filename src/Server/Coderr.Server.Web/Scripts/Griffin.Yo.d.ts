declare module Griffin.Yo.Dom {
    class ElemUtils {
        static removeChildren(n: Node): void;
        static moveChildren(source: HTMLElement, target: HTMLElement): void;
        static getIdentifier(e: HTMLElement): string;
    }
    class EventMapper {
        private scope;
        constructor(scope?: HTMLElement);
        click(selector: string, listener: (ev: MouseEvent) => any, useCapture?: boolean): void;
        change(selector: string, listener: (ev: Event) => any, useCapture?: boolean): void;
        keyUp(selector: string, listener: (ev: KeyboardEvent) => any, useCapture?: boolean): void;
        keyDown(selector: string, listener: (ev: KeyboardEvent) => any, useCapture?: boolean): void;
    }
    class FormReader {
        private container;
        private stack;
        constructor(elemOrName: HTMLElement | string);
        read(): any;
        private pullCollection(container);
        private pullElement(container);
        private adjustCheckboxes(element, dto, value);
        private processValue(value);
        private assignByName(name, parentObject, value);
        private appendObject(target, extras);
        private isObjectEmpty(data);
        private getName(el);
        private isCollection(el);
    }
    class Selector {
        private scope;
        constructor(scope?: HTMLElement);
        one(idOrselector: string): HTMLElement;
        all(selector: string): HTMLElement[];
    }
}
declare module Griffin.Yo.Net {
    class Http {
        private static cache;
        static useCaching: boolean;
        static get(url: string, callback: (name: XMLHttpRequest, success: boolean) => void, contentType?: string): void;
        static post(url: string, data: any, callback: (name: XMLHttpRequest, success: boolean) => void, options?: IHttpOptions): void;
        static put(url: string, data: any, callback: (name: XMLHttpRequest, success: boolean) => void, options?: IHttpOptions): void;
        static delete(url: string, callback: (name: XMLHttpRequest, success: boolean) => void, options?: IHttpOptions): void;
        static invokeRequest(verb: string, url: string, data: any, callback: (name: XMLHttpRequest, success: boolean) => void, options?: IHttpOptions): void;
    }
    interface IHttpOptions {
        headers?: any;
        contentType?: string;
        userName?: string;
        password?: string;
    }
}
declare module Griffin.Yo.Routing {
    interface IRoute {
        isMatch(ctx: IRouteContext): boolean;
        invoke(ctx: IRouteContext): void;
    }
    interface IRouteContext {
        url: string;
        targetElement?: HTMLElement;
    }
    interface IViewTarget {
        name: string;
        setTitle(title: string): void;
        assignOptions(options: any): void;
        attachViewModel(script: HTMLScriptElement): void;
        render(element: HTMLElement): void;
    }
    interface IRouteExecutionContext {
        routeData: any;
        route: IRoute;
        target?: IViewTarget;
    }
    interface IRouteHandler {
        invoke(ctx: IRouteExecutionContext): void;
    }
    class Route implements IRoute {
        route: string;
        handler: IRouteHandler;
        target: IViewTarget;
        private parts;
        constructor(route: string, handler: IRouteHandler, target?: IViewTarget);
        isMatch(ctx: IRouteContext): boolean;
        invoke(ctx: IRouteContext): void;
    }
    class Router {
        private routes;
        add(route: string, handler: IRouteHandler, targetElement?: any): void;
        addRoute(route: IRoute): void;
        execute(url: string, targetElement?: any): boolean;
    }
}
declare module Griffin.Yo.Routing.ViewTargets {
    import IViewTarget = Griffin.Yo.Routing.IViewTarget;
    class BootstrapModalViewTarget implements IViewTarget {
        private currentNode;
        name: string;
        assignOptions(options: any): void;
        attachViewModel(script: HTMLScriptElement): void;
        setTitle(title: string): void;
        render(element: HTMLElement): void;
    }
    class ElementViewTarget implements IViewTarget {
        private container;
        constructor(elementOrId: string | HTMLElement);
        name: string;
        assignOptions(): void;
        attachViewModel(script: HTMLScriptElement): void;
        setTitle(title: string): void;
        render(element: HTMLElement): void;
    }
}
declare module Griffin.Yo.Spa.ViewModels {
    import Dom = Yo.Dom;
    class ClassFactory {
        static getConstructor(appName: string, viewModelModuleAndName: string): any;
    }
    interface IActivationContext {
        routeData: any;
        viewContainer: HTMLElement;
        render(data: any, directives?: any): void;
        renderPartial(viewSelector: string, data: any, directives?: any): void;
        readForm(viewSelector: string | HTMLElement): any;
        select: Dom.Selector;
        handle: Dom.EventMapper;
        resolve(): void;
        reject(): void;
        applicationScope: any;
    }
    interface IViewModel {
        getTitle(): string;
        activate(context: IActivationContext): void;
        deactivate(): void;
    }
    interface IViewModelFactory {
        create(applicationName: string, fullViewModelName: string): IViewModel;
    }
}
declare module Griffin.Yo.Spa {
    import Routing = Yo.Routing;
    import ViewModels = Yo.Spa.ViewModels;
    interface IResourceLocator {
        getHtml(section: string): string;
        getScript(section: string): string;
    }
    class Config {
        static resourceLocator: IResourceLocator;
        static viewModelFactory: ViewModels.IViewModelFactory;
        static applicationScope: any;
    }
    class RouteRunner implements Routing.IRouteHandler {
        section: string;
        private html;
        private viewModelScript;
        private viewModel;
        private applicationName;
        constructor(section: string, applicationName: string);
        static replaceAll(str: string, replaceWhat: string, replaceTo: string): string;
        private applyRouteDataToLinks(viewElement, routeData);
        private moveNavigationToMain(viewElement, context);
        private removeConditions(elem, context);
        private evalInContext(code, context);
        private isIE();
        invoke(ctx: Routing.IRouteExecutionContext): void;
        private ensureResources(callback);
        private doStep(callback);
    }
    class ScriptLoader {
        private pendingScripts;
        private embeddedScripts;
        private container;
        static dummyScriptNode: any;
        constructor(container?: HTMLElement);
        private stateChange();
        loadSources(scripts: string | string[]): void;
        loadTags(scripts: HTMLScriptElement | HTMLScriptElement[]): void;
        private loadSource(source);
        private loadElement(tag);
        onScriptLoaded(script: HTMLScriptElement): void;
        runEmbeddedScripts(): void;
    }
    class SpaEngine {
        applicationName: string;
        private router;
        private basePath;
        private applicationScope;
        private viewTargets;
        private defaultViewTarget;
        constructor(applicationName: string);
        addTarget(name: string, target: Routing.IViewTarget | string): void;
        navigate(url: string, targetElement?: any): void;
        mapRoute(route: any, section: string, target?: string): void;
        run(): void;
        private mapFunctionToRouteData(f, routeData);
    }
}
declare module Griffin.Yo.Views {
    class ViewRenderer {
        private container;
        private lineage;
        private dtoStack;
        private directives;
        private static globalValueDirectives;
        static DEBUG: boolean;
        constructor(elemOrName: HTMLElement | string);
        register(directive: IViewRenderDirective): void;
        static registerGlobal(directive: IViewRenderDirective): void;
        render(data?: any, directives?: any): void;
        private renderElement(element, data, directives?);
        private renderCollection(element, data, directive?);
        private applyEmbeddedDirectives(element, data, directives);
        private runDirectives(element, data);
        private getName(el);
        private hasName(el);
        private isCollection(el);
        private evalInContext(code, context);
        private log(...args);
    }
    class ViewValueDirectiveContext {
        value: any;
        propertyName: string;
        element: HTMLElement;
        lineage: string[];
    }
    interface IViewRenderDirective {
        process(context: ViewValueDirectiveContext): boolean;
    }
}
declare module Griffin.Yo {
    class G {
        static select: Dom.Selector;
        static handle: Dom.EventMapper;
        static render(idOrElem: any, dto: any, directives?: any): void;
    }
}
declare module Griffin.Yo {
    class GlobalConfig {
        static resourceLocator: Spa.IResourceLocator;
        static viewModelFactory: Spa.ViewModels.IViewModelFactory;
        static applicationScope: {};
    }
}
