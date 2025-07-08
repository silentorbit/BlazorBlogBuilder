using System.Collections;

namespace SilentOrbit.StaticOnline.Tools;

public class PageList : IEnumerable<SitePage>
{
    readonly Dictionary<Type, SitePage> dic = new();

    public void Add(SitePage page)
    {
        var blazorType = page.GetType();
        if (page == null)
            throw new ArgumentNullException(nameof(page));
        //Debug.Assert(dic.ContainsKey(blazorType) == false);
        dic[blazorType] = page;
    }

    IEnumerator<SitePage> IEnumerable<SitePage>.GetEnumerator()
    {
        return dic.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return dic.Values.GetEnumerator();
    }
}
