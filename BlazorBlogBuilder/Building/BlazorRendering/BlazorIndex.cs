using Microsoft.AspNetCore.Components;
using System.Buffers.Text;

namespace SilentOrbit.BlazorBlogBuilder.Building.BlazorRendering;

/// <summary>
/// Initial scan of all razor files with @page attributes
/// </summary>
public class BlazorIndex(SiteBuilder builder)
{
    /// <summary>
    /// Inital scan for all blazor pages
    /// </summary>
    public void Scan()
    {
        foreach (var type in builder.Config.BuildConfig.AppType.Assembly.GetTypes())
        {
            var urls = GetUrls(type);
            foreach (var url in urls)
            {
                var page = builder.Pages.GetOrCreate(url);
                page.BlazorType = type;
            }

            //Don't generate if the post as a specific @page directive.
            if (urls.Any())
                continue;

            //Blog posts
            if (type.IsAssignableTo(typeof(BlogPost)))
            {
                var url = BlogPostUrl(type);
                var page = builder.Pages.GetOrCreate(url);
                page.BlogPostRandomURL = url;
                page.BlazorType = type;
                page.InFeed = true;
            }
        }
    }

    /// <summary>
    /// Get URL from @page "" attribute.
    /// </summary>
    IEnumerable<RelUrl> GetUrls(Type type)
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
            yield return builder.Config.BaseURL.Append(route.Template);
        }
    }

    RelUrl BlogPostUrl(Type type)
    {
        var hash =
            Base64Url.EncodeToString(
                System.Security.Cryptography.SHA1.HashData(
                    Encoding.UTF8.GetBytes(type.FullName!)));

        //Can be anything as it will be replaced after PreScan.
        const string urlPrefix = "_static_online_post/";
        var url = builder.Config.BaseURL.Append(urlPrefix + hash);
        return url;
    }
}
