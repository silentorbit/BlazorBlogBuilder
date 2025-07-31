
using Microsoft.AspNetCore.Components;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System.Buffers.Text;
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

        //TODO:
        //- new suffix: modifications, "-100x100-crop"
        //- Cache key: source path + modifications
        //- Use only hash of original source
        //- Separate cache of source hashes
        //- ImageGenerator - on demand
        //Why:
        //- Only generate images in use, ignore images only used in draft
        //- Save work when image is reused in several places

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

            // Resize the image
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

            //Use this to save in the same format as the source image: image.Metadata.DecodedImageFormat

            //Always save in webp.
            file = file.GetWithExtension("webp");
            image.Save(ms, new WebpEncoder());

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

        RelUrl url;
        if (config.BuildConfig.KeepImageFilename)
            url = config.BaseURL.Append($"media/{file.NameWithoutExtension}-{hash}{file.Extension}");
        else
            url = config.BaseURL.Append($"media/{hash}{file.Extension}");

        return url;
    }

    internal IResult MapGetContent(string filename)
    {
        var rel = new RelUrl(config.BaseURL, "media/" + filename);
        var file = config.SiteBuilder.Target.GetTarget(rel);
        return Results.File(file.Path);
    }
}
