<img style="height: 125px;" src="src/Liquidata.Client/wwwroot/icon.png" />

# Liquidata, an open source visual data extractor
Liquiddata is a free and open source web based data extraction system that can turn any website into structured data. This can be done with little to no knowledge of HTML or javacript, through a simple point and click web based system. Projects can then be reused and run manually or through a schedule, as needed.

## Capabilities
Liquidata allows for a data to be extracted through a sequence of actions. Each action represents a specific operation to perform during the execution of the project. At the end of execution then the extracted data can be visualized or exported in a json form which can be used elsewhere for specific purposes.

The actions themselves are added and configured through the use of the <a href="">Liquidata Client</a> application. The actions are grouped together in a reusable form in a project that represents the overall data extraction tasks to perform.

### Overview
- Simple point and click application
- Data can be extracted with no knowledge of HTML or Javascript
- Images, text, and links can be extracted into a structured form
- Automatically can select similar items to simplify structured extraction
- Extraction can be run locally or in the cloud

# <a href="/docs/Tutorial/00-Introduction.md;">Get Started</a>

## Client
<a href="">Liquidata Client</a> is the heart of the system. This is the application that is used to build the projects for each data extraction need. Each extractor consists of a single project, which in turn consists of one or more templates. The <strong>main</strong> template is automatically the entrypoint for the extractor and acts as the starting point for everything.

Each template consists of a series of actions that define the options to perform and the data to be extracted. In most cases the <strong>main</strong> template is sufficent. However in advanced cases complicated sites may need multiple templates, where each one has a specific purpose. Each action can then be configured and customized for the project's specific needs.

### Emporium
<a href="">Liquidata Emporium</a> is a simple fake ecommerce website that can be used to test the many capabilities of Liquidata. All the information is randomly generated the first time the site is loaded and then reused. This means the Emporium can be used as many times as needed to perfect data extraction without fear of being blocked, or getting the dreaded CAPTCHAs.

### Offscreen
<a href="">Liquidata Offscreen</a> is a way to run the Liquidata projects in the background so that projects can be repeatedly run without user interaction or on a schedule. This greatly simplifies the case where data is needed more than a single time or where complicated projects take some time to run.

Liquidata Offscreen uses a <a href="https://en.wikipedia.org/wiki/Headless_browser">headless browser</a> to execute the project exactly as a user would in a normal web browser. This avoids the web security limitions that Liquidata Client has, due to how the Client visualizes other websites for extraction. 

### Cloud
<a href="">Liquidata Cloud</a> is similar to Liquidata Offscreen, however projects can be run and scheduled in the cloud which means local resource limitations are not a concern anymore. The cloud offers the simpliest way to execute projects without a need for worrying about things directly.

Through the cloud, projects can also be run in parallel and massively scaled to ensure that complicated projects can be executed quickly and efficently. All extracted data can then be downloaded locally or visualized directly in the cloud.

## Actions
An action is a single building block that performs a specific operation. Multiple actions can be used together to perform complicated tasks or to extract whatever data is needed for specific projects.

### Selection
<div><a href="docs/Actions/Select.md">Select</a></div>
<div><a href="docs/Actions/RelativeSelect.md">Relative Select</a></div>

### Data
<div><a href="docs/Actions/BeginRecord.md">Begin Record</a></div>
<div><a href="docs/Actions/Extract.md">Extract</a></div>
<div><a href="docs/Actions/Log.md">Log</a></div>
<div><a href="docs/Actions/Scope.md">Scope</a></div>
<div><a href="docs/Actions/ScreenCapture.md">Screen Capture</a></div>
<div><a href="docs/Actions/Store.md">Store</a></div>

### Logic
<div><a href="docs/Actions/Conditional.md">Conditional</a></div>
<div><a href="docs/Actions/ExecuteTemplate.md">Execute Template</a></div>
<div><a href="docs/Actions/Foreach.md">Foreach</a></div>
<div><a href="docs/Actions/Loop.md">Loop</a></div>

### Interaction
<div><a href="docs/Actions/Click.md">Click</a></div>
<div><a href="docs/Actions/ExecuteScript.md">Execute Script</a></div>
<div><a href="docs/Actions/Hover.md">Hover</a></div>
<div><a href="docs/Actions/Input.md">Input</a></div>
<div><a href="docs/Actions/Keypress.md">Keypress</a></div>
<div><a href="docs/Actions/Reload.md">Reload</a></div>
<div><a href="docs/Actions/Scroll.md">Scroll</a></div>
<div><a href="docs/Actions/SolveCaptcha.md">Solve Captcha</a></div>
<div><a href="docs/Actions/Stop.md">Stop</a></div>
<div><a href="docs/Actions/StopIf.md">Stop If</a></div>
<div><a href="docs/Actions/Wait.md">Wait</a></div>