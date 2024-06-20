function getNode(xpath) {
    var node = LD_Document.evaluate(xpath, LD_Document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue;

    if (node == null || node == undefined) {
        return null;
    }

    return node;
}

Object.defineProperty(String.prototype, "getAttr", {
    value: function getAttr(attr) {
        var node = getNode(this);

        if (node == null) {
            return ``;
        }

        return node.getAttribute(attr);
    },
    writable: true,
    configurable: true,
});

Object.defineProperty(String.prototype, "getHeight", {
    value: function getHeight() {
        var node = getNode(this);

        if (node == null) {
            return ``;
        }

        return node.getBoundingClientRect().height.toString();
    },
    writable: true,
    configurable: true,
});

Object.defineProperty(String.prototype, "getLink", {
    value: function getLink() {
        var node = getNode(this);

        if (node == null) {
            return ``;
        }

        var startNode = node;

        while (node != null) {
            if (node.getAttribute('href') != null) {
                return node.getAttribute('href');
            }        

            if (node.getAttribute('data-filter-url') != null) {
                return node.getAttribute('data-filter-url');
            } 

            node = node.parentElement;
        }

        var searchNodes = [startNode];

        while (searchNodes.length > 0) {
            var current = searchNodes.pop();

            if (current.getAttribute('href') != null) {
                return current.getAttribute('href');
            }

            if (current.getAttribute('data-filter-url') != null) {
                return current.getAttribute('data-filter-url');
            } 

            for (let child of current.children) {
                searchNodes.push(child);
            }
        }

        return ``;
    },
    writable: true,
    configurable: true,
});

Object.defineProperty(String.prototype, "getPageUrl", {
    value: function getPageUrl() {
        var node = getNode(this);

        if (node == null) {
            return ``;
        }

        return window.location.href;
    },
    writable: true,
    configurable: true,
});

Object.defineProperty(String.prototype, "getText", {
    value: function getText() {
        var node = getNode(this);

        if (node == null) {
            return ``;
        }

        return node.innerText;
    },
    writable: true,
    configurable: true,
});

Object.defineProperty(String.prototype, "getTime", {
    value: function getTime() {
        var node = getNode(this);

        if (node == null) {
            return ``;
        }

        return new Date().toISOString();
    },
    writable: true,
    configurable: true,
});

Object.defineProperty(String.prototype, "getWidth", {
    value: function getWidth() {
        var node = getNode(this);

        if (node == null) {
            return ``;
        }

        return node.getBoundingClientRect().width.toString();
    },
    writable: true,
    configurable: true,
});

Object.defineProperty(String.prototype, "getDisabled", {
    value: function getDisabled() {
        var node = getNode(this);

        if (node == null) {
            return `false`;
        }

        return (!!(node.disabled)).toString();
    },
    writable: true,
    configurable: true,
});

Object.defineProperty(String.prototype, "click", {
    value: function click(button, shift, ctrl, alt) {
        var node = getNode(this);

        if (node == null) {
            return `false`;
        }

        var mouseEvent = new MouseEvent('click', {
            'view': LD_Document.defaultView,
            'button': button,
            'shiftKey': shift,
            'ctrlKey': ctrl,
            'altKey': alt,
            'bubbles': true,
            'cancelable': true
        });

        node.dispatchEvent(mouseEvent);
        return `true`;
    },
    writable: true,
    configurable: true,
});

Object.defineProperty(String.prototype, "dblclick", {
    value: function dblclick(button, shift, ctrl, alt) {
        var node = getNode(this);

        if (node == null) {
            return `false`;
        }

        var mouseEvent = new MouseEvent('dblclick', {
            'view': LD_Document.defaultView,
            'button': button,
            'shiftKey': shift,
            'ctrlKey': ctrl,
            'altKey': alt,
            'bubbles': true,
            'cancelable': true
        });

        node.dispatchEvent(mouseEvent);
        return `true`;
    },
    writable: true,
    configurable: true,
});

Object.defineProperty(String.prototype, "hover", {
    value: function hover() {
        var node = getNode(this);

        if (node == null) {
            return `false`;
        }

        var event = new MouseEvent('mouseover', {
            'view': LD_Document.defaultView,
            'bubbles': true,
            'cancelable': true
        });

        node.dispatchEvent(event);
        return `true`;
    },
    writable: true,
    configurable: true,
});

Object.defineProperty(String.prototype, "input", {
    value: function input(value) {
        var node = getNode(this);

        if (node == null) {
            return `false`;
        }

        node.input = value;
        return `true`;
    },
    writable: true,
    configurable: true,
});

Object.defineProperty(String.prototype, "keypress", {
    value: function keypress(key, shift, ctrl, alt) {
        var node = getNode(this);

        if (node == null) {
            return `false`;
        }

        var kdEvent = new KeyboardEvent('keydown', {
            'key': key,
            'shiftKey': shift,
            'ctrlKey': ctrl,
            'altKey': alt,
        });

        var kuEvent = new KeyboardEvent('keyup', {
            'key': key,
            'shiftKey': shift,
            'ctrlKey': ctrl,
            'altKey': alt,
        });

        node.dispatchEvent(kdEvent);
        node.dispatchEvent(kuEvent);
        return `true`;
    },
    writable: true,
    configurable: true,
});