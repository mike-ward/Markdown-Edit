// Avoid undefined script pop up messages by prefixing with window.
window.document.addEventListener("previewUpdated", function () {
  if (window.MathJax) { 
    window.MathJax.Hub.Queue(["Typeset", window.MathJax.Hub]);
    }
  });
