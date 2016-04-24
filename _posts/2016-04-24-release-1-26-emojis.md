---
layout: post-no-feature  
title: "Release 1.26 - Emojis"
---

### What's changed:

-   table text overlapped when splitting over pages \#126
-   GitHub-like Emoji rendering \#128
-   Touchpad scrolling is way too fast \#129
-   Ampersand in headings \#130
-   IgnoreYaml doesn't work \#131 (removed feature)
-   Curly quotes \#134 (added snippets)
-   New version incorrectly suggested \#136 (false positives on
    network login)

### About those Emojis

To get Emojis working, the rendering template has to be updated. This
file is located in
`C:\Users\<user folder>\AppData\roaming\Markdown Edit\user_template.html`
If you have made changes to this file, you should save it else where.
Delete the file and restart Markdown Edit. MDE will build a new
`user_template.html` file with the required changes.

Use Emojis as you would in GitHub. For instance `:smile:` creates a
:smile:. In a future release I'll add command completion to help with
typing these. In the meantime, you can add snippets for your favorites.

### Curly Quotes

I’m not a fan of these little gremlins because they don't render
reliable across code pages. Still, some prefer them. I've added a couple
of snippets to help with these.

    "         “$END$”
    '         ‘$END$’

It’s not a perfect solution, but it’s close enough. Keep in mind that
these need to encoded as UTF-8. Windows Notepad is not UTF-8 (it’s
Windows 1252). This is reason why I don’t use curly quotes (accept for
this paragraph :smile:.

### Pace of Change

Some of you may have noticed I've slowed down a bit on updates. My kids
are in rowing (Crew for you folks on East coast). It's rowing season and
being on 2 committees and the Vice President takes considerable time.
I'll poke at MDE but it will likely be mid-summer before the next
release. Keep the feedback and bugs coming.
