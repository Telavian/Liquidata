# Web Security
Due to how Liquidata operates entirely within the browser, then there are some security concerns to handle when loading other sites to interact with them. This is not a problem when running in offscreen, or the background mode, however it can be a challenge in the Liquidata Client itself. There are however simple and easy workarounds to get the client working as expected.

It should be noted that these workarounds should only be used while working with Liquidata and shouldn't be used to expose any very sensitive data.

### Command Line
To disable Web Security while using Liquidata you can start the browser with the following command-line arguments.

These commands assume a default installation location of the indicated browser. If the install location is different then the command would have to be adjusted accordingly.

#### Chrome
Windows: `"C:\Program Files\Google\Chrome\Application\chrome.exe" --profile-directory=Default --user-data-dir=%temp% --disable-web-security --disable-site-isolation-trials`
Linux: `TBD`

#### Edge
Windows: `"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe" --profile-directory=Default --user-data-dir=%temp% --disable-web-security --disable-site-isolation-trials`
Linux: `TBD`

#### Firefox
Windows: `TBD`
Linux: `TBD`

#### Safari
Windows: `TBD`
Linux: `TBD`

### Extensions
X-Frame-Options
