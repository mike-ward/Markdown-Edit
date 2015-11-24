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
    Alt+E          Gem HTML som
    F5             Gen-indlæs fil
    Alt+F4         Afslut

    Ctrl+W         Slå tekst ombrydning til/fra
    Ctrl+F7        Slå stave kontrol til/fra
    F11            Slå fuld skærm til/fra
    F12            Skift mellem forhåndsvisning/redigering/begge
    Alt+S          Slå automatiske gemme til/fra
    Ctrl+,         Slå indstillinger

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
    Alt+L          Konverter valgt tekst til liste. Skift mellem ordnet og uordnet
    Ctrl+Q         Konverter vlgt tekst&linje til block citat
    Alt+F          Ombryd og formatter tekst
    Alt+Ctrl+F     Ombryd, formatter, anvend reference links
    Alt+Shift+F    Fjern ombrydning og formatter tekst
    Ctrl+L         Indsæt hyperlink

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

Typisk er denne mappe at finde her
`C:\Users\<USER>\AppData\Roaming\Markdown Edit\user_settings.json`. Ved at
trykke `F9` åbnes filen i systemets Notepad program. Filen vil se ud ligesom
dette:

    {
        "EditorBackground": "#F7F4EF",
        "EditorForeground": "Black",
        "EditorFontFamily": "Segoe UI",
        "EditorFontSize": 14.0
    }

Når du ændrer indstillinger og gemmer denne fil, vil Markdown Edit øjeblikkeligt
reflektere ændringerne.

Farver kan defineres som RGB værdier, lige som `EditorBackground` indstillingen,
eller ved at forud definerede navne (såsom `EditorForground`). Acceptable forud
definerede navn kan findes [her](http://is.gd/IkK9i7).

Hvis du sletter filen, vil Markdown Edit gendanne den med standard
indstillinger.

Kodestumper
-----------

Kodestumper gør det muligt at hurtigt indsætte ord og vendinger ved at indtaste
et udløser ord, og derefter trykke på `TAB` tasten. Dette kan hjælpe med at
forbedre hastighed og skrivefærdighed. Kodestumper gemmes i en tekst fil som kan
redigeres ved at trykke `F6`.

Kodestumper aktiveres ved at indtaste et udløser ord og trykke `TAB`.

Kodestumper består af en enlig linje som starter med:

-   et enkelt udløser ord (kan inkluderer special tegn)
-   et eller flere mellemrum
-   tekst som vil erstatte ordet

Eksempel

    mde  [Markdown Edit](http://markdownedit.com)

Når denne kodestumper er defineret, åbn Markdown Edit og indtast

    mde[TAB]

Hvor `[TAB]` er tabulator tasten.

Teksten `mde` vil så blive erstattet med

    [Markdown Edit](http://markdownedit.com)

### Kodestump erstatnings parametre

-   $CLIPBOARD$ - Vil blive erstattet med indholdet af udklipsholderen
    (kun tekst)

-   $END$ - Placerer cursoren bag det indsatte. For eksempel

    mde [Markdown $END$ Edit](http://markdownedit.com)

    placerer cursoren mellem *Markdown* og *Edit*

-   $DATE$ - erstattes med indeværende dato og klokkeslæt

-   $DATE("format")$ - format er et gyldigt .NET dato format
    (<http://www.dotnetperls.com/datetime-format>)

-   $NAME$ - Hvor `NAME` er et vilkårligt ord, inklusiv understregninger. Når
    kodestumpen aktiveresm vil parameteren være fremhævet og klar
    til indtastning. Der kan være adskillige erstatningsparametre i
    en kodestump. *Link* kodestumpen er f.eks. defineret således:

    link [$link\_text$]($link_url$) $END$

Når kodestumpen udføres vil denne udvides til `[link_text](link_url)`, med
`link_text` markeret. Indtast teksten til linket og tryk `TAB`. Nu er `link_url`
så markeret. Indtast URL'en til linket og tryk `Enter`. Markøren flytter sig et
mellemrum forbi den afsluttende parentes.

Beskrivelsen lyder mere kompliceret end det er. Prøv det, og du vil opdage at
det er en let og naturlig arbejdsgang.

-   `\n` - Indsæt en ny linje

Hvis du sletter denne fil, vil Markdown Edit genoprette den med standard
kodestumperne.

Skabeloner
----------

Du kan ændre udseendet af forhåndsvisningen ved at ændre skabelon filen. Bruger
skabeloner fungerer ligesom bruger indstillinger. Skabelon filen gemmes i
`AppData` mappen som `user_template.html`. Den kan hurtigt åbnes ved at trykke
`F8`. Rediger den som du lyster.

Det er stærkt anbefalet at du beholder IE9 meta tagget i `<head>` sektionen.

Der skal være et `<div>` tag med ID `content`. Dette er påkrævet. Det er her den
oversatte markup indsættes i dokumentet.

Når du ændrer skabelonen og gemmer filen, vil Markdown Edit opdaterer
forhåndsvisningen med det samme.

Hvis du sletter filen, vil Markdown Edit genoprette standard skabelonen.

Stavekontrol
------------

Trykker man på `F7` bliver stavekontrol slået til eller fra. Stavekontrol
foregår løbende efterhånden som du skriver. Højre-klikker du på et ord, kan
stavekontrollen foreslå alternative stavemåder eller tilføje ordet til ordbogen.

Bruger ordbogen er en simpel tekst fil. Den gemmes samme sted som bruger
indstillinger og skabeloner. Filen kan åbnes og redigeres ved at trykke
`Shift+F7`.

Markdown Edit kommer med ordbøger til adskillige sprog. Sæt den aktuelle ordbog
ved at trykke `F9`. Ordbogen gemmes i installationsmappen under
`Spell Check\Dictionaries`.

Temaer
------

Markdown Edit indeholder et simpelt tema system. Temaer kontroller editorens
udseende og syntaksfremhævning. UI elementer (f.eks. dialoger) er ikke påvirket.

Markdown kommer med adskillige temaer som kan tilgås ved at trykke `Ctrl+T`.
Valg af et tema opdaterer dine bruger indstillinger. Du kan redigerer tema ved
at åbne dine bruger indstillinger (`F9`) og redigere tema sektionen. Dette er
den anbefalede måde at lave nye temaer på.

Temaerne findes i installationsmappen under `\Themes`.

Skaber du et fedt tema, så send til mig og så vil jeg inkluderer det i
distributionen. Jeg er en elendig kunstner. :)

Diverse
-------

-   Linje numre kan slås til og fra i filen med indstillinger
-   Liste fortsættelser - Når du redigerer lister vil Retur tasten automatisk
    tilføje den næste liste markør. Det virker også med nummererede liste.
-   Når du åbner et dokument som har være redigeret for nyligt, vil Markdown
    Edit rulle ned til den sidst gemte position. Dette kan slås fra i
    indstillingerne hvis det ikke ønskes.

Begrænsninger
-------------

-   Enkelt-dokument brugerflade.
-   Jeg skrev det ;)

Om
--

Hjemmeside: <http://markdownedit.com>  
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

