Tastenkombinationen
------------------

    F1             Anzeigen/Verbergen dieser Hilfe

    Ctrl+N         Neue Datei
    Ctrl+Shift+N   Öffnen neuer Instanz
    Ctrl+O         Datei öffnen
    Ctrl+Shift+O   Datei einfügen
    Ctrl+S         Datei Speichern
    Ctrl+Shift+S   Datei speichern unter...
    Ctrl+R         Kürzlich verwendet
    Ctrl+E         Als HTML in die Zwischenablage exportieren
    Ctrl+Shift+E   Als HTML mit Template in die Zwischenablage exportieren
    Alt+E          Datei als HTML speichern...
    F5             Datei neuladen
    Alt+F4         Beenden

    Ctrl+W         Zeilenumbruch umschalten
    Ctrl+F7        Rechtschreibüberprüfung umschalten
    F11            Vollbild
    F12            Wechseln Vorschau/Bearbeitungs/Duale Ansicht
    Alt+S          Automatisches Speichern umschalten
    Ctrl+,         Einstellungen

    Tab            Aktuelle Selektion 2 Zeichen einrücken
    Shift+Tab      Aktuelle Selektion 2 Zeichen rausrücken

    Ctrl+C         Kopieren
    Ctrl+V         Einfügen
    Ctrl+X         Ausschneiden
    Ctrl+Shift+V   Einfügen spezialer Smartcodes und Links als reinen Text

    Ctrl+F         Suchen
    F3             Nächstes Finden
    Shift+F3       Vorheriges Finden
    Ctrl+H         Finden und Ersetzen

    Ctrl+G         Zur Zeile gehen
    Alt+Up         Aktuelle Zeile raufschieben
    Alt+Down       Aktuelle Zeile runterschieben
    Ctrl+U         Auswahl der vorrigen Überschrift
    Ctrl+J         Auswahl der nächsten Überschrift

    Ctrl+Plus      Schriftgröße vergrößern
    Ctrl+Minus     Schriftgröße verringern
    Ctrl+0         Schriftgröße zurücksetzen
    Alt+Plus       Vergrößerung des Editor-/Vorschau-Fensers (nur in der Einelansicht möglich)
    Alt+Minus      Verkleinerung des Editor-/Vorschau-Fensers (nur in der Einelansicht möglich)

    Ctrl+T         Editordesign auswählen
    F6             Öffnen personalisierter Snippets
    F7             Personalisiertes Wörterbuch öffnen
    F8             Personalisiertes HTML Template öffnen
    F9             Öffnen der Einstellungensdatei

    Ctrl+B         Fett
    Ctrl+I         Kursiv
    Ctrl+K         Code
    Alt+L          Auswahl zu Liste. Umschalten zwischen nummerierter und unnummierierter Aufzählung
    Ctrl+Q         Auswahl zu Zitat konvertieren
    Alt+F          Text umbrechen & formatieren
    Ctrl+Shift+F   Umbrechen, Formatieren, Verwendung von Referenzlinks
    Alt+Shift+F    Brechung umkehren & Text formatieren
    Ctrl+L         Link einfügen

    Ctrl+1         Einfügen von # am Anfang der Zeile
    Ctrl+2         Einfügen von ## am Anfang der Zeile
    Ctrl+3         Einfügen von ### am Anfang der Zeile
    Ctrl+4         Einfügen von #### am Anfang der Zeile
    Ctrl+5         Einfügen von ##### am Anfang der Zeile
    Ctrl+6         Einfügen von ###### am Anfang der Zeile

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

Bilder hochladen
----------------

Markdown Edit (MDE) kann für dich Bilder auf [Imgur](https://imgur.com) hochladen.
Es kann es auf mehreren Arten

-   Drag and Drop: Halte und Ziehe das Bild in den Editor. MDE wird dich
    fragen ob du das Bild einfügen oder hochladen willst. Darauf hin wird
	Der Link in das Dokument eingefügt.
-   Zwischenablage: Kopiere das Bild in die Zwischenablage. Füge das Bild in MDE
	mittels Strg+V in MDE ein. Darauf hin wird das Bild zu Imgur hochgeladen und
	in das Dokument eingebunden.

Während dem Einfügen eines Textes überprüft MDE automatisch ob es sich um einen
Link handelt und wird gegebenenfalls diesen als Markdown Link in das Dokument
einfügen.
Handelt es sich bei diesem Link um einen, welcher auf ein Bild verweist, wird 
der Link in der Markdown Bild Syntax eingefügt.

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

Informationen zur Software
-----

Webseite: <http://markdownedit.com>  
Twitter: `@mikeward_aa` 
Sourcecode: <https://github.com/mike-ward/Markdown-Edit>

Markdown Edit Lizenz
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

