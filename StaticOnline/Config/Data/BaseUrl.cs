using System;

namespace SilentOrbit.StaticOnline.Config.Data;

public class BaseUrl : Url
{
    internal void Replace(Uri baseAddress)
    {
        Href = baseAddress.AbsolutePath.TrimEnd('/');
    }

    public string Href { get; private set; }

    public BaseUrl(Uri uri) : base(uri)
    {
        Href = uri.AbsolutePath.TrimEnd('/');
    }

    [return: NotNullIfNotNull(nameof(url))]
    public static implicit operator BaseUrl?(string? url)
    {
        if (url is null)
            return null;
        return new BaseUrl(new Uri(url));
    }

    [return: NotNullIfNotNull(nameof(url))]
    public static implicit operator BaseUrl?(Uri? url)
    {
        if (url is null)
            return null;
        return new BaseUrl(url);
    }

    public new RelUrl Append(string path)
    {
        if (HasQueryOrFragment)
            throw new Exception("Can't append to an URL with query or fragment: " + fullURL);

        return new RelUrl(this, path.Trim('/'));
    }

    public static RelUrl operator +(BaseUrl url, string path)
        => url.Append(path);

    /// <summary>
    /// Return the path relative to the <paramref name="baseUrl"/>
    /// </summary>
    public string GetRelativePath(Url url)
    {
        if (url.StartsWith(this) == false)
            throw new ArgumentException($"URL {url.fullURL} must start with {base.fullURL}");

        return url.fullURL.Substring(fullURL.Length).TrimStart('/');
    }
}
