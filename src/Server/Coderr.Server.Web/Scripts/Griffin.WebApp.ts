/// <reference path="typings/jquery/jquery.d.ts" />
/// <reference path="Promise.ts" />
module Griffin.WebApp {

    export interface IPagerSubscriber {
        onPager(pager: Pager): void;
    }

    export class PagerPage {
        listItem: HTMLElement;

        constructor(public pageNumber: number, public selected: boolean) {

        }

        select(): void {
            this.listItem.classList.add(Pager.LI_ACTIVE_CLASS);
            this.selected = true;
        }

        deselect(): void {
            this.listItem.classList.remove(Pager.LI_ACTIVE_CLASS);
            this.selected = true;
        }
    }

    export class Pager {
        pageCount = 0;
        pages: PagerPage[] = new Array();
        static LI_ACTIVE_CLASS = "active";
        static UL_CLASS = "pagination";
        static LI_CLASS = "page-item";
        static LINK_CLASS = "page-link";
        private _subscribers: IPagerSubscriber[] = new Array();
        private nextItem: HTMLElement;
        private prevItem: HTMLElement;
        private parent: HTMLElement;
        private partialPages: boolean;

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

            if (this.pageCount > 10) {
                this.partialPages = true;
                this.generateDynamicPaging();
            } else {
                for (i = 1; i <= this.pageCount; i++)
                    this.pages.push(new PagerPage(i, i === currentPage));
            }

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
            if (this.currentPage >= this.pageCount) {
                return;
            }
            if (this.partialPages) {
                this.currentPage += 1;
                this.generateDynamicPaging();
                this.draw(this.parent);
            } else {
                this.pages[this.currentPage - 1].deselect();
                this.currentPage += 1;
                this.pages[this.currentPage - 1].select();
            }


            if (this.currentPage === this.pageCount) {
                this.nextItem.style.display = "none";
            }
            this.prevItem.style.display = "";

            this.notify();

        }

        movePrevious() {
            if (this.currentPage <= 1) {
                return;
            }

            if (this.partialPages) {
                this.currentPage += 1;
                this.generateDynamicPaging();
                this.draw(this.parent);
            } else {
                this.pages[this.currentPage - 1].deselect();
                this.currentPage -= 1;
                this.pages[this.currentPage - 1].select();
            }

            if (this.currentPage === 1) {
                this.prevItem.style.display = "none";
            }

            this.nextItem.style.display = "";
            this.notify();
        }

        goto(pageNumber: number) {
            console.log(pageNumber, this.partialPages);
            if (this.partialPages) {
                this.currentPage = pageNumber;
                this.generateDynamicPaging();
                this.draw(this.parent);
            } else {
                this.pages[this.currentPage - 1].deselect();
                this.currentPage = pageNumber;
                this.pages[this.currentPage - 1].select();
            }

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

        private generateDynamicPaging() {
            this.pages = [];
            var firstPage = this.currentPage - 4;
            if (firstPage < 1)
                firstPage = 1;
            else
                this.pages.push(new PagerPage(1, false));
            console.log(firstPage);
            var lastPage = firstPage + 9;
            if (lastPage > this.pageCount)
                lastPage = this.pageCount;

            var i;
            for (i = firstPage; i <= lastPage; i++)
                this.pages.push(new PagerPage(i, i === this.currentPage));

            if (lastPage < this.pageCount)
                this.pages.push(new PagerPage(this.pageCount, false));
        }
        private createListItem(title: string, callback: () => void): HTMLElement {
            const btn = document.createElement("a");
            btn.innerHTML = title;
            btn.className = Pager.LINK_CLASS;
            btn.addEventListener("click",
                function (e) {
                    e.preventDefault();
                    callback();
                });
            const li = document.createElement("li");
            li.className = Pager.LI_CLASS;
            li.appendChild(btn);
            return li;
        }

        private deactivateAll() {
            this.prevItem.classList.remove(Pager.LI_ACTIVE_CLASS);
            this.nextItem.classList.remove(Pager.LI_ACTIVE_CLASS);
            this.pages.forEach(function (item) {
                item.selected = false;
                item.listItem.classList.remove(Pager.LI_ACTIVE_CLASS);
            });
        }

        draw(containerIdOrElement: string | HTMLElement): void {
            if (typeof containerIdOrElement === "string") {
                this.parent = document.getElementById(containerIdOrElement);
                if (!this.parent)
                    throw new Error(`Failed to find '${containerIdOrElement}'`);
            } else {
                if (!containerIdOrElement)
                    throw new Error("No element was specified");
                this.parent = containerIdOrElement;
            }

            while (this.parent.childElementCount > 0) {
                this.parent.removeChild(this.parent.firstElementChild);
            }
            var self = this;

            var ul = document.createElement("ul");
            ul.className = Pager.UL_CLASS;

            //prev
            var li = this.createListItem("&lt;&lt; Previous",
                function () {
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
                    function () {
                        self.goto(item.pageNumber);
                    });
                item.listItem = li;
                ul.appendChild(li);
                if (item.selected) {
                    li.classList.add(Pager.LI_ACTIVE_CLASS);
                }
            });

            //next
            var li = this.createListItem("Next &gt;&gt;",
                function () {
                    self.moveNext();
                });
            li.className = Pager.LI_CLASS;
            ul.appendChild(li);
            this.nextItem = li;
            if (this.currentPage >= this.pageCount || this.pageCount <= 1) {
                this.nextItem.style.display = "none";
            } else {
                this.nextItem.style.display = "";
            }

            while (this.parent.firstChild) {
                this.parent.removeChild(this.parent.firstChild);
            }
            this.parent.appendChild(ul);
        }

        private notify(): void {
            var self = this;
            this._subscribers.forEach(function (item) {
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