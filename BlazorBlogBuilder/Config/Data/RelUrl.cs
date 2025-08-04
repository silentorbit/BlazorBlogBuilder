namespace SilentOrbit.BlazorBlogBuilder.Config.Data;

public class RelUrl : Url
{
    public readonly BaseUrl BaseUrl;
    
    /// <summary>
    /// Relative to <see cref="BaseUrl"/>
    /// </summary>
    public string Href { get; }

    public RelUrl(BaseUrl baseUrl, string path) : base(FullURL(baseUrl, path))
    {
        this.BaseUrl = baseUrl;
        Debug.Assert(path.StartsWith('/') == false);
        this.Href = path.TrimEnd('/');
        Debug.Assert(Href.StartsWith('/') == false);
    }

    public RelUrl(BaseUrl baseUrl, Url url) : base(url)
    {
        this.BaseUrl = baseUrl;
        this.Href = baseUrl.GetRelativePath(url);
    }

    static Uri FullURL(BaseUrl baseUrl, string path)
    {
        return new Uri($"{baseUrl.fullURL.TrimEnd('/')}/{path.TrimEnd('/')}");
    }

    public new RelUrl Append(string path)
    {
        if (HasQueryOrFragment)
            throw new Exception("Can't append to an URL with query or fragment: " + fullURL);

        return new RelUrl(BaseUrl, Href + path.Trim('/'));
    }

}
