globalThis.liquidata_is_selection_mode = false;

function highlightElement(element) {
    if (element === null) {
        return;
    }

    removeOldHighlights();

    try {
        element.classList.add('liquidata_selector_highlight');
    }    
    catch (error) {
        // Nothing
    }
}

function removeCurrentHighlighting(element) {
    if (element === null) {
        return;
    }

    try {
        element.classList.remove('liquidata_selector_highlight');
    }
    catch (error) {
        // Nothing
    }
}

function removeOldHighlights() {
    try {
        var allHighlighted = LD_Document.querySelectorAll(".liquidata_selector_highlight");
        for (i = 0; i < allHighlighted.length; i++) {
            removeCurrentHighlighting(allHighlighted[i]);
        }
    }
    catch (error) {
        // Nothing
    }
}

LD_Document.addEventListener('mousemove', function (e) {
    if (!globalThis.liquidata_is_selection_mode) {
        return;
    }

    var currentElement = LD_Document.elementFromPoint(e.clientX, e.clientY);
    highlightElement(currentElement);    
}, true);

LD_Document.addEventListener('mouseleave', function (e) {
    var target = e.target;
    removeCurrentHighlighting(target);
}, true);