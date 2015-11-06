---
layout: post
title: "Post with Figure"
description: "Examples and code for displaying images in posts."
category: articles
tags: [sample post, images, test]
---

This is a post that uses a `figure`. It stacks these images and places a nice little caption below if you fill out `figcaption`.

### Single Image Figure

<figure>
	<img src="http://farm9.staticflickr.com/8426/7758832526_cc8f681e48_c.jpg">
	<figcaption>Morning Fog Emerging From Trees by A Guy Taking Pictures, on Flickr</figcaption>
</figure>

{% highlight html linenos %}
<figure>
	<img src="/images/image-filename-1.jpg">
	<figcaption>Caption describing these two images.</figcaption>
</figure>
{% endhighlight %}