namespace SilentOrbit.StaticOnline.Config.Data;

/// <summary>
/// Simplify entry of URLs using string values.
/// </summary>
public class Url : IEquatable<Url>
{
    public readonly string fullURL;

    public bool HasQueryOrFragment { get; }

    public override string ToString() => fullURL;

    #region Constructor & casts

    public Url(Uri uri)
    {
        if (uri.IsWellFormedOriginalString() == false)
            throw new ArgumentException($"URL not well formed: {uri}");
        if (uri.IsAbsoluteUri == false)
            throw new ArgumentException("Required absolute URL");
        if (uri.AbsolutePath.Contains("//") || uri.AbsolutePath.EndsWith('/'))
        {
            var port = uri.IsDefaultPort ? null : ":" + uri.Port;
            var query = uri.Query == "" ? null : "?" + uri.Query;
            uri = new Uri($"{uri.Scheme}://{uri.Host}{port}{uri.AbsolutePath.Replace("//", "/").TrimEnd('/')}{query}{uri.Fragment}", UriKind.Absolute);
        }

        fullURL = uri.AbsoluteUri;
        HasQueryOrFragment = uri.Query != "" || uri.Fragment != "";
    }

    [return: NotNullIfNotNull(nameof(url))]
    public static implicit operator Url?(string? url)
    {
        if (url is null)
            return null;
        return new Url(new Uri(url));
    }

    [return: NotNullIfNotNull(nameof(uri))]
    public static implicit operator Url?(Uri? uri)
    {
        if (uri is null)
            return null;
        return new Url(uri);
    }

    [return: NotNullIfNotNull(nameof(url))]
    public static implicit operator string?(Url? url)
    {
        return url?.fullURL;
    }

    [return: NotNullIfNotNull(nameof(url))]
    public static implicit operator Uri?(Url? url)
    {
        if (url is null)
            return null;
        return new Uri(url.fullURL);
    }

    #endregion

    #region URL components

    public string AbsoluteUrl => new Uri(fullURL).AbsoluteUri;
    public Uri Uri => new Uri(fullURL);

    /// <summary>
    /// Return https://www.exmaple.com:123/
    /// </summary>
    public Url HostURL
    {
        get
        {
            var uri = new Uri(fullURL);
            var port = uri.IsDefaultPort ? null : ":" + uri.Port;
            return $"{uri.Scheme}://{uri.Host}{port}/";
        }
    }

    #endregion

    public static string operator +(string text, Url url)
        => text + url.fullURL;

    public bool StartsWith(Url baseUrl)
    {
        return fullURL.StartsWith(baseUrl.fullURL);
    }

    public Url Append(string path)
    {
        if (HasQueryOrFragment)
            throw new Exception("Can't append to an URL with query or fragment: " + fullURL);

        return $"{fullURL.TrimEnd('/')}/{path.Trim('/')}";
    }

    #region Equals

    bool IEquatable<Url>.Equals(Url? other)
    {
        if (other is null)
            return false;
        return fullURL.Equals(other.fullURL);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;

        if (obj is Url url)
            return fullURL.Equals(url);

        if (obj is Uri otherURI)
            return fullURL.Equals(otherURI);

        return false;
    }

    public override int GetHashCode()
    {
        return fullURL.GetHashCode();
    }

    public static bool operator ==(Url? left, Url? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.fullURL == right.fullURL;
    }

    public static bool operator !=(Url? left, Url? right)
    {
        if (left is null && right is null) return false;
        if (left is null || right is null) return true;
        return left.fullURL != right.fullURL;
    }

    #endregion


}
