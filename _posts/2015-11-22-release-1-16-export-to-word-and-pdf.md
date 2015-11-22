---
layout: post  
title: "Release 1.16 - Export to Word and PDF"
---

Thanks to some excellent suggestions from GitHub user `borekb`, release
1.16 has a mother-load of new features.

-   Version Checking: When a new version of MDE is published, an icon
    will appear in the title bar. I tried to make it subtle enough to
    inform without be annoying.

    ![newversion](http://i.imgur.com/Tn3ngrl.png)  
    *The big red arrow is doesn't appear in the production version :)*

-   Alternate Line Endings: Apparently, some Mac users are defecting and
    using MDE. Scandalous! But I won't tell.

    ![lineendings](http://i.imgur.com/qQkXL9n.png)

-   Export to Word and PDF: Check out the `Save As` dialog. There are
    options to save the to `docx` and `pdf`. `html` was added to
    be consistent. Don't worry, the original shortcuts for exporting to
    `html` still work. (Issue \#68, \#69)

    ![saveas](http://i.imgur.com/OIXQ2R9.png)

-   Fix image preview: Relative paths to images were broken in
    the preview. (Issue \#70)

-   Support for custom Markdown converters: This one is kind of geeky,
    but you can use a different Markdown converter provided it's a
    console application and reads and writes to standard input/output.
    This is mostly for those who want to tweak the Pandoc settings
    MDE uses. You'll have to edit the settings file directly. Example:

        "CustomMarkdownConverter": "pandoc -f markdown_mmd -t html"

    This is also used for preview so it's best if you stick to
    converting to HTML. (Issue \#67)

-   Fix Crash on Bad Settings: MDE would unceremoniously die if the
    settings file was corrupted. (Issue \#49).

-   Remove auto image pasting from clipboard: Did you know you could
    paste an image to MDE and it would automatically upload it to MDE?
    As well intentioned as that was, it represented a privacy issue so I
    removed it. (Issue \#73).


