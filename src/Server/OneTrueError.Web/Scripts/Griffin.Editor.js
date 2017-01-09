var Griffin;
(function (Griffin) {
    /**
     * The main mother of all editors.
     */
    var Editor = (function () {
        /**
         * Create a new editor
         * @param elementOrId either an HTML id (without hash) or a HTMLTextAreaElement.
         * @param parser Used to transform markdown to HTML (or another language).
         */
        function Editor(elementOrId, parser) {
            this.keyMap = {};
            if (typeof elementOrId === "string") {
                this.containerElement = document.getElementById(elementOrId);
            }
            else {
                this.containerElement = elementOrId;
            }
            this.id = this.containerElement.id;
            var id = this.containerElement.id;
            this.element = (this.containerElement.getElementsByTagName("textarea")[0]);
            this.previewElement = document.getElementById(id + "-preview");
            this.toolbarElement = this.containerElement.getElementsByClassName("toolbar")[0];
            this.textSelector = new TextSelector(this.element);
            this.toolbarHandler = new MarkdownToolbar(parser);
            this.assignAccessKeys();
            if (typeof $().modal == "function") {
                this.dialogProvider = new BoostrapDialogs();
            }
            else {
                this.dialogProvider = new ConfirmDialogs();
                document.getElementById(id + "-imageDialog").style.display = "none";
                document.getElementById(id + "-linkDialog").style.display = "none";
            }
            this.bindEvents();
        }
        Editor.prototype.trimSpaceInSelection = function () {
            var selectedText = this.textSelector.text();
            var pos = this.textSelector.get();
            if (selectedText.substr(selectedText.length - 1, 1) === " ") {
                this.textSelector.select(pos.start, pos.end - 1);
            }
        };
        Editor.prototype.getActionNameFromClass = function (classString) {
            var classNames = classString.split(/\s+/);
            for (var i = 0; i < classNames.length; i++) {
                if (classNames[i].substr(0, 7) === "button-") {
                    return classNames[i].substr(7);
                }
            }
            return null;
        };
        Editor.prototype.assignAccessKeys = function () {
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
        };
        Editor.prototype.invokeAutoSize = function () {
            if (!this.autoSize) {
                return;
            }
            var twin = $(this).data("twin-area");
            if (typeof twin === "undefined") {
                twin = $('<textarea style="position:absolute; top: -10000px"></textarea>');
                twin.appendTo("body");
                //div.appendTo('body');
                $(this).data("twin-area", twin);
                $(this)
                    .data("originalSize", {
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
                };
                $(this.element).css(style);
                $(this).data("expandedSize", style);
            }
        };
        Editor.prototype.bindEvents = function () {
            this.bindToolbarEvents();
            this.bindAccessors();
            this.bindEditorEvents();
        };
        Editor.prototype.bindEditorEvents = function () {
            var self = this;
            this.element.addEventListener("focus", function (e) {
                //grow editor
            });
            this.element.addEventListener("blur", function (e) {
                //shrink editor
            });
            this.element.addEventListener("keyup", function (e) {
                self.preview();
                //self.invokeAutoSize();
            });
            this.element.addEventListener("paste", function (e) {
                setTimeout(function () {
                    self.preview();
                }, 100);
            });
        };
        Editor.prototype.bindToolbarEvents = function () {
            var _this = this;
            var spans = this.toolbarElement.getElementsByTagName("span");
            var len = spans.length;
            var self = this;
            for (var i = 0; i < len; i++) {
                if (spans[i].className.indexOf("button") === -1)
                    continue;
                var button = spans[i];
                button.addEventListener("click", function (e) {
                    var btn = e.target;
                    if (btn.tagName != "span") {
                        btn = e.target.parentElement;
                    }
                    var actionName = self.getActionNameFromClass(btn.className);
                    self.invokeAction(actionName);
                    self.preview();
                    return _this;
                });
            }
        };
        Editor.prototype.bindAccessors = function () {
            var _this = this;
            var self = this;
            //required to override browser keys
            document.addEventListener("keydown", function (e) {
                if (!e.ctrlKey)
                    return;
                var key = String.fromCharCode(e.which);
                if (!key || key.length === 0)
                    return;
                if (e.target !== self.element)
                    return;
                var actionName = _this.keyMap[key];
                if (actionName) {
                    e.cancelBubble = true;
                    e.stopPropagation();
                    e.preventDefault();
                }
            });
            this.element.addEventListener("keyup", function (e) {
                if (!e.ctrlKey)
                    return;
                var key = String.fromCharCode(e.which);
                if (!key || key.length === 0)
                    return;
                var actionName = _this.keyMap[key];
                if (actionName) {
                    _this.invokeAction(actionName);
                    self.preview();
                }
            });
        };
        /**
         * Invoke a toolbar action
         * @param actionName "H1", "B" or similar
         */
        Editor.prototype.invokeAction = function (actionName) {
            if (!actionName || actionName.length === 0)
                throw new Error("ActionName cannot be empty");
            this.trimSpaceInSelection();
            this.toolbarHandler.invokeAction({
                editorElement: this.element,
                editor: this,
                actionName: actionName,
                selection: this.textSelector
            });
        };
        Editor.prototype.getTopPos = function (element) {
            return element.getBoundingClientRect().top +
                window.pageYOffset -
                element.ownerDocument.documentElement.clientTop;
        };
        Editor.prototype.getLeftPos = function (element) {
            return element.getBoundingClientRect().left +
                window.pageXOffset -
                element.ownerDocument.documentElement.clientLeft;
        };
        /**
         * Update the preview window
         */
        Editor.prototype.preview = function () {
            var _this = this;
            if (this.previewElement == null) {
                return;
            }
            this.toolbarHandler.preview(this, this.previewElement, this.element.value);
            if (this.editorTimer) {
                clearTimeout(this.editorTimer);
            }
            if (this.syntaxHighlighter) {
                this.editorTimer = setTimeout(function () {
                    var tags = _this.previewElement.getElementsByTagName("code");
                    var inlineBlocks = [];
                    var codeBlocks = [];
                    for (var i = 0; i < tags.length; i++) {
                        var elem = tags[i];
                        if (elem.parentElement.tagName === "PRE") {
                            codeBlocks.push(elem);
                        }
                        else {
                            inlineBlocks.push(elem);
                        }
                    }
                    _this.syntaxHighlighter.highlight(inlineBlocks, codeBlocks);
                }, 1000);
            }
        };
        return Editor;
    }());
    Griffin.Editor = Editor;
    var ConfirmDialogs = (function () {
        function ConfirmDialogs() {
        }
        ConfirmDialogs.prototype.image = function (context, callback) {
            var url = prompt("Enter image URL", context.selection.text());
            setTimeout(function () {
                callback({
                    href: url,
                    title: "Enter title here"
                });
            });
        };
        ConfirmDialogs.prototype.link = function (context, callback) {
            var url = prompt("Enter URL", context.selection.text());
            setTimeout(function () {
                callback({
                    url: url,
                    text: "Enter title here"
                });
            });
        };
        return ConfirmDialogs;
    }());
    Griffin.ConfirmDialogs = ConfirmDialogs;
    var BoostrapDialogs = (function () {
        function BoostrapDialogs() {
        }
        BoostrapDialogs.prototype.image = function (context, callback) {
            var dialog = $("#" + context.editor.id + "-imageDialog");
            if (!dialog.data("griffin-imageDialog-inited")) {
                dialog.data("griffin-imageDialog-inited", true);
                $("[data-success]", dialog)
                    .click(function () {
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
            dialog.on("shown.bs.modal", function () {
                $('[name="imageUrl"]', dialog).focus();
            });
            dialog.modal({
                show: true
            });
        };
        BoostrapDialogs.prototype.link = function (context, callback) {
            var dialog = $("#" + context.editor.id + "-linkDialog");
            if (!dialog.data("griffin-linkDialog-inited")) {
                dialog.data("griffin-linkDialog-inited", true);
                $("[data-success]", dialog)
                    .click(function () {
                    dialog.modal("hide");
                    callback({
                        url: $('[name="linkUrl"]', dialog).val(),
                        text: $('[name="linkText"]', dialog).val()
                    });
                    context.editorElement.focus();
                });
                dialog.on("shown.bs.modal", function () {
                    $('[name="linkUrl"]', dialog).focus();
                });
                dialog.on("hidden.bs.modal", function () {
                    context.editorElement.focus();
                });
            }
            if (context.selection.isSelected()) {
                $('[name="linkText"]', dialog).val(context.selection.text());
            }
            dialog.modal({
                show: true
            });
        };
        return BoostrapDialogs;
    }());
    Griffin.BoostrapDialogs = BoostrapDialogs;
    var MarkdownToolbar = (function () {
        function MarkdownToolbar(parser) {
            this.parser = parser;
        }
        MarkdownToolbar.prototype.invokeAction = function (context) {
            //			console.log(griffinEditor);
            var method = "action" + context.actionName.substr(0, 1).toUpperCase() + context.actionName.substr(1);
            if (this[method]) {
                var args = [];
                args[0] = context.selection;
                args[1] = context;
                return this[method].apply(this, args);
            }
            else {
                if (typeof alert !== "undefined") {
                    alert("Missing " + method + " in the active textHandler (griffinEditorExtension)");
                }
            }
            return this;
        };
        MarkdownToolbar.prototype.preview = function (editor, preview, contents) {
            if (contents === null || typeof contents === "undefined") {
                throw new Error("May not be called without actual content.");
            }
            preview.innerHTML = this.parser.parse(contents);
        };
        MarkdownToolbar.prototype.removeWrapping = function (selection, wrapperString) {
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
        };
        MarkdownToolbar.prototype.actionBold = function (selection) {
            var isSelected = selection.isSelected();
            var pos = selection.get();
            if (this.removeWrapping(selection, "**")) {
                return this;
            }
            selection.replace("**" + selection.text() + "**");
            if (isSelected) {
                selection.select(pos.start, pos.end + 4);
            }
            else {
                selection.select(pos.start + 2, pos.start + 2);
            }
            return this;
        };
        MarkdownToolbar.prototype.actionItalic = function (selection) {
            var isSelected = selection.isSelected();
            var pos = selection.get();
            if (this.removeWrapping(selection, "_")) {
                return this;
            }
            selection.replace("_" + selection.text() + "_");
            if (isSelected) {
                selection.select(pos.start, pos.end + 2);
            }
            else {
                selection.select(pos.start + 1, pos.start + 1);
            }
            return this;
        };
        MarkdownToolbar.prototype.addTextToBeginningOfLine = function (selection, textToAdd) {
            var isSelected = selection.isSelected();
            if (!isSelected) {
                var text = selection.element.value;
                var orgPos = selection.get().start;
                ;
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
                        }
                        else {
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
                    }
                    else {
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
        };
        MarkdownToolbar.prototype.actionH1 = function (selection) {
            this.addTextToBeginningOfLine(selection, "# ");
        };
        MarkdownToolbar.prototype.actionH2 = function (selection) {
            this.addTextToBeginningOfLine(selection, "## ");
        };
        MarkdownToolbar.prototype.actionH3 = function (selection) {
            this.addTextToBeginningOfLine(selection, "### ");
        };
        MarkdownToolbar.prototype.actionBullets = function (selection) {
            var pos = selection.get();
            selection.replace("* " + selection.text());
            selection.select(pos.end + 2, pos.end + 2);
        };
        MarkdownToolbar.prototype.actionNumbers = function (selection) {
            this.addTextToBeginningOfLine(selection, "1. ");
        };
        MarkdownToolbar.prototype.actionSourcecode = function (selection) {
            var pos = selection.get();
            if (!selection.isSelected()) {
                selection.replace("> ");
                selection.select(pos.start + 2, pos.start + 2);
                return;
            }
            if (selection.text().indexOf("\n") === -1) {
                selection.replace("`" + selection.text() + "`");
                selection.select(pos.end + 2, pos.end + 2);
                return;
            }
            var text = "    " + selection.text().replace(/\n/g, "\n    ");
            if (text.substr(text.length - 3, 1) === " " && text.substr(text.length - 1, 1) === " ") {
                text = text.substr(0, text.length - 4);
            }
            selection.replace(text);
            selection.select(pos.start + text.length, pos.start + text.length);
        };
        MarkdownToolbar.prototype.actionQuote = function (selection) {
            var pos = selection.get();
            if (!selection.isSelected()) {
                selection.replace("> ");
                selection.select(pos.start + 2, pos.start + 2);
                return;
            }
            var text = "> " + selection.text().replace(/\n/g, "\n> ");
            if (text.substr(text.length - 3, 1) === " ") {
                text = text.substr(0, text.length - 4);
            }
            selection.replace(text);
            selection.select(pos.start + text.length, pos.start + text.length);
        };
        //context: { url: 'urlToImage' }
        MarkdownToolbar.prototype.actionImage = function (selection, context) {
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
            }
            else if (text
                .substr(-4, 4) ===
                ".png" ||
                text.substr(-4, 4) === ".gif" ||
                text.substr(-4, 4) === ".jpg") {
                options.href = text;
            }
            else {
                options.title = text;
            }
            context.editor.dialogProvider.image(options, function (result) {
                var newText = "![" + result.title + "](" + result.href + ")";
                selection.load();
                selection.replace(newText);
                selection.select(pos.start + newText.length, pos.start + newText.length);
                context.editor.preview();
            });
        };
        MarkdownToolbar.prototype.actionLink = function (selection, context) {
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
                }
                else {
                    options.text = text;
                }
            }
            context.editor.dialogProvider.link(options, function (result) {
                selection.load();
                var newText = "[" + result.text + "](" + result.url + ")";
                selection.replace(newText);
                selection.select(pos.start + newText.length, pos.start + newText.length);
                context.editor.preview();
            });
        };
        return MarkdownToolbar;
    }());
    Griffin.MarkdownToolbar = MarkdownToolbar;
    var TextSelector = (function () {
        function TextSelector(elementOrId) {
            if (typeof elementOrId === "string") {
                this.element = document.getElementById(elementOrId);
            }
            else {
                this.element = elementOrId;
            }
        }
        /** @returns object {start: X, end: Y, length: Z}
          * x = start character
          * y = end character
          * length: number of selected characters
          */
        TextSelector.prototype.get = function () {
            if (typeof this.element.selectionStart !== "undefined") {
                return {
                    start: this.element.selectionStart,
                    end: this.element.selectionEnd,
                    length: this.element.selectionEnd - this.element.selectionStart
                };
            }
            var doc = document;
            var range = doc.selection.createRange();
            var storedRange = range.duplicate();
            storedRange.moveToElementText(this.element);
            storedRange.setEndPoint("EndToEnd", range);
            var start = storedRange.text.length - range.text.length;
            var end = start + range.text.length;
            return { start: start, end: end, length: range.text.length };
        };
        /** Replace selected text with the specified one */
        TextSelector.prototype.replace = function (newText) {
            if (typeof this.element.selectionStart !== "undefined") {
                this.element.value = this.element.value.substr(0, this.element.selectionStart) +
                    newText +
                    this.element.value.substr(this.element.selectionEnd);
                return this;
            }
            this.element.focus();
            document["selection"].createRange().text = newText;
            return this;
        };
        /** Store current selection */
        TextSelector.prototype.store = function () {
            this.stored = this.get();
        };
        /** load last selection */
        TextSelector.prototype.load = function () {
            this.select(this.stored);
        };
        /** Selected the specified range
         * @param start Start character
         * @param end End character
         */
        TextSelector.prototype.select = function (startOrSelection, end) {
            var start = startOrSelection;
            if (typeof startOrSelection.start !== "undefined") {
                end = startOrSelection.end;
                start = startOrSelection.start;
            }
            if (typeof this.element.selectionStart == "number") {
                this.element.selectionStart = start;
                this.element.selectionEnd = end;
            }
            else if (typeof this.element.setSelectionRange !== "undefined") {
                this.element.focus();
                this.element.setSelectionRange(start, end);
            }
            else if (typeof this.element.createTextRange !== "undefined") {
                var range = this.element.createTextRange();
                range.collapse(true);
                range.moveEnd("character", end);
                range.moveStart("character", start);
                range.select();
            }
            return this;
        };
        /** @returns if anything is selected */
        TextSelector.prototype.isSelected = function () {
            return this.get().length !== 0;
        };
        /** @returns selected text */
        TextSelector.prototype.text = function () {
            if (typeof document["selection"] !== "undefined") {
                //elem.focus();
                //console.log(document.selection.createRange().text);
                return document["selection"].createRange().text;
            }
            return this.element.value.substr(this.element.selectionStart, this.element.selectionEnd - this.element.selectionStart);
        };
        TextSelector.prototype.moveCursor = function (position) {
            if (typeof this.element.selectionStart == "number") {
                this.element.selectionStart = position;
            }
            else if (typeof this.element.setSelectionRange !== "undefined") {
                this.element.focus();
                this.element.setSelectionRange(position, 0);
            }
            else if (typeof this.element.createTextRange !== "undefined") {
                var range = this.element.createTextRange();
                range.collapse(true);
                range.moveStart("character", position);
                range.select();
            }
        };
        return TextSelector;
    }());
    Griffin.TextSelector = TextSelector;
})(Griffin || (Griffin = {}));
//# sourceMappingURL=Griffin.Editor.js.map