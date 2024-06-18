# Stop If Action
The Stop If Action is used to conditionally stop execution of the current loop, template, or project. This can be helpful when a specific point has been reached and continuing would not be advantageous.

## Options
| Option        | Description |
| ------        | ----------- |
| Script action | Lists helpful ways to reference common predefined methods of interacting with the current item. |
| Script        | The script to run which will return a true/false value used to determine whether to stop execution. A false value will continue execution, while a value of true will stop execution based on the stop type option. |
| Stop type     | Defines the expected way to stop the current execution. This can be the currently executing loop, template, or the entire project. </br> Valid values can be Loop, Template, and Project. |