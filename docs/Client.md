# Liquidata Client

The Client is an application that can be used to create and test data extraction projects. An unlimited amount of projects can be created and used. However, it should be noted that unless a project is exported, to disk or the cloud, then it exists entirely within the local storage of the browser and can be accidently cleared if the user is not careful. 

### <a href="https://telavian.github.io/Liquidata">Liquidata</a>

### Project
A project acts as a collection of templates, collection settings, and a starting web site location. It conveniently defines the entire collection operation and is used to extract a specific set of data. A project can include any number of templates, however it must contain at least one template named `main` which acts as the entry point for the collection operation.

#### Settings
| Setting      | Mode | Description |
| ---          | ---  | --- |
| Name         | Client/Offscreen | The name of the extraction project |
| Url          | Client/Offscreen | The starting url for the data extraction operation. This is the entry point location. |
| Load Images  | Offscreen | Indicates whether to load images. Images can be disabled in order to speed up loading and save bandwidth. |
| Rotate IP Addresses  | Offscreen | Indicates whether rotate IP addresses during data collection. For this to work, then a proxy address would need to be used. |
| Concurrency  | Client/Offscreen | Defines the max concurrency to use while executing the project for data collection. |

### Template
A template is a set of actions that can be reused as needed. For instance, a template can be a focused set of operations to simplify complicated extraction needed or it can be a simple block of actions that are repeated looped over. It entirely depends on the user design style and the complexity of the site.

#### Settings
| Setting      | Description |
| ---          | --- |
| Name         | The name of the template. This is the name the template will be referenced by. |
| Url          | A helpful starting url for the template. This can be helpful when the template is used to extract data from a location that is not the entry page for the operation. For instance, a detail page or a sub page of the site. |

### Execution
Execution of the project is very straight forward and generally follows a linear format. Starting with the main template each action will be executed and then if the action has children then those will be executed. After an actions children are executed then execution will continue with the next sibling action. This pattern will continue until all actions have fully executed or execution is requested to be stopped. All extracted data is this displayed for visual inspection and can then be exported as needed.

### Actions
An action is a single building block that performs a specific operation. Multiple actions can be used together to perform complicated tasks or to extract whatever data is needed for specific projects.

#### Selection
<div><a href="Actions/Select.md">Select</a></div>
<div><a href="Actions/RelativeSelect.md">Relative Select</a></div>

#### Data
<div><a href="Actions/BeginRecord.md">Begin Record</a></div>
<div><a href="Actions/Extract.md">Extract</a></div>
<div><a href="Actions/Log.md">Log</a></div>
<div><a href="Actions/Scope.md">Scope</a></div>
<div><a href="Actions/ScreenCapture.md">Screen Capture</a></div>
<div><a href="Actions/Store.md">Store</a></div>

#### Logic
<div><a href="Actions/Conditional.md">Conditional</a></div>
<div><a href="Actions/ExecuteTemplate.md">Execute Template</a></div>
<div><a href="Actions/Foreach.md">Foreach</a></div>
<div><a href="Actions/Loop.md">Loop</a></div>

#### Interaction
<div><a href="Actions/Click.md">Click</a></div>
<div><a href="Actions/ExecuteScript.md">Execute Script</a></div>
<div><a href="Actions/Hover.md">Hover</a></div>
<div><a href="Actions/Input.md">Input</a></div>
<div><a href="Actions/Keypress.md">Keypress</a></div>
<div><a href="Actions/Reload.md">Reload</a></div>
<div><a href="Actions/Scroll.md">Scroll</a></div>
<div><a href="Actions/SolveCaptcha.md">Solve Captcha</a></div>
<div><a href="Actions/Stop.md">Stop</a></div>
<div><a href="Actions/StopIf.md">Stop If</a></div>
<div><a href="Actions/Wait.md">Wait</a></div>
