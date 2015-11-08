---
layout: page
title: "Markdown Edit"
image:
  feature: typewriter.jpg
---

Markdown Edit is a Windows **desktop**
[CommonMark](http://commonmark.org) (a.k.a. Markdown) editor with an
emphasis on content and keyboard shortcuts. There is minimal window
chrome and most functions are accessed through keyboard shortcuts. There
is no main menu, status bar, tabbed windows or other distractions.

> Markdown Edit is ready for translations. Go
> [here](http://mike-ward.net/2015/05/03/markdown-edit-1-6-ready-to-translate/)
> to find out how.

> I need themes! (Dammit Jim!) I'm a programmer, not a designer. Send me
> some cool themes and I'll include them.

> **Windows 7** - You're welcome to install MDE on Windows 7 but it's
> not supported. Some users have reported a font issue. The issue is
> cosmetic (some icons appear as blank squares). [Read more and link to
> new font](https://github.com/mike-ward/Markdown-Edit/issues/14).

`TL;DR`

-   [Download via HTTP](http://mike-ward.net/downloads) or
-   install via
    [Chocolatey](https://chocolatey.org/packages/markdown-edit) using
    `choco install markdown-edit`

Gratuitous Screen Shot

<a href="http://mike-ward.net/cdn/images/markdown-edit/markdown-edit-screenshot.png" target="_blank">![markdown-edit-screenshot.png](http://mike-ward.net/cdn/images/markdown-edit/markdown-edit-screenshot.png "Gratuitous Screen Shot")</a>

Features
--------

-   [Drag and Drop image
    uploads](http://mike-ward.net/2015/03/31/markdown-edit-1-4-imgur-uploads/)
-   [Document
    Formatting](http://mike-ward.net/2015/04/20/markdown-edit-1-5-released/).
    Make your Markdown beautiful
-   Convert `.docx` files to markdown using simple drag & drop
-   Syntax highlighting editor
-   Side-by-side HTML preview
-   Quickly show/hide preview
-   [CommonMark](http://commonmark.org) standard Markdown engine
-   GitHub Flavored Markdown supported
-   User preferences stored in a text file for easy sharing
-   Full screen covers task-bar (optional)
-   Keyboard shortcuts for bold, italic, headers, lists, block
    quotes, etc.
-   User defined snippets improve the speed and proficiency of
    writing documents.
-   Modern UI look and feel
-   **Not** a Windows Store App
-   Synchronized scrolling
-   User settable fonts, colors, themes
-   User defined style sheets
-   As you type spell checking
-   **Paste Special** replaces Microsoft Word's
    smart quotes/hyphens/etc. with plain text equivalents
-   Quickly open recent files
-   Quickly change themes
-   Word Count
-   Auto Save
-   Select previous/next header
-   Highlight current line
-   [Open Source](https://github.com/mike-ward/Markdown-Edit)
-   MIT License

*Markdown Edit's help has a more complete list of features/shortcuts*

Auto Save
---------

When Auto Save is enabled (`Alt+S`), content is saved whenever you pause
typing for 4 or more seconds.

Settings
--------

User settings are stored in a text file in the `AppData` folder. Placing
settings in a plain file allows sharing of settings on different
installations.

Typically, this folder is located at
`C:\Users\<USER>\AppData\Roaming\Markdown Edit\user_settings.json`.
Pressing `F9` will open this file in the system's Notepad editor. It
should look something like this:

    {
        "EditorBackground": "#F7F4EF",
        "EditorForeground": "Black",
        "EditorFontFamily": "Segoe UI",
        "EditorFontSize": 14.0
    }

When you change settings and save this file, Markdown Edit will
immediately update to reflect the changes.

Colors can be defined as RBG values, like the `EditorBackground`
setting, or using the predefined names (like the `EditorForground`
setting). Acceptable predefined names are listed
[here](http://msdn.microsoft.com/en-us/library/system.windows.media.colors(v=vs.110).aspx).

If you delete this file, Markdown Edit will restore it with the default
settings.

Snippets
--------

Snippets allow the quick insertion of words or phrases by typing a
trigger word and then the `TAB` key. This can improve the speed and
proficiency of writing documents. Snippets are stored in a text file
that can be edited by pressing `F6`.

Snippets are activated by typing the trigger word and pressing `TAB`.

Snippets consist of a single line starting with:

-   a single trigger word (can include non alpha-numerics)
-   one or more spaces
-   text that will replace the word

Example

    mde  [Markdown Edit](http://mike-ward.net/markdown)

With this snippet defined, open Markdown Edit and type

    mde[TAB]

Where `[TAB]` is the tab key.

The `mde` text is replaced by

    [Markdown Edit](http://mike-ward.net/markdown)

Snippets can contain special keywords.

-   $CLIPBOARD$ - is replaced with clipboard contents (text only)

-   $END$ - Positions the cursor after insertion. For instance

        mde  [Markdown $END$ Edit](http://mike-ward.net/markdown)

    positions the cursor between *Markdown* and *Edit*

-   $DATE$ - is replaced with the current date and time

-   $DATE("format")$ - format is any valid .NET date format
    (<http://www.dotnetperls.com/datetime-format>)

-   `\n` - insert a new line

If you delete this file, Markdown Edit will restore it with the default
snippets.

Templates
---------

You can change the appearance of the preview view by changing the user
template file. User templates work similar to user settings. The
template file is stored in the `AppData` Folder as `user_template.html`.
It can be quickly accessed by pressing `F8`. Edit it as you see fit.

It is strongly recommended that you keep the IE9 meta tag in the
`<head>` section.

A `<div>` with an ID of `contents` is required. This is where the
translated markup is inserted into the document.

When you change settings and save this file, Markdown Edit will
immediately update to reflect the changes.

If you delete this file, Markdown Edit will restore the default
template.

Spell Checking
--------------

Pressing `F7` will toggle spell checking. Spell checking is done as you
type. Right-click on the word to get suggested spellings or to add to
the dictionary.

The custom dictionary is a simple text file. It stored in the same
folder as the user settings and user templates. It can be accessed and
edited by pressing `Shift+F7`.

Markdown Edit ships with dictionaries for many languages. Set the
dictionary by pressing `F9`. The dictionaries are stored in the
installation folder under `Spell Check\Dictionaries`.

Themes
------

Markdown Edit has a rudimentary theme system. Themes, control the
appearance of the editor and syntax highlighting. The UI elements (i.e.
dialogs) are not affected.

Out of the box, Markdown comes with several themes which can be accessed
by pressing `Ctrl+T`. Selecting a theme updates your user settings. You
can further edit the theme by opening your user settings (`F9`) and
editing the theme section. This is the recommended way to create a new
theme.

Themes are located in the installation directory under `\Themes`.

If you create an awesome theme, send it to me and I'll add it to the
distribution. I'm a lousy artist. :)

Limitations
-----------

-   <s>Only supports CommonMark</s>
-   Single document Interface
-   <s>Syntax highlighting does not recognize multiple-line constructs.
    It uses regular expressions which don't understand the underlying
    Markdown constructs. I'm hoping as CommonMark matures that a syntax
    parser (like PEG) will emerge.</s>
-   I wrote it ;)

<button onclick="load_disqus('markdownedit', 'Markdown Eit');" class="pure-button">Comments</button>
<div id="disqus_thread"></div>
<a href="https://github.com/mike-ward/Markdown-Edit"><img style="position: absolute; top: 0; right: 0; border: 0;" src="https://camo.githubusercontent.com/652c5b9acfaddf3a9c326fa6bde407b87f7be0f4/68747470733a2f2f73332e616d617a6f6e6177732e636f6d2f6769746875622f726962626f6e732f666f726b6d655f72696768745f6f72616e67655f6666373630302e706e67" alt="Fork me on GitHub" data-canonical-src="https://s3.amazonaws.com/github/ribbons/forkme_right_orange_ff7600.png"></a>
