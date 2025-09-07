
# Static site generation from Blazor websites

Write your blazor app and have it generated into a static website or blog.

## Features

* Write your blog posts in razor files
  * Use `@inherit BlogPost` to automatically add it to the feeds and list of post
* Add metadata using `Page.Title = "Hello"`, `Page.Published = "2025-07-01 15:42"`
* Tags/Category support
  * `Page.Tags = "first, inspiring, website"`
* Add `<Summary>` and `<Update>` to your blog posts or any other page (which will appear in the feeds)
* Static comments using `<Comment>` with support for email comments
* Comment form for email comments
* Markdown
  * Write in Markdown directly inside your razor files
  * Customize where to parse as Markdown
	* Set defaults in `SiteConfig.BuildConfig.Markdown`
	* Override defaults using `Page.Markdown = false`, `<Markdown># Hello</Markdown>` or `<Summary Markdown="false">Raw text</Summary>`
* Draft pages will not be included in the build `Page.IsDraft = true`
* Schedule publishing using `Page.Publish = "2058-01-01"` in combination with scheduled GitHub builds
* Host at a subdirectory, `www.example.com/blog/`
* Generates **RSS**, **Atom** and **JSON Feed**
  * Includes `@inherit BlogPost`, `Page.InFeed = true`, `<Update Date="2025-07-01">`
  * Per tag feeds
* Generates `sitemap.xml` and `sitemap.txt`
* Integrated crawler that finds and downloads all linked pages
  * Missing pages will stop the build
* Finds all razor pages with `@page "/my/page/"` and include them in the build.
  *	Those with parameters, `@page "/subdir/{Index:int}"`, will be discovered via the integrated crawler.
* Per page/post customization of headers
  * `Page.Head.Robots.NoIndex = true`
  * `Page.Sitemap = false`
* GitHub Action generation and publishing
  * See instructions at `.github/workflows/publish.yml`

## Examples

- https://www.silentorbit.com/blazor-blog-builder/blazor/ ([source](https://github.com/silentorbit/BlazorBlogBuilderDemo/tree/main/BlazorDemo))
- https://www.silentorbit.com/blazor-blog-builder/blog/ ([source](https://github.com/silentorbit/BlazorBlogBuilderDemo/tree/main/BlogDemo))

## Getting started

1. Download the demo at https://github.com/silentorbit/BlazorBlogBuilderDemo
2. Mark `BlogDemo` as the startup project
3. Run the project

After the build is completed (in less than 2 seconds),
the website will load in the browser.

## Start from scratch

Create a new Blazor app or modify an existing one.

Use either the NuGet package or clone the latest version from git.

* NuGet: https://www.nuget.org/packages/SilentOrbit.BlazorBlogBuilder/
  * Add the NuGet package `SilentOrbit.BlazorBlogBuilder` to your Blazor app
* Git: https://github.com/silentorbit/BlazorBlogBuilder
  * Reference the project BlazorBlogBuilder

Modify your Blazor app according to the demo https://github.com/silentorbit/BlazorBlogBuilderDemo  
Look for `#region BlazorBlogBuilder` in:

* `Program.cs`
* `App.razor`
* See `Posts.razor` and `Tags.razor` on how to render blog posts and tag index

## Support

* [Discord: SilentOrbit/blazor-blog-builder](https://discord.gg/Tjd5XysYkc)
* [GitHub Issues: Bugs, features and pull requests](https://github.com/silentorbit/BlazorBlogBuilderDemo/issues)
