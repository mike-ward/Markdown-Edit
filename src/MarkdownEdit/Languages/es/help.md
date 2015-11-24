Keyboard Shortcuts
------------------

    F1             Mostrar/Ocultar esta Ayuda

    Ctrl+N         Nuevo archivo
    Ctrl+Shift+N   Abrir nueva instancia
    Ctrl+O         Abrir archivo
    Ctrl+Shift+O   Insertar archivo
    Ctrl+S         Guardar archivo
    Ctrl+Shift+S   Guardar archivo como
    Ctrl+R         Archivos recientes
    Ctrl+E         Exportar HTML al portapales
    Ctrl+Shift+E   Exportar el HTML y la plantilla al portapapeles
    Alt+E          Guardar HTML como 
	  F5             Recargar archivo
    Alt+F4         Salir

    Ctrl+W         Conmutar ajuste de línea
    Ctrl+F7        Conmutar corrector ortográfico 
    F11            Conmutar pantalla completa
    F12            Conmutar previsualizar/editar/ambos
    Alt+S          Conmutar auto guardar
    Ctrl+,         Conmutar configuración

    Tab            Sangría de linea actual/selección con 2 espacios
    Shift+Tab      Quitar sangría de linea actual/selección con 2 espacios

    Ctrl+C         Copiar
    Ctrl+V         Pegar
    Ctrl+X         Cortar
    Ctrl+Shift+V   Pegado especial de comillas inteligentes y guiones como texto plano

    Ctrl+F         Buscar
    F3             Buscar siguiente
    Shift+F3       Buscar anterior
    Ctrl+H         Buscar y reemplazar

    Ctrl+G         Ir a línea
    Alt+Up         Mover línea actual hacia arriba
    Alt+Down       Mover línea actual hacia abajo
    Ctrl+U         Seleccionar encabezado anterior
    Ctrl+J         Seleccionar encabezado posterior

    Ctrl+Plus      Incrementar tamaño fuente
    Ctrl+Minus     Disminuir tamaño fuente
    Ctrl+0         Restaurar tamaño de la fuente
    Alt+Plus       Incrementar ancho del editor/previsualizador (solo panel simple)
    Alt+Minus      Disminuir ancho del editor/previsualizador (solo panel simple)

    Ctrl+T         Seleccionar tema del editor
    F6             Abrir fragmentos de código en Bloc de Notas
    F7             Abrir diccionario personal en Bloc de Notas
    F8             Abrir plantillas HTML en Bloc de Notas
    F9             Abrir configuración en Bloc de Notas

    Ctrl+B         Selección en Negrita / sin selección inserta **
    Ctrl+I         Selección en Cursiva / sin selección inserta *
    Ctrl+K         Código (encapsula con acento grave ``) / sin selección inserta `
    Alt+L          Convertir selección a lista. Cambiar desordenado/ordenado
    Ctrl+Q         Convertir selección/linea a bloque de cita
    Alt+F          Ajustar y formatear texto
    Alt+Ctrl+F     Ajustar, formatear, usar enlaces de referencia
    Alt+Shift+F    Desajustar y formatear texto
    Ctrl+L         Insertar hipervínculo

    Ctrl+1         Insertar # al principio de la línea
    Ctrl+2         Insertar ## al principio de la línea
    Ctrl+3         Insertar ### al principio de la línea
    Ctrl+4         Insertar #### al principio de la línea
    Ctrl+5         Insertar ##### al principio de la línea
    Ctrl+6         Insertar ###### al principio de la línea

-   [Machete de Sintaxis (Tablas, tachado no 
soportado)](https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet)

-   [Tutorial Markdown](http://markdowntutorial.com/)

-   [CommonMark](http://commonmark.org)

Donar
------

¿Te gusta lo que estoy haciendo? Muestra tu apresiacion haciendo una donacion.
¿Que pasa con el dinero? Una parte es donado para los desarrolladores del 
software y herramientas que uso. El resto va a equipamiento y licenciamiento 
de software.

[Donar a través de
PayPal](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=XGGZ8BEED7R62)

Subiendo Imágenes
----------------

Markdown Edit (MDE) puede subir imágenes a [Imgur](https://imgur.com). Puede
hacerlo de distintas maneras.

-   Arrastrar y Soltar: Arrastra y suelta un archivo de imágen dentro del editor.
	MDE te preguntara por subir o insertar la imágen. El enlace es insertado en 
	el documento al completarse.
-   Portapapeles: Copia una imágen al portapapeles. En MDE, pega una imágen
	(Ctrl+V) y será subida a Imgur. El enlace es insertado en el documento al 
	completarse.

Durante cualquier operación de pegado de texto, MDE detecta si el texto es un
enlace y si lo es, lo inserta como un enlace dentro del documento. Si el enlace
es una imágen, MDE insertará un enlace de la imágen dentro del documento.

Formateo
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

Paquetes Incorporados
---------------------

### Pandoc

-   [Licencia](https://github.com/jgm/pandoc/blob/master/COPYING)
-   [Código Fuente](https://github.com/jgm/pandoc)

### Hunspell

-   [Licencia](http://sourceforge.net/directory/license:lgpl/)
-   [Código Fuente](http://sourceforge.net/projects/hunspell/)

### Fluent Assertions

-   [Licencia](https://github.com/dennisdoomen/fluentassertions/blob/develop/LICENSE)
-   [Código Fuente](https://github.com/dennisdoomen/fluentassertions)

### Avalon Edit

-   [Licencia](http://opensource.org/licenses/MIT)
-   [Código Fuente](https://github.com/icsharpcode/AvalonEdit)

### Commonmark.NET

-   [Licencia](https://github.com/Knagis/CommonMark.NET/blob/master/LICENSE.md)
-   [Código Fuente](https://github.com/Knagis/CommonMark.NET)

### HtmlAgilityPack

-   [Licencia](https://htmlagilitypack.codeplex.com/license)
-   [Código Fuente](https://htmlagilitypack.codeplex.com/)

### MahApps Metro

-   [Licencia](http://opensource.org/licenses/MS-PL)
-   [Código Fuente](https://github.com/MahApps/MahApps.Metro)

### Newtonsoft.Json

-   [Licencia](https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md)
-   [Código Fuente](https://github.com/JamesNK/Newtonsoft.Json)

### TinyIoC

-   [Licencia](https://github.com/grumpydev/TinyIoC/blob/master/licence.txt)
-   [Código Fuente](https://github.com/grumpydev/TinyIoC)

