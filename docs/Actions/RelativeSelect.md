# Relative Select Action
The Relative Select Action is used to mark a relative item on the website as a selection. This is similar to a regular Select Action, however is used when relative data is to be selected instead. For instance, in many websites data can be displayed in a structured form so a parent selection can select the record and then there can be one or more relative selections which select specific pieces of information.

## Options
| Option           | Description |
| ------           | ----------- |
| Name             | Defines the name of the selection which can be helpful when referencing it in other actions or for using the currently selected value directly. |
| XPath            | Defines the specific XPath used to select one or more items on the website. This is a relative path based on the parent selection. This is typically filled in automatically by manually selecting items. However, it can be entered directly in advanced scenarios. |
| Item wait        | Defines the amount of time to wait after a selected item is processed to process another item. Can be helpful in cases where you may not want to overload a website with quick actions. |
| Selection wait   | Defines the maximum amount of time to wait for a selection to appear on a website in order to process the items. This can be helpful in cases where the website takes some time to load and you still want the extraction project to work correctly. |