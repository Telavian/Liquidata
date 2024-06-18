# Store Action
The Store Action is used to temporarily take information from a website and store it in memory instead of extracting to the final extracted data. This is useful if information may need to be processed or combined in a specific way prior to actually extracting it.

## Options
| Option        | Description |
| ------        | ----------- |
| Name          | Defines the name of the stored data. This name will be used in other actions to reference this specific stored data. |
| Store type    | Defines storage operation to perform. This will indicate if the stored data should replace other stored data with the same name, or if it should be concatenated to any existing data of the same name. </br> Valid values can be Append, or Replace. |
| Script action | Lists helpful ways to reference common predefined methods of interacting with the current item. |
| Script        | The script to run which will return the value to store. This can be a predefined script based on the script action or can be a custom defined script to return a specific piece of information. |
| Field type    | Defines the expected type of information that will be stored. This field type will be used to convert the returned value into a specific form. </br> Valid values can be Boolean, Datetime, Numeric, Text, and Url. |