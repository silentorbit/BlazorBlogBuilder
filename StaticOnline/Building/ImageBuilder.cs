
using Microsoft.AspNetCore.Components;
using SilentOrbit.StaticOnline.Config.Data;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Buffers.Text;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace SilentOrbit.StaticOnline.Building;

/// <summary>
/// Find and published images using <see cref="SiteBuilder.Image"/>
/// </summary>
public class ImageBuilder(SiteConfig config)
{
    public MarkupString ImageTag(
        string filename,
        int width = 0,
        int height = 0,
        bool crop = false,
        string? alt = null,
        [CallerFilePath] string filePath = null!)
    {
        var url = GetUrl(filename, width, height, crop, alt, filePath);
        return new MarkupString($@"<img src=""{url.Href}"" alt=""{alt}"" />");
    }

    /// <summary>
    /// Resize image to specified dimensions.
    /// Return unique URL for resized image.
    /// </summary>
    public RelUrl GetUrl(
        string filename,
        int width = 0,
        int height = 0,
        bool crop = false,
        string? alt = null,
        [CallerFilePath] string filePath = null!)
    {
        //Find source image file
        var dir = new FilePath(filePath).Parent;
        var files = dir.GetFiles(filename + "*");
        var file = files.First();

        //No Resize
        if (width == 0 && height == 0)
            return StoreOriginal(file);

        if (filename.EndsWith(".svg"))
            throw new ArgumentException("Can't resize a SVG");

        //Load
        using var image = Image.Load(file.Path);
        {
            if (image.Width <= width && image.Height <= height)
                return StoreOriginal(file);

            // Resize the given image in place and return it for chaining.
            image.Mutate((imageProcessingContext) =>
            {
                imageProcessingContext.Resize(new ResizeOptions
                {
                    Size = new Size(width, height),
                    Mode = crop ? ResizeMode.Crop : ResizeMode.Max,
                });
            });

            //Save image data
            using var ms = new MemoryStream();
            image.Save(ms, image.Metadata.DecodedImageFormat!);

            //Hash
            ms.Seek(0, SeekOrigin.Begin);
            var url = HashStreamUrl(file, ms);

            //Store
            if (hashCache.Contains(url))
                return url;
            config.SiteBuilder.Target.Store(url, ms.ToArray());

            hashCache.Add(url);
            config.SiteBuilder.Pages.DoneStatic(url);
            return url;
        }
    }

    readonly Dictionary<FilePath, RelUrl> originalCache = new();
    readonly HashSet<RelUrl> hashCache = new();

    RelUrl StoreOriginal(FilePath file)
    {
        if (originalCache.TryGetValue(file, out var url))
            return url;

        using var sourceStream = new FileStream(file.Path, FileMode.Open);
        url = HashStreamUrl(file, sourceStream);
        config.SiteBuilder.Target.Store(url, file);

        originalCache.Add(file, url);
        hashCache.Add(url);
        config.SiteBuilder.Pages.DoneStatic(url);
        return url;
    }

    RelUrl HashStreamUrl(FilePath file, Stream stream)
    {
        var hash = Base64Url.EncodeToString(SHA1.HashData(stream));
        var url = config.BaseURL.Append($"media/{file.NameWithoutExtension}-{hash}{file.Extension}");
        return url;
    }

    internal void MapGet(WebApplication app)
    {
        app.MapGet("media/{filename}", (string filename) => MapGetContent(filename));
    }

    IResult MapGetContent(string filename)
    {
        var rel = new RelUrl(config.BaseURL, "media/" + filename);
        var file = config.SiteBuilder.Target.GetTarget(rel);
        return Results.File(file.Path);
    }
}
