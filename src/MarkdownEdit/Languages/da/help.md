Tastatur genveje
----------------

    F1             Vis/skjul denne hjælp

    Ctrl+N         Ny fil
    Ctrl+Shift+N   Åbn ny instans af programmet
    Ctrl+O         Åbn fil
    Ctrl+Shift+O   Indsæt fil
    Ctrl+S         Gem fil
    Ctrl+Shift+S   Gem fil som
    Ctrl+R         Nylige filer
    Ctrl+E         Eksporter HTML til udklipsholderen
    Ctrl+Shift+E   Eksporter HTML og skabelon til udklipsholderen
    F5             Gen-indlæs fil
    Alt+F4         Afslut

    Ctrl+W         Slå tekst ombrydning til/fra
    Ctrl+F7        Slå stave kontrol til/fra
    F11            Slå fuld skærm til/fra
    F12            Skift mellem forhåndsvisning/redigering/begge
    Alt+S          Slå automatiske gemme til/fra

    Tab            Indryk indeværende linje/valgt blok med 2 mellemrum
    Shift+Tab      Udryk indeværende linje/valgt blok med 2 mellemrum

    Ctrl+C         Kopier
    Ctrl+V         Indsæt
    Ctrl+X         Klip
    Ctrl+Shift+V   Indsæt specielle smart citater og bindestreger som almindelig tekst

    Ctrl+F         Søg
    F3             Søg næste
    Shift+F3       Søg forrige
    Ctrl+H         Søg og erstat

    Ctrl+G         Gå til linje
    Alt+Up         Flyt aktuel linje op
    Alt+Down       Flyt aktuel linje ned
    Ctrl+U         Vælg forrige overskrift
    Ctrl+J         Vælg næste overskrift

    Ctrl+Plus      Øg skrift størrelse
    Ctrl+Minus     Mindsk skrift størrelse
    Ctrl+0         Gendan skrift størrelse
    Alt+Plus       Øg editor/forhåndsvisning bredde (Kun ved enkelt panel) 
    Alt+Minus      Formindsk editor/forhåndsvisning bredde (Kun ved enkelt panel)

    Ctrl+T         Vælg editor tema
    F6             Åben bruger kodestumper i Notepad
    F7             Åben bruger ordbog i Notepad
    F8             Åben HTML skabelon i Notepad
    F9             Åben indstillinger i Notepad

    Ctrl+B         Fed skrift **
    Ctrl+I         Kursiv *
    Ctrl+K         Kode (ombryd med bagvendte accenter ``) `
    Ctrl+L         Konverter valgt tekst til liste. Skift mellem ordnet og uordnet
    Ctrl+Q         Konverter vlgt tekst&linje til block citat
    Alt+F          Ombryd og formatter tekst
    Alt+Ctrl+F     Ombryd, formatter, anvend reference links
    Alt+Shift+F    Fjern ombrydning og formatter tekst

    Ctrl+1         Indsæt # ved begyndelsen af linje
    Ctrl+2         Indsæt ## ved begyndelsen af linje
    Ctrl+3         Indsæt ### ved begyndelsen af linje
    Ctrl+4         Indsæt #### ved begyndelsen af linje
    Ctrl+5         Indsæt ##### ved begyndelsen af linje
    Ctrl+6         Indsæt ###### ved begyndelsen af linje

-   [Syntaks reference ark (Tabeller, gennemstregning er
    ikke understøttet)](https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet)

-   [Markdown vejledning](http://markdowntutorial.com/)

-   [CommonMark](http://commonmark.org)

Donationer
----------

Kan du lide hvad jeg laver? Vis din påskønnelse ved at donerer. Hvad sker der
med pengene? Nogle af dem doneres til producenter af software og værktøjer jeg
anvender. Resten går til udstyr og software licenser.

[Doner med
PayPal](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=XGGZ8BEED7R62)

Upload af billeder
------------------

Markdown Edit (MDE) kan uploade billeder til [Imgur](https://imgur.com). Dette
kan ske på flere måder.

-   Træk og slip: Træk og slip en billede fil til editoren. MDE vil vise en
    prompt for om den skal uploade eller indsætte billedet. Billedet indsættes
    ved fuldførelse.
-   Udklipsholder: Kopier et billede til udklipsholderen. I MDE, indsæt
    billedet (Ctrl+V) og det bliver uploadet til Imgur. Linket indsættes i
    dokumented ved fuldførelse.

Ved en indsæt handling som involverer tekst, finder MDE selv ud af om teksten er
et link, og i så fald, indsætter det som et link i dokumentet. Hvis linket peger
på et billede, vil MDE indsætte et billede link i dokumentet.

Formatering
-----------

MDE kan forskønne din Markdown tekst (ikke det genererede HTML) ved at ombryde
dokumentet til 80 kolonner. Listen og blok citater vil blive passende indrykket.
Husk, en del af motivationen for Markdown er at kunne lave pænt opstillet rent
tekst. Brug `Alt+F` og `Alt+Shift+F`. Der er også muligheder på højre-kliks
menuen.

Tekst formatering vil bevare YAML front-matter.

Automatisk gemmefunktion
------------------------

Når den automatiske gemmefunktion er slået til (`Alt+S`), vil indholdet blive
gemt hver gang du holder pause i din indtastning i mere end 4 sekunder.

Indstillinger
-------------

Bruger indstillinger gemmes i en tekst file som er placeret i `AppData` mappen.
Ved at gemme indstillinger i en ren tekst fil, er det muligt at dele
indstillinger mellem installationer. Det er også muligt at tilgå nogle, men ikke
alle indstillinger, ved at klikke på `Tandhjul` ikonet i titel linjen.

Typisk er denne mappe at finde her `C:\Users\<USER>\AppData\Roaming\Markdown Edit\user_settings.json`. Ved at trykke `F9` åbnes filen i systemets Notepad program.
Filen vil se ud ligesom dette:

    {
        "EditorBackground": "#F7F4EF",
        "EditorForeground": "Black",
        "EditorFontFamily": "Segoe UI",
        "EditorFontSize": 14.0
    }

Når du ændrer indstillinger og gemmer denne fil, vil Markdown Edit øjeblikkeligt reflektere ændringerne.

Farver kan defineres som RGB værdier, lige som `EditorBackground` indstillingen, eller ved at forud definerede navne (såsom `EditorForground`). Acceptable forud definerede navn kan findes [her](http://is.gd/IkK9i7). 

Hvis du sletter filen, vil Markdown Edit gendanne den med standard indstillinger.


Kodestumper
-------

Kodestumper gør det muligt at hurtigt indsætte ord og vendinger ved at indtaste et udløser ord, og derefter trykke på `TAB` tasten. Dette kan hjælpe med at forbedre hastighed og skrivefærdighed. Kodestumper gemmes i en tekst fil som kan redigeres ved at trykke `F6`.

Kodestumper aktiveres ved at indtaste et udløser ord og trykke `TAB`.

Kodestumper består af en enlig linje som starter med:

-   et enkelt udløser ord (kan inkluderer special tegn)
-   et eller flere mellemrum
-   tekst som vil erstatte ordet

Eksempel

    mde  [Markdown Edit](http://mike-ward.net/markdown)

Når denne kodestumper er defineret, åbn Markdown Edit og indtast

    mde[TAB]

Hvor `[TAB]` er tabulator tasten.

Teksten `mde` vil så blive erstattet med 

    [Markdown Edit](http://mike-ward.net/markdown)

### Kodestump erstatnings parametre

-   $CLIPBOARD$ - Vil blive erstattet med indholdet af udklipsholderen (kun tekst)

-   $END$ - Positions the cursor after insertion. For instance

    mde [Markdown $END$ Edit](http://mike-ward.net/markdown)

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

Skabeloner
----------

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

Stave kontrol
-------------

Pressing `F7` will toggle spell checking. Spell checking is done as you type.
Right-click on the word to get suggested spellings or to add to the dictionary.

The custom dictionary is a simple text file. It stored in the same folder as the
user settings and user templates. It can be accessed and edited by pressing
`Shift+F7`.

Markdown Edit ships with dictionaries for many languages. Set the dictionary by
pressing `F9`. The dictionaries are stored in the installation folder under
`Spell Check\Dictionaries`.

Temaer
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

Diverse
-------

-   Line numbers can be enabled/disabled in the settings file
-   List continuations - When editing lists, pressing enter will add the next
    list marker. Works with numbered lists as well.
-   When opening recent documents, Markdown Edit will scroll to the last
    saved position. This can be disabled in the setting if not desired.

Begrænsninger
-------------

-   Single document Interface
-   I wrote it ;)

Om
--

Version 1.11.0  
Hjemmeside: <http://mike-ward.net/markdownedit>  
Twitter: `@mikeward_aa`  
Kildekode: <https://github.com/mike-ward/Markdown-Edit>

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

