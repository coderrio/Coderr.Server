"use strict";

var Griffin;
(function (Griffin) {
    var Yo;
    (function (Yo) {
        var Dom;
        (function (Dom) {
            var ElemUtils = (function () {
                function ElemUtils() {}
                ElemUtils.removeChildren = function (n) {
                    if (!n) {
                        throw new Error("Element not set: " + n);
                    }
                    while (n.firstChild) {
                        n.removeChild(n.firstChild);
                    }
                };
                ElemUtils.moveChildren = function (source, target) {
                    while (source.firstChild) {
                        target.appendChild(source.firstChild);
                    }
                    if (source.parentElement) {
                        source.parentElement.removeChild(source);
                    } else {
                        source.remove();
                    }
                };
                ElemUtils.getIdentifier = function (e) {
                    if (e.id) return e.id;
                    var name = e.getAttribute("name");
                    if (name != null) return name;
                    name = e.getAttribute("data-name");
                    if (name != null) return name;
                    var attrs = '';
                    for (var i = 0; i < e.attributes.length; i++) {
                        attrs = attrs + e.attributes[i].name + "=" + e.attributes[i].value + ",";
                    }
                    return e.tagName + "[" + attrs.substr(0, attrs.length - 1) + "]";
                };
                return ElemUtils;
            })();
            Dom.ElemUtils = ElemUtils;
            var EventMapper = (function () {
                function EventMapper(scope) {
                    if (typeof scope === "undefined") {
                        this.scope = document;
                    } else {
                        this.scope = scope;
                    }
                }
                EventMapper.prototype.click = function (selector, listener, useCapture) {
                    var items = this.scope.querySelectorAll(selector);
                    if (items.length === 0) throw new Error("Failed to bind \"click\" to selector \"" + selector + "\", no elements found.");
                    for (var i = 0; i < items.length; i++) {
                        items[i].addEventListener("click", listener, useCapture);
                    }
                };
                EventMapper.prototype.change = function (selector, listener, useCapture) {
                    var items = this.scope.querySelectorAll(selector);
                    if (items.length === 0) throw new Error("Failed to bind \"change\" to selector \"" + selector + "\", no elements found.");
                    for (var i = 0; i < items.length; i++) {
                        items[i].addEventListener("change", listener, useCapture);
                    }
                };
                EventMapper.prototype.keyUp = function (selector, listener, useCapture) {
                    var items = this.scope.querySelectorAll(selector);
                    if (items.length === 0) throw new Error("Failed to bind \"keyup\" to selector \"" + selector + "\", no elements found.");
                    for (var i = 0; i < items.length; i++) {
                        items[i].addEventListener("keyup", listener, useCapture);
                    }
                };
                EventMapper.prototype.keyDown = function (selector, listener, useCapture) {
                    var items = this.scope.querySelectorAll(selector);
                    if (items.length === 0) throw new Error("Failed to bind \"keydown\" to selector \"" + selector + "\", no elements found.");
                    for (var i = 0; i < items.length; i++) {
                        items[i].addEventListener("keydown", listener, useCapture);
                    }
                };
                return EventMapper;
            })();
            Dom.EventMapper = EventMapper;
            var FormReader = (function () {
                function FormReader(elemOrName) {
                    this.stack = [];
                    if (typeof elemOrName === "string") {
                        this.container = document.querySelector('#' + elemOrName + ",[data-name=\"" + elemOrName + "\"]");
                        if (!this.container) {
                            throw new Error("Failed to locate '" + elemOrName + "'.");
                        }
                    } else {
                        this.container = elemOrName;
                    }
                }
                FormReader.prototype.read = function () {
                    var motherObject = {};
                    for (var i = 0; i < this.container.childElementCount; i++) {
                        var element = this.container.children[i];
                        var name = this.getName(element);
                        if (!name) {
                            var data = this.pullElement(element);
                            if (data) {
                                this.appendObject(motherObject, data);
                            }
                            continue;
                        }
                        var childValue;
                        if (this.isCollection(element)) {
                            childValue = this.pullCollection(element);
                        } else {
                            childValue = this.pullElement(element);
                            childValue = this.adjustCheckboxes(element, motherObject, childValue);
                        }
                        this.assignByName(name, motherObject, childValue);
                    }
                    return motherObject;
                };
                FormReader.prototype.pullCollection = function (container) {
                    var arr = [];
                    var currentArrayItem = {};
                    var addedItems = [];
                    var currentIndexer = null;
                    for (var i = 0; i < container.childElementCount; i++) {
                        var elem = container.children[i];
                        var name = this.getName(elem);
                        if (!name) {
                            var value = this.pullElement(elem);
                            if (!this.isObjectEmpty(value)) {
                                if (!this.isObjectEmpty(currentArrayItem)) {
                                    arr.push(currentArrayItem);
                                }
                                arr.push(value);
                                currentArrayItem = {};
                                addedItems = [];
                            }
                            continue;
                        }
                        var isOptionOrCheckbox = elem.getAttribute('type') === 'checkbox' || elem.getAttribute('type') === 'radio';
                        if (name !== '[]' && addedItems.indexOf(name) >= 0 && !isOptionOrCheckbox) {
                            arr.push(currentArrayItem);
                            currentArrayItem = {};
                            addedItems = [];
                        }
                        addedItems.push(name);
                        var value;
                        if (this.isCollection(elem)) {
                            value = this.pullCollection(elem);
                        } else {
                            value = this.pullElement(elem);
                            if (value === null) {
                                continue;
                            }
                        }
                        if (name === '[]') {
                            arr.push(value);
                        } else {
                            this.assignByName(name, currentArrayItem, value);
                        }
                    }
                    if (!this.isObjectEmpty(currentArrayItem)) {
                        arr.push(currentArrayItem);
                    }
                    return arr;
                };
                FormReader.prototype.pullElement = function (container) {
                    if (container.tagName == 'SELECT') {
                        var select = container;
                        if (select.selectedIndex == -1) {
                            return null;
                        }
                        var value1 = select.options[select.selectedIndex];
                        return this.processValue(value1.value);
                    }
                    if (container.childElementCount === 0) {
                        if (container.tagName == 'INPUT') {
                            var input = container;
                            var typeStr = container.getAttribute('type');
                            if (typeStr === 'radio' || typeStr === 'checkbox') {
                                if (input.checked) {
                                    return this.processValue(input.value);
                                }
                                return null;
                            }
                            return this.processValue(input.value);
                        } else if (container.tagName == 'TEXTAREA') {
                            var value3 = container.value;
                            return this.processValue(value3);
                        }
                    }
                    var data = {};
                    for (var i = 0; i < container.childElementCount; i++) {
                        var element = container.children[i];
                        var name = this.getName(element);
                        if (!name) {
                            var value = this.pullElement(element);
                            if (!this.isObjectEmpty(value)) {
                                this.appendObject(data, value);
                            }
                            continue;
                        }
                        var value;
                        if (this.isCollection(element)) {
                            value = this.pullCollection(element);
                        } else {
                            value = this.pullElement(element);
                            value = this.adjustCheckboxes(element, data, value);
                            if (value === null) {
                                continue;
                            }
                        }
                        this.assignByName(name, data, value);
                    }
                    return this.isObjectEmpty(data) ? null : data;
                };
                FormReader.prototype.adjustCheckboxes = function (element, dto, value) {
                    if (value !== null && element.tagName === "INPUT" && element.getAttribute("type") === "checkbox") {
                        var name = this.getName(element);
                        var currentValue = dto[name];
                        if (typeof currentValue !== "undefined") {
                            if (currentValue instanceof Array) {
                                currentValue["push"](value);
                                value = currentValue;
                            } else {
                                value = [currentValue, value];
                            }
                        } else {
                            value = [value];
                        }
                    }
                    return value;
                };
                FormReader.prototype.processValue = function (value) {
                    if (!isNaN(value)) {
                        return parseInt(value, 10);
                    } else if (value == 'true') {
                        return true;
                    } else if (value == 'false') {
                        return false;
                    }
                    return value;
                };
                FormReader.prototype.assignByName = function (name, parentObject, value) {
                    var parts = name.split('.');
                    var obj = parentObject;
                    var parent = parentObject;
                    var lastKey = '';
                    parts.forEach(function (key) {
                        lastKey = key;
                        if (!obj.hasOwnProperty(key)) {
                            obj[key] = {};
                        }
                        parent = obj;
                        obj = obj[key];
                    });
                    parent[lastKey] = value;
                };
                FormReader.prototype.appendObject = function (target, extras) {
                    for (var key in extras) {
                        if (!target.hasOwnProperty(key)) {
                            target[key] = extras[key];
                        }
                    }
                };
                FormReader.prototype.isObjectEmpty = function (data) {
                    for (var name in data) {
                        if (data.hasOwnProperty(name)) {
                            return false;
                        }
                    }
                    return true;
                };
                FormReader.prototype.getName = function (el) {
                    return el.getAttribute('name') || el.getAttribute('data-name') || el.getAttribute('data-collection');
                };
                FormReader.prototype.isCollection = function (el) {
                    return el.hasAttribute('data-collection');
                };
                return FormReader;
            })();
            Dom.FormReader = FormReader;
            var Selector = (function () {
                function Selector(scope) {
                    if (typeof scope === "undefined") {
                        this.scope = document;
                    } else {
                        this.scope = scope;
                    }
                    if (!this.scope) throw new Error("Failed to identify scope");
                }
                Selector.prototype.one = function (idOrselector) {
                    if (idOrselector.substr(0, 1) === "#") {
                        var el2 = this.scope.querySelector(idOrselector);
                        if (!el2) {
                            throw new Error("Failed to find element '" + idOrselector + "'.");
                        }
                        return el2;
                    }
                    if (idOrselector.match(/[\s\.\,\[]+/g) === null) {
                        var result = this.scope.querySelector("[data-name='" + idOrselector + "'],[data-collection='" + idOrselector + "'],[name=\"" + idOrselector + "\"],#" + idOrselector);
                        if (result) return result;
                    }
                    var item = this.scope.querySelector(idOrselector);
                    if (!item) throw Error("Failed to find \"" + idOrselector + "\".");
                    return item;
                };
                Selector.prototype.all = function (selector) {
                    var result = [];
                    var items = selector.match("[\s\.,\[]+").length === 0 ? this.scope.querySelectorAll("[data-name=\"" + selector + "\"],[data-collection='" + selector + "'],[name=\"" + selector + "\"],#" + selector) : this.scope.querySelectorAll(selector);
                    for (var i = 0; i < items.length; i++) {
                        result.push(items[i]);
                    }
                    return result;
                };
                return Selector;
            })();
            Dom.Selector = Selector;
        })(Dom = Yo.Dom || (Yo.Dom = {}));
    })(Yo = Griffin.Yo || (Griffin.Yo = {}));
})(Griffin || (Griffin = {}));
var Griffin;
(function (Griffin) {
    var Yo;
    (function (Yo) {
        var Net;
        (function (Net) {
            var Http = (function () {
                function Http() {}
                Http.get = function (url, callback, contentType) {
                    var _this = this;
                    if (contentType === void 0) {
                        contentType = "application/json";
                    }
                    var request = new XMLHttpRequest();
                    request.open("GET", url, true);
                    if (typeof this.cache[url] !== "undefined") {
                        var cache = this.cache[url];
                        request.setRequestHeader("If-Modified-Since", cache.modifiedAt);
                    }
                    request.setRequestHeader("Content-Type", contentType);
                    request.onload = function () {
                        if (request.status >= 200 && request.status < 400) {
                            if (request.status === 304) {
                                request["responseText"] = _this.cache[url].content;
                            } else {
                                var header = request.getResponseHeader("Last-Modified");
                                if (header) {
                                    _this.cache[url] = {
                                        content: request.responseText,
                                        modifiedAt: header
                                    };
                                }
                            }
                            if (contentType === "application/json") {
                                request["responseBody"] = JSON.parse(request.responseText);
                                var tempFix = request;
                                tempFix["responseJson"] = JSON.parse(request.responseText);
                            }
                            callback(request, true);
                        } else {
                            callback(request, false);
                        }
                    };
                    request.onerror = function () {
                        callback(request, false);
                    };
                    request.send();
                };
                Http.post = function (url, data, callback, options) {
                    if (!data) {
                        throw new Error("You must specify a body when using POST.");
                    }
                    Http.invokeRequest('POST', url, data, callback, options);
                };
                Http.put = function (url, data, callback, options) {
                    if (!data) {
                        throw new Error("You must specify a body when using PUT.");
                    }
                    Http.invokeRequest('PUT', url, data, callback, options);
                };
                Http["delete"] = function (url, callback, options) {
                    Http.invokeRequest('DELETE', url, null, callback, options);
                };
                Http.invokeRequest = function (verb, url, data, callback, options) {
                    if (!verb) {
                        throw new Error("You must specify a HTTP verb");
                    }
                    if (options && options.userName && !options.password) {
                        throw new Error("You must provide password when username has been specified.");
                    }
                    var request = new XMLHttpRequest();
                    if (options && options.userName) {
                        request.open(verb, url, true, options.userName, options.password);
                    } else {
                        request.open(verb, url, true);
                    }
                    var requestContentType = "application/json";
                    if (options && options.contentType) {
                        requestContentType = options.contentType;
                    }
                    if (options && options.headers) {
                        for (var key in options.headers) {
                            request.setRequestHeader(key, options.headers[key]);
                        }
                    }
                    request.onload = function () {
                        if (request.status >= 200 && request.status < 400) {
                            var contentType = request.getResponseHeader("content-type").toLocaleLowerCase();
                            if (contentType === "application/json") {
                                request.responseBody = JSON.parse(request.responseText);
                                var temp = request;
                                temp["responseJson"] = JSON.parse(request.responseText);
                            }
                            callback(request, true);
                        } else {
                            callback(request, false);
                        }
                    };
                    request.onerror = function () {
                        callback(request, false);
                    };
                    if (typeof data !== "undefined" && data !== null) {
                        request.setRequestHeader("Content-Type", requestContentType);
                        if (requestContentType === "application/json" && typeof data !== "string") {
                            data = JSON.stringify(data);
                        }
                        request.send(data);
                    } else {
                        request.send();
                    }
                };
                return Http;
            })();
            Http.cache = {};
            Http.useCaching = true;
            Net.Http = Http;
        })(Net = Yo.Net || (Yo.Net = {}));
    })(Yo = Griffin.Yo || (Griffin.Yo = {}));
})(Griffin || (Griffin = {}));
var Griffin;
(function (Griffin) {
    var Yo;
    (function (Yo) {
        var Routing;
        (function (Routing) {
            ;
            var Route = (function () {
                function Route(route, handler, target) {
                    this.route = route;
                    this.handler = handler;
                    this.target = target;
                    this.parts = [];
                    this.parts = route.replace(/^\//, "").replace(/\/$/, "").split("/");
                }
                Route.prototype.isMatch = function (ctx) {
                    var urlParts = ctx.url.split("/", 10);
                    for (var i = 0; i < this.parts.length; i++) {
                        var myPart = this.parts[i];
                        var pathPart = urlParts[i];
                        if (pathPart !== undefined) {
                            if (myPart.charAt(0) === ":") {
                                continue;
                            } else if (myPart !== pathPart) {
                                return false;
                            }
                        } else if (myPart.charAt(0) !== ":") {
                            return false;
                        } else {
                            return false;
                        }
                    }
                    return true;
                };
                Route.prototype.invoke = function (ctx) {
                    var urlParts = ctx.url.split("/", 10);
                    var routeData = {};
                    for (var i = 0; i < this.parts.length; i++) {
                        var myPart = this.parts[i];
                        var pathPart = urlParts[i];
                        if (pathPart !== "undefined") {
                            if (myPart.charAt(0) === ":") {
                                routeData[myPart.substr(1)] = pathPart;
                            }
                        } else if (myPart.charAt(0) === ":") {
                            routeData[myPart.substr(1)] = null;
                        }
                    }
                    var executionCtx = {
                        routeData: routeData,
                        route: this
                    };
                    if (typeof this.target !== "undefined") {
                        executionCtx.target = this.target;
                    }
                    this.handler.invoke(executionCtx);
                };
                return Route;
            })();
            Routing.Route = Route;
            var Router = (function () {
                function Router() {
                    this.routes = [];
                }
                Router.prototype.add = function (route, handler, targetElement) {
                    this.routes.push(new Route(route, handler, targetElement));
                };
                Router.prototype.addRoute = function (route) {
                    if (typeof route === "undefined") throw new Error("Route must be specified.");
                    this.routes.push(route);
                };
                Router.prototype.execute = function (url, targetElement) {
                    if (url.length) {
                        url = url.replace(/\/+/g, "/").replace(/^\/|\/($|\?)/, "").replace(/#.*$/, "");
                    }
                    var ctx = {
                        url: url
                    };
                    if (typeof targetElement !== "undefined") {
                        if (typeof targetElement === "string") {
                            ctx.targetElement = document.getElementById(targetElement);
                            if (!ctx.targetElement) {
                                throw new Error("Failed to identify '" + targetElement + "'");
                            }
                        }
                        ctx.targetElement = targetElement;
                    }
                    for (var i = 0; i < this.routes.length; i++) {
                        var route = this.routes[i];
                        if (route.isMatch(ctx)) {
                            route.invoke(ctx);
                            return true;
                        }
                    }
                    if (console && console.log) {
                        console.log("Route not found for \"" + url + "\"");
                    }
                    return false;
                };
                return Router;
            })();
            Routing.Router = Router;
        })(Routing = Yo.Routing || (Yo.Routing = {}));
    })(Yo = Griffin.Yo || (Griffin.Yo = {}));
})(Griffin || (Griffin = {}));
var Griffin;
(function (Griffin) {
    var Yo;
    (function (Yo) {
        var Routing;
        (function (Routing) {
            var ViewTargets;
            (function (ViewTargets) {
                var BootstrapModalViewTarget = (function () {
                    function BootstrapModalViewTarget() {
                        this.name = "BootstrapModal";
                    }
                    BootstrapModalViewTarget.prototype.assignOptions = function (options) {};
                    BootstrapModalViewTarget.prototype.attachViewModel = function (script) {
                        this.currentNode = new BootstrapModalViewTargetRequest(this.name);
                        this.currentNode.attachViewModel(script);
                    };
                    BootstrapModalViewTarget.prototype.setTitle = function (title) {
                        this.currentNode.setTitle(title);
                    };
                    BootstrapModalViewTarget.prototype.render = function (element) {
                        this.currentNode.render(element);
                        this.currentNode = null;
                    };
                    return BootstrapModalViewTarget;
                })();
                ViewTargets.BootstrapModalViewTarget = BootstrapModalViewTarget;
                var BootstrapModalViewTargetRequest = (function () {
                    function BootstrapModalViewTargetRequest(name) {
                        this.name = name;
                        this.node = document.createElement('div');
                        this.node.setAttribute('id', this.name);
                        this.node.setAttribute('class', 'modal fade view-target');
                        this.node.setAttribute('role', 'dialog');
                        document.body.appendChild(this.node);
                        var contents = '\r\n' + '  <div class="modal-dialog">\r\n' + '\r\n' + '    <div class="modal-content">\r\n' + '      <div class="modal-header">\r\n' + '        <button type="button" class="close" data-dismiss="modal">&times;</button>\r\n' + '        <h4 class="modal-title"></h4>\r\n' + '      </div>\r\n' + '      <div class="modal-body">\r\n' + '        \r\n' + '      </div>\r\n' + '      <div class="modal-footer">\r\n' + '        <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>\r\n' + '      </div>\r\n' + '    </div>\r\n' + '\r\n' + '  </div>\r\n' + '';
                        this.node.innerHTML = contents;
                    }
                    BootstrapModalViewTargetRequest.prototype.prepare = function (options) {
                        var body = this.node.querySelector('.modal-body');
                        while (body.firstChild) body.removeChild(body.firstChild);
                        var footer = this.node.querySelector('.modal-footer');
                        while (footer.firstChild) footer.removeChild(footer.firstChild);
                        if (options && options.buttons) {
                            options.buttons.forEach(function (item) {
                                var button = document.createElement('button');
                                if (item.className) {
                                    button.setAttribute('class', 'btn ' + item.className);
                                } else {
                                    button.setAttribute('class', 'btn btn-default');
                                }
                                button.setAttribute('data-dismiss', 'modal');
                                button.innerText = item.title;
                                button.addEventListener('click', function (e) {
                                    item.callback(body.firstElementChild);
                                });
                                footer.appendChild(button);
                            });
                        }
                    };
                    BootstrapModalViewTargetRequest.prototype.attachViewModel = function (script) {
                        this.node.querySelector('.modal-body').appendChild(script);
                    };
                    BootstrapModalViewTargetRequest.prototype.setTitle = function (title) {
                        this.node.querySelector('.modal-title').innerText = title;
                    };
                    BootstrapModalViewTargetRequest.prototype.render = function (element) {
                        var _this = this;
                        this.node.querySelector('.modal-body').appendChild(element);
                        var footer = this.node.querySelector('.modal-footer');
                        this.modal = $(this.node).modal();
                        var m = this.modal;
                        $(this.modal).on('hidden.bs.modal', function () {
                            _this.modal.modal('hide').data('bs.modal', null);
                            _this.node.parentElement.removeChild(_this.node);
                        });
                        var buttons = element.querySelectorAll('button,input[type="submit"],input[type="button"]');
                        if (buttons.length > 0) {
                            while (footer.firstChild) {
                                footer.removeChild(footer.firstChild);
                            }
                            for (var i = 0; i < buttons.length; i++) {
                                var button = buttons[i];
                                button.className += ' btn';
                                button.addEventListener('click', (function (button, e) {
                                    this.modal('hide');
                                    if (button.tagName === "input" && button.type !== "submit" || button.hasAttribute("data-dismiss")) {
                                        window.history.go(-1);
                                    }
                                }).bind(this, button));
                                footer.appendChild(buttons[i]);
                            }
                            if (buttons.length === 1) {
                                buttons[0].className += ' btn-primary';
                            } else {
                                buttons[0].className += ' btn-primary';
                                buttons[buttons.length - 1].className += ' btn-cancel';
                            }
                        }
                        this.modal.modal('show');
                    };
                    return BootstrapModalViewTargetRequest;
                })();
                var ElementViewTarget = (function () {
                    function ElementViewTarget(elementOrId) {
                        this.name = "";
                        if (typeof elementOrId === "string") {
                            this.container = document.getElementById(elementOrId.substr(1));
                            if (!this.container) {
                                throw "Could not locate \"" + elementOrId + "\"";
                            }
                        } else {
                            this.container = elementOrId;
                        }
                        this.name = this.container.id;
                    }
                    ElementViewTarget.prototype.assignOptions = function () {};
                    ElementViewTarget.prototype.attachViewModel = function (script) {
                        this.container.appendChild(script);
                    };
                    ElementViewTarget.prototype.setTitle = function (title) {};
                    ElementViewTarget.prototype.render = function (element) {
                        while (this.container.firstElementChild && this.container.firstElementChild.nextElementSibling != null) this.container.removeChild(this.container.firstElementChild);
                        this.container.innerHTML = "";
                        this.container.appendChild(element);
                    };
                    return ElementViewTarget;
                })();
                ViewTargets.ElementViewTarget = ElementViewTarget;
            })(ViewTargets = Routing.ViewTargets || (Routing.ViewTargets = {}));
        })(Routing = Yo.Routing || (Yo.Routing = {}));
    })(Yo = Griffin.Yo || (Griffin.Yo = {}));
})(Griffin || (Griffin = {}));
var Griffin;
(function (Griffin) {
    var Yo;
    (function (Yo) {
        var Spa;
        (function (Spa) {
            var ViewModels;
            (function (ViewModels) {
                var ClassFactory = (function () {
                    function ClassFactory() {}
                    ClassFactory.getConstructor = function (appName, viewModelModuleAndName) {
                        var nameParts = viewModelModuleAndName.split(".");
                        var fn = window[appName] || this[appName];
                        if (typeof fn === "undefined") {
                            throw new Error("Failed to load application namespace \"" + appName + "\". Have a view model been loaded successfully?");
                        }
                        for (var i = 0, len = nameParts.length; i < len; i++) {
                            if (fn.hasOwnProperty(nameParts[i])) {
                                fn = fn[nameParts[i]];
                                continue;
                            }
                            var name_1 = nameParts[i].toLowerCase();
                            var foundName = void 0;
                            for (var propertyName in fn) {
                                if (!fn.hasOwnProperty(propertyName)) {
                                    continue;
                                }
                                if (propertyName.toLowerCase() === name_1) {
                                    foundName = propertyName;
                                }
                            }
                            if (typeof foundName === "undefined") throw new Error("Could not find \"" + nameParts[i] + "\" from viewModel name, complete name: \"" + appName + "." + viewModelModuleAndName + "\".");
                            fn = fn[foundName];
                        }
                        if (typeof fn !== "function") {
                            throw new Error("Could not find view model " + viewModelModuleAndName);
                        }
                        return fn;
                    };
                    return ClassFactory;
                })();
                ViewModels.ClassFactory = ClassFactory;
            })(ViewModels = Spa.ViewModels || (Spa.ViewModels = {}));
        })(Spa = Yo.Spa || (Yo.Spa = {}));
    })(Yo = Griffin.Yo || (Griffin.Yo = {}));
})(Griffin || (Griffin = {}));
var Griffin;
(function (Griffin) {
    var Yo;
    (function (Yo) {
        var Spa;
        (function (Spa) {
            var Routing = Yo.Routing;
            var ViewTargets = Yo.Routing.ViewTargets;
            var ViewModels = Yo.Spa.ViewModels;
            var Dom = Yo.Dom;
            var Net = Yo.Net;
            var Config = (function () {
                function Config() {}
                return Config;
            })();
            Config.applicationScope = {};
            Spa.Config = Config;
            var RouteRunner = (function () {
                function RouteRunner(section, applicationName) {
                    this.section = section;
                    if (!applicationName) {
                        throw new Error("applicationName must be specified");
                    }
                    if (!section) {
                        throw new Error("section must be specified");
                    }
                    this.applicationName = applicationName;
                }
                RouteRunner.replaceAll = function (str, replaceWhat, replaceTo) {
                    replaceWhat = replaceWhat.replace(/[-\/\\^$*+?.()|[\]{}]/g, "\\$&");
                    var re = new RegExp(replaceWhat, "g");
                    return str.replace(re, replaceTo);
                };
                RouteRunner.prototype.applyRouteDataToLinks = function (viewElement, routeData) {
                    var links = viewElement.querySelectorAll("a");
                    for (var i = 0; i < links.length; i++) {
                        var link = links[i];
                        var pos = link.href.indexOf("#");
                        if (pos === -1 || link.href.substr(pos + 1, 1) !== "/") {
                            continue;
                        }
                        for (var dataName in routeData) {
                            if (routeData.hasOwnProperty(dataName)) {
                                var after = RouteRunner.replaceAll(link.href, ":" + dataName, routeData[dataName]);
                                var before = link.href;
                                link.href = RouteRunner.replaceAll(link.href, ":" + dataName, routeData[dataName]);
                            }
                        }
                    }
                };
                RouteRunner.prototype.moveNavigationToMain = function (viewElement, context) {
                    var navigations = viewElement.querySelectorAll("[data-navigation]");
                    for (var i = 0; i < navigations.length; i++) {
                        var nav = navigations[i];
                        var target = nav.getAttribute("data-navigation");
                        var targetElem = document.getElementById(target);
                        if (!targetElem) throw new Error("Failed to find target element '" + target + "' for navigation '" + nav.innerHTML + "'");
                        var ifStatement = nav.getAttribute("data-if");
                        var ifResult = !ifStatement || !this.evalInContext(ifStatement, context);
                        if (!ifResult) {
                            nav.parentNode.removeChild(nav);
                            continue;
                        }
                        this.removeConditions(nav, context);
                        Dom.ElemUtils.removeChildren(targetElem);
                        Dom.ElemUtils.moveChildren(nav, targetElem);
                    }
                };
                RouteRunner.prototype.removeConditions = function (elem, context) {
                    for (var i = 0; i < elem.childElementCount; i++) {
                        var child = elem.children[i];
                        if (!child.hasAttribute("data-if")) {
                            continue;
                        }
                        var ifStatement = child.getAttribute("data-if");
                        var ifResult = this.evalInContext(ifStatement, context);
                        if (!ifResult) {
                            child.parentNode.removeChild(child);
                            continue;
                        }
                    }
                };
                RouteRunner.prototype.evalInContext = function (code, context) {
                    var func = function func(js) {
                        return eval("with (this) { " + js + "}");
                    };
                    return func.call(context, code);
                };
                RouteRunner.prototype.isIE = function () {
                    var myNav = navigator.userAgent.toLowerCase();
                    return myNav.indexOf('msie') != -1 ? parseInt(myNav.split('msie')[1]) : false;
                };
                RouteRunner.prototype.invoke = function (ctx) {
                    var _this = this;
                    var self = this;
                    this.ensureResources(function () {
                        var viewElem = document.createElement("div");
                        viewElem.className = "ViewContainer";
                        viewElem.innerHTML = _this.html;
                        var scriptElem = document.createElement("script");
                        scriptElem.setAttribute("type", "text/javascript");
                        scriptElem.setAttribute("data-tag", "viewModel");
                        ctx.target.attachViewModel(scriptElem);
                        if (_this.isIE() <= 9) {
                            scriptElem.text = _this.viewModelScript;
                        } else {
                            scriptElem.innerHTML = _this.viewModelScript;
                        }
                        var className = _this.section.replace(/\//g, ".") + "ViewModel";
                        _this.viewModel = Config.viewModelFactory.create(_this.applicationName, className);
                        var vm = _this.viewModel;
                        if (vm.hasOwnProperty("getTargetOptions")) {
                            var options = vm["hasTargetOptions"]();
                            ctx.target.assignOptions(options);
                        } else {
                            ctx.target.assignOptions({});
                        }
                        _this.applyRouteDataToLinks(viewElem, ctx.routeData);
                        _this.moveNavigationToMain(viewElem, { model: _this.viewModel, ctx: ctx });
                        var activationContext = {
                            routeData: ctx.routeData,
                            viewContainer: viewElem,
                            render: function render(data, directives) {
                                var r = new Griffin.Yo.Views.ViewRenderer(viewElem);
                                r.render(data, directives);
                            },
                            readForm: function readForm(selector) {
                                var reader = new Dom.FormReader(selector);
                                return reader.read();
                            },
                            renderPartial: function renderPartial(selector, data, directives) {
                                var selector1 = new Dom.Selector(viewElem);
                                var target = selector1.one(selector);
                                var r = new Griffin.Yo.Views.ViewRenderer(target);
                                r.render(data, directives);
                            },
                            resolve: function resolve() {
                                document.title = vm.getTitle();
                                ctx.target.setTitle(vm.getTitle());
                                ctx.target.render(viewElem);
                                var scripts = viewElem.getElementsByTagName("script");
                                var loader = new ScriptLoader();
                                for (var i = 0; i < scripts.length; i++) {
                                    loader.loadTags(scripts[i]);
                                }
                                var allIfs = viewElem.querySelectorAll("[data-if]");
                                for (var j = 0; j < allIfs.length; j++) {
                                    var elem = allIfs[j];
                                    var condition = elem.getAttribute("data-if");
                                    if (condition.substr(0, 3) != 'vm.' && condition.substr(0, 6) != 'model.' && condition.substr(0, 4) != 'ctx.') {
                                        continue;
                                    }
                                    var result = self.evalInContext(condition, { model: vm, ctx: ctx, vm: vm });
                                    if (!result) {
                                        elem.parentNode.removeChild(elem);
                                    }
                                }
                                var allUnless = viewElem.querySelectorAll("[data-unless]");
                                for (var j = 0; j < allUnless.length; j++) {
                                    var elem = allUnless[j];
                                    var condition = elem.getAttribute("data-unless");
                                    if (condition.substr(0, 3) != 'vm.' && condition.substr(0, 6) != 'model.' && condition.substr(0, 4) != 'ctx.') {
                                        continue;
                                    }
                                    var result = self.evalInContext(condition, { ctx: ctx, vm: vm });
                                    if (!result) {
                                        elem.parentNode.removeChild(elem);
                                    }
                                }
                            },
                            reject: function reject() {},
                            handle: new Dom.EventMapper(viewElem),
                            select: new Dom.Selector(viewElem),
                            applicationScope: Config.applicationScope
                        };
                        _this.viewModel.activate(activationContext);
                    });
                };
                RouteRunner.prototype.ensureResources = function (callback) {
                    var _this = this;
                    var bothPreloaded = true;
                    var self = this;
                    if (typeof this.html === "undefined") {
                        var path = Config.resourceLocator.getHtml(this.section);
                        Net.Http.get(path, function (xhr, success) {
                            if (success) {
                                self.html = xhr.responseText;
                                _this.doStep(callback);
                            } else {
                                throw new Error(xhr.responseText);
                            }
                            bothPreloaded = false;
                        }, "text/html");
                    }
                    if (typeof this.viewModel === "undefined") {
                        var path = Config.resourceLocator.getScript(this.section);
                        Net.Http.get(path, function (xhr, success) {
                            if (success) {
                                self.viewModelScript = xhr.responseText;
                                _this.doStep(callback);
                            } else {
                                throw new Error(xhr.responseText);
                            }
                        }, "text/javascript");
                        bothPreloaded = false;
                    }
                    if (bothPreloaded) this.doStep(callback);
                };
                RouteRunner.prototype.doStep = function (callback) {
                    if (typeof this.html !== "undefined" && typeof this.viewModelScript !== "undefined") {
                        callback();
                    }
                };
                return RouteRunner;
            })();
            Spa.RouteRunner = RouteRunner;
            var ScriptLoader = (function () {
                function ScriptLoader(container) {
                    if (container === void 0) {
                        container = document.head;
                    }
                    this.pendingScripts = [];
                    this.embeddedScripts = [];
                    this.container = container;
                    if (!ScriptLoader.dummyScriptNode) ScriptLoader.dummyScriptNode = document.createElement("script");
                }
                ScriptLoader.prototype.stateChange = function () {
                    if (this.pendingScripts.length === 0) {
                        if (console && console.log) console.log("Got ready state for a non existent script: ", this);
                        return;
                    }
                    var firstScript = this.pendingScripts[0];
                    while (firstScript && firstScript.readyState === "loaded") {
                        firstScript.onreadystatechange = null;
                        this.container.appendChild(firstScript);
                        this.pendingScripts.shift();
                        firstScript = this.pendingScripts[0];
                    }
                    if (this.pendingScripts.length === 0) {
                        this.runEmbeddedScripts();
                    }
                };
                ScriptLoader.prototype.loadSources = function (scripts) {
                    if (scripts instanceof Array) {
                        for (var i = 0; i < scripts.length; i++) {
                            this.loadSource(scripts[i]);
                        }
                    } else {
                        this.loadSource(scripts);
                    }
                };
                ScriptLoader.prototype.loadTags = function (scripts) {
                    if (scripts instanceof Array) {
                        for (var i = 0; i < scripts.length; i++) {
                            this.loadElement(scripts[i]);
                        }
                    } else {
                        this.loadElement(scripts);
                    }
                    if (this.pendingScripts.length === 0) {
                        this.runEmbeddedScripts();
                    }
                };
                ScriptLoader.prototype.loadSource = function (source) {
                    var _this = this;
                    if ("async" in ScriptLoader.dummyScriptNode) {
                        var script_1 = document.createElement("script");
                        script_1.async = false;
                        this.pendingScripts.push(script_1);
                        script_1.addEventListener("load", function (e) {
                            return _this.onScriptLoaded(script_1);
                        });
                        script_1.src = source;
                        this.container.appendChild(script_1);
                    } else if (ScriptLoader.dummyScriptNode.readyState) {
                        var script = document.createElement("script");
                        this.pendingScripts.push(script);
                        script.onreadystatechange = this.stateChange;
                        script.src = source;
                    } else {
                        var script_2 = document.createElement("script");
                        script_2.defer = true;
                        this.pendingScripts.push(script_2);
                        script_2.addEventListener("load", function (e) {
                            return _this.onScriptLoaded(script_2);
                        });
                        script_2.src = source;
                        this.container.appendChild(script_2);
                    }
                };
                ScriptLoader.prototype.loadElement = function (tag) {
                    if (tag.src) {
                        this.loadSource(tag.src);
                        return;
                    }
                    var script = document.createElement("script");
                    script.text = tag.text;
                    this.embeddedScripts.push(script);
                };
                ScriptLoader.prototype.onScriptLoaded = function (script) {
                    this.pendingScripts.pop();
                    if (this.pendingScripts.length === 0) {
                        this.runEmbeddedScripts();
                    }
                };
                ScriptLoader.prototype.runEmbeddedScripts = function () {
                    for (var i = 0; i < this.embeddedScripts.length; i++) {
                        this.container.appendChild(this.embeddedScripts[i]);
                    }
                    while (this.embeddedScripts.length > 0) {
                        this.embeddedScripts.pop();
                    }
                };
                return ScriptLoader;
            })();
            Spa.ScriptLoader = ScriptLoader;
            var SpaEngine = (function () {
                function SpaEngine(applicationName) {
                    this.applicationName = applicationName;
                    this.router = new Routing.Router();
                    this.basePath = "";
                    this.applicationScope = {};
                    this.viewTargets = [];
                    this.basePath = window.location.pathname;
                    this.defaultViewTarget = new ViewTargets.ElementViewTarget("#YoView");
                }
                SpaEngine.prototype.addTarget = function (name, target) {
                    if (typeof target === "string") {
                        var id = target;
                        if (id.substr(0, 1) != '#') throw new Error("Element id must start with #.");
                        target = new Griffin.Yo.Routing.ViewTargets.ElementViewTarget(id);
                    }
                    var target2 = target;
                    target2.name = name;
                    console.log('adding view target');
                    this.viewTargets.push(target2);
                };
                SpaEngine.prototype.navigate = function (url, targetElement) {
                    this.router.execute.apply(this.applicationScope, [url, targetElement]);
                };
                SpaEngine.prototype.mapRoute = function (route, section, target) {
                    var runner = new RouteRunner(section, this.applicationName);
                    var targetObj;
                    if (!target) {
                        targetObj = this.defaultViewTarget;
                    } else {
                        for (var i = 0; i < this.viewTargets.length; i++) {
                            if (this.viewTargets[i].name === target) {
                                targetObj = this.viewTargets[i];
                                break;
                            }
                        }
                        if (!targetObj) {
                            throw "Could not find view target \"" + target + "\".";
                        }
                    }
                    this.router.add(route, runner, targetObj);
                };
                SpaEngine.prototype.run = function () {
                    var _this = this;
                    var url = window.location.hash.substring(1);
                    if (url.substr(0, 1) === "!") {
                        url = url.substr(1);
                    }
                    window.addEventListener("hashchange", function () {
                        var hash = window.location.hash;
                        if (!hash) {
                            hash = '#/';
                        }
                        if (hash.substr(1, 1) !== "/") return;
                        var changedUrl = hash.substr(2);
                        if (changedUrl.substr(0, 1) === "!") {
                            changedUrl = changedUrl.substr(1);
                        }
                        _this.router.execute(changedUrl);
                    });
                    this.router.execute(url);
                };
                SpaEngine.prototype.mapFunctionToRouteData = function (f, routeData) {
                    var re = /^\s*function\s+(?:\w*\s*)?\((.*?)\)/;
                    var args = f.toString().match(re);
                    var test = f[1].trim().split(/\s*,\s*/);
                };
                return SpaEngine;
            })();
            Spa.SpaEngine = SpaEngine;
            Config.resourceLocator = {
                getHtml: function getHtml(section) {
                    var path = window.location.pathname;
                    if (window.location.pathname.indexOf(".") > -1) {
                        var pos = window.location.pathname.lastIndexOf("/");
                        path = window.location.pathname.substr(0, pos);
                    }
                    if (path.substring(-1, 1) === "/") {
                        path = path.substring(0, path.length - 1);
                    }
                    return path + ("/Views/" + section + ".html");
                },
                getScript: function getScript(section) {
                    var path = window.location.pathname;
                    if (window.location.pathname.indexOf(".") > -1) {
                        var pos = window.location.pathname.lastIndexOf("/");
                        path = window.location.pathname.substr(0, pos);
                    }
                    if (path.substring(-1, 1) === "/") {
                        path = path.substring(0, path.length - 1);
                    }
                    return path + ("/ViewModels/" + section + "ViewModel.js");
                }
            };
            Config.viewModelFactory = {
                create: function create(applicationName, fullViewModelName) {
                    var viewModelConstructor = ViewModels.ClassFactory.getConstructor(applicationName, fullViewModelName);
                    return new viewModelConstructor(Config.applicationScope);
                }
            };
        })(Spa = Yo.Spa || (Yo.Spa = {}));
    })(Yo = Griffin.Yo || (Griffin.Yo = {}));
})(Griffin || (Griffin = {}));
var Griffin;
(function (Griffin) {
    var Yo;
    (function (Yo) {
        var Views;
        (function (Views) {
            var ViewRenderer = (function () {
                function ViewRenderer(elemOrName) {
                    this.lineage = [];
                    this.dtoStack = [];
                    this.directives = [];
                    if (typeof elemOrName === "string") {
                        if (elemOrName.substr(0, 1) === "#") {
                            this.container = document.getElementById(elemOrName.substr(1));
                        } else {
                            this.container = document.querySelector("[data-name='" + elemOrName + "'],[data-collection='" + elemOrName + "'],#" + elemOrName + ",[name=\"" + elemOrName + "\"]");
                        }
                        if (!this.container) {
                            throw new Error("Failed to locate '" + elemOrName + "'.");
                        }
                    } else {
                        this.container = elemOrName;
                    }
                }
                ViewRenderer.prototype.register = function (directive) {
                    this.directives.push(directive);
                };
                ViewRenderer.registerGlobal = function (directive) {
                    ViewRenderer.globalValueDirectives.push(directive);
                };
                ViewRenderer.prototype.render = function (data, directives) {
                    if (data === void 0) {
                        data = {};
                    }
                    if (directives === void 0) {
                        directives = {};
                    }
                    this.dtoStack.push(data);
                    if (data instanceof Array) {
                        this.renderCollection(this.container, data, directives);
                    } else {
                        this.renderElement(this.container, data, directives);
                    }
                };
                ViewRenderer.prototype.renderElement = function (element, data, directives) {
                    if (directives === void 0) {
                        directives = {};
                    }
                    var elementName = this.getName(element);
                    if (elementName) {
                        this.log('renderElement', this.getName(element));
                    }
                    if (elementName && element.tagName === "SELECT") {
                        var sel = element;
                        for (var j = 0; j < sel.options.length; j++) {
                            var opt = sel.options[j];
                            if (opt.value === data || opt.label === data) {
                                this.log('setting option ' + opt.label + " to selected");
                                opt.selected = true;
                                break;
                            }
                        }
                    } else if (element.childElementCount === 0 && elementName) {
                        if (data && data.hasOwnProperty(elementName)) {
                            data = data[elementName];
                        }
                        data = this.runDirectives(element, data);
                        if (directives) {
                            if (this.applyEmbeddedDirectives(element, data, directives)) {
                                this.log('directives applied to element, done.');
                                return;
                            }
                        }
                        if (typeof data === "undefined") {
                            this.log('directives, but no data');
                            return;
                        }
                        if (element.tagName === "INPUT") {
                            var input = element;
                            if (input.type === "radio" || input.type === "checkbox") {
                                if (input.value === data) {
                                    this.log(input.type + ".checked => true");
                                    input.checked = true;
                                }
                            } else {
                                this.log(input.type + ".value => " + data);
                                input.value = data;
                            }
                        } else if (element.tagName === "TEXTAREA") {
                            this.log('textarea => ' + data);
                            element.innerText = data;
                        } else {
                            this.log('innerHTML => ' + data);
                            element.innerHTML = data;
                        }
                    }
                    for (var i = 0; i < element.childElementCount; i++) {
                        var item = element.children[i];
                        var name = this.getName(item);
                        if (!name) {
                            this.renderElement(item, data, directives);
                            continue;
                        }
                        var childData = data[name];
                        var childDirective = null;
                        if (directives && directives.hasOwnProperty(name)) {
                            childDirective = directives[name];
                        }
                        if (typeof childData === "undefined") {
                            this.log('got no data, checking directives.');
                            var gotValueProvider = false;
                            if (childDirective) {
                                gotValueProvider = childDirective.hasOwnProperty("value") || childDirective.hasOwnProperty("text") || childDirective.hasOwnProperty("html");
                            }
                            if (!gotValueProvider) {
                                this.log('got no data and no directives.');
                                continue;
                            }
                        }
                        if (this.isCollection(item)) {
                            this.lineage.push(name);
                            this.dtoStack.push(childData);
                            this.renderCollection(item, childData, childDirective);
                            this.dtoStack.pop();
                            this.lineage.pop();
                        } else {
                            if (item.getAttribute("data-unless") === name) {
                                if (childData && childData.length > 0) {
                                    item.style.display = "none";
                                }
                            } else if (childData instanceof Array) {
                                var wrapper = document.createElement('div');
                                wrapper.appendChild(item.cloneNode(true));
                                var elementHtml = wrapper.innerHTML;
                                var elemPath = name;
                                if (this.lineage.length > 0) {
                                    elemPath = this.lineage.join();
                                }
                                throw "'" + name + "' is not set as a collection, but the data is an array.\nElement: " + elementHtml + "\nData: " + JSON.stringify(childData, null, 4);
                            }
                            this.lineage.push(name);
                            this.dtoStack.push(childData);
                            this.renderElement(item, childData, childDirective);
                            this.dtoStack.pop();
                            this.lineage.pop();
                        }
                    }
                };
                ViewRenderer.prototype.renderCollection = function (element, data, directive) {
                    var _this = this;
                    if (directive === void 0) {
                        directive = null;
                    }
                    var container = element;
                    this.log('renderCollection');
                    if (element.hasAttribute("data-unless")) {
                        var value = element.getAttribute("data-unless");
                        var name = this.getName(element);
                        var result = false;
                        if (name === value) {
                            result = data.length === 0;
                        } else {
                            var ctx = { element: element, data: data, dto: this.dtoStack[this.dtoStack.length - 2] };
                            result = this.evalInContext(value, ctx);
                        }
                        if (result) {
                            this.log('unless(show)');
                            element.style.display = "";
                        } else {
                            this.log('unless(hide)');
                            element.style.display = "none";
                        }
                    }
                    if (container.tagName === "TR" || container.tagName === "LI") {
                        container = container.parentElement;
                        this.log('correcting container element by moving to up to parent', container);
                        container.setAttribute("data-collection", element.getAttribute("data-collection"));
                        element.setAttribute("data-name", "value");
                        element.removeAttribute("data-collection");
                    }
                    var template = container.firstElementChild.cloneNode(true);
                    template.removeAttribute("data-template");
                    template.style.display = "";
                    if (!container.firstElementChild.hasAttribute("data-template")) {
                        if (container.childElementCount !== 1) {
                            throw new Error("There must be a single child element in collection containers. If you use multiple elements you need to for instance wrap them in a div. Path: '" + this.lineage.join(" -> ") + "'.");
                        }
                        var el = container.firstElementChild;
                        el.style.display = "none";
                        el.setAttribute("data-template", "true");
                    }
                    while (container.childElementCount > 1) {
                        container.removeChild(container.lastElementChild);
                    }
                    var index = 0;
                    var collection = data;
                    collection.forEach(function (item) {
                        var ourNode = template.cloneNode(true);
                        _this.lineage.push("[" + index + "]");
                        _this.dtoStack.push(item);
                        _this.renderElement(ourNode, item, directive);
                        _this.lineage.pop();
                        _this.dtoStack.pop();
                        index = index + 1;
                        container.appendChild(ourNode);
                    });
                };
                ViewRenderer.prototype.applyEmbeddedDirectives = function (element, data, directives) {
                    var isDirectiveValueSpecified = false;
                    for (var key in directives) {
                        var value = directives[key].apply(element, [data, this.dtoStack[this.dtoStack.length - 2]]);
                        if (key === "html") {
                            isDirectiveValueSpecified = true;
                            this.log('Assigning html', value, "to", element);
                            element.innerHTML = value;
                        } else if (key === "text") {
                            isDirectiveValueSpecified = true;
                            this.log('Assigning text', value, "to", element);
                            element.innerText = value;
                        } else {
                            if (key === "value") {
                                isDirectiveValueSpecified = true;
                            }
                            this.log('Assigning', value, "to", element, 'attribute', key);
                            element.setAttribute(key, value);
                        }
                    }
                    return isDirectiveValueSpecified;
                };
                ViewRenderer.prototype.runDirectives = function (element, data) {
                    var _this = this;
                    var context = {
                        element: element,
                        lineage: this.lineage,
                        propertyName: this.lineage[this.lineage.length - 1],
                        value: data
                    };
                    this.directives.forEach(function (directive) {
                        if (!directive.process(context)) {
                            _this.log('Local directive ', directive, "cancelled processing of", context);
                            return false;
                        }
                        ;
                    });
                    ViewRenderer.globalValueDirectives.forEach(function (directive) {
                        if (!directive.process(context)) {
                            _this.log('Global directive ', directive, "cancelled processing of", context);
                            return false;
                        }
                        ;
                    });
                    return context.value;
                };
                ViewRenderer.prototype.getName = function (el) {
                    return el.getAttribute("name") || el.getAttribute("data-name") || el.getAttribute("data-collection") || el.getAttribute("data-unless");
                };
                ViewRenderer.prototype.hasName = function (el) {
                    return el.hasAttribute("name") || el.hasAttribute("data-name") || el.hasAttribute("data-collection") || el.hasAttribute("data-unless");
                };
                ViewRenderer.prototype.isCollection = function (el) {
                    return el.hasAttribute("data-collection");
                };
                ViewRenderer.prototype.evalInContext = function (code, context) {
                    var func = function func(js) {
                        return eval("with (this) { " + js + "}");
                    };
                    return func.call(context, code);
                };
                ViewRenderer.prototype.log = function () {
                    var args = [];
                    for (var _i = 0; _i < arguments.length; _i++) {
                        args[_i] = arguments[_i];
                    }
                    if (ViewRenderer.DEBUG && console && console.log) {
                        args.unshift(this.dtoStack[this.dtoStack.length - 1]);
                        if (this.lineage.length == 0) {
                            args.unshift('rootNode');
                        } else {
                            args.unshift(this.lineage[this.lineage.length - 1]);
                        }
                        args.unshift(arguments.callee.caller.prototype);
                        console.log.apply(console, args);
                    }
                };
                return ViewRenderer;
            })();
            ViewRenderer.globalValueDirectives = [];
            ViewRenderer.DEBUG = false;
            Views.ViewRenderer = ViewRenderer;
            var ViewValueDirectiveContext = (function () {
                function ViewValueDirectiveContext() {}
                return ViewValueDirectiveContext;
            })();
            Views.ViewValueDirectiveContext = ViewValueDirectiveContext;
        })(Views = Yo.Views || (Yo.Views = {}));
    })(Yo = Griffin.Yo || (Griffin.Yo = {}));
})(Griffin || (Griffin = {}));
var Griffin;
(function (Griffin) {
    var Yo;
    (function (Yo) {
        var G = (function () {
            function G() {}
            G.render = function (idOrElem, dto, directives) {
                var r = new Yo.Views.ViewRenderer(idOrElem);
                r.render(dto, directives);
            };
            return G;
        })();
        G.select = new Yo.Dom.Selector();
        G.handle = new Yo.Dom.EventMapper();
        Yo.G = G;
    })(Yo = Griffin.Yo || (Griffin.Yo = {}));
})(Griffin || (Griffin = {}));
var Griffin;
(function (Griffin) {
    var Yo;
    (function (Yo) {
        var GlobalConfig = (function () {
            function GlobalConfig() {}
            return GlobalConfig;
        })();
        GlobalConfig.applicationScope = {};
        Yo.GlobalConfig = GlobalConfig;
        GlobalConfig.resourceLocator = {
            getHtml: function getHtml(section) {
                var path = window.location.pathname;
                if (window.location.pathname.indexOf(".") > -1) {
                    var pos = window.location.pathname.lastIndexOf("/");
                    path = window.location.pathname.substr(0, pos);
                }
                if (path.substring(-1, 1) === "/") {
                    path = path.substring(0, -1);
                }
                return path + ("/Views/" + section + ".html");
            },
            getScript: function getScript(section) {
                var path = window.location.pathname;
                if (window.location.pathname.indexOf(".") > -1) {
                    var pos = window.location.pathname.lastIndexOf("/");
                    path = window.location.pathname.substr(0, pos);
                }
                if (path.substring(-1, 1) === "/") {
                    path = path.substring(0, -1);
                }
                return path + ("/ViewModels/" + section + "ViewModel.js");
            }
        };
        GlobalConfig.applicationScope = {};
        GlobalConfig.viewModelFactory = {
            create: function create(applicationName, fullViewModelName) {
                var viewModelConstructor = Yo.Spa.ViewModels.ClassFactory.getConstructor(applicationName, fullViewModelName);
                return new viewModelConstructor(GlobalConfig.applicationScope);
            }
        };
    })(Yo = Griffin.Yo || (Griffin.Yo = {}));
})(Griffin || (Griffin = {}));
//# sourceMappingURL=Griffin.Yo.js.map

