# Extract Action
The Extract Action is used to extract specific pieces of information and allow them to be used outside of the website. Virtually anything can be extracted as needed.

## Options
| Option        | Description |
| ------        | ----------- |
| Name          | Defines the name of the extracted data. In the scope of an extraction, the variable <strong>$this</strong> always refers to the current select item that is being looped over. Therefore, selections can be referenced through their variable name such as <strong>$selection1</strong> or through the easier method of <strong>$this</strong>. |
| Script action | Lists helpful ways to reference common predefined methods of interacting with the current item. |
| Script        | The script to run which will return the value to extract. This can be a predefined script based on the script action or can be a custom defined script to return a specific piece of information. |
| Field type    | Defines the expected type of information that will be extracted. This field type will be used to convert the returned value into a specific form. </br> Valid values can be Boolean, Datetime, Numeric, Text, and Url. |