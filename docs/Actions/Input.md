# Input Action
The Input Action is used to send specific input to the website such as a text value for a search operation.

## Options
| Option           | Description |
| ------           | ----------- |
| Script action    | Lists helpful ways to reference common predefined methods of interacting with the current item. |
| Expression type  | Defines the type of script which is expected. The script can be a text value directly, or it can be an evaluated expression. Evaluated expressions are executed and then the return value is used as the log message. </br> Valid values can be Expression, or Text. |
| Script           | The script to run which will return the value to store. This can be a predefined script based on the script action or can be a custom defined script to return a specific piece of information. |
| Item wait        | Defines the amount of time to wait after the input is sent to continue with project execution. Can be helpful in cases where you may not want to overload a website with quick actions. |