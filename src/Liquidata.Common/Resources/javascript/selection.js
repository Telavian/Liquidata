const liquidataBrowseMode = "browse";
const liquidataSelectionMode = "selection";
const liquidataRelativeSelectionMode = "relativeSelection";
globalThis.liquidata_selection_mode = liquidataBrowseMode;

function highlightElement(element, name) {
    if (element === null || element.classList === null) {
        return;
    }

    try {
        element.classList.add(name);
    }
    catch (error) {
        // Nothing
    }
}

function removeHighlighting(element, name) {
    if (element === null || element.classList === null) {
        return;
    }

    try {
        element.classList.remove(name);
    }
    catch (error) {
        // Nothing
    }
}

function removeHighlight(name) {
    try {
        var allHighlighted = LD_Document.querySelectorAll(`.${name}`);
        for (i = 0; i < allHighlighted.length; i++) {
            removeHighlighting(allHighlighted[i], name);
        }
    }
    catch (error) {
        // Nothing
    }
}

var removeAllSelectionHighlights = function () {
    removeHighlight('liquidata_selected');
    removeHighlight('liquidata_relative');
    removeHighlight('liquidata_relative_parent');
}
globalThis.liquidata_removeAllSelectionHighlights = removeAllSelectionHighlights;

var highlightSelection = function (xpath, cssClass) {
    var nodes = LD_Document.evaluate(xpath, LD_Document, null, XPathResult.ORDERED_NODE_SNAPSHOT_TYPE, null);

    for (var index = 0; index < nodes.snapshotLength; index++) {
        var current = nodes.snapshotItem(index);
        highlightElement(current, cssClass);
    }
}
globalThis.liquidata_highlightSelection = highlightSelection;

var getSelectionDetails = function (xpath) {
    var node = LD_Document.evaluate(xpath, LD_Document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue;

    if (node == null || node == undefined) {
        return "";
    }

    var fullPath = globalThis.liquidata_xPath(node, false);
    var text = node.innerText;
    var attributes = [];

    for (var index = 0; index < node.attributes.length; index++) {
        var current = node.attributes[index];
        attributes.push(`${current.name}:${current.value}`)
    }

    var result = { FullPath: fullPath, Text: text, Attributes: attributes };
    return JSON.stringify(result);
}
globalThis.liquidata_getSelectionDetails = getSelectionDetails;

var getXPathMatches = function (xpath) {
    var matches = [];

    var nodes = LD_Document.evaluate(xpath, LD_Document, null, XPathResult.ORDERED_NODE_SNAPSHOT_TYPE, null);

    for (var index = 0; index < nodes.snapshotLength; index++) {
        var current = nodes.snapshotItem(index);
        var xpath = globalThis.liquidata_xPath(current, true);
        matches.push(xpath);
    }

    return JSON.stringify(matches);
}
globalThis.liquidata_getXPathMatches = getXPathMatches;

function selectorHighlight(name, e) {
    removeHighlight(name);

    var current = LD_Document.elementFromPoint(e.clientX, e.clientY);
    highlightElement(current, name);
}

LD_Document.addEventListener('mousemove', function (e) {
    if (globalThis.liquidata_selection_mode == liquidataBrowseMode) {
        return;
    }

    if (globalThis.liquidata_selection_mode == liquidataSelectionMode) {
        selectorHighlight('liquidata_selector_highlight', e);
    }
    else if (globalThis.liquidata_selection_mode == liquidataRelativeSelectionMode) {
        selectorHighlight('liquidata_relative_selector_highlight', e);
    }
}, true);

LD_Document.addEventListener('mouseleave', function (e) {
    var target = e.target;
    removeHighlighting(target, 'liquidata_selector_highlight');
    removeHighlighting(target, 'liquidata_relative_selector_highlight');
}, true);

LD_Document.addEventListener('click', function (e) {
    if (globalThis.liquidata_selection_mode == liquidataBrowseMode) {
        return;
    }

    e.stopPropagation();
    e.preventDefault();

    var current = LD_Document.elementFromPoint(e.clientX, e.clientY);
    var xpath = globalThis.liquidata_xPath(current, true);
    var isShift = e.shiftKey;
    var result = JSON.stringify({ XPath: xpath, IsShiftKey: isShift });

    DotNet.invokeMethodAsync('Liquidata.Client', 'ProcessSelectedItemAsync', result);
}, true);