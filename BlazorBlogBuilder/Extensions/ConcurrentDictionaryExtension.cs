using System.Collections.Concurrent;

namespace SilentOrbit.BlazorBlogBuilder.Extensions;

public static class ConcurrentDictionaryExtension
{
    public static void Remove(this ConcurrentDictionary<Url, PageData> dic, PageData pageToRemove)
    {
        var removed = dic.TryRemove(pageToRemove.URL, out var pageRemoved);
        Debug.Assert(removed);
        Debug.Assert(pageRemoved == pageToRemove);
    }
}
