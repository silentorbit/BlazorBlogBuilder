using System.Text;

namespace SilentOrbit.StaticOnline.Building.FileGeneration;

class SitemapTXT : SitemapBase
{
    public override Url URL => Config.BaseURL + "sitemap.txt";

    public override string Generate()
    {
        var sb = new StringBuilder();

        foreach (var page in SitemapPages())
        {
            sb.AppendLine(page.URL);
        }

        return sb.ToString();
    }
}
