globalThis.liquidata_is_selection_mode = false;

function highlightElement(element, name) {
    if (element === null) {
        return;
    }

    removeHighlight(name);

    try {
        element.classList.add(name);
    }    
    catch (error) {
        // Nothing
    }
}

function removeHighlighting(element, name) {
    if (element === null) {
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

var removeAllSelectionHighlights = function() {
    removeHighlight('liquidata_selected');
    removeHighlight('liquidata_selected_similar');
    removeHighlight('liquidata_relative');
    removeHighlight('liquidata_relative_similar');
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

LD_Document.addEventListener('mousemove', function (e) {
    if (!globalThis.liquidata_is_selection_mode) {
        return;
    }

    var current = LD_Document.elementFromPoint(e.clientX, e.clientY);
    highlightElement(current, 'liquidata_selector_highlight');
}, true);

LD_Document.addEventListener('mouseleave', function (e) {
    var target = e.target;
    removeHighlighting(target, 'liquidata_selector_highlight');
}, true);

LD_Document.addEventListener('click', function (e) {
    if (!globalThis.liquidata_is_selection_mode) {
        return;
    }

    e.stopPropagation();
    e.preventDefault();

    var current = LD_Document.elementFromPoint(e.clientX, e.clientY);
    var xpath = globalThis.liquidata_xPath(current, true);

    DotNet.invokeMethodAsync('Liquidata.Client', 'ProcessSelectedItemAsync', xpath);
}, true);