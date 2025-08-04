namespace SilentOrbit.BlazorBlogBuilder.Building.FileGeneration;

class SitemapTXT : SitemapBase
{
    public override RelUrl URL => Config.BaseURL + "sitemap.txt";

    public override Task<string> Generate(RelUrl url)
    {
        var sb = new StringBuilder();

        foreach (var page in SitemapPages())
        {
            sb.AppendLine(page.URL);
        }

        return Task.FromResult(sb.ToString());
    }

}
