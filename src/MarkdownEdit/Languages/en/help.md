Keyboard Shortcuts
------------------

    F1             Show/Hide this help

    Ctrl+N         New file
    Ctrl+Shift+N   Open new instance
    Ctrl+O         Open file
    Ctrl+Shift+O   Insert file
    Ctrl+S         Save file
    Ctrl+Shift+S   Save file As
    Ctrl+R         Recent files
    Ctrl+E         Export HTML to clipboard
    Ctrl+Shift+E   Export HTML with template to clipboard
    Alt+E          Save As HTML
    F5             Reload file
    Alt+F4         Exit

    Ctrl+W         Toggle word wrap
    Ctrl+F7        Toggle spell checking
    F11            Toggle full screen
    F12            Toggle preview/edit/both
    Alt+S          Toggle auto save
    Ctrl+,         Toggle settings

    Tab            Indent current line/selection 2 spaces
    Shift+Tab      Out-dent current line/selection 2 spaces

    Ctrl+C         Copy
    Ctrl+V         Paste
    Ctrl+X         Cut
    Ctrl+Shift+V   Paste special smart quotes and hypens as plain text

    Ctrl+F         Find
    F3             Find next
    Shift+F3       Find previous
    Ctrl+H         Find and replace

    Ctrl+G         Goto line
    Alt+Up         Move current line up
    Alt+Down       Move current line down
    Ctrl+U         Select previous header
    Ctrl+J         Select next header

    Ctrl+Plus      Increase font size
    Ctrl+Minus     Decrease font size
    Ctrl+0         Restore font size
    Alt+Plus       Increase editor/preview width (single pane only)
    Alt+Minus      Decrease editor/preview width (single pane only)

    Ctrl+T         Select editor theme
    F6             Open user snippets in Notepad
    F7             Open user dictionary in Notepad
    F8             Open HTML template in Notepad
    F9             Open settings in Notepad

    Ctrl+B         Bold selection / no selection insert **
    Ctrl+I         Italic selection / no selection insert *
    Ctrl+K         Code (wrap with back ticks ``) / no selection insert `
    Alt+L          Convert selection to list. Toggle unordered/ordered
    Ctrl+Q         Convert selection/line to block quote
    Alt+F          Wrap & Format text
    Alt+Ctrl+F     Wrap, Format, Use Reference Links
    Alt+Shift+F    Unwrap & Format text
    Ctrl+L         Insert Hyperlink

    Ctrl+1         Insert # at beginning of line
    Ctrl+2         Insert ## at beginning of line
    Ctrl+3         Insert ### at beginning of line
    Ctrl+4         Insert #### at beginning of line
    Ctrl+5         Insert ##### at beginning of line
    Ctrl+6         Insert ###### at beginning of line

-   [Syntax Cheat Sheet (Tables, strikethrough not
    supported)](https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet)

-   [Markdown Tutorial](http://markdowntutorial.com/)

-   [CommonMark](http://commonmark.org)

Donate
------

Like what I'm doing? Show your appreciation by donating. What happens to the
money? Some of it is donated to makers of software and tools I use. The rest
goes to equipment funding and software licensing.

[Donate with
PayPal](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=XGGZ8BEED7R62)

Uploading Images
----------------

Markdown Edit (MDE) can upload images to [Imgur](https://imgur.com). It can do
this in several ways.

-   Drag and Drop: Drag and drop an image file onto the editor. MDE will prompt
    to upload or insert the image. The link is inserted into the document on
    completion.
-   Clipboard: Copy an image to the clipboard. In MDE, paste the image (Ctrl+V)
    and it will be uploaded to Imgur. The link is inserted into the document on
    completion.

During any paste operation involving text, MDE detect if the text is an link and
if so, insert it as a link into the document. If the link points to an image,
MDE will insert an image link into the document.

Formatting
----------

MDE can make your Markdown text (not the generated HTML) pretty by reformatting
the document to 80 columns. It will indent lists and block quotes appropriately.
Remember, part of the motivation for Markdown is to make nicely formatted plain
text. Use `Alt+F` and `Alt+Shift+F`. There are also options on the right click
menu.

Text formatting will preserve YAML front-matter.

Auto Save
---------

When Auto Save is enabled (`Alt+S`), content is saved whenever you pause typing
for longer than 4 seconds.

Settings
--------

User settings are stored in a text file in the `AppData` folder. Placing
settings in a plain file allows sharing of settings on different installations.
You can also access some, but not all of settings by clicking on the, `Gear`
icon in the title bar.

Typically, this folder is located at
`C:\Users\<USER>\AppData\Roaming\Markdown Edit\user_settings.json`. Pressing
`F9` will open this file in the system's Notepad editor. It should look
something like this:

    {
        "EditorBackground": "#F7F4EF",
        "EditorForeground": "Black",
        "EditorFontFamily": "Segoe UI",
        "EditorFontSize": 14.0
    }

When you change settings and save this file, Markdown Edit will immediately
update to reflect the changes.

Colors can be defined as RBG values, like the `EditorBackground` setting, or
using the predefined names (like the `EditorForground` setting). Acceptable
predefined names are listed [here](http://is.gd/IkK9i7).

If you delete this file, Markdown Edit will restore it with the default
settings.

Snippets
--------

Snippets allow the quick insertion of words or phrases by typing a trigger word
and then the `TAB` key. This can improve the speed and proficiency of writing
documents. Snippets are stored in a text file that can be edited by pressing
`F6`.

Snippets are activated by typing the trigger word and pressing `TAB`.

Snippets consist of a single line starting with:

-   a single trigger word (can include non alpha-numerics)
-   one or more spaces
-   text that will replace the word

Example

    mde  [Markdown Edit](http://markdownedit.com)

With this snippet defined, open Markdown Edit and type

    mde[TAB]

Where `[TAB]` is the tab key.

The `mde` text is replaced by

    [Markdown Edit](http://markdownedit.com)

### Snippet Substitution Parameters

-   $CLIPBOARD$ - is replaced with clipboard contents (text only)

-   $END$ - Positions the cursor after insertion. For instance

    mde [Markdown $END$ Edit](http://markdownedit.com)

    positions the cursor between *Markdown* and *Edit*

-   $DATE$ - is replaced with the current date and time

-   $DATE("format")$ - format is any valid .NET date format
    (<http://www.dotnetperls.com/datetime-format>)

-   $NAME$ - Where `NAME` can be any word including underscores. When the
    snippet is triggered, the parameter will be highlighted waiting for input.
    There can be multiple substitution parameters in a snippet. Here's how the
    *link* snippet is defined:

    link [$link\_text$]($link_url$) $END$

When triggered, the snippet will expand as `[link_text](link_url)`, with
`link_text`, highlighted. Type the link text and press `TAB`. Now the `link_url`
text is highlighted. Enter the link URL and press `Enter`. The cursor moves one
space past the closing parenthesis.

The description sounds more complicated than it is. Try it and you'll see that
it's an easy and natural workflow.

-   `\n` - insert a new line

If you delete this file, Markdown Edit will restore it with the default
snippets.

Templates
---------

You can change the appearance of the preview view by changing the user template
file. User templates work similar to user settings. The template file is stored
in the `AppData` Folder as `user_template.html`. It can be quickly accessed by
pressing `F8`. Edit it as you see fit.

It is strongly recommended that you keep the IE9 meta tag in the `<head>`
section.

A `<div>` with an ID of `contents` is required. This is where the translated
markup is inserted into the document.

When you change settings and save this file, Markdown Edit will immediately
update to reflect the changes.

If you delete this file, Markdown Edit will restore the default template.

Spell Checking
--------------

Pressing `F7` will toggle spell checking. Spell checking is done as you type.
Right-click on the word to get suggested spellings or to add to the dictionary.

The custom dictionary is a simple text file. It stored in the same folder as the
user settings and user templates. It can be accessed and edited by pressing
`Shift+F7`.

Markdown Edit ships with dictionaries for many languages. Set the dictionary by
pressing `F9`. The dictionaries are stored in the installation folder under
`Spell Check\Dictionaries`.

Themes
------

Markdown Edit has a rudimentary theme system. Themes, control the appearance of
the editor and syntax highlighting. The UI elements (i.e. dialogs) are not
affected.

Out of the box, Markdown comes with several themes which can be accessed by
pressing `Ctrl+T`. Selecting a theme updates your user settings. You can further
edit the theme by opening your user settings (`F9`) and editing the theme
section. This is the recommended way to create a new theme.

Themes are located in the installation directory under `\Themes`.

If you create an awesome theme, send it to me and I'll add it to the
distribution. I'm a lousy artist. :)

Etc.
----

-   Line numbers can be enabled/disabled in the settings file
-   List continuations - When editing lists, pressing enter will add the next
    list marker. Works with numbered lists as well.
-   When opening recent documents, Markdown Edit will scroll to the last saved
    position. This can be disabled in the setting if not desired.

Limitations
-----------

-   Single document Interface
-   I wrote it ;)

About
-----

Home Page: <http://markdownedit.com>  
Twitter: `@mikeward_aa` 
Source: <https://github.com/mike-ward/Markdown-Edit>

Markdown Edit License
---------------------

The MIT License (MIT)

Copyright (c) 2015 Mike Ward

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

Incorporated Packages
---------------------

### Pandoc

-   [License](https://github.com/jgm/pandoc/blob/master/COPYING)
-   [Source code](https://github.com/jgm/pandoc)

### Hunspell

-   [License](http://sourceforge.net/directory/license:lgpl/)
-   [Source code](http://sourceforge.net/projects/hunspell/)

### Fluent Assertions

-   [License](https://github.com/dennisdoomen/fluentassertions/blob/develop/LICENSE)
-   [Source code](https://github.com/dennisdoomen/fluentassertions)

### Avalon Edit

-   [License](http://opensource.org/licenses/MIT)
-   [Source code](https://github.com/icsharpcode/AvalonEdit)

### Commonmark.NET

-   [License](https://github.com/Knagis/CommonMark.NET/blob/master/LICENSE.md)
-   [Source code](https://github.com/Knagis/CommonMark.NET)

### HtmlAgilityPack

-   [License](https://htmlagilitypack.codeplex.com/license)
-   [Source code](https://htmlagilitypack.codeplex.com/)

### MahApps Metro

-   [License](http://opensource.org/licenses/MS-PL)
-   [Source code](https://github.com/MahApps/MahApps.Metro)

### Newtonsoft.Json

-   [License](https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md)
-   [Source code](https://github.com/JamesNK/Newtonsoft.Json)

### TinyIoC

-   [License](https://github.com/grumpydev/TinyIoC/blob/master/licence.txt)
-   [Source code](https://github.com/grumpydev/TinyIoC)

