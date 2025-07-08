using Microsoft.AspNetCore.Components;
using System.Buffers.Text;
using System.Text;

namespace SilentOrbit.StaticOnline.Building.BlazorRendering;

/// <summary>
/// Initial scan of all razor files with @page attributes
/// </summary>
public class BlazorIndex(SiteBuilder site)
{
    /// <summary>
    /// Inital scan for all blazor pages
    /// </summary>
    public void Scan()
    {
        foreach (var type in site.Config.Type.Assembly.GetTypes())
        {
            var urls = GetUrls(type);
            foreach (var url in urls)
            {
                var page = site.Pages.GetOrCreate(url);
                page.BlazorType = type;
            }

            //Blog posts
            if (type.IsAssignableTo(typeof(BlogPost)))
            {
                var url = BlogPostUrl(type);
                var page = site.Pages.GetOrCreate(url);
                page.BlazorType = type;
                page.InFeed = true;
            }
        }
    }

    /// <summary>
    /// Get URL from @page "" attribute.
    /// </summary>
    IEnumerable<Url> GetUrls(Type type)
    {
        var attr = type.GetCustomAttributes(true)
            .OfType<RouteAttribute>()
            .ToArray();
        if (attr.Length == 0)
        {
            //No route attribute found, not a page
            yield break;
        }
        foreach (var route in attr)
        {
            if (route.Template.Contains('{'))
            {
                //Skip templates with parameters
                continue;
            }
            yield return site.Config.BaseURL.Append(route.Template);
        }
    }

    Url BlogPostUrl(Type type)
    {
        var hash =
            Base64Url.EncodeToString(
                System.Security.Cryptography.SHA1.HashData(
                    Encoding.UTF8.GetBytes(type.FullName!)));
        var url = site.Config.BaseURL.Append("post/" + hash);
        return url;
    }
}
