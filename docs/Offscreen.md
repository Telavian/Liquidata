# Liquidata Offscreen

Offscreen is the way to run projects created through Liquidata Client in the background so the data can be collected as needed. The projects can be executed with the browser visible, so users can see what is happening, or in a <a href="https://en.wikipedia.org/wiki/Headless_browser">headless mode</a> where the browser is not visible at all.

Once collection is complete, then the collected data can be saved to a file and used in whatever form is needed.

### Command Line
| Argument              | Required | Description |
| ------                |  ---     | ----------- |
| --project, -p         |  Yes | The path on disk to the saved project file. |
| --browser, -b         |  No  | The type of browser to use. Can be `Chromium`, `Firefox`, or `WebKit`. Defaults to `Chromium`. |
| --browser-path, -bp   |  No  | The path to the browser executable to use. |
| --disable-images, -di |  No  | Disables the loading of images during execution |
| --concurrency, -c     |  No  | The amount of concurrency to use during execution. |
| --output, -o          |  No  | The path on disk to save the execution results to. |
| --print-output, -po   |  No  | Indicates whether to print the execution results to standard output. |
| --headless, -h        |  No  | Indicates to run the browser in the headless mode with no browser window shown. |
| --proxy-server, -ps   |  No  | Address of proxy to use in project execution. Example: https://domain.com:1234/ |
| --proxy-user, -pu     |  No  | Username to use for the browser proxy. |
| --proxy-password, -pp |  No  | Password to use for the browser proxy |