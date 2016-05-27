interface JQuery {
    modal(options?: any);
}

declare var $: JQueryStatic;

module Griffin {

    /**
     * Context information for @see IToolbarHandler
     */
    export interface IToolbarContext {
        editorElement: HTMLTextAreaElement;
        editor: Editor;
        actionName: string;
        selection: TextSelector;
    }

    /**
     * Invokes the action once a toolbar button is pressed
     */
    export interface IToolbarHandler {
        /**
         * Trigger preview of the generated result
         * @param editor Editor that the toolbar belongs to
         * @param target Element that the generated result should be visible in
         * @param textToParse Text to convert.
         */
        preview(editor: Editor, target: HTMLElement, textToParse: string): void;

        /**
         * Invoke a toolbar button
         * @param context Context information
         */
        invokeAction(context: IToolbarContext);
    }

    /**
     * Used to be able to syntax highlight code.
     */
    export interface ISyntaxHighlighter {
        /**
         * Highlight code (transform each passed node).
         * Each array contains the element that 
         * @param blockElements Code sections (typically fenced code blocks). 
         * @param inlineElements Small code blocks (typically a variable name or similar) embedded in text.
         */
        highlight(blockElements: HTMLElement[], inlineElements: HTMLElement[]): void;
    }

    /**
     * Result from a image dialog
     * @see IDialogProvider
     */
    export interface IImageInfo {
        href: string;
        title: string;
    }

    /**
     * Used to process markdown and similar.
     */
    export interface ITextParser {
        /**
         * Parse text
         * @param text Text as the user wrote it.
         * @returns HTML 
         */
        parse(text: string): string;
    }

    /**
     * A text selection in our text box
     * @see TextSelector
     */
    export interface ITextSelection {
        start: number;
        end: number;
        length: number;
    }


    /**
     * Returns link information from IDialogProvider
     * @see IDialogProvider
     */
    export interface ILinkInfo {
        /**
         * Link URL
         */
        url: string;

        /**
         * Text to display
         */
        text: string;
    }

    /**
     * Used when a dialog is requested.
     */
    export interface IDialogProviderContext {

        /**
         * Selection if any
         */
        selection: TextSelector;

        /**
         * Editor that want to open a dialog
         */
        editor: Editor;

        /**
         * Element that the editor is wrapping
         */
        editorElement: HTMLTextAreaElement;
    }

    /**
     * Used to be able to use ones favorite javascript library to open dialogs
     */
    export interface IDialogProvider {
        /**
         * User want to embed an image in the editor
         * @param context Information that can be used to pre select an image
         * @param callback Invoke it with the image details once done.
         */
        image(context: IDialogProviderContext, callback: (result: IImageInfo) => void);

        /**
         * User want to insert a link in the editor.
         * @param context Information that can be used to pre select an image
         * @param callback Invoke it with the image details once done.
         */
        link(context: IDialogProviderContext, callback: (result: ILinkInfo) => void);
    }

    /**
     * The main mother of all editors.
     */
    export class Editor {
        dialogProvider: IDialogProvider;
        syntaxHighlighter: ISyntaxHighlighter;
        id: string;
        private containerElement: HTMLElement;
        private element: HTMLTextAreaElement;
        private textSelector: TextSelector;
        private toolbarElement: HTMLElement;
        private previewElement: HTMLElement;
        private toolbarHandler: IToolbarHandler;
        private keyMap = {};
        private editorTimer: any;

        /**
         * Let the text area grow instead of getting a vertical scroll bar.
         */
        autoSize: boolean;

        /**
         * Create a new editor
         * @param elementOrId either an HTML id (without hash) or a HTMLTextAreaElement.
         * @param parser Used to transform markdown to HTML (or another language).
         */
        constructor(elementOrId: any, parser: ITextParser) {
            if (typeof elementOrId === "string") {
                this.containerElement = document.getElementById(elementOrId);
            } else {
                this.containerElement = elementOrId;
            }

            this.id = this.containerElement.id;
            var id = this.containerElement.id;
            this.element = (this.containerElement.getElementsByTagName("textarea")[0]);
            this.previewElement = document.getElementById(id + "-preview");
            this.toolbarElement = <HTMLElement>this.containerElement.getElementsByClassName("toolbar")[0];
            this.textSelector = new TextSelector(this.element);
            this.toolbarHandler = new MarkdownToolbar(parser);
            this.assignAccessKeys();

            if (typeof $().modal == "function") {
                this.dialogProvider = new BoostrapDialogs();
            } else {
                this.dialogProvider = new ConfirmDialogs();
                document.getElementById(id + "-imageDialog").style.display = "none";
                document.getElementById(id + "-linkDialog").style.display = "none";
            }

            this.bindEvents();
        }


        private trimSpaceInSelection() {
            var selectedText = this.textSelector.text();
            var pos = this.textSelector.get();
            if (selectedText.substr(selectedText.length - 1, 1) === " ") {
                this.textSelector.select(pos.start, pos.end - 1);
            }
        }

        private getActionNameFromClass(classString: string) {
            var classNames = classString.split(/\s+/);
            for (var i = 0; i < classNames.length; i++) {
                if (classNames[i].substr(0, 7) === "button-") {
                    return classNames[i].substr(7);
                }
            }

            return null;
        }

        private assignAccessKeys() {
            var self = this;
            var spans = this.toolbarElement.getElementsByTagName("span");
            var len = spans.length;
            for (var i = 0; i < len; i++) {
                if (!spans[i].getAttribute("accesskey"))
                    continue;

                var button = spans[i];
                var title = button.getAttribute("title");
                var key = button.getAttribute("accesskey").toUpperCase();
                var actionName = self.getActionNameFromClass(button.className);
                button.setAttribute("title", title + " [CTRL+" + key + "]");
                this.keyMap[key] = actionName;
            }
        }

        invokeAutoSize() {
            if (!this.autoSize) {
                return;
            }

            var twin = $(this).data("twin-area");
            if (typeof twin === "undefined") {
                twin = $('<textarea style="position:absolute; top: -10000px"></textarea>');
                twin.appendTo("body");
                //div.appendTo('body');
                $(this).data("twin-area", twin);
                $(this).data("originalSize", {
                    width: this.element.clientWidth,
                    height: this.element.clientHeight,
                    //position: data.editor.css('position'), 
                    top: this.getTopPos(this.element),
                    left: this.getLeftPos(this.element)
                });
            }
            twin.css("height", this.element.clientHeight);
            twin.css("width", this.element.clientWidth);
            twin.html(this.element.getAttribute("value") + "some\r\nmore\r\n");
            if (twin[0].clientHeight < twin[0].scrollHeight) {
                var style = {
                    height: (this.element.clientHeight + 100) + "px",
                    width: this.element.clientWidth,
                    //position: 'absolute', 
                    top: this.getTopPos(this.element),
                    left: this.getLeftPos(this.element)
                    //zindex: 99
                };
                $(this.element).css(style);
                $(this).data("expandedSize", style);
            }
        }


        private bindEvents() {
            this.bindToolbarEvents();
            this.bindAccessors();
            this.bindEditorEvents();
        }

        private bindEditorEvents(): void {
            var self = this;
            this.element.addEventListener("focus", (e: FocusEvent) => {
                //grow editor
            });
            this.element.addEventListener("blur", (e: FocusEvent) => {
                //shrink editor
            });
            this.element.addEventListener("keyup", (e: KeyboardEvent) => {
                self.preview();
                //self.invokeAutoSize();
            });
            this.element.addEventListener("paste", (e: any) => {
                setTimeout(function() {
                    self.preview();
                }, 100);
            });
        }

        private bindToolbarEvents(): void {
            var spans = this.toolbarElement.getElementsByTagName("span");
            var len = spans.length;
            var self = this;
            for (var i = 0; i < len; i++) {
                if (spans[i].className.indexOf("button") === -1)
                    continue;
                var button = spans[i];
                button.addEventListener("click", (e: MouseEvent) => {
                    var btn = <HTMLElement>e.target;
                    if (btn.tagName != "span") {
                        btn = (<HTMLElement>e.target).parentElement;
                    }
                    var actionName = self.getActionNameFromClass(btn.className);
                    self.invokeAction(actionName);
                    self.preview();
                    return this;
                });
            }
        }

        private bindAccessors(): void {
            var self = this;

            //required to override browser keys
            document.addEventListener("keydown", (e: KeyboardEvent) => {
                if (!e.ctrlKey)
                    return;
                var key = String.fromCharCode(e.which);
                if (!key || key.length === 0)
                    return;
                if (e.target !== self.element)
                    return;

                var actionName = this.keyMap[key];
                if (actionName) {
                    e.cancelBubble = true;
                    e.stopPropagation();
                    e.preventDefault();
                }
            });
            this.element.addEventListener("keyup", (e: KeyboardEvent) => {
                if (!e.ctrlKey)
                    return;

                var key = String.fromCharCode(e.which);
                if (!key || key.length === 0)
                    return;

                var actionName = this.keyMap[key];
                if (actionName) {
                    this.invokeAction(actionName);
                    self.preview();
                }
            });
        }

        /**
         * Invoke a toolbar action
         * @param actionName "H1", "B" or similar
         */
        invokeAction(actionName) {
            if (!actionName || actionName.length === 0)
                throw new Error("ActionName cannot be empty");

            this.trimSpaceInSelection();
            this.toolbarHandler.invokeAction({
                editorElement: this.element,
                editor: this,
                actionName: actionName,
                selection: this.textSelector
            });
        }

        private getTopPos(element: HTMLElement): number {
            return element.getBoundingClientRect().top + window.pageYOffset - element.ownerDocument.documentElement.clientTop;
        }

        private getLeftPos(element: HTMLElement): number {
            return element.getBoundingClientRect().left + window.pageXOffset - element.ownerDocument.documentElement.clientLeft;
        }

        /**
         * Update the preview window
         */
        preview(): void {
            if (this.previewElement == null) {
                return;
            }

            this.toolbarHandler.preview(this, this.previewElement, this.element.value);
            if (this.editorTimer) {
                clearTimeout(this.editorTimer);
            }
            if (this.syntaxHighlighter) {
                this.editorTimer = setTimeout(() => {
                    var tags = this.previewElement.getElementsByTagName("code");
                    var inlineBlocks = [];
                    var codeBlocks = [];
                    for (var i = 0; i < tags.length; i++) {
                        var elem = <HTMLElement>tags[i];
                        if (elem.parentElement.tagName === "PRE") {
                            codeBlocks.push(elem);
                        } else {
                            inlineBlocks.push(elem);
                        }
                    }
                    this.syntaxHighlighter.highlight(inlineBlocks, codeBlocks);
                }, 1000);
            }
        }

    }

    export class ConfirmDialogs implements IDialogProvider {
        image(context: IDialogProviderContext, callback: (result: IImageInfo) => void) {
            var url = prompt("Enter image URL", context.selection.text());
            setTimeout(() => {
                callback({
                    href: url,
                    title: "Enter title here"
                });
            });
        }

        link(context: IDialogProviderContext, callback: (result: ILinkInfo) => void) {
            var url = prompt("Enter URL", context.selection.text());
            setTimeout(() => {
                callback({
                    url: url,
                    text: "Enter title here"
                });
            });
        }
    }

    export class BoostrapDialogs implements IDialogProvider {
        image(context: IDialogProviderContext, callback: (result: IImageInfo) => void) {
            var dialog = $(`#${context.editor.id}-imageDialog`);
            if (!dialog.data("griffin-imageDialog-inited")) {
                dialog.data("griffin-imageDialog-inited", true);
                $("[data-success]", dialog).click(() => {
                    dialog.modal("hide");
                    callback({
                        href: $('[name="imageUrl"]', dialog).val(),
                        title: $('[name="imageCaption"]', dialog).val()
                    });
                    context.editorElement.focus();
                });
            }

            if (context.selection.isSelected()) {
                $('[name="imageCaption"]', dialog).val(context.selection.text());
            }
            dialog.on("shown.bs.modal", () => {
                $('[name="imageUrl"]', dialog).focus();
            });


            dialog.modal({
                show: true
            });
        }

        link(context: IDialogProviderContext, callback: (result: ILinkInfo) => void) {
            var dialog = $(`#${context.editor.id}-linkDialog`);
            if (!dialog.data("griffin-linkDialog-inited")) {
                dialog.data("griffin-linkDialog-inited", true);
                $("[data-success]", dialog).click(() => {
                    dialog.modal("hide");
                    callback({
                        url: $('[name="linkUrl"]', dialog).val(),
                        text: $('[name="linkText"]', dialog).val()
                    });
                    context.editorElement.focus();
                });
                dialog.on("shown.bs.modal", () => {
                    $('[name="linkUrl"]', dialog).focus();
                });
                dialog.on("hidden.bs.modal", () => {
                    context.editorElement.focus();
                });
            }

            if (context.selection.isSelected()) {
                $('[name="linkText"]', dialog).val(context.selection.text());
            }


            dialog.modal({
                show: true
            });
        }
    }

    export class MarkdownToolbar implements IToolbarHandler {

        private parser: ITextParser;

        constructor(parser: ITextParser) {
            this.parser = parser;
        }

        invokeAction(context: IToolbarContext) {
            //			console.log(griffinEditor);

            var method = `action${context.actionName.substr(0, 1).toUpperCase()}${context.actionName.substr(1)}`;
            if (this[method]) {
                var args = [];
                args[0] = context.selection;
                args[1] = context;
                return this[method].apply(this, args);
            } else {
                if (typeof alert !== "undefined") {
                    alert(`Missing ${method} in the active textHandler (griffinEditorExtension)`);
                }
            }

            return this;
        }

        preview(editor: Editor, preview: HTMLElement, contents: string) {
            if (contents === null || typeof contents === "undefined") {
                throw new Error("May not be called without actual content.");
            }
            preview.innerHTML = this.parser.parse(contents);
        }

        private removeWrapping(selection: TextSelector, wrapperString: string) {
            var wrapperLength = wrapperString.length;
            var editor = selection.element;
            var pos = selection.get();

            // expand double click
            if (pos.start !== 0 && editor.value.substr(pos.start - wrapperLength, wrapperLength) === wrapperString) {
                selection.select(pos.start - wrapperLength, pos.end + wrapperLength);
                pos = selection.get();
            }

            // remove 
            if (selection.text().substr(0, wrapperLength) === wrapperString) {
                var text = selection.text().substr(wrapperLength, selection.text().length - (wrapperLength * 2));
                selection.replace(text);
                selection.select(pos.start, pos.end - (wrapperLength * 2));
                return true;
            }

            return false;
        }


        private actionBold(selection: TextSelector) {
            var isSelected = selection.isSelected();
            var pos = selection.get();

            if (this.removeWrapping(selection, "**")) {
                return this;
            }

            selection.replace(`**${selection.text()}**`);

            if (isSelected) {
                selection.select(pos.start, pos.end + 4);
            } else {
                selection.select(pos.start + 2, pos.start + 2);
            }

            return this;
        }

        private actionItalic(selection: TextSelector) {
            var isSelected = selection.isSelected();
            var pos = selection.get();

            if (this.removeWrapping(selection, "_")) {
                return this;
            }

            selection.replace(`_${selection.text()}_`);

            if (isSelected) {
                selection.select(pos.start, pos.end + 2);
            } else {
                selection.select(pos.start + 1, pos.start + 1);
            }

            return this;
        }

        private addTextToBeginningOfLine(selection: TextSelector, textToAdd: string) {
            var isSelected = selection.isSelected();
            if (!isSelected) {
                var text = selection.element.value;
                var orgPos = selection.get().start;;
                var xStart = selection.get().start;
                var found = false;

                //find beginning of line so that we can check
                //if the text already exists.
                while (xStart > 0) {
                    var ch = text.substr(xStart, 1);
                    if (ch === "\r" || ch === "\n") {
                        if (text.substr(xStart + 1, textToAdd.length) === textToAdd) {
                            selection.select(xStart + 1, textToAdd.length);
                            selection.replace("");
                        } else {
                            selection.replace(textToAdd);
                        }
                        found = true;
                        break;
                    }
                    xStart = xStart - 1;
                }
                if (!found) {
                    if (text.substr(0, textToAdd.length) === textToAdd) {
                        selection.select(0, textToAdd.length);
                        selection.replace("");
                    } else {
                        selection.select(0, 0);
                        selection.replace(textToAdd);
                    }
                }
                selection.moveCursor(orgPos + textToAdd.length);
                //selection.select(orgPos, 1);
                return;
            }

            var pos = selection.get();
            selection.replace(textToAdd + selection.text());
            selection.select(pos.end + textToAdd.length, pos.end + textToAdd.length);
        }

        private actionH1(selection: TextSelector) {
            this.addTextToBeginningOfLine(selection, "# ");
        }


        private actionH2(selection: TextSelector) {
            this.addTextToBeginningOfLine(selection, "## ");

        }

        private actionH3(selection: TextSelector) {
            this.addTextToBeginningOfLine(selection, "### ");
        }

        private actionBullets(selection: TextSelector) {
            var pos = selection.get();
            selection.replace(`* ${selection.text()}`);
            selection.select(pos.end + 2, pos.end + 2);
        }

        private actionNumbers(selection: TextSelector) {
            this.addTextToBeginningOfLine(selection, "1. ");
        }

        private actionSourcecode(selection: TextSelector) {
            var pos = selection.get();
            if (!selection.isSelected()) {
                selection.replace("> ");
                selection.select(pos.start + 2, pos.start + 2);
                return;
            }

            if (selection.text().indexOf("\n") === -1) {
                selection.replace(`\`${selection.text()}\``);
                selection.select(pos.end + 2, pos.end + 2);
                return;
            }

            var text = `    ${selection.text().replace(/\n/g, "\n    ")}`;
            if (text.substr(text.length - 3, 1) === " " && text.substr(text.length - 1, 1) === " ") {
                text = text.substr(0, text.length - 4);
            }
            selection.replace(text);
            selection.select(pos.start + text.length, pos.start + text.length);
        }

        private actionQuote(selection: TextSelector) {
            var pos = selection.get();
            if (!selection.isSelected()) {
                selection.replace("> ");
                selection.select(pos.start + 2, pos.start + 2);
                return;
            }


            var text = `> ${selection.text().replace(/\n/g, "\n> ")}`;
            if (text.substr(text.length - 3, 1) === " ") {
                text = text.substr(0, text.length - 4);
            }
            selection.replace(text);
            selection.select(pos.start + text.length, pos.start + text.length);
        }

        //context: { url: 'urlToImage' }
        private actionImage(selection: TextSelector, context: IToolbarContext) {
            var pos = selection.get();
            var text = selection.text();
            selection.store();

            var options = {
                editor: context.editor,
                editorElement: context.editorElement,
                selection: selection,
                href: "",
                title: ""
            };

            if (!selection.isSelected()) {
                options.href = "";
                options.title = "";
            } else if (text.substr(-4, 4) === ".png" || text.substr(-4, 4) === ".gif" || text.substr(-4, 4) === ".jpg") {
                options.href = text;
            } else {
                options.title = text;
            }
            context.editor.dialogProvider.image(options, result => {
                var newText = `![${result.title}](${result.href})`;
                selection.load();
                selection.replace(newText);
                selection.select(pos.start + newText.length, pos.start + newText.length);
                context.editor.preview();
            });
        }

        private actionLink(selection: TextSelector, context: IToolbarContext) {
            var pos = selection.get();
            var text = selection.text();
            selection.store();

            var options = {
                editor: context.editor,
                editorElement: context.editorElement,
                selection: selection,
                url: "",
                text: ""
            };

            if (selection.isSelected()) {
                if (text.substr(0, 4) === "http" || text.substr(0, 3) === "www") {
                    options.url = text;
                } else {
                    options.text = text;
                }
            }
            context.editor.dialogProvider.link(options, result => {
                selection.load();
                var newText = `[${result.text}](${result.url})`;
                selection.replace(newText);
                selection.select(pos.start + newText.length, pos.start + newText.length);
                context.editor.preview();
            });
        }
    }


    export class TextSelector {
        element: HTMLTextAreaElement;
        private stored: ITextSelection;

        constructor(elementOrId: any) {
            if (typeof elementOrId === "string") {
                this.element = <HTMLTextAreaElement>document.getElementById(elementOrId);
            } else {
                this.element = elementOrId;
            }
        }

        /** @returns object {start: X, end: Y, length: Z} 
          * x = start character
          * y = end character
          * length: number of selected characters
          */

        get(): ITextSelection {
            if (typeof this.element.selectionStart !== "undefined") {
                return {
                    start: this.element.selectionStart,
                    end: this.element.selectionEnd,
                    length: this.element.selectionEnd - this.element.selectionStart
                };
            }

            var doc = <any>document;
            var range = doc.selection.createRange();
            var storedRange = range.duplicate();
            storedRange.moveToElementText(this.element);
            storedRange.setEndPoint("EndToEnd", range);
            var start = storedRange.text.length - range.text.length;
            var end = start + range.text.length;

            return { start: start, end: end, length: range.text.length };
        }

        /** Replace selected text with the specified one */
        replace(newText: string) {
            if (typeof this.element.selectionStart !== "undefined") {
                this.element.value = this.element.value.substr(0, this.element.selectionStart)
                    + newText
                    + this.element.value.substr(this.element.selectionEnd);
                return this;
            }

            this.element.focus();
            document["selection"].createRange().text = newText;
            return this;
        }


        /** Store current selection */
        store() {
            this.stored = this.get();
        }

        /** load last selection */
        load() {
            this.select(this.stored);
        }

        /** Selected the specified range
         * @param start Start character
         * @param end End character
         */
        select(startOrSelection: any, end?: number) {
            var start = startOrSelection;

            if (typeof startOrSelection.start !== "undefined") {
                end = startOrSelection.end;
                start = startOrSelection.start;
            }
            if (typeof this.element.selectionStart == "number") {
                this.element.selectionStart = start;
                this.element.selectionEnd = end;
            } else if (typeof this.element.setSelectionRange !== "undefined") {
                this.element.focus();
                this.element.setSelectionRange(start, end);
            } else if (typeof this.element.createTextRange !== "undefined") {
                var range = this.element.createTextRange();
                range.collapse(true);
                range.moveEnd("character", end);
                range.moveStart("character", start);
                range.select();
            }

            return this;
        }

        /** @returns if anything is selected */
        isSelected() {
            return this.get().length !== 0;
        }

        /** @returns selected text */
        text() {
            if (typeof document["selection"] !== "undefined") {
                //elem.focus();
                //console.log(document.selection.createRange().text);
                return document["selection"].createRange().text;
            }

            return this.element.value.substr(this.element.selectionStart, this.element.selectionEnd - this.element.selectionStart);
        }

        moveCursor(position: number) {
            if (typeof this.element.selectionStart == "number") {
                this.element.selectionStart = position;
            } else if (typeof this.element.setSelectionRange !== "undefined") {
                this.element.focus();
                this.element.setSelectionRange(position, 0);
            } else if (typeof this.element.createTextRange !== "undefined") {
                var range = this.element.createTextRange();
                range.collapse(true);
                range.moveStart("character", position);
                range.select();
            }

        }
    }
}