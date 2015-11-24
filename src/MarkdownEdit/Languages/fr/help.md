Keyboard Shortcuts
------------------

    F1             Montrer/cacher cette aide

    Ctrl+N         Nouveau fichier
    Ctrl+Shift+N   Ouvrir une nouvelle instance
    Ctrl+O         Ouvrir un fichier
    Ctrl+Shift+O   Insérer un fichier
    Ctrl+S         Enregistrer
    Ctrl+Shift+S   Enregistrer sous
    Ctrl+R         Fichiers récents
    Ctrl+E         Exporter en HTML vers le presse papier
    Ctrl+Shift+E   Exporter en HTML avec le template vers le presse papier
    Alt+E          Enregistrer sous HTML 
    F5             Recharger fichier
    Alt+F4         Quitter

    Ctrl+W         Activer/désactiver le retour à la ligne automatique
    Ctrl+F7        Activer/désactiver la correction orthographique
    F11            Activer/désactiver le mode plein écran
    F12            Basculer entre les modes prévisulation/édition/les deux
    Alt+S          Activer/désactiver l'enregistrement automatique
    Ctrl+,         Activer/désactiver réglages

    Tab            Indenter la ligne courante/sélection de 2 espaces
    Shift+Tab      Désindenter la ligne courante/sélection de 2 espaces

    Ctrl+C         Copier
    Ctrl+V         Coller
    Ctrl+X         Couper
    Ctrl+Shift+V   Collage spécial (citations et tirets en tant que texte)

    Ctrl+F         Chercher
    F3             Chercher le suivant
    Shift+F3       Chercher le précédent
    Ctrl+H         Chercher et remplacer

    Ctrl+G         Aller à la ligne
    Alt+Up         Remonter la ligne courante
    Alt+Down       Descendre la ligne courante
    Ctrl+U         Sélectionner l'entête précédente
    Ctrl+J         Sélectionner l'entête suivante

    Ctrl+Plus      Augmenter la taille de police
    Ctrl+Minus     Diminuer la taille de police
    Ctrl+0         Restaurer la taille de police
    Alt+Plus       Augmenter la largeur de l'editeur/prévisulation (seulement en mode panneau unique)
    Alt+Minus      Diminuer la largeur de l'editeur/prévisulation (seulement en mode panneau unique)

    Ctrl+T         Sélectioner un thème
    F6             Ouvrir les snippets in Notepad
    F7             Ouvrir le dictionaire utilisateur dans Notepad
    F8             Ouvrir le modèle HTML dans Notepad
    F9             Ouvrir le fichier des réglages dans Notepad

    Ctrl+B         Mettre en gras le texte sélectionné
    Ctrl+I         Mettre en italique le texte sélectionné
    Ctrl+K         Code (entoure le mot courant ou la sélection par des ``) / insère un ` si pas de sélection
    Alt+L          Convertir le texte sélectionné en liste. Basculer entre non numérotée/numérotée
    Ctrl+Q         Convertir le texte sélectionné/la ligne en bloc de citation
    Alt+F          Couper et formater le texte
    Alt+Ctrl+F     Couper et formater le texte et lien de référence
    Alt+Shift+F    Ne pas couper et formater le texte
    Ctrl+L         Insérer un lien hypertexte

    Ctrl+1         Insérer # en début de ligne
    Ctrl+2         Insérer ## en début de ligne
    Ctrl+3         Insérer ### en début de ligne
    Ctrl+4         Insérer #### en début de ligne
    Ctrl+5         Insérer ##### en début de ligne
    Ctrl+6         Insérer ###### en début de ligne

-   [Syntaxe (Tableaux et texte barré ne sont
    pas supportés)](https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet)

-   [Tutoriel Markdown (en anglais)](http://markdowntutorial.com/)

-   [CommonMark](http://commonmark.org)

Donation
--------

Vous aimez ce que je fais? Montrez votre reconnaissance par une donation. Que
devient l'argent? Une partie revient aux créateurs des logiciels et outils que
j'utilise. Le reste finance l'équipement et les licences logicielles.

[Donnez avec
PayPal](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=XGGZ8BEED7R62)

Téléchargement des images
-------------------------

Markdown Edit (MDE) peux télécharger les images sur [Imgur](https://imgur.com),
de plusieurs façons.

-   Glissé / déposé: glissez et déposez une image dans l'éditeur. MDE va vous
    demander de choisir entre télécharger ou insérer l'image. The lien sera
    alors inséré dans le document.
-   Presse papier: copiez une image dans le presse papier. In MDE, collez
    l'image (Ctrl+V) et elle sera téléchargée sur Imgur. The lien sera alors
    inséré dans le document.

Lors d'une opération de collé contenant du texte, MDE détecte si le texte est un
lien et si tel est le cas, l’insère en tant que lien dans le document. Si le
lien pointe sur une image, MDE va insérer un lien de type image dans le
document.

Formatage
---------

MDE peux rendre votre texte Markdown (pas le HTML généré) joli en reformatant le
document à 80 colonnes. Il va indenter les listes et les blocs de citations de
manière appropriée. Souvenez vous, une partie des raisons pour lesquelles
Markdown existe est d'obtenir des textes joliment formatés. Utilisez `Alt+F` et
`Alt+Shift+F`. Ces options sont également disponibles dans le menu contextuel
(clic droit dans la fenêtre de l'éditeur).

Le formatage du texte préserve les balises YAML.

Enregistrement automatique
--------------------------

Lorsque l'enregistrement automatique est activée(`Alt+S`), le document est
enregistré dès que vous cessez de taper pendant au moins 4 secondes.

Réglages
--------

Les réglages utilisateurs sont stockés dans un fichier texte placé dans le
répertoire `AppData`. Utiliser un fichier texte permet le partage des réglages
entre plusieurs installations. Vous pouvez également accéder à certains
réglages, mais pas tous, en cliquant sur l’icône `Gear` dans la barre de titre.

Habituellement, ce fichier est
`C:\Users\<USER>\AppData\Roaming\Markdown Edit\user_settings.json`. Un appui sur
`F9` va ouvrir ce fichier avec l'éditeur système Notepad. Il devrait ressembler
à ceci:

    {
        "EditorBackground": "#F7F4EF",
        "EditorForeground": "Black",
        "EditorFontFamily": "Segoe UI",
        "EditorFontSize": 14.0
    }

Dès que vous modifiez des réglages et enregistrez ce fichier, Markdown Edit va
prendre en compte ces changements.

Les couleurs peuvent être exprimées en valeurs RBG, tel que pour la valeur
`EditorBackground`, ou en utilisant des noms prédéfinis (tel que pour la valeur
`EditorForground`). Les noms prédéfinis utilisables sont listés
[ici](http://is.gd/IkK9i7).

Si vous effacez ce fichier, Markdown Edit va le restaurer avec les réglages par
défaut.

Snippets
--------

Les snippets permettent l'insertion rapide de mots ou de phrases un saisissant
un mot clef suivi de la touche `TAB`. Cela peut améliorer la vitesse d'écriture
d'un document et la productivité. Les snippets sont stockées dans un fichier
texte qui peut être édité en appuyant sur `F6`.

Les snippets sont activées saisissant un mot clef suivi de la touche `TAB`.

Les snippets sont constituées d'une seule ligne commençant par:

-   une mot clef unique (qui peux continuer des caractères non alphanumériques)
-   une ou plusieurs espaces
-   le texte qui remplacera le mot clef

Exemple

    mde  [Markdown Edit](http://markdownedit.com)

Avec cette snippet définie, ouvrez Markdown Edit et saisissez

    mde[TAB]

Où `[TAB]` est la touche tab.

Le texte `mde` est remplacé par

    [Markdown Edit](http://markdownedit.com)

### Paramètres de substitution des snippets

-   $CLIPBOARD$ - est remplacé par le contenu du presse (texte seulement)

-   $END$ - Place le curseur après insertion. Par exemple

    mde [Markdown $END$ Edit](http://markdownedit.com)

    place le curseur entre *Markdown* et *Edit*

-   $DATE$ - est remplacé par la date et l'heure courante

-   $DATE("format")$ - un format valide de date .NET
    (<http://www.dotnetperls.com/datetime-format>)

-   $NAME$ - Où `NAME` peut être n'importe quel mot incluant des tirets bas.
    Lorsque le snippet est déclenché, le paramètre sera mis en surbrillance en
    attente de caractères. Il peut y avoir plusieurs paramètres de substitution
    dans une snippet. Voici comment *link* snippet est défini:

    link [$link\_text$]($link_url$) $END$

Lorsqu'elle est déclenchée, la snippet va insérer `[link_text](link_url)`, avec
`link_text`, mis en surbrillance. Saisissez le texte du lien et appuyez sur
`TAB`. Maintenant le texte `link_url` est mis en surbrillance. Saisissez le lien
URL et appuyez sur `Enter`. Le curseur se décale d'un espace après la parenthèse
fermante.

La description peut paraître plus compliquée qu'il en est. Essayez et vous
verrez que c'est une façon facile et naturelle de travailler.

-   `\n` - insérer une nouvelle ligne

Si vous effacez ce fichier, Markdown Edit va le restaurer avec les snippets par
défaut.

Modèles
-------

Vous pouvez changer l'apparence de la prévisualisation en changeant le fichier
de modèle utilisateur. Les modèles utilisateur fonctionne de manière similaire
aux réglages utilisateur. Le fichier modèle est placé dans le répertoire
`AppData` sous le nom `user_template.html`. Il peut être ouvert en appuyant sur
`F8`. Éditez le à votre convenance.

Il est fortement recommandé de conserver le meta tag IE9 dans la section
`<head>`.

Une `<div>` avec un ID `content` est requise. C'est l'emplacement qui sert à
stocker les traductions.

Dès que vous modifiez des réglages et enregistrez ce fichier, Markdown Edit va
prendre en compte ces changements.

Si vous effacez ce fichier, Markdown Edit va le restaurer avec le modèle par
défaut.

Correction orthographique
-------------------------

Appuyez sur `F7` pour activer/désactiver la correction orthographique. La
correction orthographique est faite au fur et à mesure de la frappe. Cliquez
avec le bouton droit sur le mot pour voir les corrections suggérées ou ajouter
le mot au dictionnaire.

Le dictionnaire personnalisé est un simple fichier texte. Il est placé dans le
même répertoire que les réglages et les modèles utilisateur. Il peut être ouvert
et modifié en appuyant sur `Shift+F7`.

Markdown Edit est livré avec des dictionnaires pour de nombreuses langues.
Choisissez le dictionnaire utilisé en appuyant sur `F9`. Les dictionnaires sont
stockés dans le sous répertoire `Spell Check\Dictionaries` du répertoire
d'installation.

Thèmes
------

Markdown Edit utilise un système de thèmes rudimentaire. Les thèmes contrôlent
l'apparence de l’éditeur et de la colorisation syntaxique. Les éléments
d'interface graphique (i.e. boites de dialogue) ne sont pas gérés.

Markdown est livré avec plusieurs thèmes qui peuvent être accédés en appuyant
sur la touche `Ctrl+T`. Choisir un thème modifie vos réglages utilisateurs. Vous
pouvez par la suite modifier le thème en ouvrant votre fichier de réglage (`F9`)
et en modifiant la section `Theme`. C'est la façon recommandée de créer un
nouveau thème.

Les thèmes sont stockés dans le sous répertoire `Themes` du répertoire
d'installation.

Si vous créez un super thème, envoyez le moi et je l'ajouterai. Je ne suis un
grand artiste. :)

Etc.
----

-   Les numéros de lignes peuvent être activés/désactivés dans le fichier
    des réglages.
-   Liste automatique - Lors de l'édition des listes, aller à la ligne ajoutera
    automatiquement un nouvel élément de liste. Cela fonctionne également avec
    les listes numérotées.
-   Lorsque un document récent est ouvert, Markdown Edit va défiler jusqu'à la
    dernière position enregistrée. Cela peut être désactivé dans le fichier
    des réglages.

Limitations
-----------

-   Interface mono document
-   Je l'ai écris ;)

A propos
--------

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

