---
layout: post-no-feature  
title: "Snippet completion more useful than I thought"
---

I was noticing some editors have a feature that will automatically add a
closing brace or parenthesis when an opening one is typed. This is
commonly referred to as auto-complete. I have a love hate relationship
with auto-complete. I often find the auto-complete triggers when I don't
want it to. Also, I find I have to arrow-press past the closing symbol
to continue typing. Most of the time I find it faster to just type
`*something*` than deal with the auto-complete.

It dawned on me the other day that I already have this facility in MDE.
I just had add some snippet definitions.

    (         ($text$) $END$
    {         {$text$} $END$
    [         [$text$] $END$
    *         *$text$* $END$
    **        **$text$** $END$
    `         `$text$` $END$

This gives me just what I want. I type `*` for instance and press `TAB`.
The closing `*` is added and the cursor moves the spot between the `**`.
I type the text and hit enter. The cursor moves past the closing `*`
plus one space. Here's an example:

<iframe src="https://player.vimeo.com/video/147768801" width="500" height="281" frameborder="0" webkitallowfullscreen mozallowfullscreen allowfullscreen>
</iframe>
<p><a href="https://vimeo.com/147768801">Markdown Edit Tab Completion</a> from <a href="https://vimeo.com/user38899386">Mike Ward</a> on <a href="https://vimeo.com">Vimeo</a>.</p>

