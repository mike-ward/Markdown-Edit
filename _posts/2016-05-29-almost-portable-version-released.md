---
layout: post-no-feature    
title: "(Almost) Portable Version Released"
---

Version 1.27's major feature is an almost portable version. It's not possible (yet) to make a truly portable .NET (with WPF) version of Markdown Edit. The target system still requires that .NET 4.5.2+ be installed. Perhaps in the future we'll get a version of WPF that is portable. Until then, this is the best I can do.

The number in the toolbar that indicates the number of words in the document can be toggled by clicking on it. It toggles between words, characters, and pages.

![mde-stats](http://i.imgur.com/u9Qb9NZ.png)

Fixed a bug where spell checking was disabled in nested lists (#135).

Updated to the latest versions of Commonmark and AvalonEdit.