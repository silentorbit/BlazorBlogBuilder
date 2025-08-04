using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace SilentOrbit.BlazorBlogBuilder.Building.BlazorRendering;

class StaticNavigation : NavigationManager, INavigationInterception, IScrollToLocationHash
{
    public StaticNavigation(Url url)
    {
        Initialize(url.AbsoluteUrl, url.AbsoluteUrl);
    }

    public void NavigateToRelative(string relativeUrl)
    {
        var url = BaseUri + relativeUrl.TrimStart('/');
        NavigateTo(url, true);
    }

    protected override void NavigateToCore([StringSyntax("Uri")] string uri, NavigationOptions options)
    {
        Uri = uri;
    }

    Task INavigationInterception.EnableNavigationInterceptionAsync()
    {
        throw new NotImplementedException();
    }

    Task IScrollToLocationHash.RefreshScrollPositionForHash(string locationAbsolute)
    {
        throw new NotImplementedException();
    }
}
