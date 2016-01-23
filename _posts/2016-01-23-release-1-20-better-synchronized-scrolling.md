---
layout: post-no-feature  
title: "Release 1.20 - Better Synchronized Scrolling"
---

Synchronizing the scroll positions between the editor and preview is a
much harder problem than might appear. Your brain is very good at
pattern matching. The computer, not so much.

![tuning-forks-synchronizing](http://i.imgur.com/MigtPB3.png)

MDE's current implementation of synchronized scrolling uses a
percentage-based offset with corrections for images. It can easily break
as some of you have reported.

The new implementation injects scroll anchors into the preview. It's
essentially an identifier based on the current block level element in
the preview document. On the editor side, MDE counts the current number
of block level elements in the Abstract-Syntax-Tree up to the top
visible line. If I've done my math correctly, the block levels should be
the same. This (in theory) should render accurate scroll positions to
the current block level (paragraph, image, list item, etc).

One of my acid tests for preview rendering is to render the CommonMark
specification, which is written in CommonMark (a.k.a. Markdown). Many
editors will not handle a document this long or detailed. MDE does so
easily. And it maintains scroll synchronization throughout the document
(victory for me!).

Most documents should just work. There will be edge cases that fail. For
instance, if you go crazy with HTML blocks with paragraphs in them, it
will break. I can't predict all the variations so I'm taking a
conservative (a.k.a. lazy) approach and doing what's simple and
predictable. If you encounter documents that don't scroll-sync, send
them to me and I'll try to enhance the algorithm to work with them.

Also in the release, a new German translation. (woot!)

**Looking Forward**

Many of you have submitted issues or enhancement requests. I've
dutifully tried to label these items in GitHub. I'm a little behind on
the enhancement requests due to the amount of effort I put into this
scroll synchronization thing. I've also recently put significant effort
into my other Open Source projects.

There's an overtype issue that's should be quick to tackle. Also, I
think I'll focus on image pasting. It should be possible to grab images
from the clipboard and paste them as Data URI's or files for instance.

That's all for now. Keep the feedback coming.

Please consider donating.

[Donate with
PayPal](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=XGGZ8BEED7R62)
