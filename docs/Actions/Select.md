# Select Action
The Select Action is used to mark an item on the website as a selection. This allows it to be used by other actions such as extraction or store. Selections can be concatenated through manually selecting items or all similar items can be automatically selected which will increase the flexibility of the data extraction project.

## Options
| Option           | Description |
| ------           | ----------- |
| Name             | Defines the name of the selection which can be helpful when referencing it in other actions or for using the currently selected value directly. |
| XPath            | Defines the specific XPath used to select one or more items on the website. This is typically filled in automatically by manually selecting items. However, it can be entered directly in advanced scenarios. |
| Item wait        | Defines the amount of time to wait after a selected item is processed to process another item. Can be helpful in cases where you may not want to overload a website with quick actions. |
| Selection wait   | Defines the maximum amount of time to wait for a selection to appear on a website in order to process the items. This can be helpful in cases where the website takes some time to load and you still want the extraction project to work correctly. |