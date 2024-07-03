// Import for the side effect of defining a global 'browser' variable
import * as _ from "/content/Blazor.BrowserExtension/lib/browser-polyfill.min.js";

// Import for the netrequest api
import "/data/scripts/netrequest.js";

browser.runtime.onInstalled.addListener(() => {
    console.log("Extension installed");

    const indexPageUrl = browser.runtime.getURL("index.html");
    browser.tabs.create({
        url: indexPageUrl
    });
});

browser.runtime.onStartup.addListener(() => {
    console.log("Extension started");

    const domains = ['telavian.github.io', 'localhost'];

    // Chromium
    app.netrequest.rules.push({
        "condition": {
            "urlFilter": '*',
            "initiatorDomains": domains, 
        },
        "action": {
            "type": "modifyHeaders",
            "responseHeaders": [{
                "operation": "remove",
                "header": "X-Frame-Options"
            }]
        }
    });

    app.netrequest.rules.push({
        "condition": {
            "urlFilter": '*',
            "initiatorDomains": domains, 
        },
        "action": {
            "type": "modifyHeaders",
            "responseHeaders": [{
                "operation": "remove",
                "header": "Frame-Options"
            }]
        }
    });
});