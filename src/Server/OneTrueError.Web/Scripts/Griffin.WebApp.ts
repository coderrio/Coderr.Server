/// <reference path="typings/jquery/jquery.d.ts" />
/// <reference path="Promise.ts" />
module Griffin.WebApp {
    export class ChangedEventArgs {
        isHandled = true;

        constructor(public model: ViewModel, public field: HTMLElement, public newValue: string) {

        }
    }

    export class ClickEventArgs {
        isHandled = true;

        constructor(public model: ViewModel, public field: HTMLElement, public newValue: string) {

        }
    }

    class FieldModel {
        elem: HTMLElement;

        equalsName(name: string) {
            return this.propertyName.toLocaleLowerCase() === name.toLocaleLowerCase();
        }

        public constructor(public propertyName: string) {

        }

        setContent(value: any) {
            const elm = this.elem as any;
            if (elm.value === "undefined") {
                elm.value = value;
            } else {
                this.elem.innerHTML = value;
            }
        }
    }

    export class ViewModel {
        private id: string;
        private jqueryMappings = new Array<FieldModel>();
        private instance = null;
        static ENFORCE_MAPPINGS = false;

        constructor(modelId: string, viewModel: any) {
            this.id = modelId;
            this.instance = viewModel;
            for (let fieldName in viewModel) {
                const underscorePos = fieldName.indexOf("_");
                if (underscorePos > 0 && typeof viewModel[fieldName] === "function") {
                    const propertyName = fieldName.substr(0, underscorePos);
                    const eventName = fieldName.substr(underscorePos + 1);
                    if (typeof this.jqueryMappings[propertyName] == "undefined") {
                        this.jqueryMappings[propertyName] = new FieldModel(propertyName);
                        var elem = this.elem(propertyName, this.id);
                        if (elem.length === 0) {
                            if (ViewModel.ENFORCE_MAPPINGS) {
                                throw `Failed to find child element "${propertyName}" in model "${this.id}"`;
                            }
                        } else {
                            this.jqueryMappings[propertyName].elem = elem[0];
                        }
                    }

                    this.jqueryMappings[propertyName][eventName] = viewModel[fieldName];
                    this.mapEvent(propertyName, eventName, fieldName);
                } else if (typeof viewModel[fieldName] !== "function") {

                    if (fieldName[0] === "_") {
                        continue;
                    }
                    var elem = this.elem(fieldName, this.id);
                    if (elem.length === 0) {
                        if (ViewModel.ENFORCE_MAPPINGS) {
                            throw `Failed to find child element "${fieldName}" in model "${this.id}"`;
                        }
                        continue;
                    }

                    if (typeof this.jqueryMappings[fieldName] == "undefined")
                        this.jqueryMappings[fieldName] = new FieldModel(fieldName);
                    this.jqueryMappings[fieldName].elem = elem[0];
                }

            }
        }

        mapChanged(elementNameOrId: string): P.Promise<ChangedEventArgs> {
            var p = P.defer<ChangedEventArgs>();

            const elem = this.elem(elementNameOrId, this.id);
            if (elem.length === 0)
                throw `Failed to find child element "${elementNameOrId}" in model "${this.id}"`;

            var model = this;
            elem.on("changed",
                function() {
                    p.resolve(new ChangedEventArgs(model, this as HTMLElement, $(this).val()));
                });

            return p.promise();
        }

        bind<TModel>() {

        }

        private getField(name: string): FieldModel {
            for (let fieldName in this.jqueryMappings) {
                const field = this.jqueryMappings[fieldName];
                if (field.equalsName(name))
                    return field;
            }
            return null;
        }

        update(json: any) {
            const model = this;
            for (let fieldName in json) {
                const field = this.getField(fieldName);
                if (field !== null) {
                    field.setContent(json[fieldName]);
                }
            }
        }

        private elem(name: string, parent: any = null): JQuery {
            const searchString = `#${name},[data-name="${name}"],[name="${name}"]`;
            if (parent == null)
                return $(searchString);

            if (typeof parent === "string") {
                return $(searchString, $(`#${parent}`));
            }

            return $(searchString, parent);
        }

        private mapEvent(propertyName, eventName, fieldName): void {
            const elem = this.elem(propertyName, this.id);
            if (elem.length === 0)
                throw `Failed to find child element "${propertyName}" in model "${this.id}" for event "${eventName}'.`;

            var binder = this;
            if (eventName === "change" || eventName === "changed") {
                elem.on("change",
                    function(e) {
                        const args = new ChangedEventArgs(binder.instance, this, $(this).val());
                        binder.jqueryMappings[propertyName][eventName].apply(binder.instance, [args]);
                        if (args.isHandled) {
                            e.preventDefault();
                        }
                    });
            } else if (eventName === "click" || eventName === "clicked") {
                elem.on("click",
                    function(e) {
                        var value = "";
                        if (this.tagName == "A") {
                            value = this.getAttribute("href");
                        } else {
                            value = $(this).val();
                        }
                        const args = new ClickEventArgs(binder.instance, this, value);
                        binder.jqueryMappings[propertyName][eventName].apply(binder.instance, [args]);
                        if (args.isHandled) {
                            e.preventDefault();
                        }
                    });
            }
        }
    }

    export class Carburator {
        static mapRoute(elementNameOrId: string, viewModel: any): ViewModel {
            return new ViewModel(elementNameOrId, viewModel);
        }

    }

    export interface IPagerSubscriber {
        onPager(pager: Pager): void;
    }

    export class PagerPage {
        listItem: HTMLElement;

        constructor(public pageNumber: number, public selected: boolean) {

        }

        select(): void {
            this.listItem.firstElementChild.setAttribute("class", `number ${Pager.BTN_ACTIVE_CLASS}`);
            this.selected = true;
        }

        deselect(): void {
            this.listItem.firstElementChild.setAttribute("class", `number ${Pager.BTN_CLASS}`);
            this.selected = true;
        }
    }

    export class Pager {
        pageCount = 0;
        pages: PagerPage[] = new Array();
        static BTN_ACTIVE_CLASS = "btn btn-success";
        static BTN_CLASS = "btn";
        private _subscribers: IPagerSubscriber[] = new Array();
        private nextItem: HTMLElement;
        private prevItem: HTMLElement;
        private parent: HTMLElement;

        constructor(public currentPage: number, public pageSize: number, public totalNumberOfItems: number) {
            this.update(currentPage, pageSize, totalNumberOfItems);
        }

        update(currentPage: number, pageSize: number, totalNumberOfItems: number) {
            if (totalNumberOfItems < pageSize || pageSize === 0 || totalNumberOfItems === 0) {
                this.pageCount = 0;
                this.pages = [];
                if (this.parent) {
                    this.draw(this.parent);
                }
                return;
            }

            const isFirstUpdate = this.totalNumberOfItems === 0;
            this.pageCount = Math.ceil(totalNumberOfItems / pageSize);
            this.currentPage = currentPage;
            this.totalNumberOfItems = totalNumberOfItems;
            this.pageSize = pageSize;
            let i = 1;
            this.pages = new Array();
            for (i = 1; i <= this.pageCount; i++)
                this.pages.push(new PagerPage(i, i === currentPage));

            if (this.parent) {
                this.draw(this.parent);
            }

            if (!isFirstUpdate) {
                this.notify();
            }
        }

        subscribe(subscriber: IPagerSubscriber): void {
            this._subscribers.push(subscriber);
        }

        moveNext() {
            if (this.currentPage < this.pageCount) {
                this.pages[this.currentPage - 1].deselect();
                this.currentPage += 1;
                this.pages[this.currentPage - 1].select();

                if (this.currentPage === this.pageCount) {
                    this.nextItem.style.display = "none";
                }
                this.prevItem.style.display = "";

                this.notify();
            }
        }

        movePrevious() {
            if (this.currentPage > 1) {
                this.pages[this.currentPage - 1].deselect();
                this.currentPage -= 1;
                this.pages[this.currentPage - 1].select();

                if (this.currentPage === 1) {
                    this.prevItem.style.display = "none";
                } else {

                }
                this.nextItem.style.display = "";
                this.notify();
            }
        }

        goto(pageNumber: number) {
            this.pages[this.currentPage - 1].deselect();
            this.currentPage = pageNumber;
            this.pages[this.currentPage - 1].select();

            if (this.currentPage === 1 || this.pageCount === 1) {
                this.prevItem.style.display = "none";
            } else {
                this.prevItem.style.display = "";
            }
            if (this.currentPage === this.pageCount || this.pageCount === 1) {
                this.nextItem.style.display = "none";
            } else {
                this.nextItem.style.display = "";
            }


            this.notify();
        }

        private createListItem(title: string, callback: () => void): HTMLElement {
            const btn = document.createElement("button");
            btn.innerHTML = title;
            btn.className = Pager.BTN_CLASS;
            btn.addEventListener("click",
                function(e) {
                    e.preventDefault();
                    callback();
                });
            const li = document.createElement("li");
            li.appendChild(btn);
            return li;
        }

        private deactivateAll() {
            this.prevItem.firstElementChild.setAttribute("class", Pager.BTN_CLASS);
            this.nextItem.firstElementChild.setAttribute("class", Pager.BTN_CLASS);
            this.pages.forEach(function(item) {
                item.selected = false;
                item.listItem.firstElementChild.setAttribute("class", Pager.BTN_CLASS);
            });
        }

        draw(containerIdOrElement: string|HTMLElement): void {
            if (typeof containerIdOrElement === "string") {
                this.parent = document.getElementById(containerIdOrElement);
                if (!this.parent)
                    throw new Error(`Failed to find '${containerIdOrElement}'`);
            } else {
                if (!containerIdOrElement)
                    throw new Error("No element was specified");
                this.parent = containerIdOrElement;
            }
            var self = this;

            var ul = document.createElement("ul");

            //prev
            var li = this.createListItem("&lt;&lt; Previous",
                function() {
                    self.movePrevious();
                });
            ul.appendChild(li);
            this.prevItem = li;
            if (this.currentPage <= 1 || this.pageCount <= 1) {
                this.prevItem.style.display = "none";
            } else {
                this.prevItem.style.display = "";
            }

            //pages
            this.pages.forEach(item => {
                var li = this.createListItem(item.pageNumber.toString(),
                    function() {
                        self.goto(item.pageNumber);
                    });
                item.listItem = li;
                ul.appendChild(li);
                if (item.selected) {
                    li.className = "active";
                    li.firstElementChild.setAttribute("class", `number ${Pager.BTN_ACTIVE_CLASS}`);
                }
            });

            //next
            var li = this.createListItem("Next &gt;&gt;",
                function() {
                    self.moveNext();
                });
            ul.appendChild(li);
            this.nextItem = li;
            if (this.currentPage >= this.pageCount || this.pageCount <= 1) {
                this.nextItem.style.display = "none";
            } else {
                this.nextItem.style.display = "";
            }

            ul.className = "pager";
            while (this.parent.firstChild) {
                this.parent.removeChild(this.parent.firstChild);
            }
            this.parent.appendChild(ul);
        }

        private notify(): void {
            var self = this;
            this._subscribers.forEach(function(item) {
                item.onPager.apply(item, [self]);
            });
        }

        reset() {
            this.pageCount = 0;
            this.pages = [];

            if (this.parent) {
                this.draw(this.parent);
            }
        }
    }

}