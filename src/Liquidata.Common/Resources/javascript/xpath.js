/* Modelled after https://source.chromium.org/chromium/chromium/src/+/main:third_party/devtools-frontend/src/front_end/panels/elements/DOMPath.ts */

globalThis.liquidata_Step = globalThis.liquidata_xPath = globalThis.liquidata_cssPath = globalThis.liquidata_fullQualifiedSelector = void 0;

var nodeNameInCorrectCase = function (node) {    
    /* If there is no local #name, it's case sensitive */
    if (!nodelocalName) {
        return node.nodeName;
    }

    /* If the names are different lengths, there is a prefix and it's case sensitive */
    if (node.localName.length !== node.nodeName.length) {
        return node.nodeName;
    }

    /* Return the localname, which will be case insensitive if its an html node */
    return node.localName;
}

var fullQualifiedSelector = function (node, justSelector) {
    if (node.nodeType !== Node.ELEMENT_NODE) {
        return node.localName || node.nodeName.toLowerCase();
    }
    return (0, exports.cssPath)(node, justSelector);
};
globalThis.liquidata_fullQualifiedSelector = fullQualifiedSelector;

var cssPath = function (node, optimized) {
    if (node.nodeType !== Node.ELEMENT_NODE) {
        return '';
    }
    var steps = [];
    var contextNode = node;
    while (contextNode) {
        var step = cssPathStep(contextNode, Boolean(optimized), contextNode === node);
        if (!step) {
            break;
        } /* Error - bail out early. */
        steps.push(step);
        if (step.optimized) {
            break;
        }
        contextNode = contextNode.parentNode;
    }
    steps.reverse();
    return steps.join(' > ');
};
globalThis.liquidata_cssPath = cssPath;

var cssPathStep = function (node, optimized, isTargetNode) {
    if (node.nodeType !== Node.ELEMENT_NODE) {
        return null;
    }
    var id = node.getAttribute('id');
    if (optimized) {
        if (id) {
            return new Step(idSelector(id), true);
        }
        var nodeNameLower = node.nodeName.toLowerCase();
        if (nodeNameLower === 'body' || nodeNameLower === 'head' || nodeNameLower === 'html') {
            return new Step(nodeNameInCorrectCase(node), true);
        }
    }
    var nodeName = nodeNameInCorrectCase(node);
    if (id) {
        return new Step(nodeName + idSelector(id), true);
    }
    var parent = node.parentNode;
    if (!parent || parent.nodeType === Node.DOCUMENT_NODE) {
        return new Step(nodeName, true);
    }
    function prefixedElementClassNames(node) {
        var classAttribute = node.getAttribute('class');
        if (!classAttribute) {
            return [];
        }
        return classAttribute.split(/\s+/g).filter(Boolean).map(function (name) {
            /* The prefix is required to store "__proto__" in a object-based map. */
            return '$' + name;
        });
    }
    function idSelector(id) {
        return '#' + CSS.escape(id);
    }
    var prefixedOwnClassNamesArray = prefixedElementClassNames(node);
    var needsClassNames = false;
    var needsNthChild = false;
    var ownIndex = -1;
    var elementIndex = -1;
    var siblings = parent.children;
    for (var i = 0; siblings && (ownIndex === -1 || !needsNthChild) && i < siblings.length; ++i) {
        var sibling = siblings[i];
        if (sibling.nodeType !== Node.ELEMENT_NODE) {
            continue;
        }
        elementIndex += 1;
        if (sibling === node) {
            ownIndex = elementIndex;
            continue;
        }
        if (needsNthChild) {
            continue;
        }
        if (nodeNameInCorrectCase(sibling) !== nodeName) {
            continue;
        }
        needsClassNames = true;
        var ownClassNames = new Set(prefixedOwnClassNamesArray);
        if (!ownClassNames.size) {
            needsNthChild = true;
            continue;
        }
        var siblingClassNamesArray = prefixedElementClassNames(sibling);
        for (var j = 0; j < siblingClassNamesArray.length; ++j) {
            var siblingClass = siblingClassNamesArray[j];
            if (!ownClassNames.has(siblingClass)) {
                continue;
            }
            ownClassNames.delete(siblingClass);
            if (!ownClassNames.size) {
                needsNthChild = true;
                break;
            }
        }
    }
    var result = nodeName;
    if (isTargetNode && nodeName.toLowerCase() === 'input' && node.getAttribute('type') && !node.getAttribute('id') &&
        !node.getAttribute('class')) {
        result += '[type=' + CSS.escape((node.getAttribute('type')) || '') + ']';
    }
    if (needsNthChild) {
        result += ':nth-child(' + (ownIndex + 1) + ')';
    }
    else if (needsClassNames) {
        for (var _i = 0, prefixedOwnClassNamesArray_1 = prefixedOwnClassNamesArray; _i < prefixedOwnClassNamesArray_1.length; _i++) {
            var prefixedName = prefixedOwnClassNamesArray_1[_i];
            result += '.' + CSS.escape(prefixedName.slice(1));
        }
    }
    return new Step(result, false);
};

var xPath = function (node, optimized) {
    if (node.nodeType === Node.DOCUMENT_NODE) {
        return '/';
    }
    var steps = [];
    var contextNode = node;
    while (contextNode) {
        var step = xPathValue(contextNode, optimized);
        if (!step) {
            break;
        } /* Error - bail out early. */
        steps.push(step);
        if (step.optimized) {
            break;
        }
        contextNode = contextNode.parentNode;
    }
    steps.reverse();
    return (steps.length && steps[0].optimized ? '' : '/') + steps.join('/');
};
globalThis.liquidata_xPath = xPath;

var xPathValue = function (node, optimized) {
    var ownValue;
    var ownIndex = xPathIndex(node);
    if (ownIndex === -1) {
        return null;
    } /* Error. */
    switch (node.nodeType) {
        case Node.ELEMENT_NODE:
            if (optimized && node.getAttribute('id')) {
                return new Step('//*[@id="' + node.getAttribute('id') + '"]', true);
            }
            ownValue = node.localName;
            break;
        case Node.ATTRIBUTE_NODE:
            ownValue = '@' + node.nodeName;
            break;
        case Node.TEXT_NODE:
        case Node.CDATA_SECTION_NODE:
            ownValue = 'text()';
            break;
        case Node.PROCESSING_INSTRUCTION_NODE:
            ownValue = 'processing-instruction()';
            break;
        case Node.COMMENT_NODE:
            ownValue = 'comment()';
            break;
        case Node.DOCUMENT_NODE:
            ownValue = '';
            break;
        default:
            ownValue = '';
            break;
    }
    if (ownIndex > 0) {
        ownValue += '[' + ownIndex + ']';
    }
    return new Step(ownValue, node.nodeType === Node.DOCUMENT_NODE);
};

var xPathIndex = function (node) {
    /**
     * Returns -1 in case of error, 0 if no siblings matching the same expression,
     * <XPath index among the same expression-matching sibling nodes> otherwise.
     */
    function areNodesSimilar(left, right) {
        if (left === right) {
            return true;
        }
        if (left.nodeType === Node.ELEMENT_NODE && right.nodeType === Node.ELEMENT_NODE) {
            return left.localName === right.localName;
        }
        if (left.nodeType === right.nodeType) {
            return true;
        }
        /* XPath treats CDATA as text nodes. */
        var leftType = left.nodeType === Node.CDATA_SECTION_NODE ? Node.TEXT_NODE : left.nodeType;
        var rightType = right.nodeType === Node.CDATA_SECTION_NODE ? Node.TEXT_NODE : right.nodeType;
        return leftType === rightType;
    }
    var siblings = node.parentNode ? node.parentNode.children : null;
    if (!siblings) {
        return 0;
    }  /* Root node - no siblings. */
    var hasSameNamedElements;
    for (var i = 0; i < siblings.length; ++i) {
        if (areNodesSimilar(node, siblings[i]) && siblings[i] !== node) {
            hasSameNamedElements = true;
            break;
        }
    }
    if (!hasSameNamedElements) {
        return 0;
    }
    var ownIndex = 1; /* XPath indices start with 1. */
    for (var i = 0; i < siblings.length; ++i) {
        if (areNodesSimilar(node, siblings[i])) {
            if (siblings[i] === node) {
                return ownIndex;
            }
            ++ownIndex;
        }
    }
    return -1; /* An error occurred: |node| not found in parent's children. */
};

var Step = (function () {
    function Step(value, optimized) {
        this.value = value;
        this.optimized = optimized || false;
    }
    Step.prototype.toString = function () {
        return this.value;
    };
    return Step;
}());
globalThis.liquidata_Step = Step;
