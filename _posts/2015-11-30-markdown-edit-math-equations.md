---
layout: post  
title: "Markdown Edit &ndash; Math Equations"
---

Checkout the equation formatting:

![math-example](http://i.imgur.com/iHnDLUD.png)

#### The Easy Way

1.  Download and install the latest version of MDE.

2.  If upgrading from an previous version, delete the
    `user_template.html` file in
    `C:\User\(your-user)\AppData\Roaming\Markdown Edit\`

    *Note: This configuration requires an Internet connection to
    download the [MathJax](http://mathjax.org) files.*

#### The Less Easy Way

If you've modified your template and want to keep those changes, you can
make the changes manually.

Before you rush off and download MDE and try this, understand that **you
have some work ahead of you**

1.  Download and install the latest version of [Markdown
    Edit](https://github.com/mike-ward/Markdown-Edit/releases/latest)
    (1.17 at the time of this writing). It won't work with
    earlier versions.

2.  Open the `user_template.html` file in notepad (do not use
    Microsoft Word!)

3.  Find the following line near the top:

        <!-- Don't delete this X-UA tag. The browser control behaves like IE8 otherwise -->
        <meta http-equiv="X-UA-Compatible" content="IE=9">

    and change it `IE=9` to `IE=Edge` (see below)

        <!-- Don't delete this X-UA tag. The browser control behaves like IE8 otherwise -->
        <meta http-equiv="X-UA-Compatible" content="IE=Edge">

4.  Add the script lines as shown below. Insert them just above the
    closing body tag.

        <!-- Scripts to handle MathJax -->
        <script async src="https://cdn.mathjax.org/mathjax/latest/MathJax.js?config=TeX-AMS-MML_HTMLorMML"></script>
        <script async src="http://markdownedit.com/assets/js/mathjax-scripts.js"></script>

        </body>

5.  Save the file and close `notepad`.

If you're familiar with scripting in Web pages this is similar to what
you already do. If not, find a knowledgeable person to walk you through
it.

#### Details, Details, Details...

There are a number of ways to display equations in Web pages, each with
their advantages and disadvantages. Instead of locking everyone into a
single system, I've opted to keep MDE flexible and agnostic about
external scripts. This gives everyone the option to hack and configure
as they see fit. However, to quote the superhero axiom,

> "With great power, comes great responsibility"

To begin, it helps to understand how the page life cycle of a the MDE
preview works. It's different than a traditional web page. If you're
accustomed to scripting in a traditional web page, you'll have to do
things differently.

-   **First the template loads**  
    The HTML template (the file you can customize to change the
    appearance of the preview) is loaded. At this point, there is no
    content from the editor loaded. The `document.loaded` event
    is triggered.

    If you used JavaScript libraries like [jQuery](https://jquery.com/),
    you're probably familiar with the `document.ready` handler. This
    will trigger as normal, but there won't be any content loaded. It
    appears that your script is not working. In reality, there's just no
    content to for your script to work on.

-   **Editor loads the document**  
    Next, MDE loads your document. Once loaded, it updates the preview
    by injecting content into the document (that's why you need that
    `<div id="content"></div>` element in the template.

-   **Editing the document**  
    Every time you make a change to the text, MDE recomputes the HTML
    and injects it into the preview.

OK, so we have two problems here. First, the `document.loaded` event
triggers before there is content to render. Secondly, the content
changes as you edit. What's a poor script writer to do?

The answer is that I had to add a custom event to preview that triggers
whenever the preview is updated. Your script then has to, "listen" for
this event and respond accordingly.

Here's what's in <http://markdownedit.com/assets/js/mathjax-scripts.js>

    // Avoid undefined script pop up messages by prefixing with window.
    window.document.addEventListener("previewUpdated", function () {
      if (window.MathJax) { 
        window.MathJax.Hub.Queue(["Typeset", window.MathJax.Hub]);
        }
      });

MDE triggers the custom `previewUpdated` event. The handler tells
MathJax to typeset the equations.

I know this is a long-winded explanation, but if you want to add
scripting to MDE's preview, it's something you must understand (and
therefore I have to document as I am doing here).

#### Some additional notes

-   It's really important to keep your scripts in separate files and
    load them asynchronously. Do not put script directly into
    the template. You'll find that script will block the preview during
    the load sequence and can lead to some weird rendering (or no
    rendering at all).

-   Obviously, you have to be connected to the Internet for the MathJax
    script to download. If this is a problem, then download the scripts
    and install them locally. Remember to load these
    scripts asynchronously.

-   It's fair to ask why I didn't just build this in. The answer is
    complicated and I won't bore you with details. Simply put, doing it
    this way allows you to choose. MathJax is only one way to display
    equations in a Web page. It may not fit every scenario. Also, this
    allows for other custom script renderings. It's the responsibility
    part of the superhero axiom.

-   Inline rendering is accomplished with `\\( ... \\)`. You can
    configure MathJax to use `$ ... $`, but it leads to some awkward
    situations with text involving currency symbols.

#### Conclusion

It took considerable effort to bring this functionality to MDE. While it
may look like I just added a few scripts, it was quite the puzzle to
understand and extend the internal Web Browser control to handle this. I
won't be surprised if there are some bugs on this one. Equation
rendering is complicated and messy.

For those of you wanting to add your own scripts, consider this a recipe
on how to do it. Just keep in mind that the Web page life cycle is
different and that everything should be asynchronous.
