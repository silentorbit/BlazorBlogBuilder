using System.Xml.Linq;

namespace SilentOrbit.BlazorBlogBuilder.Extensions;

static class XmlExtensions
{
    /// <summary>
    /// Add element with value if set.
    /// Adds no element if value is null.
    /// </summary>
    [return: NotNullIfNotNull(nameof(value))]
    public static XElement? AddElementIf(this XElement root, string name, string? value)
    {
        if (value == null)
            return null;

        var element = new XElement(root.Name.Namespace + name, value);
        root.Add(element);
        return element;
    }

    /// <summary>
    /// Get with '<?xml' header
    /// </summary>
    public static string ToUtf8String(this XElement element)
    {
        var xml = new XDeclaration("1.0", "utf-8", null);
        var stylesheet = SiteBuilder.Instance.Config.BaseURL.Href + "/" + (element.Name.LocalName switch
        {
            "urlset" => "feed/sitemap.xsl",
            "feed" => "feed/atom.xsl",
            "rss" => "feed/rss.xsl",
            _ => throw new NotImplementedException(element.Name.ToString())
        });
        var xsl = new XProcessingInstruction("xml-stylesheet", @$"type=""text/xsl"" href=""{stylesheet}""");
        var doc = new XDocument(xml, xsl, element);

        var writer = new Utf8StringWriter();
        doc.Save(writer);
        return writer.ToString();
    }

    class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}
