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

        return node.getBoundingClientRect().height;
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

        return node.getAttribute('href');
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

Object.defineProperty(String.prototype, "getStarRating", {
    value: function getStarRating(className, fullName, halfName) {
        var node = getNode(this);

        if (node == null) {
            return ``;
        }

        return "star rating";
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

        return node.getBoundingClientRect().width;
    },
    writable: true,
    configurable: true,
});