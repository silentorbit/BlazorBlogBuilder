using System.Buffers.Text;
using System.Security.Cryptography;

namespace SilentOrbit.StaticOnline.Building;

public class Hasher()
{
    /// <summary>
    /// End of filename to indicate it should be hashed
    /// </summary>
    const string hashMarker = "-hash";

    /// <summary>
    /// Key: original URL
    /// Value: Hashed URL
    /// </summary>
    readonly Dictionary<RelUrl, RelUrl> cache = new();

    static string Hash(byte[] data)
    {
        var sha = SHA1.HashData(data);
        var hash = Base64Url.EncodeToString(sha);
        return hash;
    }

    internal Url GetHashPath(FilePath f, RelUrl original)
    {
        var name = Path.GetFileNameWithoutExtension(original.Href);
        if (name.EndsWith(hashMarker) == false)
            return original;

        if (cache.TryGetValue(original, out var cachedRelPath))
            return cachedRelPath;

        var data = f.ReadAllBytes();
        var hash = "-" + Hash(data);
        var hashUrl = new RelUrl(original.BaseUrl, original.Href.Replace(hashMarker, hash));

        cache.Add(original, hashUrl);

        return hashUrl;
    }

    /// <summary>
    /// Rewrite path to hashed files
    /// </summary>
    public string RewriteHTML(string html)
    {
        foreach (var kvp in cache)
        {
            html = html
                .Replace(
                    $@"""{kvp.Key.Href}""",
                    $@"""{kvp.Value.Href}""");

            Debug.Assert(html.Contains(kvp.Key.Href) == false);
        }

        return html;
    }

}
