using SilentOrbit.Disk;
using SilentOrbit.StaticOnline.Building;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Policy;

namespace SilentOrbit.StaticOnline.Config;

public class Url : IComparable<Url>, IEquatable<Url>
{
    readonly string fullURL;
    public bool HasQueryOrFragment { get; }
    public string Href
    {
        get
        {
            var baseUrl = SiteBuilder.Instance.Config.BaseURL;
            if (fullURL.StartsWith(baseUrl.fullURL))
                return GetRelativePath(baseUrl);
            else if (StartsWith(baseUrl.HostURL))
                return GetRelativePath(baseUrl.HostURL);
            else
                return fullURL;
        }
    }

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

        Debug.Assert(uri.AbsolutePath == "/" || uri.AbsolutePath.EndsWith('/') == false);

        fullURL = uri.AbsoluteUri;
        HasQueryOrFragment = uri.Query != "" || uri.Fragment != "";
        Debug.Assert(fullURL.Contains("#") == HasQueryOrFragment);
    }

    [return: NotNullIfNotNull(nameof(url))]
    public static implicit operator Url?(string? url)
    {
        if (url is null)
            return null;
        return new Url(new Uri(url));
    }

    [return: NotNullIfNotNull(nameof(url))]
    public static implicit operator Url?(Uri? url)
    {
        if (url is null)
            return null;
        return new Url(url);
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

    public Url Append(string path)
    {
        if (HasQueryOrFragment)
            throw new Exception("Can't append to an URL with query or fragment: " + fullURL);

        return $"{fullURL.TrimEnd('/')}/{path.Trim('/')}";
    }

    public Url Append(RelDiskPath relDiskPath)
    {
        var path = relDiskPath.RelativePath.Replace('\\', '/');
        return Append(path);
    }

    public static Url operator +(Url url, string path)
        => url.Append(path);

    public static string operator +(string text, Url url)
        => text + url.fullURL;

    public bool StartsWith(Url baseUrl)
    {
        return fullURL.StartsWith(baseUrl.fullURL);
    }

    /// <summary>
    /// Return the path relative to the <paramref name="baseUrl"/>
    /// </summary>
    public string GetRelativePath(Url baseUrl)
    {
        if (StartsWith(baseUrl) == false)
            throw new ArgumentException($"URL {fullURL} must start with {baseUrl}");

        return fullURL.Substring(baseUrl.fullURL.Length).TrimStart('/');
    }


    #region Equals

    bool IEquatable<Url>.Equals(Url? other)
    {
        if (other is null)
            return false;
        return fullURL.Equals(other);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;

        if (obj is Url url)
            return this.fullURL.Equals(url);

        if (obj is Uri otherURI)
            return this.fullURL.Equals(otherURI);

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

    #region Compare

    public class Comparer : IComparer<Url>
    {
        public int Compare(Url? x, Url? y)
        {
            if (x is null && y is null) return 0;
            if (x is null) return 1;
            if (y is null) return -1;
            return x.fullURL.CompareTo(y.fullURL);
        }
    }

    int IComparable<Url>.CompareTo(Url? other)
    {
        if (other is null)
            return -1;

        return fullURL.CompareTo(other.fullURL);
    }

    #endregion

}
