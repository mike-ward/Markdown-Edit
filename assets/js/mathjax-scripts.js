// Avoid undefined script pop up messages by prefixing with window.
// window.MathJax.Hub.Config({tex2jax: {inlineMath: [['$','$'], ['\\(','\\)']]}});
window.document.addEventListener("previewUpdated", function () {
  if (window.MathJax) { 
    window.MathJax.Hub.Queue(["Typeset", window.MathJax.Hub]);
    }
  });
